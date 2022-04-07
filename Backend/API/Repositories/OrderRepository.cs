using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories
{
    public interface IOrderRepository : IEFCRUDRepository<Order>
    {

    }

    public class OrderRepository : EFCRUDRepository<Order>, IOrderRepository
    {
        public OrderRepository(NorthwindContext context) : base(context)
        {

        }
    }
}
