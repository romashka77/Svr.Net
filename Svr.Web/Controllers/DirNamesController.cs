using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Infrastructure.Data;
using Svr.Web.Models;
using Svr.Web.Models.DirNameViewModels;

namespace Svr.Web.Controllers
{
    [Authorize(Roles = "Администратор")]
    public class DirNamesController : Controller
    {
        private ILogger<DirNamesController> logger;
        private IDirNameRepository repository;
        [TempData]
        public string StatusMessage { get; set; }

        #region Конструктор
        public DirNamesController(IDirNameRepository repository, ILogger<DirNamesController> logger = null)
        {
            this.logger = logger;
            this.repository = repository;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                repository = null;
                logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion


        #region Index
        // GET: DirNames
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string searchString = null, int page = 1, int itemsPage = 10)
        {
            IEnumerable<DirName> list = await repository.ListAllAsync();
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
            var indexModel = new IndexViewModel()
            {
                ItemViewModels = itemsOnPage.Select(i => new ItemViewModel()
                {
                    Id = i.Id,
                    Name = i.Name,
                    CreatedOnUtc = i.CreatedOnUtc,
                    UpdatedOnUtc = i.UpdatedOnUtc
                }),
                PageViewModel = new PageViewModel(totalItems, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString),
                StatusMessage = StatusMessage
            };
            return View(indexModel);
        }
        #endregion
        #region Details
        // GET: DirNames/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = $"Не удалось загрузить справочник с ID = {id}.";
                return RedirectToAction(nameof(Index));
            }

            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Dirs= item.Dirs, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc };
            return View(model);
        }
        #endregion
        #region Create
        // GET: DirNames/Create
        public IActionResult Create()
        {
            return View();
        }
        
        // POST: DirNames/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                //добавляем новый регион
                var item = await repository.AddAsync(new DirName { Name = model.Name});
                if (item != null)
                {
                    StatusMessage = $"Добавлен {item} с Id={item.Id}, Name={item.Name}.";
                    return RedirectToAction(nameof(Index));
                }
            }
            ModelState.AddModelError(string.Empty, $"Ошибка: {model} - неудачная попытка регистрации.");
            return View(model);
        }
        #endregion
        #region Edit
        // GET: DirNames/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
            {
                StatusMessage = $"Ошибка: Не удалось найти справочник с ID = {id}.";
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc };
            return View(model);
        }

        // POST: DirNames/Edit/5
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
                    await repository.UpdateAsync(new DirName { Id = model.Id, Name = model.Name, Dirs = model.Dirs, CreatedOnUtc = model.CreatedOnUtc });
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
                        StatusMessage = $"Непредвиденная ошибка при обновлении региона с ID {model.Id}. {ex.Message}";
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        #endregion
        #region Delete
        // GET: DirNames/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
            {
                StatusMessage = $"Ошибка: Не удалось найти регион с ID = {id}.";
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name};
            return View(model);
        }

        // POST: DirNames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await repository.DeleteAsync(new DirName { Id = model.Id, Name = model.Name});
                StatusMessage = $"Удален {model} с Id={model.Id}, Name = {model.Name}.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка при удалении справочника с Id={model.Id}, Name = {model.Name} - {ex.Message}.";
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion
    }
}
