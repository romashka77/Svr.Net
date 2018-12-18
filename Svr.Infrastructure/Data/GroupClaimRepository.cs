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
    public class GroupClaimRepository : EfRepository<GroupClaim>, IGroupClaimRepository
    {
        public GroupClaimRepository(DataContext context) : base(context)
        {
        }
        public virtual GroupClaim GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(d => d.CategoryDispute).FirstOrDefault(r => r.Id == id);
        }
        public virtual Task<GroupClaim> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(r => r.CategoryDispute).FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
