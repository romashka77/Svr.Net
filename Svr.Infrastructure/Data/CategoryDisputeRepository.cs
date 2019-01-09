﻿using Microsoft.EntityFrameworkCore;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Svr.Infrastructure.Data
{
    public class CategoryDisputeRepository : EfRepository<CategoryDispute>, ICategoryDisputeRepository
    {
        public CategoryDisputeRepository(DataContext dbContext) : base(dbContext)
        {

        }

        public virtual CategoryDispute GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(c => c.GroupClaims).FirstOrDefault(r => r.Id == id);
        }

        public virtual async Task<CategoryDispute> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await Entities.Include(c => c.GroupClaims).FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}