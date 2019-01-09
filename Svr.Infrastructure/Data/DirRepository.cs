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
            return Entities.Include(d => d.DirName).Include(d => d.Applicants).FirstOrDefault(r => r.Id == id);
        }
        public virtual async Task<Dir> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await Entities.Include(r => r.DirName).Include(d => d.Applicants).FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
