using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Svr.Infrastructure.Data.Configurations
{
public    class FileEntityConfiguration : IEntityTypeConfiguration<FileEntity>
    {
        public void Configure(EntityTypeBuilder<FileEntity> builder)
        {
            builder.HasOne(d => d.Claim).WithMany(r => r.FileEntities).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
