using Microsoft.EntityFrameworkCore;
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
