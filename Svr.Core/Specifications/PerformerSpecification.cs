using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Svr.Core.Specifications
{
    public class PerformerSpecification : BaseSpecification<Performer>
    {
        public PerformerSpecification(long? id) : base(i => (!id.HasValue/* || i.DistrictPerformers.Where(d =>d.)*/))
        {

        }
    }
}
