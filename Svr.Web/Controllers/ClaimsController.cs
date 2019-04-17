using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Infrastructure.Data;
using Svr.Infrastructure.Identity;
using Svr.Web.Extensions;
using Svr.Web.Models;
using Svr.Web.Models.ClaimsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    [Authorize]
    public class ClaimsController : Controller
    {
        private readonly IClaimRepository repository;
        private readonly IDistrictRepository districtRepository;
        private readonly IRegionRepository regionRepository;
        private readonly ICategoryDisputeRepository categoryDisputeRepository;
        private readonly IGroupClaimRepository groupClaimRepository;
        private readonly ISubjectClaimRepository subjectClaimRepository;
        private readonly IDirRepository dirRepository;
        private readonly IPerformerRepository performerRepository;
        private readonly IApplicantRepository applicantRepository;
        private readonly IInstanceRepository instanceRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<ClaimsController> logger;

        [TempData]
        public string StatusMessage { get; set; }
        #region Конструктор
        public ClaimsController(IClaimRepository repository, IRegionRepository regionRepository, IDistrictRepository districtRepository, ICategoryDisputeRepository categoryDisputeRepository, IGroupClaimRepository groupClaimRepository, ISubjectClaimRepository subjectClaimRepository, IPerformerRepository performerRepository, IDirRepository dirRepository, IApplicantRepository applicantRepository, IInstanceRepository instanceRepository, ILogger<ClaimsController> logger, UserManager<ApplicationUser> userManager)
        {
            this.logger = logger;
            this.repository = repository;
            this.regionRepository = regionRepository;
            this.districtRepository = districtRepository;
            this.categoryDisputeRepository = categoryDisputeRepository;
            this.groupClaimRepository = groupClaimRepository;
            this.subjectClaimRepository = subjectClaimRepository;
            this.performerRepository = performerRepository;
            this.dirRepository = dirRepository;
            this.applicantRepository = applicantRepository;
            this.instanceRepository = instanceRepository;
            this.userManager = userManager;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //repository = null;
                //districtRepository = null;
                //regionRepository = null;
                //categoryDisputeRepository = null;
                //groupClaimRepository = null;
                //subjectClaimRepository = null;
                //performerRepository = null;
                //dirRepository = null;
                //applicantRepository = null;
                //instanceRepository = null;
                //logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Index
        // GET: Claims
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string lord = null, string owner = null, string searchString = null, int page = 1, int itemsPage = 10)
        {
            if (String.IsNullOrEmpty(owner))
            {
                if (String.IsNullOrEmpty(lord))
                {
                    ApplicationUser user = await userManager.FindByNameAsync(User.Identity.Name);
                    if (user != null)
                    {
                        owner = user.DistrictId.ToString();
                    }
                }
            }
            var list = repository.List(new ClaimSpecification(owner.ToLong()));
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
                    Region = i.Region,
                    District = i.District,
                }),
                PageViewModel = new PageViewModel(count, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString, owner, (await districtRepository.ListAsync(new DistrictSpecification(lord.ToLong()))).Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (owner == a.Id.ToString()) }), lord, (await regionRepository.ListAllAsync()).ToList().Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (lord == a.Id.ToString()) })),
                StatusMessage = StatusMessage
            };
            return View(indexModel);
        }
        #endregion
        #region Details
        // GET: Claims/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
                //throw new ApplicationException($"Не удалось загрузить район с ID {id}.");
            }
            var model = new ItemViewModel { Id = item.Id, Code = item.Code, Name = item.Name, Description = item.Description, /*RegionId = item.RegionId,*/ Region = item.Region, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, District = item.District, Instances = item.Instances, Meetings = item.Meetings, FileEntities = item.FileEntities };
            return View(model);
        }
        #endregion
        #region Create
        // GET: Claims/Create
        public async Task<IActionResult> Create(string lord = null, string owner = null)
        {
            ViewBag.Regions = new SelectList(await regionRepository.ListAllAsync(), "Id", "Name", lord);
            ViewBag.Districts = new SelectList(await districtRepository.ListAsync(new DistrictSpecification(lord.ToLong())), "Id", "Name", owner);
            return View();
        }
        // POST: Claims/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var item = await repository.AddAsync(new Claim { Code = GetCode(model.Name, model.DateReg, model.CreatedOnUtc.Year, model.Id)/* $"{model.Id}-{DateTime.Now.Year.ToString()}-{model.Name}/{model.DateReg.ToString("D")}"*/, Name = model.Name, Description = model.Description, RegionId = model.RegionId, DistrictId = model.DistrictId, DateReg = model.DateReg });
                if (item != null)
                {

                    StatusMessage = item.MessageAddOk();
                    return RedirectToAction(nameof(Edit), new { id = item.Id });
                }
            }
            //ModelState.AddModelError(string.Empty, model.MessageAddError());
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
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
            }
            var model = new EditViewModel { Id = item.Id, Code = item.Code, Name = item.Name, Description = item.Description, RegionId = item.RegionId, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, DistrictId = item.DistrictId, DateReg = item.DateReg, DateIn = item.DateIn, CategoryDisputeId = item.CategoryDisputeId, GroupClaimId = item.GroupClaimId, SubjectClaimId = item.SubjectClaimId, СourtId = item.СourtId, PerformerId = item.PerformerId, Sum = item.Sum, PlaintiffId = item.PlaintiffId, RespondentId = item.RespondentId, Person3rdId = item.Person3rdId };
            await SetViewBag(model);
            return View(model);
        }
        // POST: Claims/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Code = GetCode(model.Name, model.DateReg, model.CreatedOnUtc.Year, model.Id); //$"{model.Id}-{model.CreatedOnUtc.Year}-{model.Name}/{model.DateReg.ToString("D")}";
                    if ((model.RegionId != 0) && (model.DistrictId != 0))
                    {
                        await repository.UpdateAsync(new Claim { Id = model.Id, Code = model.Code, Description = model.Description, Name = model.Name, CreatedOnUtc = model.CreatedOnUtc, CategoryDisputeId = model.CategoryDisputeId, RegionId = model.RegionId, DistrictId = model.DistrictId, DateReg = model.DateReg, DateIn = model.DateIn, GroupClaimId = model.GroupClaimId, SubjectClaimId = model.SubjectClaimId, СourtId = model.СourtId, PerformerId = model.PerformerId, Sum = model.Sum, PlaintiffId = model.PlaintiffId, RespondentId = model.RespondentId, Person3rdId = model.Person3rdId });
                        StatusMessage = model.MessageEditOk();
                    }
                    else { StatusMessage = $"Проверте заполнение полей"; }
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
            }
            await SetViewBag(model);
            model.StatusMessage = StatusMessage;
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
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Code = item.Code, Name = item.Name, Description = item.Description, CategoryDisputeId = item.CategoryDisputeId, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, StatusMessage = StatusMessage, RegionId = item.RegionId, DistrictId = item.DistrictId };
            return View(model);
        }
        // POST: Claims/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await repository.DeleteAsync(new Claim { Id = model.Id, Name = model.Name, Code = model.Code, });
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
        #region Utils
        private string GetCode(string name, DateTime date, int? year, long? id)
        {
            return (id != null ? $"{id}-" : "") + (year != null ? $"{year}-" : "") + $"{name}/{date:D}";
        }

        private async Task SetViewBag(EditViewModel model)
        {
            ViewBag.Regions = new SelectList((await regionRepository.ListAllAsync()).OrderBy(n => n.Name), "Id", "Name", model.RegionId);
            ViewBag.Districts = new SelectList((await districtRepository.ListAsync(new DistrictSpecification(model.RegionId))).OrderBy(n => n.Name), "Id", "Name", model.DistrictId);

            ViewBag.CategoryDisputes = new SelectList(await categoryDisputeRepository.ListAllAsync(), "Id", "Name", model.CategoryDisputeId);

            ViewBag.GroupClaims = new SelectList((await groupClaimRepository.ListAsync(new GroupClaimSpecification(model.CategoryDisputeId))).OrderBy(n => n.Code).Select(i => new { Id = i.Id, Name = $"{i.Code} {i.Name}" }), "Id", "Name", model.GroupClaimId);
            ViewBag.SubjectClaims = new SelectList((await subjectClaimRepository.ListAsync(new SubjectClaimSpecification(model.GroupClaimId))).OrderBy(n => n.Code).Select(i => new { Id = i.Id, Name = $"{i.Code} {i.Name}" }), "Id", "Name", model.SubjectClaimId);

            ViewBag.Сourts = new SelectList((await dirRepository.ListAsync(new DirSpecification("Суд"))).OrderBy(n => n.Name), "Id", "Name", model.СourtId);

            var p = new List<Performer>();
            var district = await districtRepository.GetByIdWithItemsAsync(model.DistrictId);
            if (district != null)
            {
                var districtPerformers = district.DistrictPerformers;
                foreach (var dp in districtPerformers)
                {
                    p.Add(dp.Performer);
                }
            }
            ViewBag.Performers = new SelectList(p.OrderBy(n => n.Name), "Id", "Name", model.PerformerId);
            ViewBag.Applicants = new SelectList((await applicantRepository.ListAsync(new ApplicantSpecification(null))).OrderBy(n => n.Name).Select(a => new { Id = a.Id, Name = string.Concat(a.Name, " ", a.Address) }), "Id", "Name", model.PlaintiffId);
        }
        #endregion
    }
}
