using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Infrastructure.Data;
using Svr.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    public class PerformersController : Controller
    {
        private IPerformerRepository repository;
        private IDistrictRepository district;
        private ILogger<PerformersController> logger;

        [TempData]
        public string StatusMessage { get; set; }
        #region Конструктор
        public PerformersController(IPerformerRepository repository, IDistrictRepository district, ILogger<PerformersController> logger)
        {
            this.logger = logger;
            this.repository = repository;
            this.district = district;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                district = null;
                repository = null;
                logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Index
        // GET: Performers
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string owner = null, string searchString = null, int page = 1, int itemsPage = 10)
        {
            long? _owner = null;
            if (!String.IsNullOrEmpty(owner))
            {
                _owner = Int64.Parse(owner);
            }
            //var filterSpecification = new DistrictSpecification(_owner);



            return View(null);
        }
        #endregion
        // GET: Performers/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var performer = await _context.Performers.SingleOrDefaultAsync(m => m.Id == id);
            //if (performer == null)
            {
                return NotFound();
            }

            return View(null);
        }

        // GET: Performers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Performers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Id,CreatedOnUtc,UpdatedOnUtc")] Performer performer)
        {
            if (ModelState.IsValid)
            {
                //_context.Add(performer);
                //await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(performer);
        }

        // GET: Performers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var performer = await _context.Performers.SingleOrDefaultAsync(m => m.Id == id);
            //if (performer == null)
            {
                return NotFound();
            }
            return View(null);
        }

        // POST: Performers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Name,Id,CreatedOnUtc,UpdatedOnUtc")] Performer performer)
        {
            if (id != performer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                //try
                //{
                //    //    _context.Update(performer);
                //    //    await _context.SaveChangesAsync();
                //}
                //catch (DbUpdateConcurrencyException)
                //{
                //    //      if (!PerformerExists(performer.Id))
                //    //{
                //    //    return NotFound();
                //    //}
                //    else
                //    {
                //        throw;
                //    }
                //}
                return RedirectToAction(nameof(Index));
            }
            return View(performer);
        }

        // GET: Performers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var performer = await _context.Performers.SingleOrDefaultAsync(m => m.Id == id);
            //if (performer == null)
            {
                return NotFound();
            }

            return View(null);
        }

        // POST: Performers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            //var performer = await _context.Performers.SingleOrDefaultAsync(m => m.Id == id);
            //_context.Performers.Remove(performer);
            //await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}
