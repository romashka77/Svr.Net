using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Infrastructure.Data;
using Svr.Web.Extensions;
using Svr.Web.Models;
using Svr.Web.Models.GroupClaimsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    [Authorize(Roles = "Администратор, Администратор ОПФР")]
    public class GroupClaimsController : Controller
    {
        private IGroupClaimRepository repository;
        private ICategoryDisputeRepository categoryDisputeRepository;
        private ILogger<GroupClaimsController> logger;

        [TempData]
        public string StatusMessage { get; set; }

        #region Конструктор
        public GroupClaimsController(IGroupClaimRepository repository, ICategoryDisputeRepository categoryDisputeRepository, ILogger<GroupClaimsController> logger)
        {
            this.logger = logger;
            this.repository = repository;
            this.categoryDisputeRepository = categoryDisputeRepository;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                repository = null;
                categoryDisputeRepository = null;
                logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Index
        // GET: GroupClaims
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string owner = null, string searchString = null, int page = 1, int itemsPage = 10)
        {
            var filterSpecification = new GroupClaimSpecification(owner.ToLong());
            IEnumerable<GroupClaim> list = await repository.ListAsync(filterSpecification);
            //фильтрация
            if (!String.IsNullOrEmpty(searchString))
            {
                list = list.Where(d => d.Name.ToUpper().Contains(searchString.ToUpper()) || d.Code.ToUpper().Contains(searchString.ToUpper()));
            }
            // сортировка
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    list = list.OrderByDescending(p => p.Name);
                    break;
                case SortState.CodeAsc:
                    list = list.OrderBy(p => p.Code);
                    break;
                case SortState.CodeDesc:
                    list = list.OrderByDescending(p => p.Code);
                    break;
                case SortState.DescriptionAsc:
                    list = list.OrderBy(p => p.Description);
                    break;
                case SortState.DescriptionDesc:
                    list = list.OrderByDescending(p => p.Description);
                    break;
                case SortState.CreatedOnUtcAsc:
                    list = list.OrderBy(p => p.CreatedOnUtc);
                    break;
                case SortState.CreatedOnUtcDesc:
                    list = list.OrderByDescending(p => p.CreatedOnUtc);
                    break;
                case SortState.UpdatedOnUtcAsc:
                    list = list.OrderBy(p => p.UpdatedOnUtc);
                    break;
                case SortState.UpdatedOnUtcDesc:
                    list = list.OrderByDescending(p => p.UpdatedOnUtc);
                    break;
                case SortState.OwnerAsc:
                    list = list.OrderBy(s => s.CategoryDispute.Name);
                    break;
                case SortState.OwnerDesc:
                    list = list.OrderByDescending(s => s.CategoryDispute.Name);
                    break;
                default:
                    list = list.OrderBy(s => s.Name);
                    break;
            }
            // пагинация
            var count = list.Count();
            var itemsOnPage = list.Skip((page - 1) * itemsPage).Take(itemsPage).ToList();
            var indexModel = new IndexViewModel()
            {
                ItemViewModels = itemsOnPage.Select(i => new ItemViewModel()
                {
                    Id = i.Id,
                    Code = i.Code,
                    Name = i.Name,
                    Description = i.Description,
                    CreatedOnUtc = i.CreatedOnUtc,
                    UpdatedOnUtc = i.UpdatedOnUtc,
                    CategoryDispute = i.CategoryDispute

                }),
                PageViewModel = new PageViewModel(count, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString, owner, categoryDisputeRepository.ListAll().ToList().Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (owner == a.Id.ToString()) })),

                StatusMessage = StatusMessage
            };
            return View(indexModel);
        }
        #endregion
        #region Details
        //GET: GroupClaims/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = $"Не удалось загрузить район с ID = {id}.";
                return RedirectToAction(nameof(Index));
                //throw new ApplicationException($"Не удалось загрузить район с ID {id}.");
            }
            var model = new ItemViewModel { Id = item.Id, Code = item.Code, Name = item.Name, Description = item.Description, CategoryDisputeId = item.CategoryDisputeId, CategoryDispute = item.CategoryDispute, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, SubjectClaims = item.SubjectClaims };
            return View(model);
        }
        #endregion
        #region Create
        // GET: GroupClaims/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.CategoryDisputes = new SelectList(await categoryDisputeRepository.ListAllAsync(), "Id", "Name", 1);
            return View();
        }
        // POST: GroupClaims/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var item = await repository.AddAsync(new GroupClaim { Code = model.Code, Name = model.Name, Description = model.Description, CategoryDisputeId = model.CategoryDisputeId });
                if (item != null)
                {
                    StatusMessage = $"Добавлена группа исков с Id={item.Id}, код={item.Code}, имя={item.Name}.";
                    return RedirectToAction(nameof(Index));
                }
            }
            ModelState.AddModelError(string.Empty, $"Ошибка: {model} - неудачная попытка регистрации.");
            ViewBag.CategoryDisputes = new SelectList(await categoryDisputeRepository.ListAllAsync(), "Id", "Name", 1);
            return View(model);
        }
        #endregion
        #region Edit
        // GET: GroupClaims/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
            {
                StatusMessage = $"Ошибка: Не удалось найти группу исков с ID = {id}.";
                return RedirectToAction(nameof(Index));
                //throw new ApplicationException($"Не удалось загрузить район с ID {id}.");
            }
            var model = new ItemViewModel { Id = item.Id, Code = item.Code, Name = item.Name, Description = item.Description, CategoryDisputeId = item.CategoryDisputeId, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc };
            ViewBag.CategoryDisputes = new SelectList(await categoryDisputeRepository.ListAllAsync(), "Id", "Name", 1);
            return View(model);
        }
        // POST: GroupClaims/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await repository.UpdateAsync(new GroupClaim { Id = model.Id, Code = model.Code, Description = model.Description, Name = model.Name, CreatedOnUtc = model.CreatedOnUtc, CategoryDisputeId = model.CategoryDisputeId });
                    StatusMessage = $"{model} c ID = {model.Id} обновлен";
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!(await repository.EntityExistsAsync(model.Id)))
                    {
                        StatusMessage = $"Не удалось найти {model} с ID {model.Id}. {ex.Message}";
                    }
                    else
                    {
                        StatusMessage = $"Непредвиденная ошибка при обновлении района с ID {model.Id}. {ex.Message}";
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CategoryDisputes = new SelectList(await categoryDisputeRepository.ListAllAsync(), "Id", "Name", 1);
            return View(model);
        }
        #endregion
        #region Delete
        // GET: GroupClaims/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
            {
                StatusMessage = $"Ошибка: Не удалось найти группу исков с ID = {id}.";
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Code = item.Code, Name = item.Name, Description = item.Description, CategoryDisputeId = item.CategoryDisputeId, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, StatusMessage = StatusMessage };
            return View(model);
        }
        // POST: GroupClaims/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await repository.DeleteAsync(new GroupClaim { Id = model.Id, Name = model.Name, Code = model.Code, });
                StatusMessage = $"Удален {model} с Id={model.Id}, Name = {model.Name}, Code = {model.Code}.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка при удалении группы исков с Id={model.Id}, Name = {model.Name}, Code = {model.Code} - {ex.Message}.";
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion
    }
}
