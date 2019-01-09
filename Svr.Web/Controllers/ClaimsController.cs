using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Infrastructure.Data;
using Svr.Web.Models;
using Svr.Web.Models.ClaimsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    public class ClaimsController : Controller
    {
        private IClaimRepository repository;
        private IDistrictRepository districtRepository;
        private IRegionRepository regionRepository;
        private ILogger<ClaimsController> logger;

        [TempData]
        public string StatusMessage { get; set; }
        #region Конструктор
        public ClaimsController(IClaimRepository repository, IRegionRepository regionRepository, IDistrictRepository districtRepository, ILogger<ClaimsController> logger)
        {
            this.logger = logger;
            this.repository = repository;
            this.regionRepository = regionRepository;
            this.districtRepository = districtRepository;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                repository = null;
                districtRepository = null;
                regionRepository = null;
                logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Index
        // GET: Claims
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string lord = null, string owner = null, string searchString = null, int page = 1, int itemsPage = 10)
        {
            long? _owner = null;
            if (!String.IsNullOrEmpty(owner))
            {
                _owner = Int64.Parse(owner);
            }
            var filterSpecification = new ClaimSpecification(_owner);
            var list = repository.List(filterSpecification);
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

            long? _lord = null;
            if (!String.IsNullOrEmpty(lord))
            {
                _lord = Int64.Parse(lord);
            }

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
                    Region = i.Region,
                    District = i.District
                }),
                PageViewModel = new PageViewModel(count, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString, owner, (await districtRepository.ListAsync(new DistrictSpecification(_lord))).Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (owner == a.Id.ToString()) }), lord, (await regionRepository.ListAllAsync()).ToList().Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (lord == a.Id.ToString()) })),

                StatusMessage = StatusMessage
            };
            return View(indexModel);

            //var dataContext = _context.Claims.Include(c => c.CategoryDispute).Include(c => c.District).Include(c => c.GroupClaim).Include(c => c.Performer).Include(c => c.Person3rd).Include(c => c.Plaintiff).Include(c => c.Region).Include(c => c.Respondent).Include(c => c.Сourt);
            //    return View(await dataContext.ToListAsync());
        }
        #endregion
        #region Details
        // GET: Claims/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = $"Не удалось загрузить иск с ID = {id}.";
                return RedirectToAction(nameof(Index));
                //throw new ApplicationException($"Не удалось загрузить район с ID {id}.");
            }
            var model = new ItemViewModel { Id = item.Id, Code = item.Code, Name = item.Name, Description = item.Description, RegionId = item.RegionId, Region = item.Region, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, District = item.District };
            return View(model);
        }
        #endregion
        #region Create
        // GET: Claims/Create
        public async Task<IActionResult> Create(string lord = null, string owner = null)
        {
            long? _lord = null;
            if (!String.IsNullOrEmpty(lord))
            {
                _lord = Int64.Parse(lord);
            }
            ViewBag.Regions = new SelectList(await regionRepository.ListAllAsync(),"Id", "Name", lord);
            ViewBag.Districts = new SelectList(await districtRepository.ListAsync(new DistrictSpecification(_lord)), "Id", "Name", owner);
            return View();

            //ViewData["CategoryDisputeId"] = new SelectList(_context.CategoryDisputes, "Id", "Name");
            //ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "Code");
            //ViewData["GroupClaimId"] = new SelectList(_context.GroupClaims, "Id", "Code");
            //ViewData["PerformerId"] = new SelectList(_context.Performers, "Id", "Name");
            //ViewData["Person3rdId"] = new SelectList(_context.Applicant, "Id", "Name");
            //ViewData["PlaintiffId"] = new SelectList(_context.Applicant, "Id", "Name");
            //ViewData["RegionId"] = new SelectList(_context.Regions, "Id", "Code");
            //ViewData["RespondentId"] = new SelectList(_context.Applicant, "Id", "Name");
            //ViewData["СourtId"] = new SelectList(_context.Dir, "Id", "Name");
        }

        // POST: Claims/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                // добавляем новый Район
                var item = await repository.AddAsync(new Claim { Code = model.Code, Name = model.Name, Description = model.Description, RegionId = model.District.RegionId, DistrictId = model.DistrictId });
                if (item != null)
                {
                    StatusMessage = $"Добавлен район с Id={item.Id}, код={item.Code}, имя={item.Name}.";
                    return RedirectToAction(nameof(Edit), new { id = item.Id });
                }
            }
            ModelState.AddModelError(string.Empty, $"Ошибка: {model} - неудачная попытка регистрации.");
            ViewBag.Regions = new SelectList(await regionRepository.ListAllAsync(), "Id", "Name", model.RegionId);
            ViewBag.Districts = new SelectList(await districtRepository.ListAsync(new DistrictSpecification(model.RegionId)), "Id", "Name", model.DistrictId);
            return View(model);
        }
        #endregion
        #region Edit
        // GET: Claims/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = $"Ошибка: Не удалось найти район с ID = {id}.";
                return RedirectToAction(nameof(Index));
                //throw new ApplicationException($"Не удалось загрузить район с ID {id}.");
            }
            var model = new ItemViewModel { Id = item.Id, Code = item.Code, Name = item.Name, Description = item.Description, RegionId = item.RegionId, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, DistrictId = item.DistrictId };
            ViewBag.Regions = new SelectList(await regionRepository.ListAllAsync(), "Id", "Name", model.RegionId);
            ViewBag.Districts = new SelectList(await districtRepository.ListAsync(new DistrictSpecification(model.RegionId)), "Id", "Name", model.DistrictId);
            return View(model);
        }

        // POST: Claims/Edit/5
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
                    await repository.UpdateAsync(new Claim { Id = model.Id, Code = model.Code, Description = model.Description, Name = model.Name, CreatedOnUtc = model.CreatedOnUtc, CategoryDisputeId = model.CategoryDisputeId, RegionId = model.RegionId, DistrictId = model.DistrictId });
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
            ViewBag.Regions = new SelectList(await regionRepository.ListAllAsync(), "Id", "Name", model.RegionId);
            ViewBag.Districts = new SelectList(await districtRepository.ListAsync(new DistrictSpecification(model.RegionId)), "Id", "Name", model.DistrictId);
            return View(model);
        }
        #endregion
        #region Delete
        // GET: Claims/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
            {
                StatusMessage = $"Ошибка: Не удалось найти группу исков с ID = {id}.";
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Code = item.Code, Name = item.Name, Description = item.Description, CategoryDisputeId = item.CategoryDisputeId, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, StatusMessage = StatusMessage, RegionId = item.RegionId, DistrictId = item.DistrictId };
            return View(model);
        }

        // POST: Claims/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await repository.DeleteAsync(new Claim { Id = model.Id, Name = model.Name, Code = model.Code, });
                StatusMessage = $"Удален {model} с Id={model.Id}, Name = {model.Name}, Code = {model.Code}.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка при удалении иска с Id={model.Id}, Name = {model.Name}, Code = {model.Code} - {ex.Message}.";
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion
    }
}
