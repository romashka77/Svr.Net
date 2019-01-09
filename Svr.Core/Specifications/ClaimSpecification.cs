using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Svr.Core.Specifications
{
    public class ClaimSpecification : BaseSpecification<Claim>
    {
        public ClaimSpecification(long? id) : base(i => (!id.HasValue || i.District.Id == id))
        {
            AddInclude(d => d.District);

            //var dataContext = _context.Claims.Include(c => c.CategoryDispute).Include(c => c.District).Include(c => c.GroupClaim).Include(c => c.Performer).Include(c => c.Person3rd).Include(c => c.Plaintiff).Include(c => c.Region).Include(c => c.Respondent).Include(c => c.Сourt);
        }
    }
}
