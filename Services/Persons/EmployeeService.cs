using ERP.DTOs.Employee;
using ERP.Models.Employees;
using ERP.Repositories.Interfaces.Persons;
using ERP.Services.Interfaces.Persons;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Persons
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IErrorRepository _errorRepo;

        public EmployeeService(IEmployeeRepository employeeRepo, IErrorRepository errorRepo)
        {
            _employeeRepo = employeeRepo;
            _errorRepo = errorRepo;
        }

        // Employees
        public async Task<ResponseDTO> CreateEmployeeAsync(CreateEmployeeDTO dto, int userId)
        {
            const string fn = nameof(CreateEmployeeAsync);
            try
            {
                var emp = new Employee
                {
                    Name = dto.Name,
                    JobTitle = dto.JobTitle,
                    PhoneNumber = dto.PhoneNumber,
                    Email = dto.Email,
                    BaseSalary = dto.BaseSalary
                };
                await _employeeRepo.CreateAsync(emp , userId);
                return new ResponseDTO { IsValid = true, Data = emp.Id, Message = "Employee created" };
            }
            catch (Exception ex)
            {
                await _errorRepo.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> EditEmployeeAsync(EditEmployeeDTO dto, int userId)
        {
            const string fn = nameof(EditEmployeeAsync);
            try
            {
                var emp = await _employeeRepo.GetByIdAsync(dto.Id);
                if (emp == null) return new ResponseDTO { IsValid = false, Message = "Employee not found" };

                emp.Name = dto.Name;
                emp.JobTitle = dto.JobTitle;
                emp.PhoneNumber = dto.PhoneNumber;
                emp.Email = dto.Email;
                emp.BaseSalary = dto.BaseSalary;

                await _employeeRepo.UpdateAsync(emp,userId);

                return new ResponseDTO { IsValid = true, Message = "Employee updated" };
            }
            catch (Exception ex)
            {
                await _errorRepo.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> DeleteEmployeeAsync(int employeeId, int userId)
        {
            const string fn = nameof(DeleteEmployeeAsync);
            try
            {
                var emp = await _employeeRepo.GetByIdAsync(employeeId);
                if (emp == null) return new ResponseDTO { IsValid = false, Message = "Employee not found" };

                await _employeeRepo.SoftDeleteAsync(employeeId , userId);

                return new ResponseDTO { IsValid = true, Message = "Employee deleted" };
            }
            catch (Exception ex)
            {
                await _errorRepo.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> GetEmployeeAsync(int employeeId)
        {
            const string fn = nameof(GetEmployeeAsync);
            try
            {
                var emp = await _employeeRepo.Query() 
                    .Include(e => e.EmployeeTransactions)
                    .FirstOrDefaultAsync(e => e.Id == employeeId);

                if (emp == null) return new ResponseDTO { IsValid = false, Message = "Employee not found" };

                var txDtos = emp.EmployeeTransactions
                    .OrderByDescending(t => t.TransactionDate)
                    .Select(t => new EmployeeTransactionDTO
                    {
                        Id = t.Id,
                        TransactionDate = t.TransactionDate.ToString("yyyy-MM-dd"),
                        Description = t.Description,
                        TransactionType = t.TransactionType.ToString(),
                        Amount = t.Amount,
                        EmployeeId = t.EmployeeId,
                    }).ToList();

                var balance = emp.EmployeeTransactions.Sum(t => t.Amount);

                var dto = new EmployeeDTO
                {
                    Id = emp.Id,
                    Name = emp.Name,
                    JobTitle = emp.JobTitle,
                    PhoneNumber = emp.PhoneNumber,
                    Email = emp.Email,
                    BaseSalary = emp.BaseSalary,
                    Balance = balance,
                    Transactions = txDtos
                };

                return new ResponseDTO { IsValid = true, Data = dto };
            }
            catch (Exception ex)
            {
                await _errorRepo.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> GetAllEmployeesAsync()
        {
            const string fn = nameof(GetAllEmployeesAsync);
            try
            {
                var employees = await _employeeRepo.Query()
                    .Include(e => e.EmployeeTransactions)
                    .OrderBy(e => e.Name)
                    .ToListAsync();

                var list = employees.Select(emp =>
                {
                    var balance = emp.EmployeeTransactions.Sum(t => t.Amount);
                    return new EmployeeDTO
                    {
                        Id = emp.Id,
                        Name = emp.Name,
                        JobTitle = emp.JobTitle,
                        PhoneNumber = emp.PhoneNumber,
                        Email = emp.Email,
                        BaseSalary = emp.BaseSalary,
                        Balance = balance,
                        Transactions = emp.EmployeeTransactions
                            .OrderByDescending(t => t.TransactionDate)
                            .Select(t => new EmployeeTransactionDTO
                            {
                                Id = t.Id,
                                TransactionDate = t.TransactionDate.ToString("yyyy-MM-dd"),
                                Description = t.Description,
                                TransactionType = t.TransactionType.ToString(),
                                Amount = t.Amount,
                                EmployeeId = t.EmployeeId,
                            }).ToList()
                    };
                }).ToList();

                return new ResponseDTO { IsValid = true, Data = list };
            }
            catch (Exception ex)
            {
                await _errorRepo.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        
        // Transactions
        public async Task<ResponseDTO> AddTransactionAsync(CreateEmployeeTransactionDTO dto, int userId)
        {
            const string fn = nameof(AddTransactionAsync);
            try
            {
                var emp = await _employeeRepo.GetByIdAsync(dto.EmployeeId);
                if (emp == null)
                    return new ResponseDTO { IsValid = false, Message = "Employee not found" };

                // Parse date
                if (!DateTime.TryParse(dto.TransactionDate, out var parsedDate))
                    return new ResponseDTO { IsValid = false, Message = "Invalid transaction date" };

                // Parse type
                if (!Enum.TryParse<TransactionType>(dto.TransactionType, true, out var txType))
                    return new ResponseDTO { IsValid = false, Message = "Invalid transaction type" };

                // Signed amount
                decimal signedAmount = txType == TransactionType.Advance || txType == TransactionType.Deduction
                    ? -Math.Abs(dto.Amount)
                    : Math.Abs(dto.Amount);

                var tx = new EmployeeTransaction
                {
                    TransactionDate = parsedDate,
                    Description = dto.Description ?? string.Empty,
                    TransactionType = txType,
                    Amount = signedAmount,
                    EmployeeId = dto.EmployeeId,
                };

                var ok = await _employeeRepo.AddTransactionAsync(tx);
                return new ResponseDTO { IsValid = ok, Data = tx.Id, Message = ok ? "Transaction added" : "Failed to add transaction" };
            }
            catch (Exception ex)
            {
                await _errorRepo.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> EditTransactionAsync(EditEmployeeTransactionDTO dto, int userId)
        {
            const string fn = nameof(EditTransactionAsync);
            try
            {
                var tx = await _employeeRepo.GetTransactionByIdAsync(dto.Id);
                if (tx == null)
                    return new ResponseDTO { IsValid = false, Message = "Transaction not found" };

                // Parse date
                if (!DateTime.TryParse(dto.TransactionDate, out var parsedDate))
                    return new ResponseDTO { IsValid = false, Message = "Invalid transaction date" };

                // Parse type
                if (!Enum.TryParse<TransactionType>(dto.TransactionType, true, out var txType))
                    return new ResponseDTO { IsValid = false, Message = "Invalid transaction type" };

                // Signed amount
                decimal signedAmount = txType == TransactionType.Advance || txType == TransactionType.Deduction
                    ? -Math.Abs(dto.Amount)
                    : Math.Abs(dto.Amount);

                // Update existing entity (لا تنشئ كائن جديد)
                tx.TransactionDate = parsedDate;
                tx.Description = dto.Description ?? string.Empty;
                tx.TransactionType = txType;
                tx.Amount = signedAmount;

                var ok = await _employeeRepo.UpdateTransactionAsync(tx);
                return new ResponseDTO { IsValid = ok, Message = ok ? "Transaction updated" : "Update failed" };
            }
            catch (Exception ex)
            {
                await _errorRepo.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> DeleteTransactionAsync(int transactionId, int userId)
        {
            const string fn = nameof(DeleteTransactionAsync);
            try
            {
                var ok = await _employeeRepo.DeleteTransactionAsync(transactionId);
                return new ResponseDTO { IsValid = ok, Message = ok ? "Transaction deleted" : "Transaction not found" };
            }
            catch (Exception ex)
            {
                await _errorRepo.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> GetEmployeeTransactionsAsync(int employeeId, int page = 1, int pageSize = 50)
        {
            const string fn = nameof(GetEmployeeTransactionsAsync);
            try
            {
                var emp = await _employeeRepo.GetByIdAsync(employeeId);
                if (emp == null)
                    return new ResponseDTO { IsValid = false, Message = "Employee not found" };

                var list = await _employeeRepo.GetEmployeeTransactionsPagedAsync(employeeId, page, pageSize);

                var dtos = list.Select(t => new EmployeeTransactionDTO
                {
                    Id = t.Id,
                    TransactionDate = t.TransactionDate.ToString("yyyy-MM-dd"),
                    TransactionType = t.TransactionType.ToString(),
                    Description = t.Description,
                    Amount = t.Amount,
                    EmployeeId = t.EmployeeId,
                }).ToList();

                return new ResponseDTO { IsValid = true, Data = dtos };
            }
            catch (Exception ex)
            {
                await _errorRepo.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

    }
}
