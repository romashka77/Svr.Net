using Microsoft.EntityFrameworkCore;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Svr.Infrastructure.Data
{
    public class CategoryDisputeRepositiry : EfRepository<CategoryDispute>, ICategoryDisputeRepository
    {
        public CategoryDisputeRepositiry(DataContext dbContext) : base(dbContext)
        {

        }

        public virtual CategoryDispute GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(c => c.GroupClaims).FirstOrDefault(r => r.Id == id);
        }

        public virtual Task<CategoryDispute> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(c => c.GroupClaims).FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
