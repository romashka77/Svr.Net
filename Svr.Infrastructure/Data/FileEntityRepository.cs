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
    public class FileEntityRepository : EfRepository<FileEntity>, IFileEntityRepository
    {
        public FileEntityRepository(DataContext context) : base(context)
        {

        }
        public virtual FileEntity GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(d => d.Claim).FirstOrDefault(r => r.Id == id);
        }
        public virtual async Task<FileEntity> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await Entities.Include(d => d.Claim).FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
