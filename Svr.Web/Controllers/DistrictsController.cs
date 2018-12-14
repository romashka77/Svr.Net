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
        // GET: Districts
        public async Task<IActionResult> Index(long? region, string name, int page, SortState sortOrder = SortState.NameAsc)
        {
            var itemsPage = 10;
            var filterSpecification = new DistrictSpecification(region);
            var districts = districtRepository.List(filterSpecification);
            //фильтрация
            if (region != null && region != 0)
            {
                districts = districts.Where(d => d.RegionId == region);
            }
            if (!String.IsNullOrEmpty(name))
            {
                districts = districts.Where(d => d.Name.ToUpper().Contains(name.ToUpper()));
            }
            // сортировка
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    districts = districts.OrderByDescending(s => s.Name);
                    break;
                case SortState.CodeAsc:
                    districts = districts.OrderBy(s => s.Code);
                    break;
                case SortState.CodeDesc:
                    districts = districts.OrderByDescending(s => s.Code);
                    break;
                case SortState.RegionAsc:
                    districts = districts.OrderBy(s => s.Region.Name);
                    break;
                case SortState.RegionDesc:
                    districts = districts.OrderByDescending(s => s.Region.Name);
                    break;
                default:
                    districts = districts.OrderBy(s => s.Name);
                    break;
            }
            // пагинация
            var count = districts.Count();
            var items = districts.Skip((page - 1) * itemsPage).Take(itemsPage).ToList();
            var districtIndexModel = new DistrictIndexViewModel()
            {
                DistrictItems = items,
                PageViewModel = new PageViewModel(count, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(regionRepository.ListAll().ToList(), region, name)
            };
            return View(districtIndexModel);
        }

        // GET: Districts/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var district = await districtRepository.GetByIdAsync(id);
            if (district == null)
            {
                throw new ApplicationException($"Не удалось загрузить район с ID {id}.");
            }
            return View(district);
        }

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
        public async Task<IActionResult> Create(CreateViewModel model)
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
            ModelState.AddModelError(string.Empty, "Неудачная попытка регистрации района");
            return View(model);
        }
        // get: districts/edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            var district = await districtRepository.GetByIdAsync(id);
            if (district == null)
            {
                throw new ApplicationException($"Не удалось загрузить район с ID {id}.");
            }
            var model = new DistrictItemViewModel { Id = district.Id, Code = district.Code, Name = district.Name, Description = district.Description,RegionId = district.RegionId };
            ViewBag.StatusMessage = StatusMessage;

            return View(model);
        }

        // POST: Districts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(long id, [Bind("Name,Description,Id,CreatedOnUtc,UpdatedOnUtc")] District district)
        //{
        //    if (id != district.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(district);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!DistrictExists(district.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(district);
        //}

        ////// GET: Districts/Delete/5
        //public async Task<IActionResult> Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var district = await _context.Districts
        //        .SingleOrDefaultAsync(m => m.Id == id);
        //    if (district == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(district);
        //}

        ////// POST: Districts/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(long id)
        //{
        //    var district = await _context.Districts.SingleOrDefaultAsync(m => m.Id == id);
        //    _context.Districts.Remove(district);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool DistrictExists(long id)
        //{
        //    return _context.Districts.Any(e => e.Id == id);
        //}
    }
}
