using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Svr.Core.Interfaces
{
    public interface IGroupClaimRepository : IRepository<GroupClaim>, IAsyncRepository<GroupClaim>
    {
        GroupClaim GetByIdWithItems(long? id);
        Task<GroupClaim> GetByIdWithItemsAsync(long? id);
    }
}
