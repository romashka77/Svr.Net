using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Svr.Core.Interfaces;
using Svr.Infrastructure;
using Svr.Infrastructure.Data;
using Svr.Infrastructure.Identity;
using Svr.Infrastructure.Services;
using Svr.Web.Interfaces;
using Svr.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web
{
    public static class ConfigureContainerExtensions
    {
        public static void AddDbContext(this IServiceCollection services, string dataConnectionString = null, string authConnectionString = null)
        {
            services.AddDbContext<DataContext>(options => options.UseNpgsql(dataConnectionString ?? GetDataConnectionStringFromConfig()));

            services.AddDbContext<AppIdentityDbContext>(options => options.UseNpgsql(authConnectionString ?? GetAuthConnectionStringFromConfig()));

            services.AddIdentity<ApplicationUser, IdentityRole>(opts =>
            {
                //opts.Password.RequiredLength = 5;   // минимальная длина
                opts.Password.RequireNonAlphanumeric = false;   // требуются ли не алфавитно-цифровые символы
                opts.Password.RequireLowercase = false; // требуются ли символы в нижнем регистре
                opts.Password.RequireUppercase = false; // требуются ли символы в верхнем регистре
                opts.Password.RequireDigit = false; // требуются ли цифры
            }).AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();
        }

        public static void AddRepository(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));

            services.AddScoped<IRegionRepository, RegionRepository>();
            services.AddScoped<IDistrictRepository, DistrictRepository>();
            services.AddScoped<IPerformerRepository, PerformerRepository>();
            services.AddScoped <IDistrictPerformerRepository, DistrictPerformerRepository>();

            services.AddScoped<ICategoryDisputeRepository, CategoryDisputeRepositiry>();
            services.AddScoped<IGroupClaimRepository, GroupClaimRepository>();
            services.AddScoped<ISubjectClaimRepository, SubjectClaimRepository>();

            services.AddScoped<IDirNameRepository, DirNameRepository>();
            services.AddScoped<IDirRepository, DirRepository>();
            services.AddScoped<IApplicantRepository, ApplicantRepository>();

            services.AddScoped<IRegionService, RegionService>();


        }

        public static void AddTransientServices(this IServiceCollection services)
        {
            //services.AddTransient<IDirectoryService, DirectoryService>();
            services.AddTransient<IEmailSender, EmailSender>();
        }






        private static string GetAuthConnectionStringFromConfig()
        {
            return new DatabaseConfiguration().GetAuthConnectionString();
        }

        private static string GetDataConnectionStringFromConfig()
        {
            return new DatabaseConfiguration().GetDataConnectionString();
        }
    }
}
