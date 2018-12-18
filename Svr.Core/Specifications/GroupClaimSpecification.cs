using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Svr.Core.Specifications
{
    public class GroupClaimSpecification : BaseSpecification<GroupClaim>
    {
        public GroupClaimSpecification(long? id) : base(i => (!id.HasValue || i.CategoryDispute.Id == id))
        {
            AddInclude(d => d.CategoryDispute);
        }
    }
}
