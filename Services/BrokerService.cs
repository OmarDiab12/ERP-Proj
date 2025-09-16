using Azure;
using ERP.DTOs.Brokers;
using ERP.DTOs.Partners;
using ERP.Models.Brokers;
using ERP.Models.Partners;

namespace ERP.Services
{
    public class BrokerService : IBrokerService
    {
        private readonly IBrokerRepository _brokerRepository;
        private readonly IErrorRepository _errorRepository;

        public BrokerService(IBrokerRepository brokerRepository ,IErrorRepository errorRepository)
        {
            _brokerRepository = brokerRepository;
            _errorRepository = errorRepository;
        }

        public async Task<ResponseDTO> CreateBroker(CreateBrokerDTO dTO , int createdBy)
        {
            const string fn = nameof(CreateBroker);
            try
            {
                var broker = new Broker
                {
                    Name = dTO.Name,
                    PhoneNumber = dTO.PhoneNumber,
                    Address = dTO.Address,
                };
                var result = await _brokerRepository.CreateAsync(broker, createdBy);

                return new ResponseDTO { IsValid = true};
            }
            catch (Exception ex)
            {
                await _errorRepository.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", createdBy);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> EditBroker(EditBrokerDTO dTO , int updatedBy)
        {
            const string fn = nameof(EditBroker);
            try
            {
                var broker = new Broker
                {
                    Name = dTO.Name,
                    PhoneNumber = dTO.PhoneNumber,
                    Address = dTO.Address,
                };


                var result = await _brokerRepository.UpdateAsync(broker,updatedBy);

                return new ResponseDTO { IsValid = result };
            }
            catch (Exception ex)
            {
                await _errorRepository.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", updatedBy);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> GetBrokerAsync(int brokerId)
        {
            const string fn = nameof(GetBrokerAsync);
            try
            {
                var broker = await _brokerRepository.GetByIdAsync(brokerId);
                if (broker == null)
                {
                    return new ResponseDTO
                    {
                        IsValid = false,
                        Message = "Broker does not exist"
                    };
                }

                var commissions = await _brokerRepository.GetBrokerCommissions(brokerId);

                List<BrokerTransactionDTO> transactions = commissions.Select(c => new BrokerTransactionDTO
                {
                    ProjectId = c.ProjectId,
                    ProjectName =  string.Empty, // لو فيه علاقة Project
                    PercentOfTotal = c.PercentofTotal,
                    AmountReceived = c.AmountRecieved
                }).ToList();

                BrokerDTO dto = new BrokerDTO
                {
                    Id = broker.Id,
                    Name = broker.Name,
                    Address = broker.Address ?? string.Empty,
                    PhoneNumber = broker.PhoneNumber ?? string.Empty,
                    Transactions = transactions
                };

                return new ResponseDTO { IsValid = true, Data = dto };
            }
            catch (Exception ex)
            {
                await _errorRepository.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> GetAllBrokersAsync()
        {
            const string fn = nameof(GetAllBrokersAsync);
            try
            {
                var brokers = await _brokerRepository.GetAllAsync();

                var result = new List<BrokerDTO>();
                foreach (var broker in brokers)
                {
                    var commissions = await _brokerRepository.GetBrokerCommissions(broker.Id);

                    List<BrokerTransactionDTO> transactions = commissions.Select(c => new BrokerTransactionDTO
                    {
                        ProjectId = c.ProjectId,
                        ProjectName = string.Empty,
                        PercentOfTotal = c.PercentofTotal,
                        AmountReceived = c.AmountRecieved
                    }).ToList();

                    result.Add(new BrokerDTO
                    {
                        Id = broker.Id,
                        Name = broker.Name,
                        Address = broker.Address ?? string.Empty,
                        PhoneNumber = broker.PhoneNumber ?? string.Empty,
                        Transactions = transactions
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
