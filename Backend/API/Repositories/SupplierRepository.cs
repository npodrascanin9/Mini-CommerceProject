using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories
{
    public interface ISupplierRepository : IEFCRUDRepository<Supplier>
    {

    }

    public class SupplierRepository : EFCRUDRepository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(NorthwindContext context) : base(context)
        {

        }
    }
}
