using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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

namespace Svr.Web.Controllers
{
    public class RegionsController : Controller
    {
        private readonly ILogger<RegionsController> logger;
        private readonly IRegionRepository regionRepository;
        //private readonly IRegionService regionService;

        [TempData]
        public string StatusMessage { get; set; }

        #region конструктор
        public RegionsController(IRegionRepository regionRepository, ILogger<RegionsController> logger = null)
        {
            //this.regionService = regionService;
            this.logger = logger;
            this.regionRepository = regionRepository;
        }
        #endregion
        #region Index
        //[HttpPost]
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
            var regionIndexModel = new RegionIndexViewModel()
            {
                RegionItems = itemsOnPage.Select(i => new RegionItemViewModel()
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
                FilterViewModel = new FilterViewModel(searchString)
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
                throw new ApplicationException($"Не удалось загрузить регион с ID '{id}'.");
            }
            return View(region);
        }
        #endregion
        #region Create
        //        // GET: Regions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Regions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegionItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                // добавляем новый регион
                var region = await regionRepository.AddAsync(new Region { Code = model.Code, Name = model.Name, Description = model.Description });
                if (region != null)
                {
                    StatusMessage = $"Добавлен регион с Id='{region.Id}', код='{region.Code}', имя='{region.Name}'.";
                    return RedirectToAction(nameof(Index));
                }
            }
            ModelState.AddModelError(string.Empty, "Неудачная попытка регистрации региона");
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
                throw new ApplicationException($"Не удалось загрузить регион с ID '{id}'.");
            }
            var model = new RegionItemViewModel { Id = region.Id, Code = region.Code, Name = region.Name, Description = region.Description };
            ViewBag.StatusMessage = StatusMessage;
            return View(model);
        }

        // POST: Regions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RegionItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var region = await regionRepository.GetByIdAsync(model.Id);
                if (region == null)
                {
                    throw new ApplicationException($"Не удалось загрузить регион с ID '{model.Id}'.");
                    //return NotFound();
                }
                try
                {
                    if ((region.Name != model.Name) || (region.Description != model.Description) || (region.Code != model.Code))
                    {
                        region.Code = model.Code;
                        region.Name = model.Name;
                        region.Description = model.Description;
                        await regionRepository.UpdateAsync(region);
                        StatusMessage = "Регион обновлен";
                    }
                    else { StatusMessage = "Регион не изменен"; }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await regionRepository.EntityExistsAsync(region.Id)))
                    {
                        throw new ApplicationException($"Не удалось загрузить регион с ID '{region.Id}'.");
                    }
                    else
                    {
                        throw new ApplicationException($"Непредвиденная ошибка при обновлении региона с ID '{region.Id}'.");
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
                throw new ApplicationException($"Не удалось загрузить регион с ID '{id}'.");
                //return NotFound();
            }
            var model = new RegionItemViewModel { Id = region.Id, Code = region.Code, Name = region.Name, Description = region.Description};
            return View(model);
        }

        // POST: Regions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(RegionItemViewModel model)
        {
            try
            {
                await regionRepository.DeleteAsync(new Region { Id = model.Id, Name = model.Name, Code = model.Code });
                StatusMessage = $"Удален регион с Id='{model.Id}', код='{model.Code}', имя='{model.Name}'.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка при удалении региона с Id='{model.Id}', код='{model.Code}', имя='{model.Name}' - '{ex.Message}'.";
                return RedirectToAction(nameof(Index));
                //throw new ApplicationException(StatusMessage);
            }
        }
        #endregion
    }
}
