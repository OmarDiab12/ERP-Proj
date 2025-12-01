using ERP.Models.SuppliersManagement;
using ERP.Repositories.Interfaces.Suppliers;
using ERP.Repositories;

namespace ERP.Repositories.Suppliers
{
    public class SupplierRepository : BaseRepository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(ERPDBContext context) : base(context)
        {
        }
    }
}
