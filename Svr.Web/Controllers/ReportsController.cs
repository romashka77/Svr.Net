using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Infrastructure.Identity;
using Svr.Web.Extensions;
using Svr.Web.Models;
using Svr.Web.Models.ReportsViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    //https://zennolab.com/discussion/threads/generacija-krasivyx-excel-otchjotov-po-shablonu.33585/
    [Authorize(Roles = "Администратор")]
    public class ReportsController : Controller
    {
        private const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private readonly IHostingEnvironment hostingEnvironment;
        private const string fileDownloadName = "report.xlsx";
        private const string reportsFolder = "Reports";
        private const string templatesFolder = "Templates";
        private const string fileTemplateName = "Template1.xlsx";
        private readonly UserManager<ApplicationUser> userManager;
        private ILogger<ClaimsController> logger;
        private IRegionRepository regionRepository;
        private IDistrictRepository districtRepository;

        [TempData]
        public string StatusMessage { get; set; }
        #region Конструктор
        public ReportsController(IHostingEnvironment hostingEnvironment, UserManager<ApplicationUser> userManager, ILogger<ClaimsController> logger, IDistrictRepository districtRepository, IRegionRepository regionRepository)
        {
            this.logger = logger;
            this.userManager = userManager;
            this.regionRepository = regionRepository;
            this.districtRepository = districtRepository;
            this.hostingEnvironment = hostingEnvironment;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                districtRepository = null;
                regionRepository = null;
                logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string lord = null, string owner = null, string searchString = null, int page = 1, int itemsPage = 10, DateTime? date = null)
        {
            if (String.IsNullOrEmpty( owner))
            {
                if (String.IsNullOrEmpty(lord))
                {
                    ApplicationUser user = await userManager.FindByNameAsync(User.Identity.Name);
                    owner = user.DistrictId.ToString();
                }
            }
            var path = await GetPath(date, owner.ToLong());

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            IEnumerable<FileInfo> list = dirInfo.GetFiles();
            //фильтрация
            if (!String.IsNullOrEmpty(searchString))
            {
                list = list.Where(d => d.Name.ToUpper().Contains(searchString.ToUpper()) || d.Extension.ToUpper().Contains(searchString.ToUpper()));
            }
            // сортировка
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    list = list.OrderByDescending(p => p.Name);
                    break;
                case SortState.CodeAsc:
                    list = list.OrderBy(p => p.Extension);
                    break;
                case SortState.CodeDesc:
                    list = list.OrderByDescending(p => p.Extension);
                    break;
                case SortState.CreatedOnUtcAsc:
                    list = list.OrderBy(p => p.CreationTime);
                    break;
                case SortState.CreatedOnUtcDesc:
                    list = list.OrderByDescending(p => p.CreationTime);
                    break;
                case SortState.UpdatedOnUtcAsc:
                    list = list.OrderBy(p => p.LastWriteTime);
                    break;
                case SortState.UpdatedOnUtcDesc:
                    list = list.OrderByDescending(p => p.LastWriteTime);
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
                    Name = i.Name,
                    Code = i.Extension,
                    CreatedOnUtc = i.CreationTime,
                    UpdatedOnUtc = i.LastWriteTime
                }),
                PageViewModel = new PageViewModel(count, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString, owner, (await districtRepository.ListAsync(new DistrictSpecification(lord.ToLong()))).Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (owner == a.Id.ToString()) }), lord, (await regionRepository.ListAllAsync()).ToList().Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (lord == a.Id.ToString()) })),
                StatusMessage = StatusMessage
            };
            return View(indexModel);
        }

        public IActionResult InMemoryReport()
        {
            byte[] reportBytes;
            using (var package = createExcelPackage())
            {
                if (package == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                reportBytes = package.GetAsByteArray();
            }

            return File(reportBytes, XlsxContentType, fileDownloadName);
        }


        public async Task<IActionResult> FileReport(string lord = null, string owner = null, DateTime? date = null)
        {
            if (String.IsNullOrEmpty(owner))
            {
                if (String.IsNullOrEmpty(lord))
                {
                    ApplicationUser user = await userManager.FindByNameAsync(User.Identity.Name);
                    owner = user.DistrictId.ToString();
                }
            }
            var path = await GetPath(date, owner.ToLong());
            using (var package = createExcelPackage())
            {
                if (package == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                package.SaveAs(new FileInfo(Path.Combine(path, fileDownloadName)));
            }
            return File(path, XlsxContentType, fileDownloadName);
        }

        private ExcelPackage createExcelPackage()
        {
            FileInfo template = new FileInfo(Path.Combine(hostingEnvironment.WebRootPath, templatesFolder, fileTemplateName));
            if (!template.Exists)
            {  //Делаем проверку - если Template.xlsx отсутствует - выходим по красной ветке
                StatusMessage = $"Ошибка: Файл Excel-шаблона {fileTemplateName} отсутствует.";
                return null;
            }
            ExcelPackage package = new ExcelPackage(template, true);
            package.Workbook.Properties.Title = "Salary Report";
            package.Workbook.Properties.Author = User.Identity.Name;
            package.Workbook.Properties.Subject = "Salary Report";
            package.Workbook.Properties.Keywords = "Salary";


            var worksheet = package.Workbook.Worksheets.Add("Employee");

            //First add the headers
            //worksheet.Cells[1, 1].Value = "ID";
            //worksheet.Cells[1, 2].Value = "Name";
            //worksheet.Cells[1, 3].Value = "Gender";
            //worksheet.Cells[1, 4].Value = "Salary (in $)";

            //Add values

            var numberformat = "#,##0";
            var dataCellStyleName = "TableNumber";
            var numStyle = package.Workbook.Styles.CreateNamedStyle(dataCellStyleName);
            numStyle.Style.Numberformat.Format = numberformat;

            //worksheet.Cells[2, 1].Value = 1000;
            //worksheet.Cells[2, 2].Value = "Jon";
            //worksheet.Cells[2, 3].Value = "M";
            //worksheet.Cells[2, 4].Value = 5000;
            //worksheet.Cells[2, 4].Style.Numberformat.Format = numberformat;

            //worksheet.Cells[3, 1].Value = 1001;
            //worksheet.Cells[3, 2].Value = "Graham";
            //worksheet.Cells[3, 3].Value = "M";
            //worksheet.Cells[3, 4].Value = 10000;
            //worksheet.Cells[3, 4].Style.Numberformat.Format = numberformat;

            //worksheet.Cells[4, 1].Value = 1002;
            //worksheet.Cells[4, 2].Value = "Jenny";
            //worksheet.Cells[4, 3].Value = "F";
            //worksheet.Cells[4, 4].Value = 5000;
            //worksheet.Cells[4, 4].Style.Numberformat.Format = numberformat;

            // Add to table / Add summary row
            //var tbl = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: 4, toColumn: 4), "Data");
            //tbl.ShowHeader = true;
            //tbl.TableStyle = TableStyles.Dark9;
            //tbl.ShowTotal = true;
            //tbl.Columns[3].DataCellStyleName = dataCellStyleName;
            //tbl.Columns[3].TotalsRowFunction = RowFunctions.Sum;
            //worksheet.Cells[5, 4].Style.Numberformat.Format = numberformat;

            //// AutoFitColumns
            //worksheet.Cells[1, 1, 4, 4].AutoFitColumns();

            //worksheet.HeaderFooter.OddFooter.InsertPicture(
            //    new FileInfo(Path.Combine(hostingEnvironment.WebRootPath, "images", "captcha.jpg")),
            //    PictureAlignment.Right);
            return package;
        }
        private async Task<string> GetPath(DateTime? date, long? owner)
        {
            var path = Path.Combine(hostingEnvironment.WebRootPath, reportsFolder);
            if (date != null)
            {
                path = Path.Combine(path, ((DateTime)date).ToString("yyyy"));
            }
            if (owner != null)
            {
                var district = await districtRepository.GetByIdAsync(owner);
                path = Path.Combine(path, district.Name);
            }
            return path;
        }
    }
}