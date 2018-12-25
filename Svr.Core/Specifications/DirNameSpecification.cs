using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Svr.Core.Specifications
{
    public class DirNameSpecification : BaseSpecification<DirName>
    {
        public DirNameSpecification(Expression<Func<DirName, bool>> criteria) : base(criteria)
        {
        }
    }
}
