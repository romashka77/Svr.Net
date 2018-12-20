using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Svr.Core.Specifications
{
    public class SubjectClaimSpecification : BaseSpecification<SubjectClaim>
    {
        public SubjectClaimSpecification(long? id) : base(i => (!id.HasValue || i.GroupClaim.Id == id))
        {
            AddInclude(d => d.GroupClaim);
        }
    }
}
