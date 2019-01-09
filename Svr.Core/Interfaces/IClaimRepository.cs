using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Svr.Core.Interfaces
{
    public interface IClaimRepository : IRepository<Claim>, IAsyncRepository<Claim>
    {
        Claim GetByIdWithItems(long? id);
        Task<Claim> GetByIdWithItemsAsync(long? id);
    }
}
