using Svr.Core.Entities;
using System.Threading.Tasks;

namespace Svr.Core.Interfaces
{
    public interface IInstanceRepository : IRepository<Instance>, IRepositoryAsync<Instance>, ISort<Instance>
    {
        Instance GetByIdWithItems(long? id);
        Task<Instance> GetByIdWithItemsAsync(long? id);
    }
}
