﻿using Microsoft.EntityFrameworkCore;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Infrastructure.Data
{
    public class PerformerRepository : EfRepository<Performer>, IPerformerRepository
    {
        public PerformerRepository(DataContext context) : base(context)
        {
        }
        public virtual Performer GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            //return Entities.Include(r => r.Districts).FirstOrDefault(r => r.Id == id);
            return Entities.Include(d => d.Region).Include(d => d.DistrictPerformers).ThenInclude(e => e.District).AsNoTracking().SingleOrDefault(m => m.Id == id);
        }
        public virtual async Task<Performer> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await Entities.Include(d => d.Region).Include(d => d.DistrictPerformers).ThenInclude(e => e.District).AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);
        }
        public override IQueryable<Performer> Sort(IQueryable<Performer> source, SortState sortOrder)
        {
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    return source.OrderByDescending(p => p.Name);
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
                default:
                    return source.OrderBy(s => s.Name);
            }
        }
    }
}