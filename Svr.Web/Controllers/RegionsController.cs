using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Infrastructure.Data;
using Svr.Web.Interfaces;
using Svr.Web.Models;
using Svr.Web.Models.RegionsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    public class RegionsController : Controller
    {
        private ILogger<RegionsController> logger;
        private IRegionRepository regionRepository;
        //private readonly IRegionService regionService;

        [TempData]
        public string StatusMessage { get; set; }

        #region Конструктор
        public RegionsController(IRegionRepository regionRepository, ILogger<RegionsController> logger = null)
        {
            //this.regionService = regionService;
            this.logger = logger;
            this.regionRepository = regionRepository;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                regionRepository = null;
                logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Index
        [HttpGet]
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string currentFilter = null, string searchString = null, int page = 1)
        {
            //logger.LogInformation("Вызван RegionsController.Index.");
            var itemsPage = 10;
            //if (HttpContext.Request.Method == "GET")
            //{
            searchString = currentFilter;
            //}
            //else
            //{
            //    page = 1;
            //}
            IEnumerable<Region> regions = await regionRepository.ListAllAsync();
            //фильтрация
            if (!String.IsNullOrEmpty(searchString))
            {
                regions = regions.Where(p => p.Name.ToUpper().Contains(searchString.ToUpper()) || p.Code.ToUpper().Contains(searchString.ToUpper()));
            }
            //сортировка
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    regions = regions.OrderByDescending(p => p.Name);
                    break;
                case SortState.CodeAsc:
                    regions = regions.OrderBy(p => p.Code);
                    break;
                case SortState.CodeDesc:
                    regions = regions.OrderByDescending(p => p.Code);
                    break;
                default:
                    regions = regions.OrderBy(p => p.Name);
                    break;
            }
            //пагинация
            var totalItems = regions.Count();
            var itemsOnPage = regions.Skip((page - 1) * itemsPage).Take(itemsPage).ToList();
            var regionIndexModel = new IndexViewModel()
            {
                RegionItems = itemsOnPage.Select(i => new ItemViewModel()
                {
                    Id = i.Id,
                    Code = i.Code,
                    Name = i.Name,
                    Description = i.Description,
                    //Districts=i.Districts,
                    CreatedOnUtc = i.CreatedOnUtc,
                    UpdatedOnUtc = i.UpdatedOnUtc
                }),
                PageViewModel = new PageViewModel(totalItems, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString),
                StatusMessage = StatusMessage
            };
            return View(regionIndexModel);
        }
        #endregion
        #region Details
        // GET: Regions/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var region = await regionRepository.GetByIdWithItemsAsync(id);
            if (region == null)
            {
                StatusMessage = $"Не удалось загрузить район с ID = {id}.";
                return RedirectToAction(nameof(Index));
                //throw new ApplicationException($"Не удалось загрузить регион с ID {id}.");
            }
            var model = new ItemViewModel { Id = region.Id, Code = region.Code, Name = region.Name, Description = region.Description, Districts = region.Districts, StatusMessage = StatusMessage, CreatedOnUtc = region.CreatedOnUtc, UpdatedOnUtc = region.UpdatedOnUtc };
            return View(model);
        }
        #endregion
        #region Create
        // GET: Regions/Create
        public IActionResult Create()
        {
            return View();
        }
        // POST: Regions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                //добавляем новый регион
                var region = await regionRepository.AddAsync(new Region { Code = model.Code, Name = model.Name, Description = model.Description });
                if (region != null)
                {
                    StatusMessage = $"Добавлен {region} с Id={region.Id}, Code={region.Code}, Name={region.Name}.";
                    return RedirectToAction(nameof(Index));
                }
            }
            ModelState.AddModelError(string.Empty, $"Ошибка: {model} - неудачная попытка регистрации.");
            return View(model);
        }
        #endregion
        #region Edit
        // GET: Regions/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            var region = await regionRepository.GetByIdAsync(id);
            if (region == null)
            {
                StatusMessage = $"Ошибка: Не удалось найти регион с ID = {id}.";
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = region.Id, Code = region.Code, Name = region.Name, Description = region.Description, StatusMessage = StatusMessage, CreatedOnUtc = region.CreatedOnUtc };
            return View(model);
        }

        // POST: Regions/Edit/5
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
                    await regionRepository.UpdateAsync(new Region { Id = model.Id, Code = model.Code, Description = model.Description, Name = model.Name, Districts = model.Districts, CreatedOnUtc= model.CreatedOnUtc});
                    StatusMessage = $"{model} c ID = {model.Id} обновлен";
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!(await regionRepository.EntityExistsAsync(model.Id)))
                    {
                        StatusMessage = $"Не удалось найти {model} с ID {model.Id}. {ex.Message}";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        StatusMessage = $"Непредвиденная ошибка при обновлении региона с ID {model.Id}. {ex.Message}";
                        return RedirectToAction(nameof(Index));
                    }
                }
                //return RedirectToAction(nameof(Index));
                return RedirectToAction(nameof(Edit));
            }
            return View(model);
        }
        #endregion
        #region Delete
        // GET: Regions/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            var region = await regionRepository.GetByIdAsync(id);
            if (region == null)
            {
                StatusMessage = $"Ошибка: Не удалось найти регион с ID = {id}.";
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = region.Id, Code = region.Code, Name = region.Name, Description = region.Description };
            return View(model);
        }

        // POST: Regions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await regionRepository.DeleteAsync(new Region { Id = model.Id, Name = model.Name, Code = model.Code });
                StatusMessage = $"Удален {model} с Id={model.Id}, Name = {model.Name}, Code = {model.Code}.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка при удалении района с Id={model.Id}, Name = {model.Name}, Code = {model.Code} - {ex.Message}.";
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion
    }
}
