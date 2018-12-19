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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            this.groupClaimRepository = groupClaimRepository;
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
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc,string owner=null, string searchString=null, int page=1,int itemsPage = 10)
        {
            long? _owner = null;
            if (!String.IsNullOrEmpty(owner))
            {
                _owner = Int64.Parse(owner);
            }
            var filterSpecification = new GroupClaimSpecification(_owner);
            var list = groupClaimRepository.List(filterSpecification);
            //фильтрация
            if (owner!=null)
            {
                list = list.Where(d => d.CategoryDisputeId == _owner);
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                list = list.Where(d => d.Name.ToUpper().Contains(searchString.ToUpper())|| d.Code.ToUpper().Contains(searchString.ToUpper()));
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
                    list = list.OrderBy(s => s.CategoryDispute.Name);
                    break;
                case SortState.OwnerDesc:
                    list = list.OrderByDescending(s => s.CategoryDispute.Name);
                    break;
                default:
                    list = list.OrderBy(s => s.Name);
                    break;
            }
            // пагинация
            var count = list.Count();
            var itemsOnPage = list.Skip((page - 1) * itemsPage).Take(itemsPage).ToList();
            var groupClaimIndexModel = new IndexViewModel()
            {
                GroupClaimItems = itemsOnPage.Select(i => new ItemViewModel()
                {
                    Id = i.Id,
                    Code=i.Code,
                    Name = i.Name,
                    Description = i.Description,
                    CreatedOnUtc = i.CreatedOnUtc,
                    UpdatedOnUtc = i.UpdatedOnUtc,
                    CategoryDispute=i.CategoryDispute
                    
                }),
                PageViewModel = new PageViewModel(count, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString, owner, categoryDisputeRepository.ListAll().ToList().Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (owner == a.Id.ToString()) })),

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
