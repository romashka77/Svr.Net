using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Infrastructure.Data;
using Svr.Web.Models;
using Svr.Web.Models.GroupClaimsViewModels;

namespace Svr.Web.Controllers
{
    public class GroupClaimsController : Controller
    {
        private IGroupClaimRepository groupClaimRepository;
        private ICategoryDisputeRepository categoryDisputeRepository;
        private ILogger<GroupClaimsController> logger;

        [TempData]
        public string StatusMessage { get; set; }

        #region Конструктор
        public GroupClaimsController(IGroupClaimRepository groupClaimRepository, ICategoryDisputeRepository categoryDisputeRepository, ILogger<GroupClaimsController> logger)
        {
            this.logger = logger;
            this.groupClaimRepository= groupClaimRepository;
            this.categoryDisputeRepository = categoryDisputeRepository;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                groupClaimRepository = null;
                categoryDisputeRepository = null;
                logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Index
        // GET: GroupClaims
        public async Task<IActionResult> Index(long? categoryDispute, string name, int page, SortState sortOrder = SortState.NameAsc)
        {
            var itemsPage = 10;
            var filterSpecification = new GroupClaimSpecification(categoryDispute);
            var groupClaims = groupClaimRepository.List(filterSpecification);
            //фильтрация
            if (categoryDispute != null && categoryDispute != 0)
            {
                groupClaims = groupClaims.Where(d => d.CategoryDisputeId == categoryDispute);
            }
            if (!String.IsNullOrEmpty(name))
            {
                groupClaims = groupClaims.Where(d => d.Name.ToUpper().Contains(name.ToUpper()));
            }
            // сортировка
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    groupClaims = groupClaims.OrderByDescending(s => s.Name);
                    break;
                case SortState.CodeAsc:
                    groupClaims = groupClaims.OrderBy(s => s.Code);
                    break;
                case SortState.CodeDesc:
                    groupClaims = groupClaims.OrderByDescending(s => s.Code);
                    break;
                case SortState.CategoryDisputeAsc:
                    groupClaims = groupClaims.OrderBy(s => s.CategoryDispute.Name);
                    break;
                case SortState.CategoryDisputeDesc:
                    groupClaims = groupClaims.OrderByDescending(s => s.CategoryDispute.Name);
                    break;
                default:
                    groupClaims = groupClaims.OrderBy(s => s.Name);
                    break;
            }
            // пагинация
            var count = groupClaims.Count();
            var itemsOnPage = groupClaims.Skip((page - 1) * itemsPage).Take(itemsPage).ToList();
            var groupClaimIndexModel = new IndexViewModel()
            {
                GroupClaimItems = itemsOnPage.Select(i => new ItemViewModel()
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    CreatedOnUtc = i.CreatedOnUtc,
                    UpdatedOnUtc = i.UpdatedOnUtc
                }),
                PageViewModel = new PageViewModel(count, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(categoryDisputeRepository.ListAll().ToList(), categoryDispute, name),

                StatusMessage = StatusMessage
            };
            return View(groupClaimIndexModel);
        }
        #endregion
        // GET: GroupClaims/Details/5
        //public async Task<IActionResult> Details(long? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var groupClaim = await _context.GroupClaims
        //        .Include(g => g.CategoryDispute)
        //        .SingleOrDefaultAsync(m => m.Id == id);
        //    if (groupClaim == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(groupClaim);
        //}

        //// GET: GroupClaims/Create
        //public IActionResult Create()
        //{
        //    ViewData["CategoryDisputeId"] = new SelectList(_context.CategoryDisputes, "Id", "Name");
        //    return View();
        //}

        //// POST: GroupClaims/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("CategoryDisputeId,Code,Description,Name,Id,CreatedOnUtc,UpdatedOnUtc")] GroupClaim groupClaim)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(groupClaim);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CategoryDisputeId"] = new SelectList(_context.CategoryDisputes, "Id", "Name", groupClaim.CategoryDisputeId);
        //    return View(groupClaim);
        //}

        //// GET: GroupClaims/Edit/5
        //public async Task<IActionResult> Edit(long? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var groupClaim = await _context.GroupClaims.SingleOrDefaultAsync(m => m.Id == id);
        //    if (groupClaim == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["CategoryDisputeId"] = new SelectList(_context.CategoryDisputes, "Id", "Name", groupClaim.CategoryDisputeId);
        //    return View(groupClaim);
        //}

        //// POST: GroupClaims/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(long id, [Bind("CategoryDisputeId,Code,Description,Name,Id,CreatedOnUtc,UpdatedOnUtc")] GroupClaim groupClaim)
        //{
        //    if (id != groupClaim.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(groupClaim);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!GroupClaimExists(groupClaim.Id))
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
        //    ViewData["CategoryDisputeId"] = new SelectList(_context.CategoryDisputes, "Id", "Name", groupClaim.CategoryDisputeId);
        //    return View(groupClaim);
        //}

        //// GET: GroupClaims/Delete/5
        //public async Task<IActionResult> Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var groupClaim = await _context.GroupClaims
        //        .Include(g => g.CategoryDispute)
        //        .SingleOrDefaultAsync(m => m.Id == id);
        //    if (groupClaim == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(groupClaim);
        //}

        //// POST: GroupClaims/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(long id)
        //{
        //    var groupClaim = await _context.GroupClaims.SingleOrDefaultAsync(m => m.Id == id);
        //    _context.GroupClaims.Remove(groupClaim);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool GroupClaimExists(long id)
        //{
        //    return _context.GroupClaims.Any(e => e.Id == id);
        //}
    }
}
