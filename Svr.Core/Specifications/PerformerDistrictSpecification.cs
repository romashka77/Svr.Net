using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Svr.Core.Specifications
{
    public class PerformerDistrictSpecification : BaseSpecification<DistrictPerformer>
    {
        public PerformerDistrictSpecification(long? id) : base(i => (!id.HasValue || i.Performer.Id == id))
        {
        }
    }
}
