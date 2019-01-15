﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Infrastructure.Data;
using Svr.Web.Models;
using Svr.Web.Models.FileEntityViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    public class FileEntitiesController : Controller
    {
        private IFileEntityRepository repository;
        private IClaimRepository сlaimRepository;
        private ILogger<FileEntitiesController> logger;
        private IHostingEnvironment appEnvironment;

        [TempData]
        public string StatusMessage { get; set; }
        #region Конструктор
        public FileEntitiesController(IFileEntityRepository repository, IClaimRepository сlaimRepository, IHostingEnvironment appEnvironment, ILogger<FileEntitiesController> logger)
        {
            this.logger = logger;
            this.repository = repository;
            this.сlaimRepository = сlaimRepository;
            this.appEnvironment = appEnvironment;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                сlaimRepository = null;
                repository = null;
                appEnvironment = null;
                logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Index
        // GET: FileEntities
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string owner = null, string searchString = null, int page = 1, int itemsPage = 10)
        {
            long? _owner = null;
            if (!String.IsNullOrEmpty(owner))
            {
                _owner = Int64.Parse(owner);
            }
            var filterSpecification = new FileEntitySpecification(_owner);
            IEnumerable<FileEntity> list = await repository.ListAsync(filterSpecification);
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
                    Path = i.Path
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
        // GET: FileEntities/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = $"Не удалось загрузить файл с ID = {id}.";
                //return RedirectToAction(nameof(Index));
                //return RedirectToAction(nameof(Index), new { owner = model.ClaimId });
                throw new ApplicationException($"Не удалось загрузить инстанцию с ID {id}.");
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, Claim = item.Claim, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, ClaimId = item.ClaimId, Path = item.Path };
            return View(model);
        }
        #endregion
        #region Create
        // GET: FileEntities/Create
        public async Task<IActionResult> Create(long owner)
        {
            ViewBag.Owner = owner;
            return View();
        }

        // POST: FileEntities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Create(ItemViewModel model/*, IFormFile uploadedFile*/)
        {
            if ((ModelState.IsValid) /*&& (uploadedFile != null)*/)
            {
                // путь к папке Files
                model.Path = $"{appEnvironment.WebRootPath}/Files/{model.ClaimId}_{model.UploadedFile.FileName}";
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(model.Path , FileMode.Create))
                {
                    await model.UploadedFile.CopyToAsync( fileStream);
                }

                var item = await repository.AddAsync(new FileEntity { Name = model.Name, ClaimId = model.ClaimId, Description = model.Description, Claim = model.Claim, Path = model.Path });
                if (item != null)
                {


                    StatusMessage = $"Добавлено заседание с Id={item.Id}, имя={item.Name}.";
                    return RedirectToAction(nameof(Index), new { owner = item.ClaimId });
                }
            }
            ModelState.AddModelError(string.Empty, $"Ошибка: {model} - неудачная попытка регистрации.");
            await SetViewBag(model);
            return View(model);
        }
        #endregion
        // GET: FileEntities/Edit/5
        //public async Task<IActionResult> Edit(long? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var fileEntity = await _context.FileEntities.SingleOrDefaultAsync(m => m.Id == id);
        //    if (fileEntity == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["ClaimId"] = new SelectList(_context.Claims, "Id", "Code", fileEntity.ClaimId);
        //    return View(fileEntity);
        //}

        //// POST: FileEntities/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(long id, [Bind("ClaimId,Path,Description,Name,Id,CreatedOnUtc,UpdatedOnUtc")] FileEntity fileEntity)
        //{
        //    if (id != fileEntity.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(fileEntity);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!FileEntityExists(fileEntity.Id))
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
        //    ViewData["ClaimId"] = new SelectList(_context.Claims, "Id", "Code", fileEntity.ClaimId);
        //    return View(fileEntity);
        //}

        //// GET: FileEntities/Delete/5
        //public async Task<IActionResult> Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var fileEntity = await _context.FileEntities
        //        .Include(f => f.Claim)
        //        .SingleOrDefaultAsync(m => m.Id == id);
        //    if (fileEntity == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(fileEntity);
        //}

        //// POST: FileEntities/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(long id)
        //{
        //    var fileEntity = await _context.FileEntities.SingleOrDefaultAsync(m => m.Id == id);
        //    _context.FileEntities.Remove(fileEntity);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private async Task SetViewBag(ItemViewModel model)
        {
        }
    }
}