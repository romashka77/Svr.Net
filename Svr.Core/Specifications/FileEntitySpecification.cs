using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Svr.Core.Specifications
{
    public class FileEntitySpecification : BaseSpecification<FileEntity>
    {
        public FileEntitySpecification(long? id) : base(i => (!id.HasValue || i.Claim.Id == id))
        {
            AddInclude(d => d.Claim);
        }
    }
}
