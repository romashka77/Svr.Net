using Microsoft.EntityFrameworkCore;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using System;
using System.Linq;
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
            return Entities.Include(d => d.TypeApplicant).Include(d => d.Opf).AsNoTracking().SingleOrDefault(r => r.Id == id);
        }
        public virtual async Task<Applicant> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await Entities.Include(d => d.TypeApplicant).Include(d => d.Opf).AsNoTracking().SingleOrDefaultAsync(r => r.Id == id);
        }
        public override IQueryable<Applicant> Sort(IQueryable<Applicant> source, SortState sortOrder)
        {
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    return source.OrderByDescending(p => p.Name);
                case SortState.CreatedOnUtcAsc:
                    return source.OrderBy(p => p.CreatedOnUtc);
                case SortState.CreatedOnUtcDesc:
                    return source.OrderByDescending(p => p.CreatedOnUtc);
                case SortState.UpdatedOnUtcAsc:
                    return source.OrderBy(p => p.UpdatedOnUtc);
                case SortState.UpdatedOnUtcDesc:
                    return source.OrderByDescending(p => p.UpdatedOnUtc);
                case SortState.OwnerAsc:
                    return source.OrderBy(s => s.TypeApplicant.Name);
                case SortState.OwnerDesc:
                    return source.OrderByDescending(s => s.TypeApplicant.Name);
                case SortState.NameAsc:
                    return source.OrderBy(s => s.Name);
                case SortState.CodeAsc:
                    return source;
                case SortState.CodeDesc:
                    return source;
                case SortState.DescriptionAsc:
                    return source;
                case SortState.DescriptionDesc:
                    return source;
                case SortState.LordAsc:
                    return source;
                case SortState.LordDesc:
                    return source;
                default:
                    return source.OrderBy(s => s.Name);
            }
        }
    }
}
