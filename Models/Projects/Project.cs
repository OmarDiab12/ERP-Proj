using ERP.Models.Brokers;

namespace ERP.Models.Projects
{
    public class Project : BaseEntity
    {
        public virtual ICollection<BrokerComission> BrokerComissions { get; set; } = new List<BrokerComission>();
    }
}
