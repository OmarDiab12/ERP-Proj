using ERP.DTOs.Invoices;
using ERP.Helpers;
using ERP.Models.InvoicesManagement;
using ERP.Repositories.Interfaces;
using ERP.Repositories.Interfaces.Inventory;
using ERP.Repositories.Interfaces.Invoices;
using ERP.Repositories.Interfaces.Suppliers;
using ERP.Services.Interfaces.Notifications;
using ERP.Services.Interfaces.Invoices;
using Microsoft.AspNetCore.Http;

namespace ERP.Services.Invoices
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _repo;
        private readonly IInventoryRepository _inventoryRepo;
        private readonly ISupplierRepository _supplierRepo;
        private readonly IErrorRepository _errors;
        private readonly IFileStorageService _fileStorage;
        private readonly IReportExportService _reportExporter;
        private readonly INotificationService _notifications;

        public InvoiceService(IInvoiceRepository repo, IInventoryRepository inventoryRepo, ISupplierRepository supplierRepo, IErrorRepository errors, IFileStorageService fileStorage, IReportExportService reportExporter, INotificationService notifications)
        {
            _repo = repo;
            _inventoryRepo = inventoryRepo;
            _supplierRepo = supplierRepo;
            _errors = errors;
            _fileStorage = fileStorage;
            _reportExporter = reportExporter;
            _notifications = notifications;
        }

        public async Task<ResponseDTO> CreateAsync(CreateInvoiceDTO dto, int userId)
        {
            const string fn = nameof(CreateAsync);
            try
            {
                if (!Enum.TryParse<Enums.InvoiceType>(dto.Type, true, out var invoiceType))
                    return new ResponseDTO { IsValid = false, Message = "Invalid invoice type" };
                if (!Enum.TryParse<Enums.InvoicePaymentType>(dto.PaymentType, true, out var paymentType))
                    return new ResponseDTO { IsValid = false, Message = "Invalid payment type" };

                if (!DateTime.TryParse(dto.InvoiceDate, out var invoiceDate))
                    return new ResponseDTO { IsValid = false, Message = "Invalid invoice date" };

                DateTime? dueDate = null;
                if (!string.IsNullOrWhiteSpace(dto.DueDate))
                {
                    if (!DateTime.TryParse(dto.DueDate, out var parsedDue))
                        return new ResponseDTO { IsValid = false, Message = "Invalid due date" };
                    dueDate = parsedDue;
                }

                if (invoiceType == Enums.InvoiceType.Purchase && dto.SupplierId == null)
                    return new ResponseDTO { IsValid = false, Message = "Supplier is required for purchase invoices" };

                if (dto.SupplierId.HasValue)
                {
                    var supplier = await _supplierRepo.GetByIdAsync(dto.SupplierId.Value);
                    if (supplier == null)
                        return new ResponseDTO { IsValid = false, Message = "Supplier not found" };
                }

                var items = BuildItems(dto.Items, userId);
                var totals = CalculateTotals(items, dto.Discount, dto.Tax, dto.PaidAmount);

                var schedules = BuildSchedules(dto.PaymentSchedules, userId, dueDate);

                var invoice = new Invoice
                {
                    InvoiceNumber = dto.InvoiceNumber,
                    InvoiceDate = invoiceDate,
                    DueDate = dueDate,
                    Type = invoiceType,
                    PaymentType = paymentType,
                    PaymentMethod = dto.PaymentMethod,
                    Discount = dto.Discount,
                    Tax = dto.Tax,
                    SubTotal = totals.SubTotal,
                    Total = totals.Total,
                    PaidAmount = dto.PaidAmount,
                    Currency = dto.Currency,
                    Notes = dto.Notes,
                    SupplierId = dto.SupplierId,
                    ClientId = dto.ClientId,
                    ProjectId = dto.ProjectId,
                    Status = totals.Status,
                    Items = items,
                    PaymentSchedules = schedules
                };

                await _repo.CreateAsync(invoice, userId);

                if (dto.Attachments?.Any() == true)
                {
                    var attachments = await SaveAttachments(dto.Attachments, "Invoices", userId);
                    await _repo.AddAttachmentsAsync(invoice.Id, attachments, userId);
                }

                await ApplyInventoryImpact(invoice, userId);
                await UpdateSupplierBalance(invoice, userId, isCreation: true);

                return new ResponseDTO { IsValid = true, Data = invoice.Id, Message = "Invoice created" };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> GetAllAsync()
        {
            const string fn = nameof(GetAllAsync);
            try
            {
                var invoices = await _repo.GetAllWithDetailsAsync();
                var dtos = invoices.Select(MapInvoice).ToList();
                return new ResponseDTO { IsValid = true, Data = dtos };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> GetAsync(int id)
        {
            const string fn = nameof(GetAsync);
            try
            {
                var invoice = await _repo.GetWithDetailsAsync(id);
                if (invoice == null)
                    return new ResponseDTO { IsValid = false, Message = "Invoice not found" };

                return new ResponseDTO { IsValid = true, Data = MapInvoice(invoice) };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> FilterAsync(InvoiceFilterDTO dto)
        {
            const string fn = nameof(FilterAsync);
            try
            {
                Enums.InvoiceType? type = null;
                if (!string.IsNullOrWhiteSpace(dto.Type))
                {
                    if (!Enum.TryParse(dto.Type, true, out Enums.InvoiceType parsedType))
                        return new ResponseDTO { IsValid = false, Message = "Invalid invoice type" };
                    type = parsedType;
                }

                Enums.InvoicePaymentType? paymentType = null;
                if (!string.IsNullOrWhiteSpace(dto.PaymentType))
                {
                    if (!Enum.TryParse(dto.PaymentType, true, out Enums.InvoicePaymentType parsedPayment))
                        return new ResponseDTO { IsValid = false, Message = "Invalid payment type" };
                    paymentType = parsedPayment;
                }

                Enums.InvoiceStatus? status = null;
                if (!string.IsNullOrWhiteSpace(dto.Status))
                {
                    if (!Enum.TryParse(dto.Status, true, out Enums.InvoiceStatus parsedStatus))
                        return new ResponseDTO { IsValid = false, Message = "Invalid invoice status" };
                    status = parsedStatus;
                }

                DateTime? from = null;
                if (!string.IsNullOrWhiteSpace(dto.FromDate))
                {
                    if (!DateTime.TryParse(dto.FromDate, out var parsedFrom))
                        return new ResponseDTO { IsValid = false, Message = "Invalid from date" };
                    from = parsedFrom;
                }

                DateTime? to = null;
                if (!string.IsNullOrWhiteSpace(dto.ToDate))
                {
                    if (!DateTime.TryParse(dto.ToDate, out var parsedTo))
                        return new ResponseDTO { IsValid = false, Message = "Invalid to date" };
                    to = parsedTo;
                }

                var invoices = await _repo.GetFilteredAsync(type, paymentType, status, dto.SupplierId, dto.ClientId, dto.ProjectId, from, to);
                var dtos = invoices.Select(MapInvoice).ToList();
                return new ResponseDTO { IsValid = true, Data = dtos };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> UpdateAsync(UpdateInvoiceDTO dto, int userId)
        {
            const string fn = nameof(UpdateAsync);
            try
            {
                var existing = await _repo.GetWithDetailsAsync(dto.Id);
                if (existing == null)
                    return new ResponseDTO { IsValid = false, Message = "Invoice not found" };

                if (!Enum.TryParse<Enums.InvoiceType>(dto.Type, true, out var invoiceType))
                    return new ResponseDTO { IsValid = false, Message = "Invalid invoice type" };
                if (!Enum.TryParse<Enums.InvoicePaymentType>(dto.PaymentType, true, out var paymentType))
                    return new ResponseDTO { IsValid = false, Message = "Invalid payment type" };

                if (!DateTime.TryParse(dto.InvoiceDate, out var invoiceDate))
                    return new ResponseDTO { IsValid = false, Message = "Invalid invoice date" };

                DateTime? dueDate = null;
                if (!string.IsNullOrWhiteSpace(dto.DueDate))
                {
                    if (!DateTime.TryParse(dto.DueDate, out var parsedDue))
                        return new ResponseDTO { IsValid = false, Message = "Invalid due date" };
                    dueDate = parsedDue;
                }

                var previousItems = existing.Items.ToList();
                var previousType = existing.Type;
                var previousSupplierId = existing.SupplierId;
                var previousTotal = existing.Total;
                var previousSchedules = existing.PaymentSchedules.ToList();

                var items = BuildItems(dto.Items, userId);
                var totals = CalculateTotals(items, dto.Discount, dto.Tax, dto.PaidAmount);
                var schedules = BuildSchedules(dto.PaymentSchedules, userId, dueDate);

                existing.InvoiceNumber = dto.InvoiceNumber;
                existing.InvoiceDate = invoiceDate;
                existing.DueDate = dueDate;
                existing.Type = invoiceType;
                existing.PaymentType = paymentType;
                existing.PaymentMethod = dto.PaymentMethod;
                existing.Discount = dto.Discount;
                existing.Tax = dto.Tax;
                existing.SubTotal = totals.SubTotal;
                existing.Total = totals.Total;
                existing.PaidAmount = dto.PaidAmount;
                existing.Currency = dto.Currency;
                existing.Notes = dto.Notes;
                existing.SupplierId = dto.SupplierId;
                existing.ClientId = dto.ClientId;
                existing.ProjectId = dto.ProjectId;
                existing.Status = totals.Status;

                await _repo.ReplaceItemsAsync(existing.Id, items, userId);
                if (schedules.Any())
                    await _repo.ReplaceSchedulesAsync(existing.Id, schedules, userId);
                else if (previousSchedules.Any())
                    await _repo.ReplaceSchedulesAsync(existing.Id, new List<InvoicePaymentSchedule>(), userId);
                await _repo.UpdateAsync(existing, userId);

                if (dto.Attachments?.Any() == true)
                {
                    var attachments = await SaveAttachments(dto.Attachments, "Invoices", userId);
                    await _repo.AddAttachmentsAsync(existing.Id, attachments, userId);
                }

                await ApplyInventoryImpact(existing, userId, isUpdate: true, previousItems: previousItems, previousType: previousType);
                await UpdateSupplierBalance(existing, userId, isCreation: false, previousTotal: previousTotal, previousSupplierId: previousSupplierId, previousType: previousType);

                return new ResponseDTO { IsValid = true, Message = "Invoice updated" };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> GetDueRemindersAsync(InvoiceReminderFilterDTO dto)
        {
            const string fn = nameof(GetDueRemindersAsync);
            try
            {
                var reminders = await BuildDueRemindersAsync(dto);
                return new ResponseDTO { IsValid = true, Data = reminders };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> NotifyDueRemindersAsync(InvoiceReminderFilterDTO dto)
        {
            const string fn = nameof(NotifyDueRemindersAsync);
            try
            {
                var reminders = await BuildDueRemindersAsync(dto);
                await _notifications.BroadcastDueRemindersAsync(reminders);
                return new ResponseDTO { IsValid = true, Data = reminders, Message = "Reminders broadcasted" };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> ExportAsync(InvoiceExportRequestDTO dto)
        {
            const string fn = nameof(ExportAsync);
            try
            {
                var filter = dto.Filter ?? new InvoiceFilterDTO();
                var filtered = await FilterAsync(filter);
                if (!filtered.IsValid)
                    return filtered;

                var invoices = filtered.Data as List<InvoiceDTO> ?? new();
                if (!invoices.Any())
                    return new ResponseDTO { IsValid = false, Message = "No invoices found for the provided filter" };

                var path = await _reportExporter.ExportInvoicesAsync(invoices, dto.Format, dto.Branding);
                return new ResponseDTO { IsValid = true, Data = path, Message = "Export created" };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        private async Task<List<InvoiceDueReminderDTO>> BuildDueRemindersAsync(InvoiceReminderFilterDTO dto)
        {
            var invoices = await _repo.GetAllWithDetailsAsync();
            var today = DateTime.UtcNow.Date;
            var horizon = dto.DaysAhead.HasValue ? today.AddDays(dto.DaysAhead.Value) : (DateTime?)null;

            var reminders = new List<InvoiceDueReminderDTO>();

            foreach (var inv in invoices)
            {
                if (inv.Total <= inv.PaidAmount)
                    continue;

                var nextSchedule = inv.PaymentSchedules
                    .Where(s => !s.IsPaid)
                    .OrderBy(s => s.DueDate)
                    .FirstOrDefault();

                DateTime? targetDate = null;
                decimal? targetAmount = null;
                var dueSource = "InvoiceDueDate";

                if (nextSchedule != null)
                {
                    targetDate = nextSchedule.DueDate.Date;
                    targetAmount = nextSchedule.Amount;
                    dueSource = "Schedule";
                }
                else if (inv.DueDate.HasValue)
                {
                    targetDate = inv.DueDate.Value.Date;
                    targetAmount = inv.Total - inv.PaidAmount;
                }

                if (targetDate == null)
                    continue;

                var daysUntil = (int)(targetDate.Value - today).TotalDays;
                var isOverdue = targetDate.Value < today;

                if (!dto.IncludeOverdue && isOverdue)
                    continue;
                if (horizon.HasValue && targetDate.Value > horizon.Value)
                    continue;

                reminders.Add(new InvoiceDueReminderDTO
                {
                    InvoiceId = inv.Id,
                    InvoiceNumber = inv.InvoiceNumber,
                    DueDate = inv.DueDate?.ToString("yyyy-MM-dd"),
                    Total = inv.Total,
                    PaidAmount = inv.PaidAmount,
                    DaysUntilDue = daysUntil,
                    IsOverdue = isOverdue,
                    NextDueDate = targetDate?.ToString("yyyy-MM-dd"),
                    NextDueAmount = targetAmount,
                    NextDueIsOverdue = isOverdue,
                    DueSource = dueSource,
                    SupplierId = inv.SupplierId,
                    ClientId = inv.ClientId,
                    ProjectId = inv.ProjectId,
                    Status = inv.Status.ToString()
                });
            }

            return reminders
                .OrderBy(r => r.IsOverdue ? -1 : r.DaysUntilDue)
                .ThenBy(r => r.NextDueDate)
                .ToList();
        }

        private async Task ApplyInventoryImpact(Invoice invoice, int userId, bool isUpdate = false, List<InvoiceItem>? previousItems = null, Enums.InvoiceType? previousType = null)
        {
            if (isUpdate && previousItems != null && previousType.HasValue)
            {
                var reverseDirection = previousType == Enums.InvoiceType.Purchase ? -1 : 1;
                foreach (var oldItem in previousItems)
                {
                    if (oldItem.ProductId.HasValue)
                    {
                        var delta = (int)Math.Round(oldItem.Quantity, MidpointRounding.AwayFromZero) * reverseDirection;
                        await _inventoryRepo.AdjustStockAsync(oldItem.ProductId.Value, delta, userId);
                    }
                }
            }

            if (!invoice.Items.Any())
                return;

            var direction = invoice.Type == Enums.InvoiceType.Purchase ? 1 : -1;

            foreach (var item in invoice.Items)
            {
                if (item.ProductId.HasValue)
                {
                    var delta = (int)Math.Round(item.Quantity, MidpointRounding.AwayFromZero) * direction;
                    await _inventoryRepo.AdjustStockAsync(item.ProductId.Value, delta, userId);
                }
            }
        }

        private (decimal SubTotal, decimal Total, Enums.InvoiceStatus Status) CalculateTotals(IEnumerable<InvoiceItem> items, decimal discount, decimal tax, decimal paid)
        {
            var sub = items.Sum(i => i.Total);
            var total = Math.Round(sub - discount + tax, 2);
            var status = paid >= total ? Enums.InvoiceStatus.Paid
                        : paid > 0 ? Enums.InvoiceStatus.PartiallyPaid
                        : Enums.InvoiceStatus.Unpaid;
            return (sub, total, status);
        }

        private List<InvoiceItem> BuildItems(List<InvoiceItemInputDTO> items, int userId)
        {
            return items.Select(it => new InvoiceItem
            {
                ProductId = it.ProductId,
                Description = it.Description,
                Quantity = it.Quantity,
                UnitPrice = it.UnitPrice,
                Discount = it.Discount,
                Total = Math.Round((it.Quantity * it.UnitPrice) - it.Discount, 2),
                CreatedBy = userId,
                UpdatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToList();
        }

        private List<InvoicePaymentSchedule> BuildSchedules(List<InvoicePaymentScheduleInputDTO> schedules, int userId, DateTime? defaultDue)
        {
            var list = new List<InvoicePaymentSchedule>();
            if (schedules != null && schedules.Count > 0)
            {
                foreach (var schedule in schedules)
                {
                    if (!DateTime.TryParse(schedule.DueDate, out var due))
                        continue;

                    list.Add(new InvoicePaymentSchedule
                    {
                        DueDate = due.Date,
                        Amount = schedule.Amount,
                        IsPaid = schedule.IsPaid,
                        PaidDate = schedule.IsPaid ? DateTime.UtcNow : null,
                        Notes = schedule.Notes,
                        CreatedBy = userId,
                        UpdatedBy = userId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            return list;
        }

        private InvoiceDTO MapInvoice(Invoice invoice)
        {
            return new InvoiceDTO
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate.ToString("yyyy-MM-dd"),
                DueDate = invoice.DueDate?.ToString("yyyy-MM-dd"),
                Type = invoice.Type,
                Status = invoice.Status,
                PaymentType = invoice.PaymentType,
                PaymentMethod = invoice.PaymentMethod,
                SubTotal = invoice.SubTotal,
                Discount = invoice.Discount,
                Tax = invoice.Tax,
                Total = invoice.Total,
                PaidAmount = invoice.PaidAmount,
                Currency = invoice.Currency,
                Notes = invoice.Notes,
                SupplierId = invoice.SupplierId,
                ClientId = invoice.ClientId,
                ProjectId = invoice.ProjectId,
                Items = invoice.Items.Select(it => new InvoiceItemDTO
                {
                    Id = it.Id,
                    ProductId = it.ProductId,
                    Description = it.Description,
                    Quantity = it.Quantity,
                    UnitPrice = it.UnitPrice,
                    Discount = it.Discount,
                    Total = it.Total
                }).ToList(),
                Attachments = invoice.Attachments.Select(att => new InvoiceAttachmentDTO
                {
                    Id = att.Id,
                    FileName = att.FileName,
                    FilePath = att.FilePath
                }).ToList(),
                PaymentSchedules = invoice.PaymentSchedules.Select(ps => new InvoicePaymentScheduleDTO
                {
                    Id = ps.Id,
                    Amount = ps.Amount,
                    DueDate = ps.DueDate.ToString("yyyy-MM-dd"),
                    IsPaid = ps.IsPaid,
                    Notes = ps.Notes
                }).OrderBy(p => p.DueDate).ToList()
            };
        }

        private async Task<List<InvoiceAttachment>> SaveAttachments(IFormFileCollection attachments, string folder, int userId)
        {
            var list = new List<InvoiceAttachment>();
            foreach (var file in attachments)
            {
                var saved = await _fileStorage.SaveFileAsync(file, folder);
                if (saved != null)
                {
                    list.Add(new InvoiceAttachment
                    {
                        FileName = saved.FileName,
                        FilePath = saved.RelativePath,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        CreatedBy = userId,
                        UpdatedBy = userId
                    });
                }
            }
            return list;
        }

        private async Task UpdateSupplierBalance(Invoice invoice, int userId, bool isCreation, decimal previousTotal = 0m, int? previousSupplierId = null, Enums.InvoiceType? previousType = null)
        {
            if (invoice.SupplierId == null)
                return;

            var supplier = await _supplierRepo.GetByIdAsync(invoice.SupplierId.Value);
            if (supplier == null)
                return;

            if (!isCreation && previousSupplierId.HasValue && previousType.HasValue)
            {
                var oldSupplier = previousSupplierId == invoice.SupplierId ? supplier : await _supplierRepo.GetByIdAsync(previousSupplierId.Value);
                if (oldSupplier != null)
                {
                    var previousImpact = previousType == Enums.InvoiceType.Purchase ? previousTotal : -previousTotal;
                    oldSupplier.CurrentBalance -= previousImpact;
                    await _supplierRepo.UpdateAsync(oldSupplier, userId);
                }
            }

            var impact = invoice.Type == Enums.InvoiceType.Purchase ? invoice.Total : -invoice.Total;
            supplier.CurrentBalance += impact;
            await _supplierRepo.UpdateAsync(supplier, userId);
        }
    }
}
