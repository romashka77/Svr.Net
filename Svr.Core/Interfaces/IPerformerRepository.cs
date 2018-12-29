using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Svr.Core.Interfaces
{
    public interface IPerformerRepository: IRepository<Performer>, IAsyncRepository<Performer>
    {
        Performer GetByIdWithItems(long? id);
        Task<Performer> GetByIdWithItemsAsync(long? id);
    }
}
