﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Web.Models;
using Svr.Web.Models.ApplicantViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    public class ApplicantsController : Controller
    {
        private IApplicantRepository repository;
        private IDirRepository dirRepository;
        private ILogger<ApplicantsController> logger;

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
                repository = null;
                dirRepository = null;
                logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Index
        // GET: Applicants
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string owner = null, string parentowner = null, string searchString = null, int page = 1, int itemsPage = 10)
        {
            long? _owner = null;
            if (!String.IsNullOrEmpty(owner))
            {
                _owner = Int64.Parse(owner);
            }
            var filterSpecification = new ApplicantSpecification(_owner);
            IEnumerable<Applicant> list = repository.List(filterSpecification);
            //фильтрация
            //if (_owner != null)
            //{
            //    list = list.Where(d => d.TypeApplicantId == _owner);
            //}
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
                    list = list.OrderBy(s => s.TypeApplicant.Name);
                    break;
                case SortState.OwnerDesc:
                    list = list.OrderByDescending(s => s.TypeApplicant.Name);
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
                    CreatedOnUtc = i.CreatedOnUtc,
                    UpdatedOnUtc = i.UpdatedOnUtc,
                    TypeApplicant = i.TypeApplicant,
                    Opf = i.Opf
                }),
                PageViewModel = new PageViewModel(count, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString, owner, await GetOwners(owner)),
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
                StatusMessage = $"Не удалось загрузить значение с ID = {id} из справочника.";
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, FullName = item.FullName, TypeApplicant = item.TypeApplicant, TypeApplicantId = item.TypeApplicantId, Opf = item.Opf, Address = item.Address, AddressBank = item.AddressBank, Inn = item.Inn, OpfId = item.OpfId, Born = item.Born };
            return View(model);
        }
        #endregion
        #region Create
        // GET: Applicants/Create
        public async Task<ActionResult> Create()
        {
            //SelectList dirName = new SelectList(dirRepository.ListAll(), "Id", "Name", 1);
            ViewBag.Dir = await GetOwners();
            return View();
        }

        // POST: Applicants/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                // добавляем новый Район
                var item = await repository.AddAsync(new Applicant { Name = model.Name, TypeApplicantId = model.TypeApplicantId, Description = model.Description, FullName = model.FullName, OpfId = model.OpfId, Inn = model.Inn, Address = model.Address, AddressBank = model.AddressBank, Born = model.Born });
                if (item != null)
                {
                    StatusMessage = $"Добавлен элемент с Id={item.Id}, имя={item.Name}.";
                    return RedirectToAction(nameof(Index));
                }
            }
            ModelState.AddModelError(string.Empty, $"Ошибка: {model} - неудачная попытка регистрации.");
            ViewBag.Dir = await GetOwners(model.TypeApplicantId.ToString());
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
                StatusMessage = $"Ошибка: Не удалось найти элемент с ID = {id}.";
                return RedirectToAction(nameof(Index));
            }

            var model = new ItemViewModel { Id = item.Id, CreatedOnUtc = item.CreatedOnUtc, Name = item.Name, TypeApplicantId = item.TypeApplicantId, Description = item.Description, FullName = item.FullName, OpfId = item.OpfId, Address = item.Address, AddressBank = item.AddressBank, Born = item.Born, Inn = item.Inn };
            ViewBag.Dir = await GetOwners(item.TypeApplicantId.ToString());
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
                        StatusMessage = $"Непредвиденная ошибка при обновлении элемента с ID {model.Id}. {ex.Message}";
                    }
                }
                //return RedirectToAction(nameof(Index));
                return View(model);
            }
            StatusMessage = $"{model} c ID = {model.Id} проверте правильность заполнения полей";
            ModelState.AddModelError(string.Empty, StatusMessage);
            ViewBag.Dir = await GetOwners(model.TypeApplicantId.ToString());
            //return RedirectToAction($"{nameof(Edit)}/{model.Id}");
            return View(model);
        }
        #endregion
        #region Delete
        // GET: Applicants/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
            {
                StatusMessage = $"Ошибка: Не удалось найти район с ID = {id}.";
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, TypeApplicantId = item.TypeApplicantId, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, StatusMessage = StatusMessage };
            return View(model);
        }
        // POST: Applicants/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await repository.DeleteAsync(new Applicant { Id = model.Id, Name = model.Name });
                StatusMessage = $"Удален {model} с Id={model.Id}, Name = {model.Name}.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка при удалении элемента с Id={model.Id}, Name = {model.Name} - {ex.Message}.";
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion
        private async Task<IEnumerable<SelectListItem>> GetOwners(string owner = null)
        {
            return (await dirRepository.ListAsync(new DirSpecification("Тип контрагента"))).Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (owner == a.Id.ToString()) });
            //return dirRepository.List(new DirSpecification(dirNameId)).Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (owner == a.Id.ToString()) });
        }
    }
}