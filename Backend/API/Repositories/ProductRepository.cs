using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories
{
    public interface IProductRepository : IEFCRUDRepository<Product>
    {
    }

    public class ProductRepository : EFCRUDRepository<Product>, IProductRepository
    {
        public ProductRepository(NorthwindContext context) : base(context)
        {

        }
    }
}
