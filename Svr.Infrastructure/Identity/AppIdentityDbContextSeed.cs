using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Infrastructure.Identity
{
    /// <summary>
    /// Класс для создания администратора и ролей
    /// </summary>
    public class AppIdentityDbContextSeed

    {
        /// <summary>
        /// Создание администратора и ролей
        /// </summary>
        /// <param name="userManager">Менеджер пользователей</param>
        /// <param name="roleManager">Менеджер ролей</param>
        /// <returns></returns>
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string roleAdmin = "Администратор";
            string roleUser = "Пользователь";
            string adminEmail = "romashka_77@mail.ru";
            if (await roleManager.FindByNameAsync(roleAdmin) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleAdmin));
            }
            if (await roleManager.FindByNameAsync(roleUser) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleUser));
            }
            roleUser = "Администратор ОПФР";
            if (await roleManager.FindByNameAsync(roleUser) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleUser));
            }
            roleUser = "Администратор УПФР";
            if (await roleManager.FindByNameAsync(roleUser) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleUser));
            }
            roleUser = "Пользователь ОПФР";
            if (await roleManager.FindByNameAsync(roleUser) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleUser));
            }
            roleUser = "Пользователь УПФР";
            if (await roleManager.FindByNameAsync(roleUser) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleUser));
            }
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                ApplicationUser admin = new ApplicationUser { Email = adminEmail, UserName = adminEmail, FirstName = "Роман", LastName = "Макаров", MiddleName = "Александрович", DateofBirth = DateTime.Parse("12.04.1977") };
                IdentityResult result = await userManager.CreateAsync(admin, "Ram270984");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, roleAdmin);
                }
            }
        }
    }
}
