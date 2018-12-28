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
        private readonly string schema = "jurist";

        public DbSet<Region> Regions { get; set; }
        public DbSet<District> Districts { get; set; }

        public DbSet<CategoryDispute> CategoryDisputes { get; set; }
        public DbSet<GroupClaim> GroupClaims { get; set; }
        public DbSet<SubjectClaim> SubjectClaims { get; set; }

        public DbSet<DirName> DirName { get; set; }
        public DbSet<Dir> Dir { get; set; }
        public DbSet<Applicant> Applicant { get; set; }

        public DbSet<Performer> Performers { get; set; }


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

            modelBuilder.ApplyConfiguration(new CategoryDisputeConfiguration());
            modelBuilder.ApplyConfiguration(new GroupClaimConfiguration());
            modelBuilder.ApplyConfiguration(new SubjectClaimConfiguration());


            modelBuilder.ApplyConfiguration(new DirNameConfiguration());
            modelBuilder.ApplyConfiguration(new DirConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicantConfiguration());

        modelBuilder.ApplyConfiguration(new PerformerConfiguration());

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
