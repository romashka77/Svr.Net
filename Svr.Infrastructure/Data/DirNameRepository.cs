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
    public class DirNameRepository : EfRepository<DirName>, IDirNameRepository
    {
        public DirNameRepository(DataContext dbContext) : base(dbContext)
        {

        }

        public virtual DirName GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(r => r.Dirs).FirstOrDefault(r => r.Id == id);
        }

        public virtual Task<DirName> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(r => r.Dirs).FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
