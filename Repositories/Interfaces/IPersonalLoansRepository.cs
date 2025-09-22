using ERP.Models;

namespace ERP.Repositories.Interfaces
{
    public interface IPersonalLoansRepository : IBaseRepository<PersonalLoan>
    {
        Task<List<PersonalLoan>> GetPagedAsync(int page, int pageSize);
        Task<List<PersonalLoan>> GetOverdueLoansAsync(DateTime asOfDate);
    }
}
