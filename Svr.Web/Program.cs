﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Svr.Infrastructure.Data;
using Svr.Infrastructure.Identity;
using System;

namespace Svr.Web
{
    public class Program
    {
        //public static async Task Main(string[] args)
        public static void Main(string[] args)
        {
            //BuildWebHost(args).Run();
            var host = BuildWebHost(args);
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    // Создание менеджера пользователей
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    // Создание менеджера ролей
                    var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    /*await*/
                    AppIdentityDbContextSeed.SeedAsync(userManager, rolesManager);

                    var dataContext = services.GetRequiredService<DataContext>();
                    //static
                    DataContextSeed.SeedAsync(dataContext/*, loggerFactory*/).Wait();

                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Произошла ошибка при заполнении базы данных.");
                }
            }
            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .Build();
    }
}
