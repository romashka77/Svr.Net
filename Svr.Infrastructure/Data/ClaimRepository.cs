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
    public class ClaimRepository : EfRepository<Claim>, IClaimRepository
    {
        public ClaimRepository(DataContext context) : base(context)
        {

        }
        public virtual Claim GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(f => f.FileEntities).Include(m =>m.Meetings).Include(i => i.Instances).Include(d => d.District).ThenInclude(e => e.Region).SingleOrDefault(m => m.Id == id);
        }

        public virtual async Task<Claim> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await Entities.Include(f => f.FileEntities).Include(m => m.Meetings).Include(i => i.Instances).Include(d => d.District).ThenInclude(e => e.Region).SingleOrDefaultAsync(m => m.Id == id);
        }
    }
}
