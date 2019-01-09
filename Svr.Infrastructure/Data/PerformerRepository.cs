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
    public class PerformerRepository : EfRepository<Performer>, IPerformerRepository
    {
        public PerformerRepository(DataContext context) : base(context)
        {
        }
        public virtual Performer GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            //return Entities.Include(r => r.Districts).FirstOrDefault(r => r.Id == id);
            return Entities.Include(d =>d.Region).Include(d => d.DistrictPerformers).ThenInclude(e => e.District).SingleOrDefault(m => m.Id == id);
        }
        public virtual async Task<Performer> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await Entities.Include(d => d.Region).Include(d => d.DistrictPerformers).ThenInclude(e => e.District).SingleOrDefaultAsync(m => m.Id == id);
        }
    }
}
