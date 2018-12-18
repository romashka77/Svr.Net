using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Svr.Core.Specifications
{
    public class CategoryDisputeSpecification : BaseSpecification<CategoryDispute>
    {
        public CategoryDisputeSpecification(Expression<Func<CategoryDispute, bool>> criteria) : base(criteria)
        {
        }
    }
}
