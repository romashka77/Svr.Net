using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Infrastructure.Data;
using Svr.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Svr.Web.Models.SubjectClaimsViewModels;
using Microsoft.AspNetCore.Authorization;
using Svr.Web.Extensions;

namespace Svr.Web.Controllers
{
    [Authorize(Roles = "Администратор, Администратор ОПФР")]
    public class SubjectClaimsController : Controller
    {
        private IGroupClaimRepository groupClaimRepository;
        private ISubjectClaimRepository subjectClaimRepository;
        private ILogger<SubjectClaimsController> logger;
        [TempData]
        public string StatusMessage { get; set; }
        #region Конструктор
        public SubjectClaimsController(IGroupClaimRepository groupClaimRepository, ISubjectClaimRepository subjectClaimRepository, ILogger<SubjectClaimsController> logger)
        {
            this.logger = logger;
            this.groupClaimRepository = groupClaimRepository;
            this.subjectClaimRepository = subjectClaimRepository;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                groupClaimRepository = null;
                subjectClaimRepository = null;
                logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Index
        // GET: SubjectClaims
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string owner = null, string searchString = null, int page = 1, int itemsPage = 10)
        {
            var filterSpecification = new SubjectClaimSpecification(owner.ToLong());
            IEnumerable<SubjectClaim> list = await subjectClaimRepository.ListAsync(filterSpecification);
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
                    list = list.OrderBy(s => s.GroupClaim.Name);
                    break;
                case SortState.OwnerDesc:
                    list = list.OrderByDescending(s => s.GroupClaim.Name);
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
                    Code = i.Code,
                    Name = i.Name,
                    Description = i.Description,
                    CreatedOnUtc = i.CreatedOnUtc,
                    UpdatedOnUtc = i.UpdatedOnUtc,
                    GroupClaim= i.GroupClaim
                }),
                PageViewModel = new PageViewModel(count, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString, owner, groupClaimRepository.ListAll().ToList().Select(a => new SelectListItem { Text = $"{a.Code} {a.Name}", Value = a.Id.ToString(), Selected = (owner == a.Id.ToString()) })),
                StatusMessage = StatusMessage
            };
            return View(indexModel);
        }
        #endregion
        #region Details
        // GET: SubjectClaims/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var item = await subjectClaimRepository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = $"Не удалось загрузить предмет иска с ID = {id}.";
                return RedirectToAction(nameof(Index));
                //throw new ApplicationException($"Не удалось загрузить район с ID {id}.");
            }
            var model = new ItemViewModel { Id = item.Id, Code = item.Code, Name = item.Name, Description = item.Description, GroupClaimId= item.GroupClaimId, GroupClaim = item.GroupClaim, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc };
            return View(model);
        }
        #endregion
        #region Create
        // GET: SubjectClaims/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.groupClaims = new SelectList((await groupClaimRepository.ListAllAsync()).Select(a => new { Id = a.Id, Name = $"{a.Code} {a.Name}" }), "Id", "Name", 1);
            return View();
        }
        // POST: SubjectClaims/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var item = await  subjectClaimRepository.AddAsync(new SubjectClaim { Code = model.Code, Name = model.Name, Description = model.Description, GroupClaimId = model.GroupClaimId });
                if (item != null)
                {
                    StatusMessage = $"Добавлен предмет иска с Id={item.Id}, код={item.Code}, имя={item.Name}.";
                    return RedirectToAction(nameof(Index));
                }
            }
            ModelState.AddModelError(string.Empty, $"Ошибка: {model} - неудачная попытка регистрации.");
            ViewBag.groupClaims = new SelectList((await groupClaimRepository.ListAllAsync()).Select(a => new { Id = a.Id, Name = $"{a.Code} {a.Name}" }), "Id", "Name", 1);
            return View(model);
        }
        #endregion
        #region Edit
        // GET: SubjectClaims/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            var item = await subjectClaimRepository.GetByIdAsync(id);
            if (item == null)
            {
                StatusMessage = $"Ошибка: Не удалось найти предмет иска с ID = {id}.";
                return RedirectToAction(nameof(Index));
                //throw new ApplicationException($"Не удалось загрузить район с ID {id}.");
            }
            var model = new ItemViewModel { Id = item.Id, Code = item.Code, Name = item.Name, Description = item.Description, GroupClaimId= item. GroupClaimId, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc };
            ViewBag.groupClaims = new SelectList((await groupClaimRepository.ListAllAsync()).Select(a => new { Id = a.Id, Name = $"{a.Code} {a.Name}" }), "Id", "Name", 1);
            return View(model);
        }
        // POST: SubjectClaims/Edit/5
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
                    await subjectClaimRepository.UpdateAsync(new SubjectClaim{ Id = model.Id, Code = model.Code, Description = model.Description, Name = model.Name, CreatedOnUtc = model.CreatedOnUtc, GroupClaimId = model.GroupClaimId });
                    StatusMessage = $"{model} c ID = {model.Id} обновлен";
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!(await subjectClaimRepository.EntityExistsAsync(model.Id)))
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
            ViewBag.groupClaims = new SelectList((await groupClaimRepository.ListAllAsync()).Select(a => new { Id = a.Id, Name = $"{a.Code} {a.Name}" }), "Id", "Name", 1);
            return View(model);
        }
        #endregion
        #region Delete
        // GET: SubjectClaims/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            var item = await subjectClaimRepository.GetByIdAsync(id);
            if (item == null)
            {
                StatusMessage = $"Ошибка: Не удалось найти группу исков с ID = {id}.";
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Code = item.Code, Name = item.Name, Description = item.Description, GroupClaimId= item.GroupClaimId, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, StatusMessage = StatusMessage };
            return View(model);
        }

        // POST: SubjectClaims/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await subjectClaimRepository.DeleteAsync(new SubjectClaim { Id = model.Id, Name = model.Name, Code = model.Code, });
                StatusMessage = $"Удален {model} с Id={model.Id}, Name = {model.Name}, Code = {model.Code}.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка при удалении {model} с Id={model.Id}, Name = {model.Name}, Code = {model.Code} - {ex.Message}.";
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion
    }
}
