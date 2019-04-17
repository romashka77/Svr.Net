using System.Threading.Tasks;
using Svr.Core.Entities;

namespace Svr.Core.Interfaces
{
    public interface IDirNameRepository : IRepository<DirName>, IRepositoryAsync<DirName>, ISort<DirName>
    {
        DirName GetByIdWithItems(long? id);
        Task<DirName> GetByIdWithItemsAsync(long? id);
    }
}
