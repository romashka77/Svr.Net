using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Svr.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Svr.Infrastructure.Data.Configurations
{
    public class CategoryDisputeConfiguration : IEntityTypeConfiguration<CategoryDispute>
    {
        public void Configure(EntityTypeBuilder<CategoryDispute> builder)
        {
            //builder.Property(d => d.Name).IsRequired().HasMaxLength(100).IsConcurrencyToken();
        }
    }
}
