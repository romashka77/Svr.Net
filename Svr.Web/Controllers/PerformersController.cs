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
using Svr.Web.Models.PerformersViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    [Authorize(Roles = "Администратор, Администратор ОПФР")]
    public class PerformersController : Controller
    {
        private IPerformerRepository repository;
        private IRegionRepository regionRepository;
        private IDistrictRepository districtRepository;
        private IDistrictPerformerRepository districtPerformerRepository;
        private ILogger<PerformersController> logger;

        [TempData]
        public string StatusMessage { get; set; }
        #region Конструктор
        public PerformersController(IPerformerRepository repository, IRegionRepository regionRepository, IDistrictRepository districtRepository, IDistrictPerformerRepository districtPerformerRepository, ILogger<PerformersController> logger)
        {
            this.logger = logger;
            this.repository = repository;
            this.regionRepository = regionRepository;
            this.districtRepository = districtRepository;
            this.districtPerformerRepository = districtPerformerRepository;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                districtRepository = null;
                repository = null;
                regionRepository = null;
                districtPerformerRepository = null;
                logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Index
        // GET: Performers
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string owner = null, string searchString = null, int page = 1, int itemsPage = 10)
        {
            var filterSpecification = new PerformerSpecification(owner.ToLong());
            IEnumerable<Performer> list = await repository.ListAsync(filterSpecification);
            //фильтрация
            if (!String.IsNullOrEmpty(searchString))
            {
                list = list.Where(d => d.Name.ToUpper().Contains(searchString.ToUpper()));
            }
            // сортировка
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
        // GET: Performers/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = $"Не удалось загрузить исполнителя с ID = {id}.";
                return RedirectToAction(nameof(Index));
                //throw new ApplicationException($"Не удалось загрузить район с ID {id}.");
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, RegionId = item.RegionId, Region = item.Region, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, DistrictPerformers = item.DistrictPerformers };
            return View(model);
        }
        #endregion
        #region Create
        // GET: Performers/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Regions = new SelectList(await regionRepository.ListAllAsync(), "Id", "Name", 1);
            return View();
        }

        // POST: Performers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                // добавляем новый Район
                var item = await repository.AddAsync(new Performer { Name = model.Name, Description = model.Description, RegionId = model.RegionId });
                if (item != null)
                {
                    StatusMessage = $"Добавлен исполнитель" +
                        $" с Id={item.Id}, имя={item.Name}.";
                    return RedirectToAction(nameof(Index));
                }
            }
            ModelState.AddModelError(string.Empty, $"Ошибка: {model} - неудачная попытка регистрации.");
            ViewBag.Regions = new SelectList(await regionRepository.ListAllAsync(), "Id", "Name", 1);
            return View(model);
        }
        #endregion
        #region Edit
        // GET: Performers/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            var item = await repository.GetByIdAsync(id);
            //District item = await repository.Table().Include(e => e.DistrictPerformers).SingleOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                StatusMessage = $"Ошибка: Не удалось найти район с ID = {id}.";
                return RedirectToAction(nameof(Index));
                //throw new ApplicationException($"Не удалось загрузить район с ID {id}.");
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, RegionId = item.RegionId, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, DistrictPerformers = item.DistrictPerformers };
            ViewBag.Regions = new SelectList(await regionRepository.ListAllAsync(), "Id", "Name", 1);
            ViewBag.Districts = await districtRepository.ListAsync(new DistrictSpecification(model.RegionId));
            return View(model);
        }
        // POST: Performers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ItemViewModel model, long[] selectedDistricts)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var filterSpecification = new PerformerDistrictSpecification(model.Id);
                    await districtPerformerRepository.ClearAsync(filterSpecification);

                    if (selectedDistricts != null)
                    {
                        foreach (var d in selectedDistricts)
                        {
                            await districtPerformerRepository.AddAsync(new DistrictPerformer { DistrictId = d, PerformerId = model.Id });
                        }
                    }

                    await repository.UpdateAsync(new Performer { Id = model.Id, Description = model.Description, Name = model.Name, CreatedOnUtc = model.CreatedOnUtc, RegionId = model.RegionId });

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
            ViewBag.Regions = new SelectList(await regionRepository.ListAllAsync(), "Id", "Name", 1);
            ViewBag.Districts = await districtRepository.ListAsync(new DistrictSpecification(model.RegionId));
            return View(model);
        }
        #endregion
        // GET: Performers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
            {
                StatusMessage = $"Ошибка: Не удалось найти исполнителя с ID = {id}.";
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, StatusMessage = StatusMessage };
            return View(model);
        }

        // POST: Performers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await repository.DeleteAsync(new Performer { Id = model.Id, Name = model.Name });
                StatusMessage = $"Удален {model} с Id={model.Id}, Name = {model.Name}.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка при удалении исполнителя с Id={model.Id}, Name = {model.Name} - {ex.Message}.";
                return RedirectToAction(nameof(Index));
            }
        }


    }
}
