using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Svr.Core.Specifications
{
    public class DistrictPerformerSpecification : BaseSpecification<DistrictPerformer>
    {
        public DistrictPerformerSpecification(long? id) : base(i => (!id.HasValue || i.DistrictId == id))
        {
            //AddInclude(d => d.District);
        }

    }
}
