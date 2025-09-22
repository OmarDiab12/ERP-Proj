using ERP.Models;
using ERP.Models.ClientsManagement;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ERP.Repositories
{
    public class ClientRepository : BaseRepository<Client>, IClientRepository
    {
        public ClientRepository(ERPDBContext context) : base(context) { }

        public async Task<List<ClientAccountStatement>> getClientStatements(int clientId)
        {
            return await _context.ClientStatements.Where(c=>c.ClientId == clientId).ToListAsync();
        }

        public async Task<List<ClientAccountStatement>> getStatementsforClientperProject(int clientId , int projectId)
        {
            return await _context.ClientStatements.Where(c => c.ClientId == clientId && c.ProjectId == projectId).ToListAsync();

        }

    }
}
