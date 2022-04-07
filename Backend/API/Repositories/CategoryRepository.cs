using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories
{
    public interface ICategoryRepository : IEFCRUDRepository<Category>
    {

    }

    public class CategoryRepository : EFCRUDRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(NorthwindContext context) : base(context)
        {

        }
    }
}
