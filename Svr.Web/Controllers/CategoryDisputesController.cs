using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Infrastructure.Data;
using Svr.Web.Models;
using Svr.Web.Models.CategoryDisputesViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    public class CategoryDisputesController : Controller
    {
        private ILogger<CategoryDisputesController> logger;
        private ICategoryDisputeRepository сategoryDisputeRepository;

        [TempData]
        public string StatusMessage { get; set; }
        #region Конструктор
        public CategoryDisputesController(ICategoryDisputeRepository сategoryDisputeRepository, ILogger<CategoryDisputesController> logger = null)
        {
            this.сategoryDisputeRepository = сategoryDisputeRepository;
            this.logger = logger;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                сategoryDisputeRepository = null;
                logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Index
        // GET: CategoryDisputes
        [HttpGet]
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string currentFilter = null, string searchString = null, int page = 1, int itemsPage = 10)
        {
            IEnumerable<CategoryDispute> list = await сategoryDisputeRepository.ListAllAsync();
            //фильтрация
            if (!String.IsNullOrEmpty(searchString))
            {
                list = list.Where(p => p.Name.ToUpper().Contains(searchString.ToUpper()));
            }
            //сортировка
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    list = list.OrderByDescending(p => p.Name);
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
                default:
                    list = list.OrderBy(p => p.Name);
                    break;
            }
            //пагинация
            var totalItems = list.Count();
            var itemsOnPage = list.Skip((page - 1) * itemsPage).Take(itemsPage).ToList();
            var categoryDisputeIndexModel = new IndexViewModel()
            {
                CategoryDisputeItems = itemsOnPage.Select(i => new ItemViewModel()
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    CreatedOnUtc = i.CreatedOnUtc,
                    UpdatedOnUtc = i.UpdatedOnUtc
                }),
                PageViewModel = new PageViewModel(totalItems, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString),
                StatusMessage = StatusMessage
            };
            return View(categoryDisputeIndexModel);
        }
        #endregion
        #region Details
        // GET: CategoryDisputes/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var сategoryDispute = await сategoryDisputeRepository.GetByIdWithItemsAsync(id);
            if (сategoryDispute == null)
            {
                StatusMessage = $"Не удалось загрузить категорию споров с ID = {id}.";
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = сategoryDispute.Id, Name = сategoryDispute.Name, Description = сategoryDispute.Description, GroupClaims = сategoryDispute.GroupClaims, StatusMessage = StatusMessage, CreatedOnUtc = сategoryDispute.CreatedOnUtc, UpdatedOnUtc = сategoryDispute.UpdatedOnUtc };
            return View(model);
        }
        #endregion
        #region Create
        // GET: CategoryDisputes/Create
        public IActionResult Create()
        {
            return View();
        }
        // POST: CategoryDisputes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                //добавляем новый регион
                var сategoryDispute = await сategoryDisputeRepository.AddAsync(new CategoryDispute { Name = model.Name, Description = model.Description });
                if (сategoryDispute != null)
                {
                    StatusMessage = $"Добавлен {сategoryDispute} с Id={сategoryDispute.Id}, Name={сategoryDispute.Name}.";
                    return RedirectToAction(nameof(Index));
                }
            }
            ModelState.AddModelError(string.Empty, $"Ошибка: {model} - неудачная попытка регистрации.");
            return View(model);
        }
        #endregion
        #region Edit
        // GET: CategoryDisputes/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            var сategoryDispute = await сategoryDisputeRepository.GetByIdAsync(id);
            if (сategoryDispute == null)
            {
                StatusMessage = $"Ошибка: Не удалось найти категорию споров с ID = {id}.";
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = сategoryDispute.Id, Name = сategoryDispute.Name, Description = сategoryDispute.Description, StatusMessage = StatusMessage, CreatedOnUtc = сategoryDispute.CreatedOnUtc };
            return View(model);
        }

        // POST: CategoryDisputes/Edit/5
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
                    await сategoryDisputeRepository.UpdateAsync(new CategoryDispute { Id = model.Id, Description = model.Description, Name = model.Name, CreatedOnUtc = model.CreatedOnUtc /*Districts = model.Districts*/});
                    StatusMessage = $"{model} c ID = {model.Id} обновлен";
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!(await сategoryDisputeRepository.EntityExistsAsync(model.Id)))
                    {
                        StatusMessage = $"Не удалось найти {model} с ID {model.Id}. {ex.Message}";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        StatusMessage = $"Непредвиденная ошибка при обновлении категории споров с ID {model.Id}. {ex.Message}";
                        return RedirectToAction(nameof(Index));
                    }
                }
                return RedirectToAction(nameof(Edit));
            }
            return View(model);
        }
        #endregion
        #region Delete
        // GET: CategoryDisputes/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            var сategoryDispute = await сategoryDisputeRepository.GetByIdAsync(id);
            if (сategoryDispute == null)
            {
                StatusMessage = $"Ошибка: Не удалось найти категорию диспутов с ID = {id}.";
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = сategoryDispute.Id, Name = сategoryDispute.Name, Description = сategoryDispute.Description };
            return View(model);
        }

        // POST: CategoryDisputes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await сategoryDisputeRepository.DeleteAsync(new CategoryDispute { Id = model.Id, Name = model.Name});
                StatusMessage = $"Удален {model} с Id={model.Id}, Name = {model.Name}.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка при удалении района с Id={model.Id}, Name = {model.Name} - {ex.Message}.";
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion
    }
}
