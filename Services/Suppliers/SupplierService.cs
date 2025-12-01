using ERP.DTOs.Suppliers;
using ERP.Models.SuppliersManagement;
using ERP.Repositories.Interfaces;
using ERP.Repositories.Interfaces.Suppliers;
using ERP.Services.Interfaces.Suppliers;

namespace ERP.Services.Suppliers
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _repo;
        private readonly IErrorRepository _errors;

        public SupplierService(ISupplierRepository repo, IErrorRepository errors)
        {
            _repo = repo;
            _errors = errors;
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
                supplier.OpeningBalance = dto.OpeningBalance;

                await _repo.UpdateAsync(supplier, userId);
                return new ResponseDTO { IsValid = true, Message = "Supplier updated" };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }
    }
}
