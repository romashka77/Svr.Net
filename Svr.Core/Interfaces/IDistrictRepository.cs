using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Svr.Core.Entities;

namespace Svr.Core.Interfaces
{
    public interface IDistrictRepository : IRepository<District>, IRepositoryAsync<District>
    {
        District GetByIdWithItems(long? id);
        Task<District> GetByIdWithItemsAsync(long? id);
    }
}
