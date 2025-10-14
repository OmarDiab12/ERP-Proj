using ERP.DTOs.Qiotation;
using ERP.Models.QoutationManagement;

namespace ERP.Services
{
    public class QuotationService : IQuotationService
    {
        private readonly IQuotationRepository _quotationRepo;
        private readonly IQuotationItemRepository _itemRepo;
        private readonly IErrorRepository _errors;

        public QuotationService(
            IQuotationRepository quotationRepo,
            IQuotationItemRepository itemRepo,
            IErrorRepository errors)
        {
            _quotationRepo = quotationRepo;
            _itemRepo = itemRepo;
            _errors = errors;
        }

        // Helper: compute final price applying discount semantics
        private static decimal ComputeFinalPrice(decimal unitPrice, decimal quantity, decimal discount)
        {
            // discount may be fraction (0..1) or percent (0..100)
            decimal discountFactor;
            if (discount <= 1m) // treat as fraction
                discountFactor = 1m - discount;
            else // treat as percent
                discountFactor = 1m - (discount / 100m);

            if (discountFactor < 0) discountFactor = 0m;
            var raw = unitPrice * quantity * discountFactor;
            return Math.Round(raw, 2);
        }

        public async Task<ResponseDTO> CreateAsync(CreateQuotationDTO dto, int userId)
        {
            const string fn = nameof(CreateAsync);
            try
            {
                if (!DateTime.TryParse(dto.QuotationDate, out var qDate))
                    return new ResponseDTO { IsValid = false, Message = "Invalid QuotationDate" };

                var entity = new Quotation
                {
                    ClientId = dto.ClientId,
                    QuotationDate = qDate,
                    Status = Enum.TryParse<QuotationStatus>(dto.Status, true, out var st) ? st : QuotationStatus.Draft,
                    GeneralNotes = dto.GeneralNotes ?? string.Empty,
                    TotalAmount = 0m
                };

                // create quotation first
                await _quotationRepo.CreateAsync(entity , userId);

                decimal total = 0m;
                foreach (var it in dto.Items)
                {
                    var final = ComputeFinalPrice(it.UnitPrice, it.Quantity, it.DiscountPercentage);
                    total += final;

                    var item = new QuotationItem
                    {
                        QuotationId = entity.Id,
                        Description = it.Description,
                        UnitPrice = it.UnitPrice,
                        Quantity = it.Quantity,
                        DiscountPercentage = it.DiscountPercentage,
                        ItemNotes = it.ItemNotes ?? string.Empty
                        // FinalPrice is computed property
                    };

                    await _itemRepo.CreateAsync(item, userId);
                }

                
                // update total
                entity.TotalAmount = Math.Round(total, 2);
                await _quotationRepo.UpdateAsync(entity, userId);

                return new ResponseDTO { IsValid = true, Data = entity.Id, Message = "Quotation created" };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error while creating quotation" };
            }
        }

        public async Task<ResponseDTO> EditAsync(EditQuotationDTO dto, int userId)
        {
            const string fn = nameof(EditAsync);
            try
            {
                var entity = await _quotationRepo.GetByIdAsync(dto.Id);
                if (entity == null)
                    return new ResponseDTO { IsValid = false, Message = "Quotation not found" };

                if (!DateTime.TryParse(dto.QuotationDate, out var qDate))
                    return new ResponseDTO { IsValid = false, Message = "Invalid QuotationDate" };

                entity.ClientId = dto.ClientId;
                entity.QuotationDate = qDate;
                entity.Status = Enum.TryParse<QuotationStatus>(dto.Status, true, out var st) ? st : entity.Status;
                entity.GeneralNotes = dto.GeneralNotes ?? string.Empty;

                // Remove existing items and re-add (simple approach)
                await _itemRepo.DeleteByQuotationIdAsync(entity.Id);

                decimal total = 0m;
                foreach (var it in dto.Items)
                {
                    var final = ComputeFinalPrice(it.UnitPrice, it.Quantity, it.DiscountPercentage);
                    total += final;

                    var item = new QuotationItem
                    {
                        QuotationId = entity.Id,
                        Description = it.Description,
                        UnitPrice = it.UnitPrice,
                        Quantity = it.Quantity,
                        DiscountPercentage = it.DiscountPercentage,
                        ItemNotes = it.ItemNotes ?? string.Empty
                    };

                    await _itemRepo.CreateAsync(item , userId);
                }

                // update total
                entity.TotalAmount = Math.Round(total, 2);
                await _quotationRepo.UpdateAsync(entity , userId);


                return new ResponseDTO { IsValid = true, Message = "Quotation updated" };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error while updating quotation" };
            }
        }

        public async Task<ResponseDTO> DeleteAsync(int id, int userId)
        {
            const string fn = nameof(DeleteAsync);
            try
            {
                var q = await _quotationRepo.GetByIdAsync(id);
                if (q == null) return new ResponseDTO { IsValid = false, Message = "Quotation not found" };

                // delete items first
                await _itemRepo.DeleteByQuotationIdAsync(id);

                await _quotationRepo.SoftDeleteAsync(id , userId);

                return new ResponseDTO { IsValid = true, Message = "Quotation deleted" };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error while deleting quotation" };
            }
        }

        public async Task<ResponseDTO> GetAsync(int id)
        {
            const string fn = nameof(GetAsync);
            try
            {
                var q = await _quotationRepo.GetByIdAsync(id);
                if (q == null) return new ResponseDTO { IsValid = false, Message = "Quotation not found" };

                // load items
                var items = await _itemRepo.GetByQuotationIdAsync(q.Id);

                var dto = new QuotationDTO
                {
                    Id = q.Id,
                    ClientId = q.ClientId,
                    QuotationDate = q.QuotationDate.ToString("yyyy-MM-dd"),
                    Status = q.Status.ToString(),
                    TotalAmount = q.TotalAmount,
                    GeneralNotes = q.GeneralNotes
                };

                dto.Items = items.Select(it => new QuotationItemDTO
                {
                    Id = it.Id,
                    Description = it.Description,
                    UnitPrice = it.UnitPrice,
                    Quantity = it.Quantity,
                    DiscountPercentage = it.DiscountPercentage,
                    FinalPrice = ComputeFinalPrice(it.UnitPrice, it.Quantity, it.DiscountPercentage),
                    ItemNotes = it.ItemNotes
                }).ToList();

                return new ResponseDTO { IsValid = true, Data = dto };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error while fetching quotation" };
            }
        }

        public async Task<ResponseDTO> GetAllAsync()
        {
            const string fn = nameof(GetAllAsync);
            try
            {
                var list = await _quotationRepo.GetAllAsync();

                var dtos = list.Select(q => new QuotationBasicDTO
                {
                    Id = q.Id,
                    ClientId = q.ClientId,
                    QuotationDate = q.QuotationDate.ToString("yyyy-MM-dd"),
                    Status = q.Status.ToString(),
                    TotalAmount = q.TotalAmount
                }).ToList();

                return new ResponseDTO { IsValid = true, Data = dtos };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error while fetching quotations" };
            }
        }
    }
}
