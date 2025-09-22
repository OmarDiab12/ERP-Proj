using ERP.DTOs.PersonalLoans;
using ERP.Models;

namespace ERP.Services
{
    public class PersonalLoanService : IPersonalLoanService
    {
        private readonly IPersonalLoansRepository _repo;
        private readonly IErrorRepository _errors;

        public PersonalLoanService(IPersonalLoansRepository repo, IErrorRepository errors)
        {
            _repo = repo;
            _errors = errors;
        }

        public async Task<ResponseDTO> CreateAsync(CreatePersonalLoanDTO dto, int userId)
        {
            const string fn = nameof(CreateAsync);
            try
            {
                if (!DateTime.TryParse(dto.IssueDate, out var issueDate))
                    return new ResponseDTO { IsValid = false, Message = "Invalid IssueDate" };

                if (!DateTime.TryParse(dto.RepaymentDate, out var repaymentDate))
                    return new ResponseDTO { IsValid = false, Message = "Invalid RepaymentDate" };

                var entity = new PersonalLoan
                {
                    PersonName = dto.PersonName,
                    IssueDate = issueDate,
                    Amount = dto.Amount,
                    RepaymentDate = repaymentDate,
                    IsRepaid = false
                };

                await _repo.CreateAsync(entity, userId);

                return new ResponseDTO { IsValid = true, Data = entity.Id, Message = "Loan created" };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error" };
            }
        }

        public async Task<ResponseDTO> EditAsync(EditPersonalLoanDTO dto, int userId)
        {
            const string fn = nameof(EditAsync);
            try
            {
                var loan = await _repo.GetByIdAsync(dto.Id);
                if (loan == null)
                    return new ResponseDTO { IsValid = false, Message = "Loan not found" };

                if (!DateTime.TryParse(dto.IssueDate, out var issueDate))
                    return new ResponseDTO { IsValid = false, Message = "Invalid IssueDate" };

                if (!DateTime.TryParse(dto.RepaymentDate, out var repaymentDate))
                    return new ResponseDTO { IsValid = false, Message = "Invalid RepaymentDate" };

                loan.PersonName = dto.PersonName;
                loan.IssueDate = issueDate;
                loan.Amount = dto.Amount;
                loan.RepaymentDate = repaymentDate;
                loan.IsRepaid = dto.IsRepaid;

                await _repo.UpdateAsync(loan, userId);

                return new ResponseDTO { IsValid = true, Message = "Loan updated" };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error" };
            }
        }

        public async Task<ResponseDTO> DeleteAsync(int id, int userId)
        {
            const string fn = nameof(DeleteAsync);
            try
            {
                var loan = await _repo.GetByIdAsync(id);
                if (loan == null)
                    return new ResponseDTO { IsValid = false, Message = "Loan not found" };

                await _repo.SoftDeleteAsync(id, userId);

                return new ResponseDTO { IsValid = true, Message = "Loan deleted" };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error" };
            }
        }

        public async Task<ResponseDTO> GetAsync(int id)
        {
            const string fn = nameof(GetAsync);
            try
            {
                var l = await _repo.GetByIdAsync(id);
                if (l == null)
                    return new ResponseDTO { IsValid = false, Message = "Loan not found" };

                var dto = new PersonalLoanDTO
                {
                    Id = l.Id,
                    PersonName = l.PersonName,
                    IssueDate = l.IssueDate.ToString("yyyy-MM-dd"),
                    Amount = l.Amount,
                    RepaymentDate = l.RepaymentDate.ToString("yyyy-MM-dd"),
                    IsRepaid = l.IsRepaid,
                    LastReminderSent = l.LastReminderSent?.ToString("yyyy-MM-dd")
                };

                return new ResponseDTO { IsValid = true, Data = dto };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error" };
            }
        }

        public async Task<ResponseDTO> GetAllAsync(int page = 1, int pageSize = 50)
        {
            const string fn = nameof(GetAllAsync);
            try
            {
                var list = await _repo.GetPagedAsync(page, pageSize);
                var dtos = list.Select(l => new PersonalLoanDTO
                {
                    Id = l.Id,
                    PersonName = l.PersonName,
                    IssueDate = l.IssueDate.ToString("yyyy-MM-dd"),
                    Amount = l.Amount,
                    RepaymentDate = l.RepaymentDate.ToString("yyyy-MM-dd"),
                    IsRepaid = l.IsRepaid,
                    LastReminderSent = l.LastReminderSent?.ToString("yyyy-MM-dd")
                }).ToList();

                return new ResponseDTO { IsValid = true, Data = dtos };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error" };
            }
        }

        public async Task<ResponseDTO> GetOverdueAsync(string asOfDate)
        {
            const string fn = nameof(GetOverdueAsync);
            try
            {
                if (!DateTime.TryParse(asOfDate, out var date))
                    return new ResponseDTO { IsValid = false, Message = "Invalid date" };

                var list = await _repo.GetOverdueLoansAsync(date);
                var dtos = list.Select(l => new PersonalLoanDTO
                {
                    Id = l.Id,
                    PersonName = l.PersonName,
                    IssueDate = l.IssueDate.ToString("yyyy-MM-dd"),
                    Amount = l.Amount,
                    RepaymentDate = l.RepaymentDate.ToString("yyyy-MM-dd"),
                    IsRepaid = l.IsRepaid,
                    LastReminderSent = l.LastReminderSent?.ToString("yyyy-MM-dd")
                }).ToList();

                return new ResponseDTO { IsValid = true, Data = dtos };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error" };
            }
        }
    }
}
