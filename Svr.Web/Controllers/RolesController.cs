using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Infrastructure.Identity;
using Svr.Web.Models;
using Svr.Web.Models.RoleViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    [Authorize(Roles = "Администратор")]
    public class RolesController : Controller
    {
        RoleManager<IdentityRole> roleManager;
        UserManager<ApplicationUser> userManager;
        IDistrictRepository districtRepository;
        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IDistrictRepository districtRepository)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.districtRepository = districtRepository;
        }
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                districtRepository = null;
            }
            base.Dispose(disposing);
        }
        #endregion


        public IActionResult Index() => View(roleManager.Roles.ToList());

        [Authorize(Roles = "Администратор")]
        public IActionResult Create() => View();
        [Authorize(Roles = "Администратор")]
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IdentityResult result = await this.roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(name);
        }

        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await roleManager.DeleteAsync(role);
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult UserList() => View(userManager.Users.ToList());

        public async Task<IActionResult> Edit(string userId)
        {
            // получаем пользователя
            ApplicationUser user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = await userManager.GetRolesAsync(user);
                var allRoles = roleManager.Roles.ToList();
                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserRoles = userRoles,
                    AllRoles = allRoles,
                    DistrictId = user.DistrictId
                };
                ViewBag.Districts = new SelectList(await districtRepository.ListAsync(new DistrictSpecification(null)), "Id", "Name", model.DistrictId);
                return View(model);
            }
            return NotFound();
        }

        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> ResetPassword(string userId)
        {
            ApplicationUser user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await userManager.RemovePasswordAsync(user);
                await userManager.AddPasswordAsync(user, "Test123456789");
                return RedirectToAction(nameof(UserList));
            }
            return NotFound();
        }



        [HttpPost]
        public async Task<IActionResult> Edit(string userId, List<string> roles, long? districtId)
        {
            // получаем пользователя
            ApplicationUser user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.DistrictId = districtId;
                await userManager.UpdateAsync(user);
                //await userManager.AccessFailedAsync(user);
                // получем список ролей пользователя
                var userRoles = await userManager.GetRolesAsync(user);
                // получаем все роли
                var allRoles = roleManager.Roles.ToList();
                // получаем список ролей, которые были добавлены
                var addedRoles = roles.Except(userRoles);
                // получаем роли, которые были удалены
                var removedRoles = userRoles.Except(roles);

                await userManager.AddToRolesAsync(user, addedRoles);

                await userManager.RemoveFromRolesAsync(user, removedRoles);

                return RedirectToAction(nameof(UserList));
            }

            return NotFound();
        }

    }
}