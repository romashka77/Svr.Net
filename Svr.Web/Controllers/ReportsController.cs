using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
//using OfficeOpenXml.Table;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Infrastructure.Extensions;
using Svr.Infrastructure.Identity;
using Svr.Web.Extensions;
using Svr.Web.Models;
using Svr.Web.Models.ReportsViewModels;
using System;
using System.Collections;
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
    [Authorize]
    public class ReportsController : Controller
    {
        private const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private readonly IHostingEnvironment hostingEnvironment;
        private const string fileDownloadName = "report.xlsx";
        private const string reportsFolder = "Reports";
        private const string templatesFolder = "Templates";
        private const string fileTemplateNameOut = "0901.xlsx";//"Template1.xlsx";
        private const string fileTemplateNameIn = "0902.xlsx";
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<ClaimsController> logger;
        private readonly IRegionRepository regionRepository;
        private readonly IDistrictRepository districtRepository;

        private readonly ICategoryDisputeRepository categoryDisputeRepository;
        private readonly IGroupClaimRepository groupClaimRepository;
        private readonly IClaimRepository claimRepository;
        private readonly IInstanceRepository instanceRepository;

        [TempData]
        public string StatusMessage { get; set; }
        #region Конструктор
        public ReportsController(IHostingEnvironment hostingEnvironment, UserManager<ApplicationUser> userManager, ILogger<ClaimsController> logger, IDistrictRepository districtRepository, IRegionRepository regionRepository, ICategoryDisputeRepository categoryDisputeRepository, IGroupClaimRepository groupClaimRepository, IClaimRepository claimRepository, IInstanceRepository instanceRepository)
        {
            this.logger = logger;
            this.userManager = userManager;
            this.regionRepository = regionRepository;
            this.districtRepository = districtRepository;
            this.hostingEnvironment = hostingEnvironment;

            this.categoryDisputeRepository = categoryDisputeRepository;
            this.groupClaimRepository = groupClaimRepository;
            this.claimRepository = claimRepository;
            this.instanceRepository = instanceRepository;
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
        private string ListSum(ExcelWorksheet worksheet, List<int> items, int c)
        {
            //var result = "=SUM(";
            var result = "=";
            foreach (var item in items)
            {
                //result = string.Concat(result, worksheet.Cells[item, c].Address, ";");
                result = string.Concat(result, worksheet.Cells[item, c].Address, "+");
            }
            //result = string.Concat(result, ")");
            result = string.Concat(result, "0");
            return result;
        }
        private FileInfo GetFileTemplateName(string category)
        {
            string fileTemplateName;
            if (category.ToLong() == null)
            {
                StatusMessage = $"Ошибка: Выберите категорию.";
                return null;
            }
            else if (category.ToLong() == 3)
                fileTemplateName = fileTemplateNameIn;
            else
                fileTemplateName = fileTemplateNameOut;
            FileInfo template = new FileInfo(Path.Combine(hostingEnvironment.WebRootPath, templatesFolder, fileTemplateName));
            if (!template.Exists)
            {
                StatusMessage = $"Ошибка: Файл Excel-шаблона {fileTemplateName} отсутствует.";
                return null;
            }
            return template;
        }
        private async Task<ExcelPackage> createExcelPackage(string lord = null, string owner = null, DateTime? dateS = null, DateTime? datePo = null, string category = null)
        {
            var template = GetFileTemplateName(category);
            if (template == null) return null;
            ExcelPackage package = new ExcelPackage(template, true);
            package.Workbook.Properties.Author = User.Identity.Name;
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            //Группы споров
            var groupClaims = (await groupClaimRepository.ListAsync(new GroupClaimSpecificationReport(category.ToLong()))).OrderBy(a => a.Code.ToLong());
            foreach (var groupClaim in groupClaims)
            {
                /// Предметы иска
                //var subjectClaims = groupClaim.SubjectClaims.OrderBy(a => a.Code, codeComparer);
                foreach (var subjectClaim in groupClaim.SubjectClaims)
                {
                    var claims = claimRepository.List(new ClaimSpecificationRepost(owner.ToLong())).Where(c => c.SubjectClaimId == subjectClaim.Id);
                    if (dateS != null)
                    {
                        claims = claims.Where(c => c.DateReg >= dateS);
                    }
                    if (datePo != null)
                    {
                        claims = claims.Where(c => c.DateReg <= datePo);
                    }
                    var count = await claims.CountAsync();
                    if (count > 0)
                    {
                        var acells = from cell in worksheet.Cells["A:A"] where cell.Text.Equals(subjectClaim.Code) select cell;
                        worksheet.Cells[$"C{acells.Last().End.Row}"].Value = count;
                        var sum = await claims.SumAsync(c => c.Sum);
                        if (sum!=null)
                        {
                            worksheet.Cells[$"D{acells.Last().End.Row}"].Value = sum;
                        }
                    }
                    var instances = instanceRepository.ListReport().Where(i => i.Claim.SubjectClaimId == subjectClaim.Id);
                    if (dateS != null)
                    {
                        instances = instances.Where(c => c.DateCourtDecision >= dateS);
                    }
                    if (datePo != null)
                    {
                        instances = instances.Where(c => c.DateCourtDecision <= datePo);
                    }
                    count = await claims.CountAsync();
                    if (count > 0)
                    {
                        var acells = from cell in worksheet.Cells["A:A"] where cell.Text.Equals(subjectClaim.Code) select cell;
                    }

                    }
            }
            return package;
        }
        private async Task<ExcelPackage> createExcelPackage0(string lord = null, string owner = null, DateTime? dateS = null, DateTime? datePo = null, string category = null)
        {
            var template = GetFileTemplateName(category);
            if (template==null) return null;
            
            ExcelPackage package = new ExcelPackage(template, true);
            package.Workbook.Properties.Title = "Salary Report";
            package.Workbook.Properties.Author = User.Identity.Name;
            package.Workbook.Properties.Subject = "Salary Report";
            package.Workbook.Properties.Keywords = "Salary";

            //var worksheet = package.Workbook.Worksheets.Add("Employee");
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            Font font10 = new Font("Times New Roman", 10);
            Font font8 = new Font("Times New Roman", 8);
            var numberformat = "#,##0.00";
            //var dataCellStyleName = "TableNumber";
            //var numStyle = package.Workbook.Styles.CreateNamedStyle(dataCellStyleName);
            //numStyle.Style.Numberformat.Format = numberformat;

            worksheet.Cells[2, 4].Style.Numberformat.Format = numberformat;
            CodeComparer codeComparer = new CodeComparer();
            //Regex regex = new Regex(@"[0-9]*[0-9]\.");

            int i = 14;//срока
            int j = 1;//столбец
            int start = i;
            int s = 0;

            bool flg = true;
            List<int> list0 = new List<int>();
            List<int> list1 = new List<int>();

            list0.Add(i);//"Споры, рассмотренные в арбитражных судах";
            worksheet.Cells[$"B{list0.Last()}"].Value = "Споры, рассмотренные в арбитражных судах";
            worksheet.Cells[$"A{i}:AI{i}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[$"A{i}:AI{i}"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ff6600"));
            i++;

            var groupClaims = (await groupClaimRepository.ListAsync(new GroupClaimSpecificationReport(category.ToLong()))).OrderBy(a => a.Code.ToLong());
            foreach (var groupClaim in groupClaims)
            {
                list1.Add(i); //1
                worksheet.Cells[i, j].Value = groupClaim.Code;//A15
                worksheet.Cells[i, j + 1].Value = groupClaim.Name;
                worksheet.Cells[worksheet.Cells[i, 1].Address + ":" + worksheet.Cells[i, 35].Address].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[worksheet.Cells[i, 1].Address + ":" + worksheet.Cells[i, 35].Address].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#2fcdcd"));
                i++;
                s++;
                var subjectClaims = groupClaim.SubjectClaims.OrderBy(a => a.Code, codeComparer);

                foreach (var subjectClaim in subjectClaims)
                {
                    worksheet.Cells[i, 1].Value = subjectClaim.Code;
                    worksheet.Cells[i, 2].Value = subjectClaim.Name;
                    var claims = claimRepository.List(new ClaimSpecificationRepost(owner.ToLong())).Where(c => c.SubjectClaimId == subjectClaim.Id);
                    if (dateS != null)
                    {
                        claims = claims.Where(c => c.DateReg >= dateS);
                    }
                    if (datePo != null)
                    {
                        claims = claims.Where(c => c.DateReg <= datePo);
                    }
                    worksheet.Cells[i, 3].Value = await claims.CountAsync();
                    worksheet.Cells[i, 4].Value = await claims.SumAsync(c => c.Sum);
                    worksheet.Cells[i, 4].Value = worksheet.Cells[i, 4].Value ?? 0;
                    worksheet.Cells[i, 4].Style.Numberformat.Format = numberformat;
                    worksheet.Cells[i, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[i, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[i, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[i, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    i++;
                    s++;
                }
                worksheet.Cells[list1.Last(), 3].Formula = $"=SUM(" + worksheet.Cells[list1.Last() + 1, 3].Address + ":" + worksheet.Cells[i - 1, 3].Address + ")";
                worksheet.Cells[list1.Last(), 4].Formula = $"=SUM(" + worksheet.Cells[list1.Last() + 1, 4].Address + ":" + worksheet.Cells[i - 1, 4].Address + ")";
                worksheet.Cells[list1.Last(), 4].Style.Numberformat.Format = numberformat;
                if ((list1.Count == 7) && (flg))
                {
                    flg = false;
                    worksheet.Cells[list0.Last(), 3].Formula = ListSum(worksheet, list1, 3);
                    worksheet.Cells[list0.Last(), 4].Formula = ListSum(worksheet, list1, 4);
                    worksheet.Cells[list0.Last(), 4].Style.Numberformat.Format = numberformat;

                    list1.Clear();
                    list0.Add(i);
                    worksheet.Cells[list0.Last(), 2].Value = "Споры, рассмотренные в судах общей юрисдикции";
                    worksheet.Cells[worksheet.Cells[i, 1].Address + ":" + worksheet.Cells[i, 35].Address].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[worksheet.Cells[i, 1].Address + ":" + worksheet.Cells[i, 35].Address].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ff6600"));
                    i++; s++;
                }

            }
            worksheet.Cells[list0.Last(), 3].Formula = ListSum(worksheet, list1, 3);
            worksheet.Cells[list0.Last(), 4].Formula = ListSum(worksheet, list1, 4);
            worksheet.Cells[list0.Last(), 4].Style.Numberformat.Format = numberformat;


            worksheet.Cells[worksheet.Cells[start, 1].Address + ":" + worksheet.Cells[start + s, 1].Address].Style.Font.SetFromFont(font10);
            worksheet.Cells[worksheet.Cells[start, 2].Address + ":" + worksheet.Cells[start + s, 2].Address].Style.Font.SetFromFont(font8);
            worksheet.Cells[worksheet.Cells[start, 1].Address + ":" + worksheet.Cells[start + s, 35].Address].Style.WrapText = true;

            worksheet.Cells[worksheet.Cells[start, 1].Address + ":" + worksheet.Cells[start + s, 35].Address].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[worksheet.Cells[start, 1].Address + ":" + worksheet.Cells[start + s, 35].Address].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

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