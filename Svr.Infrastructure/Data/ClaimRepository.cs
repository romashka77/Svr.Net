﻿using Microsoft.EntityFrameworkCore;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Infrastructure.Data
{
    public class ClaimRepository : EfRepository<Claim>, IClaimRepository
    {
        public ClaimRepository(DataContext context) : base(context)
        {

        }
        public virtual Claim GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(f => f.FileEntities).Include(m =>m.Meetings).Include(i => i.Instances).Include(d => d.District).Include(e => e.Region).AsNoTracking().SingleOrDefault(m => m.Id == id);
        }

        public virtual async Task<Claim> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await Entities.Include(f => f.FileEntities).Include(m => m.Meetings).Include(i => i.Instances).Include(d => d.District).Include(e => e.Region).AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);
        }
        public override IQueryable<Claim> Sort(IQueryable<Claim> source, SortState sortOrder)
        {
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    return source.OrderByDescending(p => p.Name);
                case SortState.CodeAsc:
                    return source.OrderBy(p => p.Code);
                case SortState.CodeDesc:
                    return source.OrderByDescending(p => p.Code);
                case SortState.DescriptionAsc:
                    return source.OrderBy(p => p.Description);
                case SortState.DescriptionDesc:
                    return source.OrderByDescending(p => p.Description);
                case SortState.CreatedOnUtcAsc:
                    return source.OrderBy(p => p.CreatedOnUtc);
                case SortState.CreatedOnUtcDesc:
                    return source.OrderByDescending(p => p.CreatedOnUtc);
                case SortState.UpdatedOnUtcAsc:
                    return source.OrderBy(p => p.UpdatedOnUtc);
                case SortState.UpdatedOnUtcDesc:
                    return source.OrderByDescending(p => p.UpdatedOnUtc);
                case SortState.OwnerAsc:
                    return source.OrderBy(s => s.Region.Name);
                case SortState.OwnerDesc:
                    return source.OrderByDescending(s => s.Region.Name);
                case SortState.NameAsc:
                    return source.OrderBy(s => s.Name);
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
