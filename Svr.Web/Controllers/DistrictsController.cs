using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Web.Extensions;
using Svr.Web.Models;
using Svr.Web.Models.DistrictsViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    [Authorize(Roles = "Администратор, Администратор ОПФР")]
    public class DistrictsController : Controller
    {
        private readonly IDistrictRepository repository;
        private readonly IRegionRepository regionRepository;
        private readonly IDistrictPerformerRepository districtPerformerRepository;
        private readonly IPerformerRepository performerRepository;
        private readonly ILogger<DistrictsController> logger;

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
                //repository = null;
                //regionRepository = null;
                //performerRepository = null;
                //districtPerformerRepository = null;
                //logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Index
        // GET: Districts
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string owner = null, string searchString = null, int page = 1, int itemsPage = 10)
        {
            var list = repository.List(new DistrictSpecification(owner.ToLong()));
            //фильтрация
            if (!String.IsNullOrEmpty(searchString))
            {
                list = list.Where(d => d.Name.ToUpper().Contains(searchString.ToUpper()) || d.Code.ToUpper().Contains(searchString.ToUpper()));
            }
            // сортировка
            list = repository.Sort(list, sortOrder);
            // пагинация
            var count = await list.CountAsync();
            var itemsOnPage = await list.Skip((page - 1) * itemsPage).Take(itemsPage).AsNoTracking().ToListAsync();
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
                FilterViewModel = new FilterViewModel(searchString, owner, (await regionRepository.ListAllAsync()).Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (owner == a.Id.ToString()) })),

                StatusMessage = StatusMessage
            };
            return View(indexModel);
        }
        #endregion
        #region Details
        // GET: Districts/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Code = item.Code, Name = item.Name, Description = item.Description, RegionId = item.RegionId, Region = item.Region, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, DistrictPerformers = item.DistrictPerformers };
            return View(model);
        }
        #endregion
        #region Create
        // GET: Districts/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Regions = new SelectList(await regionRepository.ListAllAsync(), "Id", "Name", 1);
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
                var item = await repository.AddAsync(new District { Code = model.Code, Name = model.Name, Description = model.Description, RegionId = model.RegionId });
                if (item != null)
                {
                    StatusMessage = item.MessageAddOk();
                    logger.LogInformation($"{model} create");
                    return RedirectToAction(nameof(Index));
                }
            }
            ModelState.AddModelError(string.Empty, model.MessageAddError());
            ViewBag.Regions = new SelectList(await regionRepository.ListAllAsync(), "Id", "Name", 1);
            return View(model);
        }
        #endregion
        #region Edit
        // get: districts/edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Code = item.Code, Name = item.Name, Description = item.Description, RegionId = item.RegionId, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, DistrictPerformers = item.DistrictPerformers };
            ViewBag.Regions = new SelectList(await regionRepository.ListAllAsync(), "Id", "Name", 1);
            ViewBag.Performers = await performerRepository. ListAsync(new PerformerSpecification(model.RegionId));
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
                    await districtPerformerRepository.ClearAsync(new DistrictPerformerSpecification(model.Id));
                    if (selectedPerformers != null)
                    {
                        foreach (var p in selectedPerformers)
                        {
                            await districtPerformerRepository.AddAsync(new DistrictPerformer { DistrictId = model.Id, PerformerId = p });
                        }
                    }
                    await repository.UpdateAsync(new District { Id = model.Id, Code = model.Code, Description = model.Description, Name = model.Name, CreatedOnUtc = model.CreatedOnUtc, RegionId = model.RegionId });
                    logger.LogInformation($"{model} edit");
                    StatusMessage = model.MessageEditOk();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!(await repository.EntityExistsAsync(model.Id)))
                    {
                        StatusMessage = $"{model.MessageEditError()} {ex.Message}";
                    }
                    else
                    {
                        StatusMessage = $"{model.MessageEditErrorNoknow()} {ex.Message}";
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Regions = new SelectList(await regionRepository.ListAllAsync(), "Id", "Name", 1);
            ViewBag.Performers = await performerRepository.ListAsync(new PerformerSpecification(model.RegionId));
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
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Code = item.Code, Name = item.Name, Description = item.Description, RegionId = item.RegionId, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, StatusMessage = StatusMessage };
            return View(model);
        }
        // POST: Districts/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Администратор")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await repository.DeleteAsync(new District { Id = model.Id, Name = model.Name, Code = model.Code, });
                StatusMessage = model.MessageDeleteOk();
                logger.LogInformation($"{model} delete");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                StatusMessage = $"{model.MessageDeleteError()} {ex.Message}.";
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion
    }
}
