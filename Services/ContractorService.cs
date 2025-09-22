using ERP.DTOs.Brokers;
using ERP.DTOs.Contractors;
using ERP.Models.ContractorsManagement;
using Newtonsoft.Json;
using System.Diagnostics.Contracts;
using System.Text.Json;

namespace ERP.Services
{
    public class ContractorService : IContractorService
    {
        private readonly IContractorRepository _contractorRepository;
        private readonly IErrorRepository _errorRepository;

        public ContractorService(IContractorRepository contractorRepository, IErrorRepository errorRepository)
        {
            _contractorRepository = contractorRepository;
            _errorRepository = errorRepository;
        }

        public async Task<ResponseDTO> CreateContractor(CreateContratorDTO dTO, int createdBy)
        {
            const string fn = nameof(CreateContractor);
            try
            {
                var contractor = new Contractor
                {
                    Name = dTO.Name,
                    PhoneNumber = dTO.PhoneNumber,
                    Address = dTO.Address,
                };
                var result = await _contractorRepository.CreateAsync(contractor, createdBy);

                return new ResponseDTO { IsValid = true };
            }
            catch (Exception ex)
            {
                await _errorRepository.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", createdBy);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> EditContractor(EditContractorDTO dTO, int updatedBy)
        {
            const string fn = nameof(EditContractor);
            try
            {
                var contractor = new Contractor
                {
                    Name = dTO.Name,
                    PhoneNumber = dTO.PhoneNumber,
                    Address = dTO.Address,
                };


                var result = await _contractorRepository.UpdateAsync(contractor, updatedBy);

                return new ResponseDTO { IsValid = result };
            }
            catch (Exception ex)
            {
                await _errorRepository.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", updatedBy);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> GetContractorAsync(int contractorId)
        {
            const string fn = nameof(GetContractorAsync);
            try
            {
                var broker = await _contractorRepository.GetByIdAsync(contractorId);
                if (broker == null)
                {
                    return new ResponseDTO
                    {
                        IsValid = false,
                        Message = "Broker does not exist"
                    };
                }

                var contracts = await _contractorRepository.GetContractorContracts(contractorId);

                List<ContractofContractorDTO> transactions = contracts.Select(c => new ContractofContractorDTO
                {
                    ProjectId = c.ProjectId,
                    ProjectName = string.Empty, // لو فيه علاقة Project
                    ContractAmount = c.ContractAmount,
                    TotalWithdrawals = c.TotalWithdrawals,
                    ContractDocuments = string.IsNullOrEmpty(c.ContractDocuments)  ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(c.ContractDocuments)
                }).ToList();

                ContractorDTO dto = new ContractorDTO
                {
                    Id = broker.Id,
                    Name = broker.Name,
                    Address = broker.Address ?? string.Empty,
                    PhoneNumber = broker.PhoneNumber ?? string.Empty,
                    contractofContractorDTOs = transactions
                };

                return new ResponseDTO { IsValid = true, Data = dto };
            }
            catch (Exception ex)
            {
                await _errorRepository.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> GetAllContractorsAsync()
        {
            const string fn = nameof(GetAllContractorsAsync);
            try
            {
                var contractora = await _contractorRepository.GetAllAsync();

                var result = new List<ContractorDTO>();
                foreach (var contractor in contractora)
                {
                    var contracts = await _contractorRepository.GetContractorContracts(contractor.Id);

                    List<ContractofContractorDTO> transactions = contracts.Select(c => new ContractofContractorDTO
                    {
                        ProjectId = c.ProjectId,
                        ProjectName = string.Empty, // لو فيه علاقة Project
                        ContractAmount = c.ContractAmount,
                        TotalWithdrawals = c.TotalWithdrawals,
                        ContractDocuments = string.IsNullOrEmpty(c.ContractDocuments) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(c.ContractDocuments)
                    }).ToList();

                    result.Add(new ContractorDTO
                    {
                        Id = contractor.Id,
                        Name = contractor.Name,
                        Address = contractor.Address ?? string.Empty,
                        PhoneNumber = contractor.PhoneNumber ?? string.Empty,
                        contractofContractorDTOs = transactions
                    });
                }

                return new ResponseDTO { IsValid = true, Data = result };
            }
            catch (Exception ex)
            {
                await _errorRepository.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }
    }
}
