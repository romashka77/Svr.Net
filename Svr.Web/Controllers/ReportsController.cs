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
    //https://stackoverflow.com/questions/3604562/download-file-of-any-type-in-asp-net-mvc-using-fileresult
    [Authorize]
    public class ReportsController : Controller
    {
        private const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private readonly IHostingEnvironment hostingEnvironment;
        private FileInfo template;
        private string reportsFolder = "Reports";
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
        private int GetSumInstances(List<Instance> instances, out int countSatisfied, out decimal sumSatisfied, out int countDenied, out decimal sumDenied, out int countEnd, out decimal sumEnd, out int countNo, out decimal sumNo, ref int countDutySatisfied, ref decimal dutySatisfied, ref int countDutyDenied, ref decimal dutyDenied, ref int countServicesSatisfied, ref decimal servicesSatisfied, ref int countServicesDenied, ref decimal servicesDenied, ref int countСostSatisfied, ref decimal costSatisfied, ref int countСostDenied, ref decimal costDenied, ref int countDutyPaid, ref decimal dutyPaid)
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
                    if (item?.DutySatisfied != null && item.DutySatisfied > 0)
                    {
                        countDutySatisfied++;
                        dutySatisfied = dutySatisfied + (item?.DutySatisfied ?? 0);
                    }
                    if (item?.DutyDenied != null && item?.DutyDenied > 0)
                    {
                        countDutyDenied++;
                        dutyDenied = dutyDenied + (item?.DutyDenied ?? 0);
                    }
                    if (item?.ServicesSatisfied != null && item?.ServicesSatisfied > 0)
                    {
                        countServicesSatisfied++;
                        servicesSatisfied = servicesSatisfied + (item?.ServicesSatisfied ?? 0);
                    }
                    if (item?.ServicesDenied != null && item?.ServicesDenied > 0)
                    {
                        countServicesDenied++;
                        servicesDenied = servicesDenied + (item?.ServicesDenied ?? 0);
                    }
                    if (item?.СostSatisfied != null && item?.СostSatisfied > 0)
                    {
                        countСostSatisfied++;
                        costSatisfied = costSatisfied + (item?.СostSatisfied ?? 0);
                    }
                    if (item?.СostDenied != null && item?.СostDenied > 0)
                    {
                        countСostDenied++;
                        costDenied = costDenied + (item?.СostDenied ?? 0);
                    }
                    //-----------------
                    if (item?.DutyPaid != null && item?.DutyPaid > 0)
                    {
                        countDutyPaid++;
                        dutyPaid = dutyPaid + (item?.DutyPaid ?? 0);
                    }
                }
            }
            return result;
        }

        private async Task<ExcelPackage> createExcelPackage(string lord = null, string owner = null, DateTime? dateS = null, DateTime? datePo = null, string category = null)
        {
            template = await GetFileTemplateName(category);
            if (template == null) return null;
            ExcelPackage package = new ExcelPackage( template, true);
            package.Workbook.Properties.Author = User.Identity.Name;
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            //Группы споров
            var groupClaims = (await groupClaimRepository.ListAsync(new GroupClaimSpecificationReport(category.ToLong()))).OrderBy(a => a.Code.ToLong());

            int countDutySatisfied = 0;
            decimal dutySatisfied = 0;
            int countDutyDenied = 0;
            decimal dutyDenied = 0;
            int countServicesSatisfied = 0;
            decimal servicesSatisfied = 0;
            int countServicesDenied = 0;
            decimal servicesDenied = 0;
            int countСostSatisfied = 0;
            decimal costSatisfied = 0;
            int countСostDenied = 0;
            decimal costDenied = 0;


            int countDutySatisfied1 = 0;
            decimal dutySatisfied1 = 0;
            int countDutySatisfied11 = 0;
            decimal dutySatisfied11 = 0;
            int countDutyDenied1 = 0;
            decimal dutyDenied1 = 0;
            int countDutyDenied11 = 0;
            decimal dutyDenied11 = 0;
            int countServicesSatisfied1 = 0;
            decimal servicesSatisfied1 = 0;
            int countServicesSatisfied11 = 0;
            decimal servicesSatisfied11 = 0;
            int countServicesDenied1 = 0;
            decimal servicesDenied1 = 0;
            int countServicesDenied11 = 0;
            decimal servicesDenied11 = 0;
            int countСostSatisfied1 = 0;
            decimal costSatisfied1 = 0;
            int countСostSatisfied11 = 0;
            decimal costSatisfied11 = 0;
            int countСostDenied1 = 0;
            decimal costDenied1 = 0;
            int countСostDenied11 = 0;
            decimal costDenied11 = 0;

            int countDutySatisfied2 = 0;
            decimal dutySatisfied2 = 0;
            int countDutySatisfied22 = 0;
            decimal dutySatisfied22 = 0;
            int countDutyDenied2 = 0;
            decimal dutyDenied2 = 0;
            int countDutyDenied22 = 0;
            decimal dutyDenied22 = 0;
            int countServicesSatisfied2 = 0;
            decimal servicesSatisfied2 = 0;
            int countServicesSatisfied22 = 0;
            decimal servicesSatisfied22 = 0;
            int countServicesDenied2 = 0;
            decimal servicesDenied2 = 0;
            int countServicesDenied22 = 0;
            decimal servicesDenied22 = 0;
            int countСostSatisfied2 = 0;
            decimal costSatisfied2 = 0;
            int countСostSatisfied22 = 0;
            decimal costSatisfied22 = 0;
            int countСostDenied2 = 0;
            decimal costDenied2 = 0;
            int countСostDenied22 = 0;
            decimal costDenied22 = 0;

            int countDutySatisfied3 = 0;
            decimal dutySatisfied3 = 0;
            int countDutySatisfied33 = 0;
            decimal dutySatisfied33 = 0;
            int countDutyDenied3 = 0;
            decimal dutyDenied3 = 0;
            int countDutyDenied33 = 0;
            decimal dutyDenied33 = 0;
            int countServicesSatisfied3 = 0;
            decimal servicesSatisfied3 = 0;
            int countServicesSatisfied33 = 0;
            decimal servicesSatisfied33 = 0;
            int countServicesDenied3 = 0;
            decimal servicesDenied3 = 0;
            int countServicesDenied33 = 0;
            decimal servicesDenied33 = 0;
            int countСostSatisfied3 = 0;
            decimal costSatisfied3 = 0;
            int countСostSatisfied33 = 0;
            decimal costSatisfied33 = 0;
            int countСostDenied3 = 0;
            decimal costDenied3 = 0;
            int countСostDenied33 = 0;
            decimal costDenied33 = 0;

            int countDutySatisfied4 = 0;
            decimal dutySatisfied4 = 0;
            int countDutySatisfied44 = 0;
            decimal dutySatisfied44 = 0;
            int countDutyDenied4 = 0;
            decimal dutyDenied4 = 0;
            int countDutyDenied44 = 0;
            decimal dutyDenied44 = 0;
            int countServicesSatisfied4 = 0;
            decimal servicesSatisfied4 = 0;
            int countServicesSatisfied44 = 0;
            decimal servicesSatisfied44 = 0;
            int countServicesDenied4 = 0;
            decimal servicesDenied4 = 0;
            int countServicesDenied44 = 0;
            decimal servicesDenied44 = 0;
            int countСostSatisfied4 = 0;
            decimal costSatisfied4 = 0;
            int countСostSatisfied44 = 0;
            decimal costSatisfied44 = 0;
            int countСostDenied4 = 0;
            decimal costDenied4 = 0;
            int countСostDenied44 = 0;
            decimal costDenied44 = 0;

            int countDutyPaid = 0;
            decimal dutyPaid = 0;

            foreach (var groupClaim in groupClaims)
            {
                byte flg = 0;
                long? groupClaimCode = groupClaim.Code.ToLong();
                if ((template.Name.Equals(fileTemplateNameIn) && (groupClaimCode > 0) && (groupClaimCode < 5)) || (template.Name.Equals(fileTemplateNameOut) && (groupClaimCode > 0) && (groupClaimCode < 5)))
                {
                    flg = 1;
                }
                else if ((template.Name.Equals(fileTemplateNameIn) && (groupClaimCode > 4) && (groupClaimCode < 25)) || (template.Name.Equals(fileTemplateNameOut) && (groupClaimCode > 4) && (groupClaimCode < 18)))
                {
                    flg = 2;
                }
                /// Предметы иска
                //var subjectClaims = groupClaim.SubjectClaims.OrderBy(a => a.Code, codeComparer);
                foreach (var subjectClaim in groupClaim.SubjectClaims)
                {
                    var claims = claimRepository.List(new ClaimSpecificationReport(owner.ToLong())).Where(c => c.SubjectClaimId == subjectClaim.Id);
                    var acells = from cell in worksheet.Cells["A:A"] where cell.Text.Equals(subjectClaim.Code) select cell;
                    if (dateS != null)
                    {
                        claims = claims.Where(c => c.DateIn >= dateS);
                    }
                    if (datePo != null)
                    {
                        claims = claims.Where(c => c.DateIn <= datePo);
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
                        instances = instances.Where(c => c.DateInCourtDecision >= dateS);
                    }
                    if (datePo != null)
                    {
                        instances = instances.Where(c => c.DateInCourtDecision <= datePo);
                    }

                    int countEnd0 = 0;
                    decimal sumEnd0 = 0;
                    int countNo0 = 0;
                    decimal sumNo0 = 0;
                    int countSatisfied = 0; decimal sumSatisfied = 0; int countDenied = 0; decimal sumDenied = 0; int countEnd = 0; decimal sumEnd = 0; int countNo = 0; decimal sumNo = 0;

                    //int countDutySatisfied = 0;
                    //decimal dutySatisfied = 0;

                    var instances1 = await instances.Where(i => i.Number == 1).AsNoTracking().ToListAsync();
                    if (instances1.Count > 0)
                    {
                        if (flg == 1)
                        {
                            count = GetSumInstances(instances1, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, ref countDutySatisfied1, ref dutySatisfied1, ref countDutyDenied1, ref dutyDenied1, ref countServicesSatisfied1, ref servicesSatisfied1, ref countServicesDenied1, ref servicesDenied1, ref countСostSatisfied1, ref costSatisfied1, ref countСostDenied1, ref costDenied1, ref countDutyPaid, ref dutyPaid);
                        }
                        else if (flg == 2)
                        {
                            count = GetSumInstances(instances1, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, ref countDutySatisfied11, ref dutySatisfied11, ref countDutyDenied11, ref dutyDenied11, ref countServicesSatisfied11, ref servicesSatisfied11, ref countServicesDenied11, ref servicesDenied11, ref countСostSatisfied11, ref costSatisfied11, ref countСostDenied11, ref costDenied11, ref countDutyPaid, ref dutyPaid);
                        }
                        else
                            count = GetSumInstances(instances1, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, ref countDutySatisfied, ref dutySatisfied, ref countDutyDenied, ref dutyDenied, ref countServicesSatisfied, ref servicesSatisfied, ref countServicesDenied, ref servicesDenied, ref countСostSatisfied, ref costSatisfied, ref countСostDenied, ref costDenied, ref countDutyPaid, ref dutyPaid);
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
                        if (flg == 1)
                        {
                            count = GetSumInstances(instances2, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, ref countDutySatisfied2, ref dutySatisfied2, ref countDutyDenied2, ref dutyDenied2, ref countServicesSatisfied2, ref servicesSatisfied2, ref countServicesDenied2, ref servicesDenied2, ref countСostSatisfied2, ref costSatisfied2, ref countСostDenied2, ref costDenied2, ref countDutyPaid, ref dutyPaid);

                        }
                        else if (flg == 2)
                        {
                            count = GetSumInstances(instances2, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, ref countDutySatisfied22, ref dutySatisfied22, ref countDutyDenied22, ref dutyDenied22, ref countServicesSatisfied22, ref servicesSatisfied22, ref countServicesDenied22, ref servicesDenied22, ref countСostSatisfied22, ref costSatisfied22, ref countСostDenied22, ref costDenied22, ref countDutyPaid, ref dutyPaid);
                        }
                        else
                            count = GetSumInstances(instances2, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, ref countDutySatisfied, ref dutySatisfied, ref countDutyDenied, ref dutyDenied, ref countServicesSatisfied, ref servicesSatisfied, ref countServicesDenied, ref servicesDenied, ref countСostSatisfied, ref costSatisfied, ref countСostDenied, ref costDenied, ref countDutyPaid, ref dutyPaid);
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
                        if (flg == 1)
                        {
                            count = GetSumInstances(instances3, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, ref countDutySatisfied3, ref dutySatisfied3, ref countDutyDenied3, ref dutyDenied3, ref countServicesSatisfied3, ref servicesSatisfied3, ref countServicesDenied3, ref servicesDenied3, ref countСostSatisfied3, ref costSatisfied3, ref countСostDenied3, ref costDenied3, ref countDutyPaid, ref dutyPaid);
                        }
                        else if (flg == 2)
                        {
                            count = GetSumInstances(instances3, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, ref countDutySatisfied33, ref dutySatisfied33, ref countDutyDenied33, ref dutyDenied33, ref countServicesSatisfied33, ref servicesSatisfied33, ref countServicesDenied33, ref servicesDenied33, ref countСostSatisfied33, ref costSatisfied33, ref countСostDenied33, ref costDenied33, ref countDutyPaid, ref dutyPaid);
                        }
                        else
                            count = GetSumInstances(instances3, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, ref countDutySatisfied, ref dutySatisfied, ref countDutyDenied, ref dutyDenied, ref countServicesSatisfied, ref servicesSatisfied, ref countServicesDenied, ref servicesDenied, ref countСostSatisfied, ref costSatisfied, ref countСostDenied, ref costDenied, ref countDutyPaid, ref dutyPaid);
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
                        if (flg == 1)
                        {
                            count = GetSumInstances(instances4, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, ref countDutySatisfied4, ref dutySatisfied4, ref countDutyDenied4, ref dutyDenied4, ref countServicesSatisfied4, ref servicesSatisfied4, ref countServicesDenied4, ref servicesDenied4, ref countСostSatisfied4, ref costSatisfied4, ref countСostDenied4, ref costDenied4, ref countDutyPaid, ref dutyPaid);
                        }
                        else if (flg == 2)
                        {
                            count = GetSumInstances(instances4, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, ref countDutySatisfied44, ref dutySatisfied44, ref countDutyDenied44, ref dutyDenied44, ref countServicesSatisfied44, ref servicesSatisfied44, ref countServicesDenied44, ref servicesDenied44, ref countСostSatisfied44, ref costSatisfied44, ref countСostDenied44, ref costDenied44, ref countDutyPaid, ref dutyPaid);
                        }
                        else
                            count = GetSumInstances(instances4, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, ref countDutySatisfied, ref dutySatisfied, ref countDutyDenied, ref dutyDenied, ref countServicesSatisfied, ref servicesSatisfied, ref countServicesDenied, ref servicesDenied, ref countСostSatisfied, ref costSatisfied, ref countСostDenied, ref costDenied, ref countDutyPaid, ref dutyPaid);
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
            //if (template.Name.Equals(fileTemplateNameIn))
            {
                if (countDutySatisfied1 > 0 || countDutyDenied1 > 0 || countDutySatisfied2 > 0 || countDutyDenied2 > 0 || countDutySatisfied3 > 0 || countDutyDenied3 > 0 || countDutySatisfied4 > 0 || countDutyDenied4 > 0)
                {
                    string cat = "";
                    if (template.Name.Equals(fileTemplateNameIn))
                        cat = "25.1";
                    else
                        cat = "20.1";
                    var acells = from cell in worksheet.Cells["A:A"] where cell.Text.Equals(cat) select cell;
                    worksheet.Cells[$"I{acells.Last().End.Row}"].Value = countDutySatisfied1 + int.Parse(worksheet.Cells[$"I{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"J{acells.Last().End.Row}"].Value = dutySatisfied1 + decimal.Parse(worksheet.Cells[$"J{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"U{acells.Last().End.Row}"].Value = countDutyDenied1 + int.Parse(worksheet.Cells[$"U{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"V{acells.Last().End.Row}"].Value = dutyDenied1 + decimal.Parse(worksheet.Cells[$"V{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"K{acells.Last().End.Row}"].Value = countDutySatisfied2 + int.Parse(worksheet.Cells[$"K{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"L{acells.Last().End.Row}"].Value = dutySatisfied2 + decimal.Parse(worksheet.Cells[$"L{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"W{acells.Last().End.Row}"].Value = countDutyDenied2 + int.Parse(worksheet.Cells[$"W{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"X{acells.Last().End.Row}"].Value = dutyDenied2 + decimal.Parse(worksheet.Cells[$"X{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"M{acells.Last().End.Row}"].Value = countDutySatisfied3 + int.Parse(worksheet.Cells[$"M{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"N{acells.Last().End.Row}"].Value = dutySatisfied3 + decimal.Parse(worksheet.Cells[$"N{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"Y{acells.Last().End.Row}"].Value = countDutyDenied3 + int.Parse(worksheet.Cells[$"Y{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"Z{acells.Last().End.Row}"].Value = dutyDenied3 + decimal.Parse(worksheet.Cells[$"Z{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"O{acells.Last().End.Row}"].Value = countDutySatisfied4 + int.Parse(worksheet.Cells[$"O{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"P{acells.Last().End.Row}"].Value = dutySatisfied4 + decimal.Parse(worksheet.Cells[$"P{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"AA{acells.Last().End.Row}"].Value = countDutyDenied4 + int.Parse(worksheet.Cells[$"AA{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"AB{acells.Last().End.Row}"].Value = dutyDenied4 + decimal.Parse(worksheet.Cells[$"AB{acells.Last().End.Row}"].Text);
                }
                if (countServicesSatisfied1 > 0 || countServicesDenied1 > 0 || countServicesSatisfied2 > 0 || countServicesDenied2 > 0 || countServicesSatisfied3 > 0 || countServicesDenied3 > 0 || countServicesSatisfied4 > 0 || countServicesDenied4 > 0)
                {
                    string cat = "";
                    if (template.Name.Equals(fileTemplateNameIn))
                        cat = "25.2";
                    var acells = from cell in worksheet.Cells["A:A"] where cell.Text.Equals(cat) select cell;
                    worksheet.Cells[$"I{acells.Last().End.Row}"].Value = countServicesSatisfied1 + int.Parse(worksheet.Cells[$"I{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"J{acells.Last().End.Row}"].Value = servicesSatisfied1 + decimal.Parse(worksheet.Cells[$"J{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"U{acells.Last().End.Row}"].Value = countServicesDenied1 + int.Parse(worksheet.Cells[$"U{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"V{acells.Last().End.Row}"].Value = servicesDenied1 + decimal.Parse(worksheet.Cells[$"V{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"K{acells.Last().End.Row}"].Value = countServicesSatisfied2 + int.Parse(worksheet.Cells[$"K{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"L{acells.Last().End.Row}"].Value = servicesSatisfied2 + decimal.Parse(worksheet.Cells[$"L{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"W{acells.Last().End.Row}"].Value = countServicesDenied2 + int.Parse(worksheet.Cells[$"W{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"X{acells.Last().End.Row}"].Value = servicesDenied2 + decimal.Parse(worksheet.Cells[$"X{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"M{acells.Last().End.Row}"].Value = countServicesSatisfied3 + int.Parse(worksheet.Cells[$"M{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"N{acells.Last().End.Row}"].Value = servicesSatisfied3 + decimal.Parse(worksheet.Cells[$"N{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"Y{acells.Last().End.Row}"].Value = countServicesDenied3 + int.Parse(worksheet.Cells[$"Y{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"Z{acells.Last().End.Row}"].Value = servicesDenied3 + decimal.Parse(worksheet.Cells[$"Z{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"O{acells.Last().End.Row}"].Value = countServicesSatisfied4 + int.Parse(worksheet.Cells[$"O{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"P{acells.Last().End.Row}"].Value = servicesSatisfied4 + decimal.Parse(worksheet.Cells[$"P{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"AA{acells.Last().End.Row}"].Value = countServicesDenied4 + int.Parse(worksheet.Cells[$"AA{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"AB{acells.Last().End.Row}"].Value = servicesDenied4 + decimal.Parse(worksheet.Cells[$"AB{acells.Last().End.Row}"].Text);
                }
                if (countСostSatisfied1 > 0 || countСostDenied1 > 0 || countСostSatisfied2 > 0 || countСostDenied2 > 0 || countСostSatisfied3 > 0 || countСostDenied3 > 0 || countСostSatisfied4 > 0 || countСostDenied4 > 0)
                {
                    string cat = "";
                    if (template.Name.Equals(fileTemplateNameIn))
                        cat = "25.3";
                    else
                        cat = "20.2";
                    var acells = from cell in worksheet.Cells["A:A"] where cell.Text.Equals(cat) select cell;
                    worksheet.Cells[$"I{acells.Last().End.Row}"].Value = countСostSatisfied1 + int.Parse(worksheet.Cells[$"I{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"J{acells.Last().End.Row}"].Value = costSatisfied1 + decimal.Parse(worksheet.Cells[$"J{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"U{acells.Last().End.Row}"].Value = countСostDenied1 + int.Parse(worksheet.Cells[$"U{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"V{acells.Last().End.Row}"].Value = costDenied1 + decimal.Parse(worksheet.Cells[$"V{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"K{acells.Last().End.Row}"].Value = countСostSatisfied2 + int.Parse(worksheet.Cells[$"K{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"L{acells.Last().End.Row}"].Value = costSatisfied2 + decimal.Parse(worksheet.Cells[$"L{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"W{acells.Last().End.Row}"].Value = countСostDenied2 + int.Parse(worksheet.Cells[$"W{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"X{acells.Last().End.Row}"].Value = costDenied2 + decimal.Parse(worksheet.Cells[$"X{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"M{acells.Last().End.Row}"].Value = countСostSatisfied3 + int.Parse(worksheet.Cells[$"M{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"N{acells.Last().End.Row}"].Value = costSatisfied3 + decimal.Parse(worksheet.Cells[$"N{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"Y{acells.Last().End.Row}"].Value = countСostDenied3 + int.Parse(worksheet.Cells[$"Y{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"Z{acells.Last().End.Row}"].Value = costDenied3 + decimal.Parse(worksheet.Cells[$"Z{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"O{acells.Last().End.Row}"].Value = countСostSatisfied4 + int.Parse(worksheet.Cells[$"O{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"P{acells.Last().End.Row}"].Value = costSatisfied4 + decimal.Parse(worksheet.Cells[$"P{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"AA{acells.Last().End.Row}"].Value = countСostDenied4 + int.Parse(worksheet.Cells[$"AA{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"AB{acells.Last().End.Row}"].Value = costDenied4 + decimal.Parse(worksheet.Cells[$"AB{acells.Last().End.Row}"].Text);
                }

                if (countDutySatisfied11 > 0 || countDutyDenied11 > 0 || countDutySatisfied22 > 0 || countDutyDenied22 > 0 || countDutySatisfied33 > 0 || countDutyDenied33 > 0 || countDutySatisfied44 > 0 || countDutyDenied44 > 0)
                {
                    string cat = "";
                    if (template.Name.Equals(fileTemplateNameIn))
                        cat = "25.4";
                    else
                        cat = "20.3";
                    var acells = from cell in worksheet.Cells["A:A"] where cell.Text.Equals(cat) select cell;
                    worksheet.Cells[$"I{acells.Last().End.Row}"].Value = countDutySatisfied11 + int.Parse(worksheet.Cells[$"I{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"J{acells.Last().End.Row}"].Value = dutySatisfied11 + decimal.Parse(worksheet.Cells[$"J{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"U{acells.Last().End.Row}"].Value = countDutyDenied11 + int.Parse(worksheet.Cells[$"U{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"V{acells.Last().End.Row}"].Value = dutyDenied11 + decimal.Parse(worksheet.Cells[$"V{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"K{acells.Last().End.Row}"].Value = countDutySatisfied22 + int.Parse(worksheet.Cells[$"K{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"L{acells.Last().End.Row}"].Value = dutySatisfied22 + decimal.Parse(worksheet.Cells[$"L{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"W{acells.Last().End.Row}"].Value = countDutyDenied22 + int.Parse(worksheet.Cells[$"W{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"X{acells.Last().End.Row}"].Value = dutyDenied22 + decimal.Parse(worksheet.Cells[$"X{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"M{acells.Last().End.Row}"].Value = countDutySatisfied33 + int.Parse(worksheet.Cells[$"M{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"N{acells.Last().End.Row}"].Value = dutySatisfied33 + decimal.Parse(worksheet.Cells[$"N{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"Y{acells.Last().End.Row}"].Value = countDutyDenied33 + int.Parse(worksheet.Cells[$"Y{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"Z{acells.Last().End.Row}"].Value = dutyDenied33 + decimal.Parse(worksheet.Cells[$"Z{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"O{acells.Last().End.Row}"].Value = countDutySatisfied44 + int.Parse(worksheet.Cells[$"O{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"P{acells.Last().End.Row}"].Value = dutySatisfied44 + decimal.Parse(worksheet.Cells[$"P{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"AA{acells.Last().End.Row}"].Value = countDutyDenied44 + int.Parse(worksheet.Cells[$"AA{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"AB{acells.Last().End.Row}"].Value = dutyDenied44 + decimal.Parse(worksheet.Cells[$"AB{acells.Last().End.Row}"].Text);
                }
                if (countServicesSatisfied11 > 0 || countServicesDenied11 > 0 || countServicesSatisfied22 > 0 || countServicesDenied22 > 0 || countServicesSatisfied33 > 0 || countServicesDenied33 > 0 || countServicesSatisfied44 > 0 || countServicesDenied44 > 0)
                {
                    string cat = "";
                    if (template.Name.Equals(fileTemplateNameIn))
                        cat = "25.5";
                    var acells = from cell in worksheet.Cells["A:A"] where cell.Text.Equals(cat) select cell;
                    worksheet.Cells[$"I{acells.Last().End.Row}"].Value = countServicesSatisfied11 + int.Parse(worksheet.Cells[$"I{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"J{acells.Last().End.Row}"].Value = servicesSatisfied11 + decimal.Parse(worksheet.Cells[$"J{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"U{acells.Last().End.Row}"].Value = countServicesDenied11 + int.Parse(worksheet.Cells[$"U{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"V{acells.Last().End.Row}"].Value = servicesDenied11 + decimal.Parse(worksheet.Cells[$"V{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"K{acells.Last().End.Row}"].Value = countServicesSatisfied22 + int.Parse(worksheet.Cells[$"K{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"L{acells.Last().End.Row}"].Value = servicesSatisfied22 + decimal.Parse(worksheet.Cells[$"L{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"W{acells.Last().End.Row}"].Value = countServicesDenied22 + int.Parse(worksheet.Cells[$"W{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"X{acells.Last().End.Row}"].Value = servicesDenied22 + decimal.Parse(worksheet.Cells[$"X{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"M{acells.Last().End.Row}"].Value = countServicesSatisfied33 + int.Parse(worksheet.Cells[$"M{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"N{acells.Last().End.Row}"].Value = servicesSatisfied33 + decimal.Parse(worksheet.Cells[$"N{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"Y{acells.Last().End.Row}"].Value = countServicesDenied33 + int.Parse(worksheet.Cells[$"Y{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"Z{acells.Last().End.Row}"].Value = servicesDenied33 + decimal.Parse(worksheet.Cells[$"Z{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"O{acells.Last().End.Row}"].Value = countServicesSatisfied44 + int.Parse(worksheet.Cells[$"O{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"P{acells.Last().End.Row}"].Value = servicesSatisfied44 + decimal.Parse(worksheet.Cells[$"P{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"AA{acells.Last().End.Row}"].Value = countServicesDenied44 + int.Parse(worksheet.Cells[$"AA{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"AB{acells.Last().End.Row}"].Value = servicesDenied44 + decimal.Parse(worksheet.Cells[$"AB{acells.Last().End.Row}"].Text);
                }
                if (countСostSatisfied11 > 0 || countСostDenied11 > 0 || countСostSatisfied22 > 0 || countСostDenied22 > 0 || countСostSatisfied33 > 0 || countСostDenied33 > 0)
                {
                    string cat = "";
                    if (template.Name.Equals(fileTemplateNameIn))
                        cat = "25.6";
                    else
                        cat = "20.4";
                    var acells = from cell in worksheet.Cells["A:A"] where cell.Text.Equals(cat) select cell;
                    worksheet.Cells[$"I{acells.Last().End.Row}"].Value = countСostSatisfied11 + int.Parse(worksheet.Cells[$"I{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"J{acells.Last().End.Row}"].Value = costSatisfied11 + decimal.Parse(worksheet.Cells[$"J{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"U{acells.Last().End.Row}"].Value = countСostDenied11 + int.Parse(worksheet.Cells[$"U{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"V{acells.Last().End.Row}"].Value = costDenied11 + decimal.Parse(worksheet.Cells[$"V{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"K{acells.Last().End.Row}"].Value = countСostSatisfied22 + int.Parse(worksheet.Cells[$"K{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"L{acells.Last().End.Row}"].Value = costSatisfied22 + decimal.Parse(worksheet.Cells[$"L{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"W{acells.Last().End.Row}"].Value = countСostDenied22 + int.Parse(worksheet.Cells[$"W{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"X{acells.Last().End.Row}"].Value = costDenied22 + decimal.Parse(worksheet.Cells[$"X{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"M{acells.Last().End.Row}"].Value = countСostSatisfied33 + int.Parse(worksheet.Cells[$"M{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"N{acells.Last().End.Row}"].Value = costSatisfied33 + decimal.Parse(worksheet.Cells[$"N{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"Y{acells.Last().End.Row}"].Value = countСostDenied33 + int.Parse(worksheet.Cells[$"Y{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"Z{acells.Last().End.Row}"].Value = costDenied33 + decimal.Parse(worksheet.Cells[$"Z{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"O{acells.Last().End.Row}"].Value = countСostSatisfied44 + int.Parse(worksheet.Cells[$"O{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"P{acells.Last().End.Row}"].Value = costSatisfied44 + decimal.Parse(worksheet.Cells[$"P{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"AA{acells.Last().End.Row}"].Value = countСostDenied44 + int.Parse(worksheet.Cells[$"AA{acells.Last().End.Row}"].Text);
                    worksheet.Cells[$"AB{acells.Last().End.Row}"].Value = costDenied44 + decimal.Parse(worksheet.Cells[$"AB{acells.Last().End.Row}"].Text);
                }
            }
            return package;
        }
        private string GetFileName(DateTime? dateS, DateTime? datePo)
        {
            return $"{dateS?.ToString("yyyy.MM.dd")}-{datePo?.ToString("yyyy.MM.dd")} {template.Name}";
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