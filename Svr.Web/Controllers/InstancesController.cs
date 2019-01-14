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
        private IDirRepository dirRepository;
        private ILogger<InstancesController> logger;

        [TempData]
        public string StatusMessage { get; set; }

        #region Конструктор
        public InstancesController(IInstanceRepository repository, IClaimRepository сlaimRepository, IDirRepository dirRepository, ILogger<InstancesController> logger)
        {
            this.logger = logger;
            this.repository = repository;
            this.сlaimRepository = сlaimRepository;
            this.dirRepository = dirRepository;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                сlaimRepository = null;
                repository = null;
                dirRepository = null;
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
                    Claim = i.Claim,
                    Number = i.Number
                }),
                Claim = (await сlaimRepository.GetByIdAsync(_owner)),
                PageViewModel = new PageViewModel(count, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString, owner),

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
                //return RedirectToAction(nameof(Index));
                return RedirectToAction(nameof(Index), new { owner = model.ClaimId });

                //throw new ApplicationException($"Не удалось загрузить район с ID {id}.");
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, Claim = item.Claim, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, CourtDecision = item.CourtDecision, DateCourtDecision = item.DateCourtDecision, DateInCourtDecision = item.DateInCourtDecision, DateSFine = item.DateSFine, DateSPenalty = item.DateSPenalty, DateSShortage = item.DateSShortage, DateToFine = item.DateToFine, DateToPenalty = item.DateToPenalty, DateToShortage = item.DateToShortage, DateTransfer = item.DateTransfer, DutyDenied = item.DutyDenied, DutyPaid = item.DutyPaid, DutySatisfied = item.DutySatisfied, FFOMSFine = item.FFOMSFine, FFOMSShortage = item.FFOMSShortage, FundedPartPFRFine = item.InsurancePartPFRFine, FundedPartPFRShortage = item.FundedPartPFRShortage, InsurancePartPFRFine = item.InsurancePartPFRFine, InsurancePartPFRShortage = item.InsurancePartPFRShortage, PaidVoluntarily = item.PaidVoluntarily, ServicesDenied = item.ServicesDenied, ServicesSatisfied = item.ServicesSatisfied, SumDenied = item.SumDenied, SumPenalty = item.SumPenalty, SumSatisfied = item.SumSatisfied, TFOMSFine = item.TFOMSFine, TFOMSShortage = item.TFOMSShortage, СostDenied = item.СostDenied, СostSatisfied = item.СostSatisfied };
            return View(model);
        }
        #endregion
        #region Create
        // GET: Instances/Create
        public async Task<IActionResult> Create(long owner)
        {
            ViewBag.Number = (await repository.ListAsync(new InstanceSpecification(owner))).Count() + 1;
            ViewBag.Owner = owner;
            switch (ViewBag.Number)
            {
                case 1:
                    ViewBag.CourtDecisions = new SelectList(await dirRepository.ListAsync(new DirSpecification("Решения суда 1-ой инстанции")), "Id", "Name", null);
                    break;
                case 2:
                    ViewBag.CourtDecisions = new SelectList(await dirRepository.ListAsync(new DirSpecification("Решения суда 2-ой инстанции")), "Id", "Name", null);
                    break;
                case 3:
                    ViewBag.CourtDecisions = new SelectList(await dirRepository.ListAsync(new DirSpecification("Решения суда 3-ей инстанции")), "Id", "Name", null);
                    break;
                case 4:
                    ViewBag.CourtDecisions = new SelectList(await dirRepository.ListAsync(new DirSpecification("Решения суда 4-ой инстанции")), "Id", "Name", null);
                    break;
                default:
                    ViewBag.CourtDecisions = null;
                    break;
            }

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
                var item = await repository.AddAsync(new Instance { Name = model.Name, ClaimId = model.ClaimId, Description = model.Description, Claim = model.Claim, CourtDecision = model.CourtDecision, DateCourtDecision = model.DateCourtDecision, СostSatisfied = model.СostSatisfied, СostDenied = model.СostDenied, TFOMSShortage = model.TFOMSShortage, TFOMSFine = model.TFOMSFine, DateInCourtDecision = model.DateInCourtDecision, DateSFine = model.DateSFine, DateSPenalty = model.DateSPenalty, DateSShortage = model.DateSShortage, DateToFine = model.DateToFine, DateToPenalty = model.DateToPenalty, DateToShortage = model.DateToShortage, DateTransfer = model.DateTransfer, DutyDenied = model.DutyDenied, DutyPaid = model.DutyPaid, DutySatisfied = model.DutySatisfied, FFOMSFine = model.FFOMSFine, FFOMSShortage = model.FFOMSShortage, FundedPartPFRFine = model.FundedPartPFRFine, FundedPartPFRShortage = model.FundedPartPFRShortage, InsurancePartPFRFine = model.InsurancePartPFRFine, InsurancePartPFRShortage = model.InsurancePartPFRShortage, PaidVoluntarily = model.PaidVoluntarily, ServicesDenied = model.ServicesDenied, ServicesSatisfied = model.ServicesSatisfied, SumDenied = model.SumDenied, SumPenalty = model.SumPenalty, SumSatisfied = model.SumSatisfied, CourtDecisionId = model.CourtDecisionId, Number = model.Number });
                if (item != null)
                {
                    StatusMessage = $"Добавлена инстанция с Id={item.Id}, имя={item.Name}.";
                    return RedirectToAction(nameof(Index), new { owner = item.ClaimId });
                }
            }
            ModelState.AddModelError(string.Empty, $"Ошибка: {model} - неудачная попытка регистрации.");
            await SetViewBag(model);
            return View(model);
        }
        #endregion
        #region Edit
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
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, TFOMSShortage = item.TFOMSShortage, СostDenied = item.СostDenied, СostSatisfied = item.СostSatisfied, TFOMSFine = item.TFOMSFine, PaidVoluntarily = item.PaidVoluntarily, SumSatisfied = item.SumSatisfied, SumPenalty = item.SumPenalty, SumDenied = item.SumDenied, ServicesSatisfied = item.ServicesSatisfied, ServicesDenied = item.ServicesDenied, CourtDecision = item.CourtDecision, DateCourtDecision = item.DateCourtDecision, DateInCourtDecision = item.DateInCourtDecision, DateSFine = item.DateSFine, DateSPenalty = item.DateSPenalty, DateSShortage = item.DateSShortage, DateToFine = item.DateToFine, DateToPenalty = item.DateToPenalty, DateToShortage = item.DateToShortage, DateTransfer = item.DateTransfer, DutyDenied = item.DutyDenied, DutyPaid = item.DutyPaid, DutySatisfied = item.DutySatisfied, FFOMSFine = item.FFOMSFine, FFOMSShortage = item.FFOMSShortage, FundedPartPFRFine = item.FundedPartPFRFine, FundedPartPFRShortage = item.FundedPartPFRShortage, Claim = item.Claim, InsurancePartPFRFine = item.InsurancePartPFRFine, InsurancePartPFRShortage = item.InsurancePartPFRShortage, ClaimId = item.ClaimId, Number = item.Number };

            await SetViewBag(model);

            return View(model);
        }

        // POST: Instances/Edit/5
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
                    await repository.UpdateAsync(new Instance { Id = model.Id, Description = model.Description, Name = model.Name, CreatedOnUtc = model.CreatedOnUtc, ClaimId = model.ClaimId, CourtDecision = model.CourtDecision, DateCourtDecision = model.DateCourtDecision, DateInCourtDecision = model.DateInCourtDecision, DateSFine = model.DateSFine, DateSPenalty = model.DateSPenalty, DateSShortage = model.DateSShortage, DateToFine = model.DateToFine, DateToPenalty = model.DateToPenalty, DateToShortage = model.DateToShortage, DateTransfer = model.DateTransfer, DutyDenied = model.DutyDenied, DutySatisfied = model.DutySatisfied, DutyPaid = model.DutyPaid, FFOMSFine = model.FFOMSFine, FFOMSShortage = model.FFOMSShortage, FundedPartPFRFine = model.FundedPartPFRFine, FundedPartPFRShortage = model.FundedPartPFRShortage, InsurancePartPFRFine = model.InsurancePartPFRFine, InsurancePartPFRShortage = model.InsurancePartPFRShortage, PaidVoluntarily = model.PaidVoluntarily, ServicesDenied = model.ServicesDenied, SumDenied = model.SumDenied, ServicesSatisfied = model.ServicesSatisfied, SumPenalty = model.SumPenalty, SumSatisfied = model.SumSatisfied, TFOMSFine = model.TFOMSFine, TFOMSShortage = model.TFOMSShortage, СostDenied = model.СostDenied, СostSatisfied = model.СostSatisfied, Number = model.Number });

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
                        StatusMessage = $"Непредвиденная ошибка при обновлении инстанции с ID {model.Id}. {ex.Message}";
                    }
                }
                //return RedirectToAction(nameof(Index));
            }
            await SetViewBag(model);

            return View(model);
        }
        #endregion
        #region Delete
        // GET: Instances/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
            {
                StatusMessage = $"Ошибка: Не удалось найти группу исков с ID = {id}.";
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, StatusMessage = StatusMessage, ClaimId = item.ClaimId };
            return View(model);
        }

        // POST: Instances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await repository.DeleteAsync(new Instance { Id = model.Id, Name = model.Name });
                StatusMessage = $"Удален {model} с Id={model.Id}, Name = {model.Name}.";
                return RedirectToAction(nameof(Index), new { owner = model.ClaimId });
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка при удалении инстанции с Id={model.Id}, Name = {model.Name} - {ex.Message}.";
                return RedirectToAction(nameof(Index), new { owner = model.ClaimId });
            }
        }
        #endregion
        private async Task SetViewBag(ItemViewModel model)
        {
            switch (model.Number)
            {
                case 1:
                    ViewBag.CourtDecisions = new SelectList(await dirRepository.ListAsync(new DirSpecification("Решения суда 1-ой инстанции")), "Id", "Name", null);
                    break;
                case 2:
                    ViewBag.CourtDecisions = new SelectList(await dirRepository.ListAsync(new DirSpecification("Решения суда 2-ой инстанции")), "Id", "Name", null);
                    break;
                case 3:
                    ViewBag.CourtDecisions = new SelectList(await dirRepository.ListAsync(new DirSpecification("Решения суда 3-ей инстанции")), "Id", "Name", null);
                    break;
                case 4:
                    ViewBag.CourtDecisions = new SelectList(await dirRepository.ListAsync(new DirSpecification("Решения суда 4-ой инстанции")), "Id", "Name", null);
                    break;
                default:
                    ViewBag.CourtDecisions = null;
                    break;
            }
        }
    }
}
