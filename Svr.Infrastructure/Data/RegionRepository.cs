using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Svr.Core.Entities;
using Svr.Core.Interfaces;

namespace Svr.Infrastructure.Data
{
    public class RegionRepository : EfRepository<Region>, IRegionRepository
    {
        public RegionRepository(DataContext dbContext) : base(dbContext)
        {
            
        }

        public virtual Region GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(r => r.Districts).Include(p => p.Performers).FirstOrDefault(r => r.Id == id);
        }

        public virtual async Task<Region> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await Entities.Include(r => r.Districts).Include(p =>p.Performers).FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
