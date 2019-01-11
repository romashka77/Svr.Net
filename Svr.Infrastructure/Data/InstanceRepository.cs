using Microsoft.EntityFrameworkCore;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Infrastructure.Data
{
    public class InstanceRepository : EfRepository<Instance>, IInstanceRepository
    {
        public InstanceRepository(DataContext context) : base(context)
        {
        }
        public virtual Instance GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(d => d.Claim).FirstOrDefault(r => r.Id == id);
        }
        public virtual async Task<Instance> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await Entities.Include(d => d.Claim).FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
