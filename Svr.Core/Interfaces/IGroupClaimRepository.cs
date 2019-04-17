using Svr.Core.Entities;
using System.Threading.Tasks;

namespace Svr.Core.Interfaces
{
    public interface IGroupClaimRepository : IRepository<GroupClaim>, IRepositoryAsync<GroupClaim>, ISort<GroupClaim>
    {
        GroupClaim GetByIdWithItems(long? id);
        Task<GroupClaim> GetByIdWithItemsAsync(long? id);
    }
}
