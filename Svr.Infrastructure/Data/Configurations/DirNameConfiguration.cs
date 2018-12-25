using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Svr.Infrastructure.Data.Configurations
{
    public class DirNameConfiguration : IEntityTypeConfiguration<DirName>
    {
        public void Configure(EntityTypeBuilder<DirName> builder)
        {
            
        }
    }
}
