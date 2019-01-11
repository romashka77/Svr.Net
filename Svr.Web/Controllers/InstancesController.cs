using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Infrastructure.Data;
using Svr.Web.Models;
using Svr.Web.Models.InstanceViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    public class InstancesController : Controller
    {
        private IInstanceRepository repository;
        private IClaimRepository сlaimRepository;
        private ILogger<InstancesController> logger;

        [TempData]
        public string StatusMessage { get; set; }

        #region Конструктор
        public InstancesController(IInstanceRepository repository, IClaimRepository сlaimRepository, ILogger<InstancesController> logger)
        {
            this.logger = logger;
            this.repository = repository;
            this.сlaimRepository = сlaimRepository;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                сlaimRepository = null;
                repository = null;
                logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Index
        // GET: Instances
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string owner = null, string searchString = null, int page = 1, int itemsPage = 10)
        {
            long? _owner = null;
            if (!String.IsNullOrEmpty(owner))
            {
                _owner = Int64.Parse(owner);
            }
            var filterSpecification = new InstanceSpecification(_owner);
            IEnumerable<Instance> list = await repository.ListAsync(filterSpecification);
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
                    list = list.OrderBy(s => s.Claim.Name);
                    break;
                case SortState.OwnerDesc:
                    list = list.OrderByDescending(s => s.Claim.Name);
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
                    Claim = i.Claim
                }),
                PageViewModel = new PageViewModel(count, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString, owner, (await сlaimRepository.ListAsync(new ClaimSpecification(null))).ToList().Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (owner == a.Id.ToString()) })),

                StatusMessage = StatusMessage
            };

            return View(indexModel);
        }
        #endregion
        #region Details
        // GET: Instances/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = $"Не удалось загрузить иск с ID = {id}.";
                return RedirectToAction(nameof(Index));
                //throw new ApplicationException($"Не удалось загрузить район с ID {id}.");
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, Claim = item.Claim, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, CourtDecision = item.CourtDecision, DateCourtDecision = item.DateCourtDecision, DateInCourtDecision = item.DateInCourtDecision, DateSFine = item.DateSFine, DateSPenalty = item.DateSPenalty, DateSShortage = item.DateSShortage, DateToFine = item.DateToFine, DateToPenalty = item.DateToPenalty, DateToShortage = item.DateToShortage, DateTransfer = item.DateTransfer, DutyDenied = item.DutyDenied, DutyPaid = item.DutyPaid, DutySatisfied = item.DutySatisfied, FFOMSFine = item.FFOMSFine, FFOMSShortage = item.FFOMSShortage, FundedPartPFRFine = item.InsurancePartPFRFine, FundedPartPFRShortage = item.FundedPartPFRShortage, InsurancePartPFRFine = item.InsurancePartPFRFine, InsurancePartPFRShortage = item.InsurancePartPFRShortage, PaidVoluntarily = item.PaidVoluntarily, ServicesDenied = item.ServicesDenied, ServicesSatisfied = item.ServicesSatisfied, SumDenied = item.SumDenied, SumPenalty = item.SumPenalty, SumSatisfied = item.SumSatisfied, TFOMSFine = item.TFOMSFine, TFOMSShortage = item.TFOMSShortage, СostDenied = item.СostDenied, СostSatisfied = item.СostSatisfied };
            return View(model);
        }
        #endregion
        #region Create
        // GET: Instances/Create
        public async Task<IActionResult> Create()
        {
            //   ViewBag.Regions = new SelectList(await regionRepository.ListAllAsync(), "Id", "Name", 1);
            return View();
        }

        // POST: Instances/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                // добавляем новый Район
                var item = await repository.AddAsync(new Instance { Name = model.Name, Description = model.Description, Claim = model.Claim, CourtDecision = model.CourtDecision, DateCourtDecision = model.DateCourtDecision, СostSatisfied = model.СostSatisfied, СostDenied = model.СostDenied, TFOMSShortage = model.TFOMSShortage, TFOMSFine = model.TFOMSFine, DateInCourtDecision = model.DateInCourtDecision, DateSFine = model.DateSFine, DateSPenalty = model.DateSPenalty, DateSShortage = model.DateSShortage, DateToFine = model.DateToFine, DateToPenalty = model.DateToPenalty, DateToShortage = model.DateToShortage, DateTransfer = model.DateTransfer, DutyDenied = model.DutyDenied, DutyPaid = model.DutyPaid, DutySatisfied = model.DutySatisfied, FFOMSFine = model.FFOMSFine, FFOMSShortage = model.FFOMSShortage, FundedPartPFRFine = model.FundedPartPFRFine, FundedPartPFRShortage = model.FundedPartPFRShortage, InsurancePartPFRFine = model.InsurancePartPFRFine, InsurancePartPFRShortage = model.InsurancePartPFRShortage, PaidVoluntarily = model.PaidVoluntarily, ServicesDenied = model.ServicesDenied, ServicesSatisfied = model.ServicesSatisfied, SumDenied = model.SumDenied, SumPenalty = model.SumPenalty, SumSatisfied = model.SumSatisfied });
                if (item != null)
                {
                    StatusMessage = $"Добавлена инстанция с Id={item.Id}, имя={item.Name}.";
                    return RedirectToAction(nameof(Edit), new { id = item.Id });
                }
            }
            ModelState.AddModelError(string.Empty, $"Ошибка: {model} - неудачная попытка регистрации.");
            //ViewBag.Regions = new SelectList(await regionRepository.ListAllAsync(), "Id", "Name", 1);
            return View(model);
        }
        #endregion
        // GET: Instances/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = $"Ошибка: Не удалось найти инстанцию с ID = {id}.";
                return RedirectToAction(nameof(Index));
                //throw new ApplicationException($"Не удалось загрузить район с ID {id}.");
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, TFOMSShortage = item.TFOMSShortage, СostDenied = item.СostDenied, СostSatisfied = item.СostSatisfied, TFOMSFine = item.TFOMSFine, PaidVoluntarily = item.PaidVoluntarily, SumSatisfied = item.SumSatisfied, SumPenalty = item.SumPenalty, SumDenied = item.SumDenied, ServicesSatisfied = item.ServicesSatisfied, ServicesDenied = item.ServicesDenied, CourtDecision = item.CourtDecision, DateCourtDecision = item.DateCourtDecision, DateInCourtDecision = item.DateInCourtDecision, DateSFine = item.DateSFine, DateSPenalty = item.DateSPenalty, DateSShortage = item.DateSShortage, DateToFine = item.DateToFine, DateToPenalty = item.DateToPenalty, DateToShortage = item.DateToShortage, DateTransfer = item.DateTransfer, DutyDenied = item.DutyDenied, DutyPaid = item.DutyPaid, DutySatisfied = item.DutySatisfied, FFOMSFine = item.FFOMSFine, FFOMSShortage = item.FFOMSShortage, FundedPartPFRFine = item.FundedPartPFRFine, FundedPartPFRShortage = item.FundedPartPFRShortage, Claim = item.Claim, InsurancePartPFRFine = item.InsurancePartPFRFine, InsurancePartPFRShortage = item.InsurancePartPFRShortage };

            await SetViewBag(model);

            return View(model);
        }

        // POST: Instances/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("ClaimId,DateTransfer,CourtDecision,DateCourtDecision,DateInCourtDecision,SumDenied,SumSatisfied,PaidVoluntarily,DutySatisfied,DutyDenied,ServicesSatisfied,ServicesDenied,СostSatisfied,СostDenied,DutyPaid,DateSShortage,DateToShortage,InsurancePartPFRShortage,FundedPartPFRShortage,FFOMSShortage,TFOMSShortage,DateSFine,DateToFine,InsurancePartPFRFine,FundedPartPFRFine,FFOMSFine,TFOMSFine,DateSPenalty,DateToPenalty,SumPenalty,Description,Name,Id,CreatedOnUtc,UpdatedOnUtc")] Instance instance)
        {
            if (id != instance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstanceExists(instance.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClaimId"] = new SelectList(_context.Claims, "Id", "Code", instance.ClaimId);
            return View(instance);
        }

        // GET: Instances/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instance = await _context.Instances
                .Include(i => i.Claim)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (instance == null)
            {
                return NotFound();
            }

            return View(instance);
        }

        // POST: Instances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var instance = await _context.Instances.SingleOrDefaultAsync(m => m.Id == id);
            _context.Instances.Remove(instance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}
