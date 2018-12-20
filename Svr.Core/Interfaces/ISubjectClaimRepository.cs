using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Svr.Core.Interfaces
{
    public interface ISubjectClaimRepository : IRepository<SubjectClaim>, IAsyncRepository<SubjectClaim>
    {
        SubjectClaim GetByIdWithItems(long? id);
        Task<SubjectClaim> GetByIdWithItemsAsync(long? id);
    }
}
