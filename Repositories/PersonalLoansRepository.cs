using ERP.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.Repositories
{
    public class PersonalLoansRepository : BaseRepository<PersonalLoan>, IPersonalLoansRepository
    {
        public PersonalLoansRepository(ERPDBContext context) : base(context) { }

        public async Task<List<PersonalLoan>> GetPagedAsync(int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;
            return await _context.Set<PersonalLoan>()
                                 .OrderByDescending(l => l.IssueDate)
                                 .Skip(skip).Take(pageSize)
                                 .ToListAsync();
        }

        public async Task<List<PersonalLoan>> GetOverdueLoansAsync(DateTime asOfDate)
        {
            return await _context.Set<PersonalLoan>()
                                 .Where(l => !l.IsRepaid && l.RepaymentDate < asOfDate)
                                 .OrderBy(l => l.RepaymentDate)
                                 .ToListAsync();
        }
    }
}
