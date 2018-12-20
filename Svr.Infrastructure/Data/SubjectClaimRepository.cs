using Microsoft.EntityFrameworkCore;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Infrastructure.Data
{
    public class SubjectClaimRepository : EfRepository<SubjectClaim>, ISubjectClaimRepository
    {
        public SubjectClaimRepository(DataContext context) : base(context)
        {

        }
        public virtual SubjectClaim GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(d => d.GroupClaim).FirstOrDefault(r => r.Id == id);
        }
        public virtual Task<SubjectClaim> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(r => r.GroupClaim).FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
