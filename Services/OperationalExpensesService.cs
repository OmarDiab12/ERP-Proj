using ERP.Helpers; // for IFileStorageService
using ERP.DTOs.OperationalExpenses;
using ERP.Models;

namespace ERP.Services
{
    public class OperationalExpensesService : IOperationalExpensesService
    {
        private readonly IOperationalExpensesRepository _repo;
        private readonly IErrorRepository _errors;
        private readonly IFileStorageService _fileStorage;

        public OperationalExpensesService(
            IOperationalExpensesRepository repo,
            IErrorRepository errors,
            IFileStorageService fileStorage)
        {
            _repo = repo;
            _errors = errors;
            _fileStorage = fileStorage;
        }

        public async Task<ResponseDTO> GetAllAsync(int page = 1, int pageSize = 50)
        {
            const string fn = nameof(GetAllAsync);
            try
            {
                var list = await _repo.GetPagedAsync(page, pageSize);
                var dtos = list.Select(e => new OperationalExpenseDTO { Id = e.Id, Description = e.Description, Amount = e.Amount, ExpenseDate = e.ExpenseDate.ToString("yyyy-MM-dd"), Category = e.Category }).ToList();
                return new ResponseDTO { IsValid = true, Data = dtos };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }


        public async Task<ResponseDTO> GetByDateRangeAsync(ExpenseRangeRequestDTO req)
        {
            const string fn = nameof(GetByDateRangeAsync); try
            {
                if (!DateTime.TryParse(req.DateFrom, out var from))
                    return new ResponseDTO { IsValid = false, Message = "Invalid DateFrom" };
                if (!DateTime.TryParse(req.DateTo, out var to))
                    return new ResponseDTO { IsValid = false, Message = "Invalid DateTo" };
                // اجعل النهاية شاملة اليوم كله لو حابب: // 
                to = to.Date.AddDays(1).AddTicks(-1);
                var list = await _repo.GetByDateRangeAsync(from, to, req.Category);
                var total = await _repo.GetTotalByDateRangeAsync(from, to, req.Category);
                var dtos = list.Select(e => new OperationalExpenseDTO
                { Id = e.Id, Description = e.Description, Amount = e.Amount, ExpenseDate = e.ExpenseDate.ToString("yyyy-MM-dd"), Category = e.Category }).ToList();
                return new ResponseDTO { IsValid = true, Data = new { total, items = dtos } };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }
        // ==========================================================
        // CREATE
        // ==========================================================
        public async Task<ResponseDTO> CreateAsync(CreateOperationalExpenseDTO dto, int userId)
        {
            const string fn = nameof(CreateAsync);
            try
            {
                if (!DateTime.TryParse(dto.ExpenseDate, out var date))
                    return new ResponseDTO { IsValid = false, Message = "Invalid ExpenseDate" };

                var entity = new OperationalExpense
                {
                    Description = dto.Description ?? string.Empty,
                    Amount = Math.Round(dto.Amount, 2),
                    ExpenseDate = date,
                    Category = dto.Category ?? string.Empty
                };

                // ✅ Handle file upload
                if (dto.File != null)
                {
                    var result = await _fileStorage.SaveFileAsync(dto.File, "OperationalExpenses");
                    if (result != null)
                    {
                        entity.fileName = result.FileName;
                        entity.filePath = result.FilePath;
                    }
                }

                await _repo.CreateAsync(entity, userId);

                return new ResponseDTO
                {
                    IsValid = true,
                    Data = entity.Id,
                    Message = "Expense created successfully."
                };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        // ==========================================================
        // EDIT
        // ==========================================================
        public async Task<ResponseDTO> EditAsync(EditOperationalExpenseDTO dto, int userId)
        {
            const string fn = nameof(EditAsync);
            try
            {
                var entity = await _repo.GetByIdAsync(dto.Id);
                if (entity == null)
                    return new ResponseDTO { IsValid = false, Message = "Expense not found" };

                if (!DateTime.TryParse(dto.ExpenseDate, out var date))
                    return new ResponseDTO { IsValid = false, Message = "Invalid ExpenseDate" };

                entity.Description = dto.Description ?? string.Empty;
                entity.Amount = Math.Round(dto.Amount, 2);
                entity.ExpenseDate = date;
                entity.Category = dto.Category ?? string.Empty;

                // ✅ Replace file if new one uploaded
                if (dto.File != null)
                {
                    // delete old file if exists
                    if (!string.IsNullOrWhiteSpace(entity.filePath))
                        await _fileStorage.DeleteFileAsync(entity.filePath);

                    var result = await _fileStorage.SaveFileAsync(dto.File, "OperationalExpenses");
                    if (result != null)
                    {
                        entity.fileName = result.FileName;
                        entity.filePath = result.FilePath;
                    }
                }

                await _repo.UpdateAsync(entity, userId);

                return new ResponseDTO { IsValid = true, Message = "Expense updated successfully." };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        // ==========================================================
        // DELETE
        // ==========================================================
        public async Task<ResponseDTO> DeleteAsync(int id, int userId)
        {
            const string fn = nameof(DeleteAsync);
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity == null)
                    return new ResponseDTO { IsValid = false, Message = "Expense not found" };

                // ✅ Delete associated file
                if (!string.IsNullOrWhiteSpace(entity.filePath))
                    await _fileStorage.DeleteFileAsync(entity.filePath);

                await _repo.SoftDeleteAsync(id, userId);

                return new ResponseDTO { IsValid = true, Message = "Expense deleted successfully." };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        // ==========================================================
        // GET
        // ==========================================================
        public async Task<ResponseDTO> GetAsync(int id)
        {
            const string fn = nameof(GetAsync);
            try
            {
                var e = await _repo.GetByIdAsync(id);
                if (e == null)
                    return new ResponseDTO { IsValid = false, Message = "Expense not found" };

                var dto = new OperationalExpenseDTO
                {
                    Id = e.Id,
                    Description = e.Description,
                    Amount = e.Amount,
                    ExpenseDate = e.ExpenseDate.ToString("yyyy-MM-dd"),
                    Category = e.Category,
                    fileName = e.fileName,
                    filepath = e.filePath
                };

                return new ResponseDTO { IsValid = true, Data = dto };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }
    }
}
