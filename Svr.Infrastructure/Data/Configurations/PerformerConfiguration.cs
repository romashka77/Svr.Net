using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Svr.Infrastructure.Data.Configurations
{
    class PerformerConfiguration : IEntityTypeConfiguration<Performer>
    {
        public void Configure(EntityTypeBuilder<Performer> builder)
        {
            builder.HasOne(d => d.Region).WithMany(r => r.Performers).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
