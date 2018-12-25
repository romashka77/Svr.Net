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
    public class DirRepository : EfRepository<Dir>, IDirRepository
    {
        public DirRepository(DataContext context) : base(context)
        {
        }
        public virtual Dir GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            //return Entities.Include(r => r.Districts).FirstOrDefault(r => r.Id == id);
            return Entities.Include(d => d.DirName).FirstOrDefault(r => r.Id == id);
        }
        public virtual Task<Dir> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(r => r.DirName).FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
