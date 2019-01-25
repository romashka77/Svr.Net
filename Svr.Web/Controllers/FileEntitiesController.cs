using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Infrastructure.Data;
using Svr.Web.Extensions;
using Svr.Web.Models;
using Svr.Web.Models.FileEntityViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    [Authorize]
    public class FileEntitiesController : Controller
    {
        private const string filesFolder = "Files";
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
            var filterSpecification = new FileEntitySpecification(owner.ToLong());
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
                Claim = (await сlaimRepository.GetByIdAsync(owner.ToLong())),
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
            if ((ModelState.IsValid) && (model.UploadedFile != null))
            {
                // путь к папке Files
                model.Path = $"{model.ClaimId}_{model.UploadedFile.FileName}";
                model.Name = model.UploadedFile.FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(GetFile(model.Path), FileMode.Create))
                {
                    await model.UploadedFile.CopyToAsync(fileStream);
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
        public async Task<IActionResult> Download(string path)
        {
            if (path == null)
                throw new ApplicationException($"Проверте имя файла.");

            var memory = new MemoryStream();
            using (var stream = new FileStream(GetFile(path), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        #region Edit
        // GET: FileEntities/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = $"Ошибка: Не удалось найти заседание с ID = {id}.";
                //return RedirectToAction(nameof(Index));
                throw new ApplicationException($"Не удалось загрузить заседание с ID {id}.");
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, Claim = item.Claim, ClaimId = item.ClaimId, Path = item.Path };
            await SetViewBag(model);
            return View(model);
        }

        //// POST: FileEntities/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //using (var fileStream = new FileStream(model.Path, FileMode.Create))
                    //{
                    //    await model.UploadedFile.CopyToAsync(fileStream);
                    //}
                    await repository.UpdateAsync(new FileEntity { Id = model.Id, Description = model.Description, Name = model.Name, CreatedOnUtc = model.CreatedOnUtc, ClaimId = model.ClaimId, Path = model.Path });
                    StatusMessage = $"{model} c ID = {model.Id} обновлен";
                    return RedirectToAction(nameof(Index), new { owner = model.ClaimId });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!(await repository.EntityExistsAsync(model.Id)))
                    {
                        StatusMessage = $"Не удалось найти {model} с ID {model.Id}. {ex.Message}";
                    }
                    else
                    {
                        StatusMessage = $"Непредвиденная ошибка при обновлении заседания с ID {model.Id}. {ex.Message}";
                    }
                }
                //return RedirectToAction(nameof(Index));
            }
            await SetViewBag(model);
            return View(model);
        }
        #endregion
        #region Delete
        // GET: FileEntities/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
            {
                //StatusMessage = $"Ошибка: Не удалось найти группу исков с ID = {id}.";
                //return RedirectToAction(nameof(Index));
                throw new ApplicationException($"Не удалось найти заседание с ID {id}.");
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, StatusMessage = StatusMessage, ClaimId = item.ClaimId, Path = item.Path };
            return View(model);
        }

        // POST: FileEntities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await repository.DeleteAsync(new FileEntity { Id = model.Id, Name = model.Name });
                StatusMessage = $"Удален {model} с Id={model.Id}, Name = {model.Name}.";

                FileInfo fileInf = new FileInfo(GetFile(model.Path));
                if (fileInf.Exists)
                {
                    fileInf.Delete();
                    // альтернатива с помощью класса File
                    // File.Delete(path);
                }

                return RedirectToAction(nameof(Index), new { owner = model.ClaimId });
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка при удалении заседания с Id={model.Id}, Name = {model.Name} - {ex.Message}.";
                return RedirectToAction(nameof(Index), new { owner = model.ClaimId });
            }
        }
        #endregion
        private async Task SetViewBag(ItemViewModel model)
        {
        }
        private string GetFile(string patn)
        {
            return $"{appEnvironment.WebRootPath }/{filesFolder}/{patn}";
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}
