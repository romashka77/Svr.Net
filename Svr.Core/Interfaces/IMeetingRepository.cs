using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Svr.Core.Interfaces
{
    public interface IMeetingRepository : IRepository<Meeting>, IAsyncRepository<Meeting>
    {
        Meeting GetByIdWithItems(long? id);
        Task<Meeting> GetByIdWithItemsAsync(long? id);
    }
}
