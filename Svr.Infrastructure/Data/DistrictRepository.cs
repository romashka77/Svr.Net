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
            return Entities.Include(d => d.Region).FirstOrDefault(r => r.Id == id);
        }
        public virtual Task<District> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(r => r.Region).FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
