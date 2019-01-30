using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
//using OfficeOpenXml.Table;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Infrastructure.Identity;
using Svr.Web.Extensions;
using Svr.Web.Models;
using Svr.Web.Models.ReportsViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    //https://zennolab.com/discussion/threads/generacija-krasivyx-excel-otchjotov-po-shablonu.33585/

    //https://habr.com/ru/post/109820/
    //http://www.pvsm.ru/programmirovanie/49187#begin

    //https://riptutorial.com/ru/epplus/example/26411/text-alignment-and-word-wrap
    //https://ru.inettools.net/image/opredelit-tsvet-piksela-na-kartinke-onlayn
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
        private readonly ILogger<ClaimsController> logger;
        private readonly IRegionRepository regionRepository;
        private readonly IDistrictRepository districtRepository;

        private readonly ICategoryDisputeRepository categoryDisputeRepository;
        private readonly IGroupClaimRepository groupClaimRepository;
        private readonly ISubjectClaimRepository subjectClaimRepository;

        [TempData]
        public string StatusMessage { get; set; }
        #region Конструктор
        public ReportsController(IHostingEnvironment hostingEnvironment, UserManager<ApplicationUser> userManager, ILogger<ClaimsController> logger, IDistrictRepository districtRepository, IRegionRepository regionRepository, ICategoryDisputeRepository categoryDisputeRepository, IGroupClaimRepository groupClaimRepository, ISubjectClaimRepository subjectClaimRepository)
        {
            this.logger = logger;
            this.userManager = userManager;
            this.regionRepository = regionRepository;
            this.districtRepository = districtRepository;
            this.hostingEnvironment = hostingEnvironment;

            this.categoryDisputeRepository = categoryDisputeRepository;
            this.groupClaimRepository = groupClaimRepository;
            this.subjectClaimRepository = subjectClaimRepository;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //districtRepository = null;
                //regionRepository = null;
                //logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string lord = null, string owner = null, string searchString = null, int page = 1, int itemsPage = 10, DateTime? dateS = null, DateTime? datePo = null, string category = null)
        {
            if (String.IsNullOrEmpty(owner))
            {
                if (String.IsNullOrEmpty(lord))
                {
                    ApplicationUser user = await userManager.FindByNameAsync(User.Identity.Name);
                    owner = user.DistrictId.ToString();
                    lord = "1";
                }
            }
            var path = await GetPath(lord.ToLong(), owner.ToLong());

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
                FilterViewModel = new FilterViewModel(searchString, owner, (await districtRepository.ListAsync(new DistrictSpecification(lord.ToLong()))).Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (owner == a.Id.ToString()) }), lord, (await regionRepository.ListAllAsync()).ToList().Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (lord == a.Id.ToString()) }), dateS, datePo, category, (await categoryDisputeRepository.ListAllAsync()).Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (category == a.Id.ToString()) })),
                StatusMessage = StatusMessage
            };
            return View(indexModel);
        }

        public async Task<IActionResult> InMemoryReport(string lord = null, string owner = null, DateTime? dateS = null, DateTime? datePo = null, string category = null)
        {
            byte[] reportBytes;
            using (var package = await createExcelPackage(lord, owner, dateS, datePo, category))
            {
                if (package == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                reportBytes = package.GetAsByteArray();
            }
            return File(reportBytes, XlsxContentType, GetFileName(dateS, datePo));
        }


        public async Task<IActionResult> FileReport(string lord = null, string owner = null, DateTime? dateS = null, DateTime? datePo = null, string category = null)
        {
            if (String.IsNullOrEmpty(owner))
            {
                if (String.IsNullOrEmpty(lord))
                {
                    ApplicationUser user = await userManager.FindByNameAsync(User.Identity.Name);
                    owner = user.DistrictId.ToString();
                }
            }
            var path = await GetPath(lord.ToLong(), owner.ToLong());
            using (var package = await createExcelPackage(lord, owner, dateS, datePo, category))
            {
                if (package == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                package.SaveAs(new FileInfo(Path.Combine(path, GetFileName(dateS, datePo))));
            }
            return File(path, XlsxContentType, GetFileName(dateS, datePo));
        }



        private async Task<ExcelPackage> createExcelPackage(string lord = null, string owner = null, DateTime? dateS = null, DateTime? datePo = null, string category = null)
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


            //var worksheet = package.Workbook.Worksheets.Add("Employee");
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();



            int i = 14;
            int start = i;
            int s = 0;
            worksheet.Cells[i, 2].Value = "Споры, рассмотренные в арбитражных судах";
            worksheet.Cells[$"A{i}:B{i}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[$"A{i}:B{i}"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ff6600"));
            i++;

            var list = (await groupClaimRepository.ListAsync(new GroupClaimSpecificationReport(category.ToLong()))).OrderBy(a => a.Code.ToLong());
            foreach (var item in list)
            {

                worksheet.Cells[i, 1].Value = item.Code;
                worksheet.Cells[i, 2].Value = item.Name;
                worksheet.Cells[$"A{i}:B{i}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[$"A{i}:B{i}"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#2fcdcd"));
                i++;

                s++;
                //worksheet.Cells[i, 3].Value = item.Name;= СУММ(C$16:C$27)
                var i0 = i;
                var items = item.SubjectClaims.OrderBy(a => a.Code);
                foreach (var item2 in items)
                {
                    worksheet.Cells[i, 1].Value = item2.Code;
                    worksheet.Cells[i++, 2].Value = item2.Name;
                    s++;
                }
                //= СУММ(C$15; C$28; C$31; C$34; C$41; C$47; C$48) Times New Roman
                var i1 = i;
            }
            Font font10 = new Font("Times New Roman", 10);
            Font font8 = new Font("Times New Roman", 8);

            worksheet.Cells[$"A{start}:A{start + s}"].Style.Font.SetFromFont(font10);
            worksheet.Cells[$"B{start}:B{start + s}"].Style.Font.SetFromFont(font8);
            worksheet.Cells[$"B{start}:B{start + s}"].Style.WrapText = true;
            worksheet.Cells[$"A{start}:B{start + s}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[$"A{start}:B{start + s}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;



            //var query = from b in groupClaimRepository.Table().Where(b => b.CategoryDisputeId == 2)
            //            join c in subjectClaimRepository.Table() on b equals c.GroupClaim into gj
            //            from bc in gj.DefaultIfEmpty()
            //            select new { GroupClaimCode = b.Code, GroupClaimName = b.Name, SubjectClaimId = bc?. ?? String.Empty };
            //groupClaimRepository.Table()..In  .Where(b => b.CategoryDisputeId == 2).



            //var list = groupClaimRepository.Table().Where(b => b.CategoryDisputeId == 2).Join(subjectClaimRepository.Table(), b => b.Id, c => c.GroupClaimId, (b, c) => new { GroupClaimCode=b.Code, GroupClaimName = b.Name, SubjectClaimId=c.Id, SubjectClaimName=c.Name});




            //First add the headers
            //worksheet.Cells[1, 1].Value = "ID";
            //worksheet.Cells[1, 2].Value = "Name";
            //worksheet.Cells[1, 3].Value = "Gender";
            //worksheet.Cells[1, 4].Value = "Salary (in $)";

            //Add values

            //var numberformat = "#,##0";
            //var dataCellStyleName = "TableNumber";
            //var numStyle = package.Workbook.Styles.CreateNamedStyle(dataCellStyleName);
            //numStyle.Style.Numberformat.Format = numberformat;

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
        private string GetFileName(DateTime? dateS, DateTime? datePo)
        {
            return $"{dateS?.ToString("yyyy.MM.dd")}-{datePo?.ToString("yyyy.MM.dd")} {fileDownloadName}";
        }

        private async Task<string> GetPath(long? lord, long? owner)
        {
            var path = Path.Combine(hostingEnvironment.WebRootPath, reportsFolder);
            if (lord != null)
            {
                var region = await regionRepository.GetByIdAsync(lord);
                path = Path.Combine(path, region.Name);
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