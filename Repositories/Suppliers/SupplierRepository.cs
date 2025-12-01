using ERP.Models.SuppliersManagement;
using ERP.Repositories.Interfaces.Suppliers;
using ERP.Repositories;
using ERP.Models;

namespace ERP.Repositories.Suppliers
{
    public class SupplierRepository : BaseRepository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(ERPDBContext context) : base(context)
        {
        }
    }
}
