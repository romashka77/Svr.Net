using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Svr.Core.Entities;

namespace Svr.Core.Interfaces
{
    public interface IDirNameRepository : IRepository<DirName>, IAsyncRepository<DirName>
    {
        DirName GetByIdWithItems(long? id);
        Task<DirName> GetByIdWithItemsAsync(long? id);
    }
}
