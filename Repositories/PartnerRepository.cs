using ERP.DTOs.Partners;
using ERP.Models;
using ERP.Models.Partners;
using ERP.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ERP.Repositories
{
    public class PartnerRepository : BaseRepository<Partner>, IPartnerRepository
    {
        public PartnerRepository(ERPDBContext context) : base(context) { }

        public async Task<List<PartnerTransaction>> GetTransactionsForPartner(int partnerId)
        {
            return await _context.PartnerTransactions.Where(c=>c.PartnerId == partnerId).ToListAsync();
        }

        public async Task<bool> CreateNewPartner(List<PartnersShareDTO> partnersShares, Partner newPartner)
        {
            var partnerIdsToUpdate = partnersShares.Select(s => s.Id).ToList();

            var partnersFromDb = await _context.Partners
                .Where(p => partnerIdsToUpdate.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            foreach (var shareInfo in partnersShares)
            {
                if (partnersFromDb.TryGetValue(shareInfo.Id, out var partnerToUpdate))
                {
                    partnerToUpdate.ProjectShare = shareInfo.share;
                }
                else
                {
                    throw new Exception($"Partner {shareInfo.Name} not exist");
                }
            }
            newPartner.CreatedAt = DateTime.Now;
            newPartner.UpdatedAt = DateTime.Now;
            
            
            _context.Partners.Add(newPartner);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EditPartnerAsync(int partnerIdToEdit, EditPartnerDTO updateData)
        {

            var partner = await _context.Partners.FindAsync(partnerIdToEdit);

            if (partner == null)
            {
                throw new KeyNotFoundException($"Partner with ID {partnerIdToEdit} not found.");
            }

            partner.FullName = updateData.Name;
            partner.PhoneNumber = updateData.Phone;
            partner.AssignedTasks = updateData.AssignedTasks;
            partner.ProjectShare = updateData.share;
            partner.UpdatedAt = DateTime.Now;

            if (updateData.partnersShareDTOs != null && updateData.partnersShareDTOs.Any())
            {
                var partnerIdsToUpdate = updateData.partnersShareDTOs.Select(s => s.Id).ToList();

                var associatedPartnersFromDb = await _context.Partners
                    .Where(p => partnerIdsToUpdate.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id);

                foreach (var shareInfo in updateData.partnersShareDTOs)
                {
                    if (associatedPartnersFromDb.TryGetValue(shareInfo.Id, out var partnerToUpdate))
                    {
                        partnerToUpdate.ProjectShare = shareInfo.share;
                    }
                }
            }

           
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
