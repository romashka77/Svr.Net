using System;
using System.Linq.Expressions;
using Svr.Core.Entities;

namespace Svr.Core.Specifications
{
    public class RegionSpecification : BaseSpecification<Region>
    {
        public RegionSpecification(Expression<Func<Region, bool>> criteria) : base(criteria)
        {
        }
    }
}
