using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Svr.Infrastructure.Data.Configurations
{
    public class DirConfiguration : IEntityTypeConfiguration<Dir>
    {
        public void Configure(EntityTypeBuilder<Dir> builder)
        {
            builder.HasOne(d => d.DirName).WithMany(r => r.Dirs).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
