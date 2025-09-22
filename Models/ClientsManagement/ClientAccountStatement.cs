using ERP.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.ClientsManagement
{
    public class ClientAccountStatement : BaseEntity
    {
        [ForeignKey(nameof(Client))]
        public int ClientId { get; set; }
        [ForeignKey(nameof(Project))]
        public int? ProjectId { get; set; }
        public DateTime Date { get; set; }

        public string Description { get; set; }

        // Amount of the transaction (positive for income, negative for expense).
        [Precision(18, 2)]
        public decimal Amount { get; set; }

        public virtual Client Client { get; set; }
        public virtual Project Project { get; set; }
    }
}
