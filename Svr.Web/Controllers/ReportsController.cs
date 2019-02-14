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
            if (String.IsNullOrEmpty(owner))
            {
                if (String.IsNullOrEmpty(lord))
                {
                    ApplicationUser user = await userManager.FindByNameAsync(User.Identity.Name);
                    owner = user.DistrictId.ToString();
                }
            }
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
            //byte[] reportBytes;
            using (var package = await createExcelPackage(lord, owner, dateS, datePo, category))
            {
                if (package == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                //reportBytes = package.GetAsByteArray();
                package.SaveAs(new FileInfo(Path.Combine(path, GetFileName(dateS, datePo))));
            }
            return File(/*path*//*reportBytes*/Path.Combine(path, GetFileName(dateS, datePo)), XlsxContentType, GetFileName(dateS, datePo));
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
        private async Task<FileInfo> GetFileTemplateName(string category)
        {
            string fileTemplateName;
            if (category.ToLong() == null)
            {
                StatusMessage = $"Ошибка: Выберите категорию.";
                return null;
            }
            else if ((await categoryDisputeRepository.GetByIdAsync(category.ToLong())).Name.ToUpper().Equals("Входящие".ToUpper()))
                fileTemplateName = fileTemplateNameIn;
            else if ((await categoryDisputeRepository.GetByIdAsync(category.ToLong())).Name.ToUpper().Equals("Исходящие".ToUpper()))
                fileTemplateName = fileTemplateNameOut;
            else
            {
                StatusMessage = $"Ошибка: Категория не определена.";
                return null;
            }
            FileInfo template = new FileInfo(Path.Combine(hostingEnvironment.WebRootPath, templatesFolder, fileTemplateName));
            if (!template.Exists)
            {
                StatusMessage = $"Ошибка: Файл Excel-шаблона {fileTemplateName} отсутствует.";
                return null;
            }
            return template;
        }
        private int GetSumInstances(List<Instance> instances, out int countSatisfied, out decimal sumSatisfied, out int countDenied, out decimal sumDenied, out int countEnd, out decimal sumEnd, out int countNo, out decimal sumNo)
        {
            int result = 0;
            countSatisfied = 0;
            sumSatisfied = 0;
            countDenied = 0;
            sumDenied = 0;
            countEnd = 0;
            sumEnd = 0;
            countNo = 0;
            sumNo = 0;
            foreach (var item in instances)
            {
                if (item.CourtDecision != null)
                {
                    result++;
                    if (item.CourtDecision.Name.ToUpper().Equals("Удовлетворено (частично)".ToUpper()))
                    {
                        countSatisfied++;
                        sumSatisfied = sumSatisfied + (item?.SumSatisfied ?? 0);
                        sumDenied = sumDenied + (item?.SumDenied ?? 0);
                    }
                    else
                    if (item.CourtDecision.Name.ToUpper().Equals("Отказано".ToUpper()))
                    {
                        countDenied++;
                        sumDenied = sumDenied + (item?.SumDenied ?? 0);
                    }
                    else
                    if (item.CourtDecision.Name.ToUpper().Equals("Прекращено".ToUpper()))
                    {
                        countEnd++;
                        sumEnd = sumEnd + (item?.Claim?.Sum ?? 0);
                    }
                    else
                    if (item.CourtDecision.Name.ToUpper().Equals("Оставлено без рассмотрения".ToUpper()))
                    {
                        countNo++;
                        sumNo = sumNo + (item?.Claim?.Sum ?? 0);
                    }
                }
            }
            return result;
        }


        private async Task<ExcelPackage> createExcelPackage(string lord = null, string owner = null, DateTime? dateS = null, DateTime? datePo = null, string category = null)
        {
            var template = await GetFileTemplateName(category);
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
                    var acells = from cell in worksheet.Cells["A:A"] where cell.Text.Equals(subjectClaim.Code) select cell;
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
                        worksheet.Cells[$"C{acells.Last().End.Row}"].Value = count;
                        var sum = await claims.SumAsync(c => c.Sum);
                        if (sum != null)
                        {
                            worksheet.Cells[$"D{acells.Last().End.Row}"].Value = sum;
                        }
                    }
                    var instances = instanceRepository.ListReport().Where(i => i.Claim.SubjectClaimId == subjectClaim.Id);
                    if (owner != null)
                    {
                        instances = instances.Where(i => i.Claim.DistrictId == owner.ToLong());
                    }
                    if (dateS != null)
                    {
                        instances = instances.Where(c => c.DateCourtDecision >= dateS);
                    }
                    if (datePo != null)
                    {
                        instances = instances.Where(c => c.DateCourtDecision <= datePo);
                    }
                    int countSatisfied = 0;
                    decimal sumSatisfied = 0;
                    int countDenied = 0;
                    decimal sumDenied = 0;
                    int countEnd = 0;
                    decimal sumEnd = 0;
                    int countNo = 0;
                    decimal sumNo = 0;
                    int countEnd0 = 0;
                    decimal sumEnd0 = 0;
                    int countNo0 = 0;
                    decimal sumNo0 = 0;
                    var instances1 = await instances.Where(i => i.Number == 1).AsNoTracking().ToListAsync();
                    if (instances1.Count > 0)
                    {
                        count = GetSumInstances(instances1, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo);
                        if (count > 0)
                        {
                            worksheet.Cells[$"I{acells.Last().End.Row}"].Value = countSatisfied;
                            worksheet.Cells[$"J{acells.Last().End.Row}"].Value = sumSatisfied;
                            worksheet.Cells[$"U{acells.Last().End.Row}"].Value = countDenied;
                            worksheet.Cells[$"V{acells.Last().End.Row}"].Value = sumDenied;
                            countEnd0 = countEnd0 + countEnd;
                            sumEnd0 = sumEnd0 + sumEnd;
                            countNo0 = countNo0 + countNo;
                            sumNo0 = sumNo0 + sumNo;
                        }
                    }
                    var instances2 = await instances.Where(i => i.Number == 2).AsNoTracking().ToListAsync();
                    if (instances2.Count > 0)
                    {
                        count = GetSumInstances(instances2, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo);
                        if (count > 0)
                        {
                            worksheet.Cells[$"K{acells.Last().End.Row}"].Value = countSatisfied;
                            worksheet.Cells[$"L{acells.Last().End.Row}"].Value = sumSatisfied;
                            worksheet.Cells[$"W{acells.Last().End.Row}"].Value = countDenied;
                            worksheet.Cells[$"X{acells.Last().End.Row}"].Value = sumDenied;
                            countEnd0 = countEnd0 + countEnd;
                            sumEnd0 = sumEnd0 + sumEnd;
                            countNo0 = countNo0 + countNo;
                            sumNo0 = sumNo0 + sumNo;
                        }
                    }
                    var instances3 = await instances.Where(i => i.Number == 3).AsNoTracking().ToListAsync();
                    if (instances3.Count > 0)
                    {
                        count = GetSumInstances(instances3, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo);
                        if (count > 0)
                        {
                            worksheet.Cells[$"M{acells.Last().End.Row}"].Value = countSatisfied;
                            worksheet.Cells[$"N{acells.Last().End.Row}"].Value = sumSatisfied;
                            worksheet.Cells[$"Y{acells.Last().End.Row}"].Value = countDenied;
                            worksheet.Cells[$"Z{acells.Last().End.Row}"].Value = sumDenied;
                            countEnd0 = countEnd0 + countEnd;
                            sumEnd0 = sumEnd0 + sumEnd;
                            countNo0 = countNo0 + countNo;
                            sumNo0 = sumNo0 + sumNo;
                        }
                    }
                    var instances4 = await instances.Where(i => i.Number == 4).AsNoTracking().ToListAsync();
                    if (instances4.Count > 0)
                    {
                        count = GetSumInstances(instances4, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo);
                        if (count > 0)
                        {
                            worksheet.Cells[$"O{acells.Last().End.Row}"].Value = countSatisfied;
                            worksheet.Cells[$"P{acells.Last().End.Row}"].Value = sumSatisfied;
                            worksheet.Cells[$"AA{acells.Last().End.Row}"].Value = countDenied;
                            worksheet.Cells[$"AB{acells.Last().End.Row}"].Value = sumDenied;
                            countEnd0 = countEnd0 + countEnd;
                            sumEnd0 = sumEnd0 + sumEnd;
                            countNo0 = countNo0 + countNo;
                            sumNo0 = sumNo0 + sumNo;
                        }
                    }
                    if (countEnd0 > 0)
                    {
                        worksheet.Cells[$"AE{acells.Last().End.Row}"].Value = countEnd0;
                        worksheet.Cells[$"AF{acells.Last().End.Row}"].Value = sumEnd0;
                    }
                    if (countNo0 > 0)
                    {
                        worksheet.Cells[$"AG{acells.Last().End.Row}"].Value = countNo0;
                        worksheet.Cells[$"AH{acells.Last().End.Row}"].Value = sumNo0;
                    }

                }
            }
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