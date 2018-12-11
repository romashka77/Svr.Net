using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Svr.Core.Entities;

namespace Svr.Core.Interfaces
{
    public interface IRegionRepository : IRepository<Region>, IAsyncRepository<Region>
    {
        Region GetByIdWithItems(long? id);
        Task<Region> GetByIdWithItemsAsync(long? id);
    }
}
