using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories
{
    public interface IOrderDetailRepository : IEFCRUDRepository<OrderDetail>
    {
    }

    public class OrderDetailRepository : EFCRUDRepository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(NorthwindContext context) : base(context)
        {

        }
    }
}
