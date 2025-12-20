using ERP.DTOs;
using ERP.DTOs.Partners;
using ERP.Models;
using ERP.Models.Partners;
using ERP.Repositories;
using ERP.Repositories.Interfaces.Persons;
using ERP.Services.Interfaces.Persons;

namespace ERP.Services.Persons
{
    public class PartnerService : IPartnerService
    {
        private readonly IPartnerRepository _partnerRepository;
        private readonly IErrorRepository _errorRepository;

        public PartnerService(IPartnerRepository partnerRepository,IErrorRepository errorRepository)
        {
            _partnerRepository = partnerRepository;
            _errorRepository = errorRepository;
        }

        public async Task<ResponseDTO> GetAllPartners()
        {
            const string fn = nameof(GetAllPartners);
            try
            {
                var result = await _partnerRepository.GetAllAsync();
                List<partnerDTO> partnersDTOs = result.Select(x => new partnerDTO
                {
                    Id = x.Id,
                    Name = x.FullName,
                    AssignedTasks = x.AssignedTasks,
                    Phone = x.PhoneNumber,
                    Share = x.ProjectShare
                }).ToList();

                return new ResponseDTO { Data = partnersDTOs, IsValid = true };
            }
            catch (Exception ex)
            {
                await _errorRepository.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> CreateNewPartner(CreatePartnerDTO dTO)
        {
            const string fn = nameof(CreateNewPartner);
            try
            {
                double checkShares = dTO.share + dTO.partnersShareDTOs.Sum(c => c.share);
                if(checkShares != 1)
                    return new ResponseDTO { IsValid = false , Message = "Shares not equal 1" };

                var partner = new Partner
                {
                    FullName = dTO.Name,
                    PhoneNumber = dTO.Phone,
                    AssignedTasks = dTO.AssignedTasks,
                    ProjectShare = dTO.share,
                };
                var result = await _partnerRepository.CreateNewPartner(dTO.partnersShareDTOs, partner);

                return new ResponseDTO {IsValid = result };
            }
            catch (Exception ex)
            {
                await _errorRepository.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> EditPartner(EditPartnerDTO dTO)
        {
            const string fn = nameof(EditPartner);
            try
            {
                double checkShares = dTO.share + dTO.partnersShareDTOs.Sum(c => c.share);
                if (checkShares != 1)
                    return new ResponseDTO { IsValid = false, Message = "Shares not equal 1" };

                
                var result = await _partnerRepository.EditPartnerAsync(dTO.Id,dTO);

                return new ResponseDTO { IsValid = result };
            }
            catch (Exception ex)
            {
                await _errorRepository.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> GetPartner(int partnerId)
        {
            const string fn = nameof(GetPartner);
            try
            {
                var r = await _partnerRepository.GetByIdAsync(partnerId);
                if (r == null)
                    return new ResponseDTO
                    {
                        IsValid = false,
                        Message = "Parner not Existed"
                    };

                partnerDTO partnersDTOs = new partnerDTO
                {
                    Id = r.Id,
                    Name = r.FullName,
                    AssignedTasks = r.AssignedTasks,
                    Phone = r.PhoneNumber,
                    Share = r.ProjectShare
                };

                return new ResponseDTO { Data = partnersDTOs, IsValid = true };
            }
            catch (Exception ex)
            {
                await _errorRepository.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

    }
}
