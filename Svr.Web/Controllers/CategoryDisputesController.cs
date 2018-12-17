using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Infrastructure.Data;
using Svr.Web.Models;
using Svr.Web.Models.CategoryDisputesViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    public class CategoryDisputesController : Controller
    {
        private ILogger<CategoryDisputesController> logger;
        private ICategoryDisputeRepository сategoryDisputeRepository;

        [TempData]
        public string StatusMessage { get; set; }
        #region Конструктор
        public CategoryDisputesController(ICategoryDisputeRepository сategoryDisputeRepository, ILogger<CategoryDisputesController> logger = null)
        {
            this.сategoryDisputeRepository = сategoryDisputeRepository;
            this.logger = logger;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                сategoryDisputeRepository = null;
                logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Index
        // GET: CategoryDisputes
        [HttpGet]
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string currentFilter = null, string searchString = null, int page = 1)
        {
            var itemsPage = 10;
            //if (HttpContext.Request.Method == "GET")
            //{
            searchString = currentFilter;
            //}
            //else
            //{
            //    page = 1;
            //}
            IEnumerable<CategoryDispute> categoryDisputes = await сategoryDisputeRepository.ListAllAsync();
            //фильтрация
            if (!String.IsNullOrEmpty(searchString))
            {
                categoryDisputes = categoryDisputes.Where(p => p.Name.ToUpper().Contains(searchString.ToUpper()));
            }
            //сортировка
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    categoryDisputes = categoryDisputes.OrderByDescending(p => p.Name);
                    break;
                default:
                    categoryDisputes = categoryDisputes.OrderBy(p => p.Name);
                    break;
            }
            //пагинация
            var totalItems = categoryDisputes.Count();
            var itemsOnPage = categoryDisputes.Skip((page - 1) * itemsPage).Take(itemsPage).ToList();
            var categoryDisputeIndexModel = new IndexViewModel()
            {
                CategoryDisputeItems = itemsOnPage.Select(i => new ItemViewModel()
                {
                    Id = i.Id,
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
            return View(categoryDisputeIndexModel);
        }
        #endregion

        //// GET: CategoryDisputes/Details/5
        //public async Task<IActionResult> Details(long? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var categoryDispute = await _context.CategoryDisputes
        //        .SingleOrDefaultAsync(m => m.Id == id);
        //    if (categoryDispute == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(categoryDispute);
        //}

        //// GET: CategoryDisputes/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: CategoryDisputes/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Description,Name,Id,CreatedOnUtc,UpdatedOnUtc")] CategoryDispute categoryDispute)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(categoryDispute);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(categoryDispute);
        //}

        //// GET: CategoryDisputes/Edit/5
        //public async Task<IActionResult> Edit(long? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var categoryDispute = await _context.CategoryDisputes.SingleOrDefaultAsync(m => m.Id == id);
        //    if (categoryDispute == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(categoryDispute);
        //}

        //// POST: CategoryDisputes/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(long id, [Bind("Description,Name,Id,CreatedOnUtc,UpdatedOnUtc")] CategoryDispute categoryDispute)
        //{
        //    if (id != categoryDispute.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(categoryDispute);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CategoryDisputeExists(categoryDispute.Id))
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
        //    return View(categoryDispute);
        //}

        //// GET: CategoryDisputes/Delete/5
        //public async Task<IActionResult> Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var categoryDispute = await _context.CategoryDisputes
        //        .SingleOrDefaultAsync(m => m.Id == id);
        //    if (categoryDispute == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(categoryDispute);
        //}

        //// POST: CategoryDisputes/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(long id)
        //{
        //    var categoryDispute = await _context.CategoryDisputes.SingleOrDefaultAsync(m => m.Id == id);
        //    _context.CategoryDisputes.Remove(categoryDispute);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool CategoryDisputeExists(long id)
        //{
        //    return _context.CategoryDisputes.Any(e => e.Id == id);
        //}        //// GET: CategoryDisputes/Details/5
        //public async Task<IActionResult> Details(long? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var categoryDispute = await _context.CategoryDisputes
        //        .SingleOrDefaultAsync(m => m.Id == id);
        //    if (categoryDispute == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(categoryDispute);
        //}

        //// GET: CategoryDisputes/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: CategoryDisputes/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Description,Name,Id,CreatedOnUtc,UpdatedOnUtc")] CategoryDispute categoryDispute)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(categoryDispute);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(categoryDispute);
        //}

        //// GET: CategoryDisputes/Edit/5
        //public async Task<IActionResult> Edit(long? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var categoryDispute = await _context.CategoryDisputes.SingleOrDefaultAsync(m => m.Id == id);
        //    if (categoryDispute == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(categoryDispute);
        //}

        //// POST: CategoryDisputes/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(long id, [Bind("Description,Name,Id,CreatedOnUtc,UpdatedOnUtc")] CategoryDispute categoryDispute)
        //{
        //    if (id != categoryDispute.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(categoryDispute);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CategoryDisputeExists(categoryDispute.Id))
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
        //    return View(categoryDispute);
        //}

        //// GET: CategoryDisputes/Delete/5
        //public async Task<IActionResult> Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var categoryDispute = await _context.CategoryDisputes
        //        .SingleOrDefaultAsync(m => m.Id == id);
        //    if (categoryDispute == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(categoryDispute);
        //}

        //// POST: CategoryDisputes/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(long id)
        //{
        //    var categoryDispute = await _context.CategoryDisputes.SingleOrDefaultAsync(m => m.Id == id);
        //    _context.CategoryDisputes.Remove(categoryDispute);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool CategoryDisputeExists(long id)
        //{
        //    return _context.CategoryDisputes.Any(e => e.Id == id);
        //}
    }
}
