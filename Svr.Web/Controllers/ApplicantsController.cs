using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Web.Extensions;
using Svr.Web.Models;
using Svr.Web.Models.ApplicantViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    [Authorize]
    public class ApplicantsController : Controller
    {
        private readonly IApplicantRepository repository;
        private readonly IDirRepository dirRepository;
        private readonly ILogger<ApplicantsController> logger;

        [TempData]
        public string StatusMessage { get; set; }
        #region Конструктор
        public ApplicantsController(IApplicantRepository repository, IDirRepository dirRepository, ILogger<ApplicantsController> logger)
        {
            this.logger = logger;
            this.repository = repository;
            this.dirRepository = dirRepository;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //repository = null;
                //dirRepository = null;
                //logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Index
        // GET: Applicants
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string owner = null, string parentowner = null, string searchString = null, int page = 1, int itemsPage = 10)
        {
            var list = repository.List(new ApplicantSpecification(owner.ToLong()));
            //фильтрация
            if (!String.IsNullOrEmpty(searchString))
            {
                list = list.Where(d => d.Name.ToUpper().Contains(searchString.ToUpper()));
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
                    Name = i.Name,
                    CreatedOnUtc = i.CreatedOnUtc,
                    UpdatedOnUtc = i.UpdatedOnUtc,
                    TypeApplicant = i.TypeApplicant,
                    Opf = i.Opf
                }),
                PageViewModel = new PageViewModel(count, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString, owner, await GetTypeApplicants(owner)),
                StatusMessage = StatusMessage
            };
            return View(indexModel);
        }
        #endregion
        #region Details
        // GET: Applicants/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, FullName = item.FullName, TypeApplicant = item.TypeApplicant, TypeApplicantId = item.TypeApplicantId, Opf = item.Opf, Address = item.Address, AddressBank = item.AddressBank, Inn = item.Inn, OpfId = item.OpfId, Born = item.Born, IsMan= item.TypeApplicant.Name == "Физическое лицо" };
            return View(model);
        }
        #endregion
        #region Create
        // GET: Applicants/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.TypeApplicants = await GetTypeApplicants();
            ViewBag.Opfs = await GetOpfs();
            return View();
        }

        // POST: Applicants/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var item = await repository.AddAsync(new Applicant { Name = model.Name, TypeApplicantId = model.TypeApplicantId, /*Description = model.Description, FullName = model.FullName, OpfId = model.OpfId, Inn = model.Inn, Address = model.Address, AddressBank = model.AddressBank, Born = model.Born */});
                if (item != null)
                {
                    StatusMessage = item.MessageAddOk();
                    return RedirectToAction(nameof(Index));
                    model.IsMan = item.TypeApplicant.Name == "Физическое лицо";
                    ViewBag.TypeApplicants = await GetTypeApplicants(model.TypeApplicantId.ToString());
                    if (!model.IsMan)
                    {
                        ViewBag.Opfs = await GetOpfs(model.OpfId.ToString());
                    }
                    return RedirectToAction( nameof(Edit), new {id = item.Id });
                }
            }
            ModelState.AddModelError(string.Empty, model.MessageAddError());
            ViewBag.TypeApplicants = await GetTypeApplicants(model.TypeApplicantId.ToString());
            if (!model.IsMan)
            {
                ViewBag.Opfs = await GetOpfs(model.OpfId.ToString());
            }
            return View(model);
        }
        #endregion
        #region Edit
        // GET: Applicants/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
            }

            var model = new ItemViewModel { Id = item.Id, CreatedOnUtc = item.CreatedOnUtc, Name = item.Name, TypeApplicantId = item.TypeApplicantId, Description = item.Description, FullName = item.FullName, OpfId = item.OpfId, Address = item.Address, AddressBank = item.AddressBank, Born = item.Born, Inn = item.Inn, IsMan= item.TypeApplicant.Name== "Физическое лицо" };
            ViewBag.TypeApplicants = await GetTypeApplicants(item.TypeApplicantId.ToString());
            if (!model.IsMan)
            {
                ViewBag.Opfs = await GetOpfs(item.OpfId.ToString());
            }
            return View(model);
        }
        // POST: Applicants/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await repository.UpdateAsync(new Applicant { Id = model.Id, Name = model.Name, CreatedOnUtc = model.CreatedOnUtc, TypeApplicantId = model.TypeApplicantId, Description = model.Description, FullName = model.FullName, OpfId = model.OpfId, Address = model.Address, AddressBank = model.AddressBank, Born = model.Born, Inn = model.Inn });
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
                    return RedirectToAction(nameof(Index));
                }
                model.IsMan = (await repository.GetByIdWithItemsAsync(model.Id)).TypeApplicant.Name == "Физическое лицо";
                ViewBag.TypeApplicants = await GetTypeApplicants(model.TypeApplicantId.ToString());
                if (!model.IsMan)
                {
                    ViewBag.Opfs = await GetOpfs(model.OpfId.ToString());
                }
                return View(model);
            }
            ModelState.AddModelError(string.Empty, StatusMessage);
            ViewBag.TypeApplicants = await GetTypeApplicants(model.TypeApplicantId.ToString());
            if (!model.IsMan)
            {
                ViewBag.Opfs = await GetOpfs(model.OpfId.ToString());
            }
            return View(model);
        }
        #endregion
        #region Delete
        [HttpGet]
        // GET: Applicants/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, TypeApplicantId = item.TypeApplicantId, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, StatusMessage = StatusMessage };
            return View(model);
        }
        // POST: Applicants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор, Администратор ОПФР, Администратор УПФР")]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await repository.DeleteAsync(new Applicant { Id = model.Id, Name = model.Name });
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
        private async Task<IEnumerable<SelectListItem>> GetTypeApplicants(string owner = null)
        {
            return (await dirRepository.ListAsync(new DirSpecification("Тип контрагента"))).Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (owner == a.Id.ToString()) });
        }
        private async Task<IEnumerable<SelectListItem>> GetOpfs(string owner = null)
        {
            return (await dirRepository.ListAsync(new DirSpecification("ОПФ"))).Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (owner == a.Id.ToString()) });
        }
    }
}