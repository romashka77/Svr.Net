using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Extensions;
using Svr.Core.Interfaces;
using Svr.Infrastructure.Data;
using Svr.Web.Extensions;
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
    [Authorize(Roles = "Администратор, Администратор ОПФР")]
    public class RegionsController : Controller
    {
        private ILogger<RegionsController> logger;
        private IRegionRepository regionRepository;

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
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string searchString = null, int page = 1, int itemsPage = 10)
        {
            var list = regionRepository.Table();
            //фильтрация
            if (!String.IsNullOrEmpty(searchString))
            {
                list = list.Where(p => p.Name.ToUpper().Contains(searchString.ToUpper()) || p.Code.ToUpper().Contains(searchString.ToUpper()));
            }
            //сортировка
            list = regionRepository.Sort(list, sortOrder);
            //пагинация
            var totalItems = await list.CountAsync();
            var itemsOnPage = await list.Skip((page - 1) * itemsPage).Take(itemsPage).AsNoTracking().ToListAsync();
            var indexModel = new IndexViewModel()
            {
                ItemViewModels = itemsOnPage.Select(i => new ItemViewModel()
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
            return View(indexModel);
        }
        #endregion
        #region Details
        // GET: Regions/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var region = await regionRepository.GetByIdWithItemsAsync(id);
            if (region == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = region.Id, Code = region.Code, Name = region.Name, Description = region.Description, Districts = region.Districts, StatusMessage = StatusMessage, CreatedOnUtc = region.CreatedOnUtc, UpdatedOnUtc = region.UpdatedOnUtc,Performers = region.Performers };
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
                var item = await regionRepository.AddAsync(new Region { Code = model.Code, Name = model.Name, Description = model.Description });
                if (item != null)
                {
                    StatusMessage = item.MessageAddOk();
                    return RedirectToAction(nameof(Index));
                }
            }
            ModelState.AddModelError(string.Empty, model.MessageAddError());
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
                StatusMessage = id.ToString().ErrorFind();
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
                    StatusMessage = model.MessageEditOk();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!(await regionRepository.EntityExistsAsync(model.Id)))
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
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = region.Id, Code = region.Code, Name = region.Name, Description = region.Description };
            return View(model);
        }

        // POST: Regions/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Администратор")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await regionRepository.DeleteAsync(new Region { Id = model.Id, Name = model.Name, Code = model.Code });
                StatusMessage = model.MessageDeleteOk();
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
