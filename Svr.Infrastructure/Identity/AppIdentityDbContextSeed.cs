using Microsoft.AspNetCore.Identity;
using Svr.Core.Entities;
using Svr.Core.Extensions;
using System;
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
            const string adminEmail = "romashka_77@mail.ru";
            foreach (RoleState item in Enum.GetValues(typeof(RoleState)))
            {
                if (await roleManager.FindByNameAsync(item.GetDescription()) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(item.GetDescription()));
                }
            }
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                ApplicationUser admin = new ApplicationUser { Email = adminEmail, UserName = adminEmail, FirstName = "Роман", LastName = "Макаров", MiddleName = "Александрович", DateofBirth = DateTime.Parse("12.04.1977") };
                IdentityResult result = await userManager.CreateAsync(admin, "Ram270984");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, RoleState.Administrator.GetDescription());
                }
            }
        }
    }
}
