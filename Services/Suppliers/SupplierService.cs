using ERP.DTOs.Suppliers;
using ERP.Models.SuppliersManagement;
using ERP.Repositories.Interfaces;
using ERP.Repositories.Interfaces.Invoices;
using ERP.Repositories.Interfaces.Suppliers;
using ERP.Services.Interfaces.Suppliers;

namespace ERP.Services.Suppliers
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _repo;
        private readonly IErrorRepository _errors;
        private readonly IInvoiceRepository _invoiceRepo;

        public SupplierService(ISupplierRepository repo, IErrorRepository errors, IInvoiceRepository invoiceRepo)
        {
            _repo = repo;
            _errors = errors;
            _invoiceRepo = invoiceRepo;
        }

        public async Task<ResponseDTO> CreateAsync(CreateSupplierDTO dto, int userId)
        {
            const string fn = nameof(CreateAsync);
            try
            {
                var supplier = new Supplier
                {
                    Name = dto.Name,
                    PhoneNumber = dto.PhoneNumber,
                    Email = dto.Email,
                    Address = dto.Address,
                    Notes = dto.Notes,
                    OpeningBalance = dto.OpeningBalance,
                    CurrentBalance = dto.OpeningBalance
                };

                await _repo.CreateAsync(supplier, userId);
                return new ResponseDTO { IsValid = true, Data = supplier.Id, Message = "Supplier created" };
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
                var list = await _repo.GetAllAsync();
                var dtos = list.Select(s => new SupplierDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    PhoneNumber = s.PhoneNumber,
                    Email = s.Email,
                    Address = s.Address,
                    Notes = s.Notes,
                    OpeningBalance = s.OpeningBalance,
                    CurrentBalance = s.CurrentBalance
                }).ToList();

                return new ResponseDTO { IsValid = true, Data = dtos };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> GetWithInvoicesAsync(int id)
        {
            const string fn = nameof(GetWithInvoicesAsync);
            try
            {
                var supplierRes = await GetAsync(id);
                if (!supplierRes.IsValid)
                    return supplierRes;

                if (supplierRes.Data is not SupplierDTO supplierDto)
                    return new ResponseDTO { IsValid = false, Message = "Supplier data unavailable" };

                var invoices = await _invoiceRepo.GetFilteredAsync(null, null, null, id, null, null, null, null);
                var invoiceDtos = invoices.Select(MapInvoice).ToList();
                var outstanding = invoices.Sum(inv => Math.Max(0, inv.Total - inv.PaidAmount));

                var dto = new SupplierWithInvoicesDTO
                {
                    Supplier = supplierDto,
                    Invoices = invoiceDtos,
                    OutstandingBalance = outstanding
                };

                return new ResponseDTO { IsValid = true, Data = dto };
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
                var supplier = await _repo.GetByIdAsync(id);
                if (supplier == null)
                    return new ResponseDTO { IsValid = false, Message = "Supplier not found" };

                var dto = new SupplierDTO
                {
                    Id = supplier.Id,
                    Name = supplier.Name,
                    PhoneNumber = supplier.PhoneNumber,
                    Email = supplier.Email,
                    Address = supplier.Address,
                    Notes = supplier.Notes,
                    OpeningBalance = supplier.OpeningBalance,
                    CurrentBalance = supplier.CurrentBalance
                };

                return new ResponseDTO { IsValid = true, Data = dto };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> UpdateAsync(UpdateSupplierDTO dto, int userId)
        {
            const string fn = nameof(UpdateAsync);
            try
            {
                var supplier = await _repo.GetByIdAsync(dto.Id);
                if (supplier == null)
                    return new ResponseDTO { IsValid = false, Message = "Supplier not found" };

                supplier.Name = dto.Name;
                supplier.PhoneNumber = dto.PhoneNumber;
                supplier.Email = dto.Email;
                supplier.Address = dto.Address;
                supplier.Notes = dto.Notes;
                if (supplier.OpeningBalance != dto.OpeningBalance)
                {
                    var diff = dto.OpeningBalance - supplier.OpeningBalance;
                    supplier.OpeningBalance = dto.OpeningBalance;
                    supplier.CurrentBalance += diff;
                }

                await _repo.UpdateAsync(supplier, userId);
                return new ResponseDTO { IsValid = true, Message = "Supplier updated" };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        private DTOs.Invoices.InvoiceDTO MapInvoice(ERP.Models.InvoicesManagement.Invoice invoice)
        {
            return new DTOs.Invoices.InvoiceDTO
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
                Items = invoice.Items.Select(it => new DTOs.Invoices.InvoiceItemDTO
                {
                    Id = it.Id,
                    ProductId = it.ProductId,
                    Description = it.Description,
                    Quantity = it.Quantity,
                    UnitPrice = it.UnitPrice,
                    Discount = it.Discount,
                    Total = it.Total
                }).ToList(),
                Attachments = invoice.Attachments.Select(att => new DTOs.Invoices.InvoiceAttachmentDTO
                {
                    Id = att.Id,
                    FileName = att.FileName,
                    FilePath = att.FilePath
                }).ToList(),
                PaymentSchedules = invoice.PaymentSchedules
                    .OrderBy(ps => ps.DueDate)
                    .Select(ps => new DTOs.Invoices.InvoicePaymentScheduleDTO
                    {
                        Id = ps.Id,
                        Amount = ps.Amount,
                        DueDate = ps.DueDate.ToString("yyyy-MM-dd"),
                        IsPaid = ps.IsPaid,
                        Notes = ps.Notes
                    }).ToList()
            };
        }
    }
}
