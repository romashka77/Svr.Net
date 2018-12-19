using Microsoft.AspNetCore.Mvc;
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
        private IDistrictRepository districtRepository;
        private IRegionRepository regionRepository;
        private ILogger<DistrictsController> logger;

        [TempData]
        public string StatusMessage { get; set; }
        #region Конструктор
        public DistrictsController(IDistrictRepository districtRepository, IRegionRepository regionRepository, ILogger<DistrictsController> logger)
        {
            this.logger = logger;
            this.districtRepository = districtRepository;
            this.regionRepository = regionRepository;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                districtRepository = null;
                regionRepository = null;
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
            IEnumerable<District> list = districtRepository.List(filterSpecification);
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
            var districtIndexModel = new IndexViewModel()
            {
                DistrictItems = itemsOnPage.Select(i => new ItemViewModel()
                {
                    Id = i.Id,
                    Code = i.Code,
                    Name = i.Name,
                    Description = i.Description,
                    CreatedOnUtc = i.CreatedOnUtc,
                    UpdatedOnUtc = i.UpdatedOnUtc,
                    Region=i.Region
                }),
                PageViewModel = new PageViewModel(count, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString, owner, regionRepository.ListAll().ToList().Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (owner == a.Id.ToString()) })),

                StatusMessage = StatusMessage
            };
            return View(districtIndexModel);
        }
        #endregion
        #region Details
        // GET: Districts/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var district = await districtRepository.GetByIdWithItemsAsync(id);
            if (district == null)
            {
                StatusMessage = $"Не удалось загрузить район с ID = {id}.";
                return RedirectToAction(nameof(Index));
                //throw new ApplicationException($"Не удалось загрузить район с ID {id}.");
            }
            var model = new ItemViewModel { Id = district.Id, Code = district.Code, Name = district.Name, Description = district.Description, RegionId = district.RegionId, Region = district.Region, StatusMessage = StatusMessage, CreatedOnUtc = district.CreatedOnUtc, UpdatedOnUtc = district.UpdatedOnUtc };
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
                var district = await districtRepository.AddAsync(new District { Code = model.Code, Name = model.Name, Description = model.Description, RegionId = model.RegionId });
                if (district != null)
                {
                    StatusMessage = $"Добавлен район с Id={district.Id}, код={district.Code}, имя={district.Name}.";
                    return RedirectToAction(nameof(Index));
                }
            }
            ModelState.AddModelError(string.Empty, $"Ошибка: {model} - неудачная попытка регистрации.");
            SelectList regions = new SelectList(regionRepository.ListAll(), "Id", "Name", 1);
            ViewBag.Regions = regions;
            return View(model);
        }
        #endregion
        // get: districts/edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            var district = await districtRepository.GetByIdAsync(id);
            if (district == null)
            {
                StatusMessage = $"Ошибка: Не удалось найти район с ID = {id}.";
                return RedirectToAction(nameof(Index));
                //throw new ApplicationException($"Не удалось загрузить район с ID {id}.");
            }
            var model = new ItemViewModel { Id = district.Id, Code = district.Code, Name = district.Name, Description = district.Description, RegionId = district.RegionId, StatusMessage = StatusMessage, CreatedOnUtc = district.CreatedOnUtc };
            SelectList regions = new SelectList(regionRepository.ListAll(), "Id", "Name", 1);
            ViewBag.Regions = regions;
            return View(model);
        }

        // POST: Districts/Edit/5
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
                    await districtRepository.UpdateAsync(new District { Id = model.Id, Code = model.Code, Description = model.Description, Name = model.Name, CreatedOnUtc = model.CreatedOnUtc, RegionId = model.RegionId });
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
                        StatusMessage = $"Непредвиденная ошибка при обновлении района с ID {model.Id}. {ex.Message}";
                        return RedirectToAction(nameof(Index));
                    }
                }
                //return RedirectToAction(nameof(Index));
                return RedirectToAction(nameof(Edit));
            }
            return View(model);
        }

        //// GET: Districts/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            var district = await districtRepository.GetByIdAsync(id);
            if (district == null)
            {
                StatusMessage = $"Ошибка: Не удалось найти район с ID = {id}.";
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = district.Id, Code = district.Code, Name = district.Name, Description = district.Description, RegionId = district.RegionId, CreatedOnUtc = district.CreatedOnUtc, UpdatedOnUtc = district.UpdatedOnUtc, StatusMessage = StatusMessage };
            return View(model);
        }

        //// POST: Districts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await districtRepository.DeleteAsync(new District { Id = model.Id, Name = model.Name, Code = model.Code, });
                StatusMessage = $"Удален {model} с Id={model.Id}, Name = {model.Name}, Code = {model.Code}.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка при удалении района с Id={model.Id}, Name = {model.Name}, Code = {model.Code} - {ex.Message}.";
                return RedirectToAction(nameof(Index));
            }
        }

        //private bool DistrictExists(long id)
        //{
        //    return _context.Districts.Any(e => e.Id == id);
        //}
    }
}
