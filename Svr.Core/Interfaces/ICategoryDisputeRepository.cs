using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Svr.Core.Interfaces
{
    public interface ICategoryDisputeRepository : IRepository<CategoryDispute>, IAsyncRepository<CategoryDispute>
    {
        CategoryDispute GetByIdWithItems(long? id);
        Task<CategoryDispute> GetByIdWithItemsAsync(long? id);
    }
}
