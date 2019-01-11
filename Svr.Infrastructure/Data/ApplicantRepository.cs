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
    public class ApplicantRepository : EfRepository<Applicant>, IApplicantRepository
    {
        public ApplicantRepository(DataContext context) : base(context)
        {
        }
        public virtual Applicant GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(d => d.TypeApplicant).Include(d => d.Opf).FirstOrDefault(r => r.Id == id);
        }
        public virtual async Task<Applicant> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await Entities.Include(d => d.TypeApplicant).Include(d => d.Opf).FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
