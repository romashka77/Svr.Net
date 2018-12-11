using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Svr.Core.Entities;
using Svr.Infrastructure.Data.Configurations;
using Svr.Infrastructure.Data.Extentions;

namespace Svr.Infrastructure.Data
{
    public class DataContext : DbContext
    {
        private readonly string schema = "pereplata";

        public DbSet<Region> Regions { get; set; }
        public DbSet<District> Districts { get; set; }

        //public DbSet<Directory> Directories { get; set; }

        public DbSet<Man> Men { get; set; }
        

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            //автоматическая проверка наличия базы данных и, если она отсуствует, создаст ее.
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Fluent API, https://metanit.com/sharp/entityframeworkcore/2.3.php
            modelBuilder.HasDefaultSchema(this.schema);
            modelBuilder.ApplyConfiguration(new RegionConfiguration());
            modelBuilder.ApplyConfiguration(new DistrictConfiguration());

            modelBuilder.ApplyConfiguration(new ManConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ChangeTracker.ApplyAuditInformation();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangeOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            ChangeTracker.ApplyAuditInformation();
            return await base.SaveChangesAsync(acceptAllChangeOnSuccess, cancellationToken);
        }
    }
}
