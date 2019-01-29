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
    public class GroupClaimSpecificationReport : BaseSpecification<GroupClaim>
    {
        public GroupClaimSpecificationReport(string owner) : base(i => (String.IsNullOrEmpty(owner) || i.CategoryDispute.Name == owner))
        {
            AddInclude(d => d.CategoryDispute);
            AddInclude(d => d.SubjectClaims);
        }
    }
}
