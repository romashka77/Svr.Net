﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Infrastructure.Data;
using Svr.Web.Models;
using Svr.Web.Models.DistrictsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    public class DistrictsController : Controller
    {
        private IDistrictRepository repository;
        private IRegionRepository regionRepository;
        private IDistrictPerformerRepository districtPerformerRepository;
        private IPerformerRepository performerRepository;
        private ILogger<DistrictsController> logger;

        [TempData]
        public string StatusMessage { get; set; }
        #region Конструктор
        public DistrictsController(IDistrictRepository repository, IRegionRepository regionRepository, IPerformerRepository performerRepository, IDistrictPerformerRepository districtPerformerRepository, ILogger<DistrictsController> logger)
        {
            this.logger = logger;
            this.repository = repository;
            this.regionRepository = regionRepository;
            this.performerRepository = performerRepository;
            this.districtPerformerRepository = districtPerformerRepository;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                repository = null;
                regionRepository = null;
                performerRepository = null;
                districtPerformerRepository = null;
                logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Index
        // GET: Districts
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string owner = null, string searchString = null, int page = 1, int itemsPage = 10)
        {
            long? _owner = null;
            if (!String.IsNullOrEmpty(owner))
            {
                _owner = Int64.Parse(owner);
            }
            var filterSpecification = new DistrictSpecification(_owner);
            IEnumerable<District> list = repository.List(filterSpecification);
            //фильтрация
            if (_owner != null)
            {
                list = list.Where(d => d.RegionId == _owner);
            }
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
                    list = list.OrderBy(s => s.Region.Name);
                    break;
                case SortState.OwnerDesc:
                    list = list.OrderByDescending(s => s.Region.Name);
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
                    Region = i.Region
                }),
                PageViewModel = new PageViewModel(count, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString, owner, regionRepository.ListAll().ToList().Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (owner == a.Id.ToString()) })),

                StatusMessage = StatusMessage
            };
            return View(indexModel);
        }
        #endregion
        #region Details
        // GET: Districts/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            District item = await repository.Table().Include(e => e.Region).Include(e => e.DistrictPerformers).ThenInclude(e => e.Performer).SingleOrDefaultAsync(m => m.Id == id);
            //var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = $"Не удалось загрузить район с ID = {id}.";
                return RedirectToAction(nameof(Index));
                //throw new ApplicationException($"Не удалось загрузить район с ID {id}.");
            }
            var model = new ItemViewModel { Id = item.Id, Code = item.Code, Name = item.Name, Description = item.Description, RegionId = item.RegionId, Region = item.Region, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, DistrictPerformers = item.DistrictPerformers };
            return View(model);
        }
        #endregion
        #region Create
        // GET: Districts/Create
        public IActionResult Create()
        {
            SelectList regions = new SelectList(regionRepository.ListAll(), "Id", "Name", 1);
            ViewBag.Regions = regions;
            return View();
        }

        // POST: Districts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                // добавляем новый Район
                var item = await repository.AddAsync(new District { Code = model.Code, Name = model.Name, Description = model.Description, RegionId = model.RegionId });
                if (item != null)
                {
                    StatusMessage = $"Добавлен район с Id={item.Id}, код={item.Code}, имя={item.Name}.";
                    return RedirectToAction(nameof(Index));
                }
            }
            ModelState.AddModelError(string.Empty, $"Ошибка: {model} - неудачная попытка регистрации.");
            SelectList regions = new SelectList(regionRepository.ListAll(), "Id", "Name", 1);
            ViewBag.Regions = regions;
            return View(model);
        }
        #endregion
        #region Edit
        // get: districts/edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            //District item = await repository.GetByIdAsync(id);
            District item = await repository.Table().Include(e => e.DistrictPerformers).SingleOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                StatusMessage = $"Ошибка: Не удалось найти район с ID = {id}.";
                return RedirectToAction(nameof(Index));
                //throw new ApplicationException($"Не удалось загрузить район с ID {id}.");
            }
            var model = new ItemViewModel { Id = item.Id, Code = item.Code, Name = item.Name, Description = item.Description, RegionId = item.RegionId, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, DistrictPerformers = item.DistrictPerformers };
            SelectList regions = new SelectList(await regionRepository.ListAllAsync(), "Id", "Name", 1);
            ViewBag.Regions = regions;

            ViewBag.Performers = await performerRepository.ListAllAsync();
            return View(model);
        }
        // POST: Districts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ItemViewModel model, long[] selectedPerformers)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var filterSpecification = new DistrictPerformerSpecification(model.Id);
                    await districtPerformerRepository.ClearAsync(filterSpecification);

                    if (selectedPerformers != null)
                    {
                        foreach (var p in selectedPerformers)
                        {
                            await districtPerformerRepository.AddAsync(new DistrictPerformer { DistrictId = model.Id, PerformerId = p });
                        }
                    }
                    
                    await repository.UpdateAsync(new District { Id = model.Id, Code = model.Code, Description = model.Description, Name = model.Name, CreatedOnUtc = model.CreatedOnUtc, RegionId = model.RegionId });

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
            SelectList regions = new SelectList(regionRepository.ListAll(), "Id", "Name", 1);
            ViewBag.Regions = regions;
            return View(model);
        }
        #endregion
        #region Delete
        // GET: Districts/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
            {
                StatusMessage = $"Ошибка: Не удалось найти район с ID = {id}.";
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Code = item.Code, Name = item.Name, Description = item.Description, RegionId = item.RegionId, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, StatusMessage = StatusMessage };
            return View(model);
        }
        // POST: Districts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await repository.DeleteAsync(new District { Id = model.Id, Name = model.Name, Code = model.Code, });
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
