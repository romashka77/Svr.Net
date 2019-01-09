using Microsoft.EntityFrameworkCore;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Infrastructure.Data
{
    public class DistrictRepository : EfRepository<District>, IDistrictRepository
    {
        public DistrictRepository(DataContext context) : base(context)
        {
        }
        public virtual District GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            //return Entities.Include(r => r.Districts).FirstOrDefault(r => r.Id == id);
            return Entities.Include(d => d.Region).Include(d => d.DistrictPerformers).ThenInclude(e => e.Performer).SingleOrDefault(m => m.Id == id);
        }
        public virtual async Task<District> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await Entities.Include(d => d.Region).Include(d => d.DistrictPerformers).ThenInclude(e => e.Performer).SingleOrDefaultAsync(m => m.Id == id);
        }
    }
}
