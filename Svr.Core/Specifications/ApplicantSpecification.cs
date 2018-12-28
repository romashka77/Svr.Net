using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Svr.Core.Specifications
{
    public class ApplicantSpecification : BaseSpecification<Applicant>
    {

        public ApplicantSpecification(long? id) : base(i => (!id.HasValue || i.TypeApplicant.Id == id))
        {
            AddInclude(d => d.TypeApplicant);
        }
    }
}
