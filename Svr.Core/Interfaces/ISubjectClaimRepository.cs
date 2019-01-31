using Svr.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Svr.Core.Interfaces
{
    public interface ISubjectClaimRepository : IRepository<SubjectClaim>, IRepositoryAsync<SubjectClaim>, ISort<SubjectClaim>
    {
        SubjectClaim GetByIdWithItems(long? id);
        Task<SubjectClaim> GetByIdWithItemsAsync(long? id);
    }
}
