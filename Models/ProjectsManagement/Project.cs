using ERP.Models.Brokers;
using ERP.Models.ClientsManagement;
using ERP.Models.ContractorsManagement;

namespace ERP.Models.Projects
{
    public class Project : BaseEntity
    {
        public virtual ICollection<BrokerComission> BrokerComissions { get; set; } = new List<BrokerComission>();
        public virtual ICollection<ContractOfContractor> ContractOfContracts { get; set; } = new List<ContractOfContractor>();
        public virtual ICollection<ClientAccountStatement> ClientAccountStatements { get; set; } = new List<ClientAccountStatement>();
    }
}
