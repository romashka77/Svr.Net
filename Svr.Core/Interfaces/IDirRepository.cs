using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Svr.Core.Entities;

namespace Svr.Core.Interfaces
{
    public interface IDirRepository : IRepository<Dir>, IAsyncRepository<Dir>
    {
        Dir GetByIdWithItems(long? id);
        Task<Dir> GetByIdWithItemsAsync(long? id);
    }
}
