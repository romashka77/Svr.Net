using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
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
using System.Globalization;
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
        private const string fileTemplateNameOut = "0901.xlsx"; //"Template1.xlsx";
        private const string fileTemplateNameIn = "0902.xlsx";
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<ClaimsController> logger;
        private readonly IRegionRepository regionRepository;
        private readonly IDistrictRepository districtRepository;

        private readonly ICategoryDisputeRepository categoryDisputeRepository;
        private readonly IGroupClaimRepository groupClaimRepository;
        private readonly IClaimRepository claimRepository;
        private readonly IInstanceRepository instanceRepository;

        [TempData] public string StatusMessage { get; set; }

        #region Конструктор

        public ReportsController(IHostingEnvironment hostingEnvironment, UserManager<ApplicationUser> userManager,
            ILogger<ClaimsController> logger, IDistrictRepository districtRepository,
            IRegionRepository regionRepository, ICategoryDisputeRepository categoryDisputeRepository,
            IGroupClaimRepository groupClaimRepository, IClaimRepository claimRepository,
            IInstanceRepository instanceRepository)
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

        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string lord = null,
            string owner = null, string searchString = null, int page = 1, int itemsPage = 10, DateTime? dateS = null,
            DateTime? datePo = null, string category = null)
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
                list = list.Where(d =>
                    d.Name.ToUpper().Contains(searchString.ToUpper()) ||
                    d.Extension.ToUpper().Contains(searchString.ToUpper()));
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
                FilterViewModel = new FilterViewModel(searchString, owner,
                    (await districtRepository.ListAsync(new DistrictSpecification(lord.ToLong()))).Select(a =>
                        new SelectListItem
                        { Text = a.Name, Value = a.Id.ToString(), Selected = (owner == a.Id.ToString()) }), lord,
                    (await regionRepository.ListAllAsync()).ToList().Select(a => new SelectListItem
                    { Text = a.Name, Value = a.Id.ToString(), Selected = (lord == a.Id.ToString()) }), dateS, datePo,
                    category,
                    (await categoryDisputeRepository.ListAllAsync()).Select(a => new SelectListItem
                    { Text = a.Name, Value = a.Id.ToString(), Selected = (category == a.Id.ToString()) })),
                StatusMessage = StatusMessage
            };
            return View(indexModel);
        }

        public async Task<IActionResult> InMemoryReport(string lord = null, string owner = null, DateTime? dateS = null,
            DateTime? datePo = null, string category = null)
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


        public async Task<IActionResult> FileReport(string lord = null, string owner = null, DateTime? dateS = null,
            DateTime? datePo = null, string category = null)
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

            return File( /*path*/ /*reportBytes*/Path.Combine(path, GetFileName(dateS, datePo)), XlsxContentType,
                GetFileName(dateS, datePo));
        }

        private async Task<FileInfo> GetFileTemplateName(string category)
        {
            string fileTemplateName;
            if (category.ToLong() == null)
            {
                StatusMessage = $"Ошибка: Выберите категорию.";
                return null;
            }
            else if ((await categoryDisputeRepository.GetByIdAsync(category.ToLong())).Name.ToUpper()
                .Equals("Входящие".ToUpper()))
                fileTemplateName = fileTemplateNameIn;
            else if ((await categoryDisputeRepository.GetByIdAsync(category.ToLong())).Name.ToUpper()
                .Equals("Исходящие".ToUpper()))
                fileTemplateName = fileTemplateNameOut;
            else
            {
                StatusMessage = $"Ошибка: Категория не определена.";
                return null;
            }

            FileInfo template =
                new FileInfo(Path.Combine(hostingEnvironment.WebRootPath, templatesFolder, fileTemplateName));
            if (!template.Exists)
            {
                StatusMessage = $"Ошибка: Файл Excel-шаблона {fileTemplateName} отсутствует.";
                return null;
            }

            return template;
        }

        private int GetSumInstances(List<Instance> instances, out int countSatisfied, out decimal sumSatisfied,
            out int countDenied, out decimal sumDenied, out int countEnd, out decimal sumEnd, out int countNo,
            out decimal sumNo, Record dutySatisfied, Record dutyDenied, Record servicesSatisfied,
            Record servicesDenied, Record costSatisfied, Record costDenied, Record dutyPaid)
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
                    else if (item.CourtDecision.Name.ToUpper().Equals("Отказано".ToUpper()))
                    {
                        countDenied++;
                        sumDenied = sumDenied + (item?.SumDenied ?? 0);
                    }
                    else if (item.CourtDecision.Name.ToUpper().Equals("Прекращено".ToUpper()))
                    {
                        countEnd++;
                        sumEnd = sumEnd + (item?.Claim?.Sum ?? 0);
                    }
                    else if (item.CourtDecision.Name.ToUpper().Equals("Оставлено без рассмотрения".ToUpper()))
                    {
                        countNo++;
                        sumNo = sumNo + (item?.Claim?.Sum ?? 0);
                    }

                    if (item?.DutySatisfied != null && item.DutySatisfied > 0)
                    {
                        dutySatisfied.Count++;
                        dutySatisfied.Sum = dutySatisfied.Sum + (item?.DutySatisfied ?? 0);
                    }

                    if (item?.DutyDenied != null && item?.DutyDenied > 0)
                    {
                        dutyDenied.Count++;
                        dutyDenied.Sum = dutyDenied.Sum + (item?.DutyDenied ?? 0);
                    }

                    if (item?.ServicesSatisfied != null && item?.ServicesSatisfied > 0)
                    {
                        servicesSatisfied.Count++;
                        servicesSatisfied.Sum = servicesSatisfied.Sum + (item?.ServicesSatisfied ?? 0);
                    }

                    if (item?.ServicesDenied != null && item?.ServicesDenied > 0)
                    {
                        servicesDenied.Count++;
                        servicesDenied.Sum = servicesDenied.Sum + (item?.ServicesDenied ?? 0);
                    }

                    if (item?.СostSatisfied != null && item?.СostSatisfied > 0)
                    {
                        costSatisfied.Count++;
                        costSatisfied.Sum = costSatisfied.Sum + (item?.СostSatisfied ?? 0);
                    }

                    if (item?.СostDenied != null && item?.СostDenied > 0)
                    {
                        costDenied.Count++;
                        costDenied.Sum = costDenied.Sum + (item?.СostDenied ?? 0);
                    }

                    //-----------------
                    if (item?.DutyPaid != null && item?.DutyPaid > 0)
                    {
                        dutyPaid.Count++;
                        dutyPaid.Sum = dutyPaid.Sum + (item?.DutyPaid ?? 0);
                    }
                }
            }

            return result;
        }

        private static int CellToInt(string text, int count)
        {
            return int.TryParse(text, out var t) ? count + t : count;
        }

        private static decimal CellToDec(string text, decimal count)
        {
            return decimal.TryParse(text, out var t) ? count + t : count;
        }

        private struct Record
        {
            public int Count { get; set; }

            public decimal Sum { get; set; }
        }

        
        private enum TypeRecord
        {
            All, Satisfied, Denied
        }

        private async Task<ExcelPackage> createExcelPackage(string lord = null, string owner = null,
            DateTime? dateS = null, DateTime? datePo = null, string category = null)
        {
            template = await GetFileTemplateName(category);
            if (template == null) return null;
            var package = new ExcelPackage(template, true);
            package.Workbook.Properties.Author = User.Identity.Name;
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            //Группы споров
            var groupClaims =
                    (await groupClaimRepository.ListAsync(new GroupClaimSpecificationReport(category.ToLong())))
                    .OrderBy(a => a.Code.ToLong());
            //Гос.пошлина удов
            var dutySatisfied = new Record { Count = 0, Sum = 0 };
            var dutySatisfied1 = new Record { Count = 0, Sum = 0 };
            var dutySatisfied11 = new Record { Count = 0, Sum = 0 };
            var dutySatisfied2 = new Record { Count = 0, Sum = 0 };
            var dutySatisfied22 = new Record { Count = 0, Sum = 0 };
            var dutySatisfied3 = new Record { Count = 0, Sum = 0 };
            var dutySatisfied33 = new Record { Count = 0, Sum = 0 };
            var dutySatisfied4 = new Record { Count = 0, Sum = 0 };
            var dutySatisfied44 = new Record { Count = 0, Sum = 0 };




            //Гос.пошлина отк.
            var dutyDenied = new Record { Count = 0, Sum = 0 };
            var dutyDenied1 = new Record { Count = 0, Sum = 0 };
            var dutyDenied11 = new Record { Count = 0, Sum = 0 };
            var dutyDenied2 = new Record { Count = 0, Sum = 0 };
            var dutyDenied22 = new Record { Count = 0, Sum = 0 };
            var dutyDenied3 = new Record { Count = 0, Sum = 0 };
            var dutyDenied33 = new Record { Count = 0, Sum = 0 };
            var dutyDenied4 = new Record { Count = 0, Sum = 0 };
            var dutyDenied44 = new Record { Count = 0, Sum = 0 };

            //Услуги пред.удов.
            var servicesSatisfied = new Record { Count = 0, Sum = 0 };
            var servicesSatisfied1 = new Record { Count = 0, Sum = 0 };
            var servicesSatisfied11 = new Record { Count = 0, Sum = 0 };
            var servicesSatisfied2 = new Record { Count = 0, Sum = 0 };
            var servicesSatisfied22 = new Record { Count = 0, Sum = 0 };
            var servicesSatisfied3 = new Record { Count = 0, Sum = 0 };
            var servicesSatisfied33 = new Record { Count = 0, Sum = 0 };
            var servicesSatisfied4 = new Record { Count = 0, Sum = 0 };
            var servicesSatisfied44 = new Record { Count = 0, Sum = 0 };

            //Услуги пред.отк.
            var servicesDenied = new Record { Count = 0, Sum = 0 };
            var servicesDenied1 = new Record { Count = 0, Sum = 0 };
            var servicesDenied11 = new Record { Count = 0, Sum = 0 };
            var servicesDenied2 = new Record { Count = 0, Sum = 0 };
            var servicesDenied22 = new Record { Count = 0, Sum = 0 };
            var servicesDenied3 = new Record { Count = 0, Sum = 0 };
            var servicesDenied33 = new Record { Count = 0, Sum = 0 };
            var servicesDenied4 = new Record { Count = 0, Sum = 0 };
            var servicesDenied44 = new Record { Count = 0, Sum = 0 };

            //Суд.издер.удов.
            var costSatisfied = new Record { Count = 0, Sum = 0 };
            var costSatisfied1 = new Record { Count = 0, Sum = 0 };
            var costSatisfied11 = new Record { Count = 0, Sum = 0 };
            var costSatisfied2 = new Record { Count = 0, Sum = 0 };
            var costSatisfied22 = new Record { Count = 0, Sum = 0 };
            var costSatisfied3 = new Record { Count = 0, Sum = 0 };
            var costSatisfied33 = new Record { Count = 0, Sum = 0 };
            var costSatisfied4 = new Record { Count = 0, Sum = 0 };
            var costSatisfied44 = new Record { Count = 0, Sum = 0 };

            //Суд.издер.отк.
            var costDenied = new Record { Count = 0, Sum = 0 };
            var costDenied1 = new Record { Count = 0, Sum = 0 };
            var costDenied11 = new Record { Count = 0, Sum = 0 };
            var costDenied2 = new Record { Count = 0, Sum = 0 };
            var costDenied22 = new Record { Count = 0, Sum = 0 };
            var costDenied3 = new Record { Count = 0, Sum = 0 };
            var costDenied33 = new Record { Count = 0, Sum = 0 };
            var costDenied4 = new Record { Count = 0, Sum = 0 };
            var costDenied44 = new Record { Count = 0, Sum = 0 };

            var dutyPaid = new Record { Count = 0, Sum = 0 };

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
                    //var acells = from cell in worksheet.Cells["A:A"] where cell.Text.Equals(subjectClaim.Code) select cell;
                    var n = (from cell in worksheet.Cells["A:A"] where cell.Text.Equals(subjectClaim.Code) select cell)
                        ?.Last().End.Row;
                    if (n != null)
                    {
                        var cells = worksheet.Cells;
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
                            cells[$"C{n}"].Value = count;
                            var sum = await claims.SumAsync(c => c.Sum);
                            if (sum != null)
                            {
                                cells[$"D{n}"].Value = sum;
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
                                count = GetSumInstances(instances1, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, dutySatisfied1, dutyDenied1, servicesSatisfied1, servicesDenied1, costSatisfied1, costDenied1, dutyPaid);
                            }
                            else if (flg == 2)
                            {
                                count = GetSumInstances(instances1, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, dutySatisfied11, dutyDenied11, servicesSatisfied11, servicesDenied11, costSatisfied11, costDenied11, dutyPaid);
                            }
                            else
                                count = GetSumInstances(instances1, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, dutySatisfied, dutyDenied, servicesSatisfied, servicesDenied, costSatisfied, costDenied, dutyPaid);
                            if (count > 0)
                            {
                                cells[$"I{n}"].Value = countSatisfied;
                                cells[$"J{n}"].Value = sumSatisfied;
                                cells[$"U{n}"].Value = countDenied;
                                cells[$"V{n}"].Value = sumDenied;
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
                                count = GetSumInstances(instances2, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, dutySatisfied2, dutyDenied2, servicesSatisfied2, servicesDenied2, costSatisfied2, costDenied2, dutyPaid);

                            }
                            else if (flg == 2)
                            {
                                count = GetSumInstances(instances2, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, dutySatisfied22, dutyDenied22, servicesSatisfied22, servicesDenied22, costSatisfied22, costDenied22, dutyPaid);
                            }
                            else
                                count = GetSumInstances(instances2, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, dutySatisfied, dutyDenied, servicesSatisfied, servicesDenied, costSatisfied, costDenied, dutyPaid);
                            if (count > 0)
                            {
                                cells[$"K{n}"].Value = countSatisfied;
                                cells[$"L{n}"].Value = sumSatisfied;
                                cells[$"W{n}"].Value = countDenied;
                                cells[$"X{n}"].Value = sumDenied;
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
                                count = GetSumInstances(instances3, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, dutySatisfied3, dutyDenied3, servicesSatisfied3, servicesDenied3, costSatisfied3, costDenied3, dutyPaid);
                            }
                            else if (flg == 2)
                            {
                                count = GetSumInstances(instances3, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, dutySatisfied33, dutyDenied33, servicesSatisfied33, servicesDenied33, costSatisfied33, costDenied33, dutyPaid);
                            }
                            else
                                count = GetSumInstances(instances3, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, dutySatisfied, dutyDenied, servicesSatisfied, servicesDenied, costSatisfied, costDenied, dutyPaid);
                            if (count > 0)
                            {
                                cells[$"M{n}"].Value = countSatisfied;
                                cells[$"N{n}"].Value = sumSatisfied;
                                cells[$"Y{n}"].Value = countDenied;
                                cells[$"Z{n}"].Value = sumDenied;
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
                                count = GetSumInstances(instances4, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, dutySatisfied4, dutyDenied4, servicesSatisfied4, servicesDenied4, costSatisfied4, costDenied4, dutyPaid);
                            }
                            else if (flg == 2)
                            {
                                count = GetSumInstances(instances4, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, dutySatisfied44, dutyDenied44, servicesSatisfied44, servicesDenied44, costSatisfied44, costDenied44, dutyPaid);
                            }
                            else
                                count = GetSumInstances(instances4, out countSatisfied, out sumSatisfied, out countDenied, out sumDenied, out countEnd, out sumEnd, out countNo, out sumNo, dutySatisfied, dutyDenied, servicesSatisfied, servicesDenied, costSatisfied, costDenied, dutyPaid);
                            if (count > 0)
                            {
                                cells[$"O{n}"].Value = countSatisfied;
                                cells[$"P{n}"].Value = sumSatisfied;
                                cells[$"AA{n}"].Value = countDenied;
                                cells[$"AB{n}"].Value = sumDenied;
                                countEnd0 = countEnd0 + countEnd;
                                sumEnd0 = sumEnd0 + sumEnd;
                                countNo0 = countNo0 + countNo;
                                sumNo0 = sumNo0 + sumNo;
                            }
                        }
                        if (countEnd0 > 0)
                        {
                            cells[$"AE{n}"].Value = countEnd0;
                            cells[$"AF{n}"].Value = sumEnd0;
                        }
                        if (countNo0 > 0)
                        {
                            cells[$"AG{n}"].Value = countNo0;
                            cells[$"AH{n}"].Value = sumNo0;
                        }
                    }
                }
            }
            if (dutySatisfied1.Count > 0 || dutyDenied1.Count > 0 || dutySatisfied2.Count > 0 || dutyDenied2.Count > 0 || dutySatisfied3.Count > 0 || dutyDenied3.Count > 0 || dutySatisfied4.Count > 0 || dutyDenied4.Count > 0)
            {
                var cat = template.Name.Equals(fileTemplateNameIn) ? "25.1" : "20.1";
                var n = (from cell in worksheet.Cells["A:A"] where cell.Text.Equals(cat) select cell)?.Last().End.Row;
                if (n != null)
                {
                    var cells = worksheet.Cells;
                    cells[$"I{n}"].Value = CellToInt(cells[$"I{n}"].Text, dutySatisfied1.Count);
                    cells[$"J{n}"].Value = CellToDec(cells[$"J{n}"].Text, dutySatisfied1.Sum);
                    cells[$"U{n}"].Value = CellToInt(cells[$"U{n}"].Text, dutyDenied1.Count);
                    cells[$"V{n}"].Value = CellToDec(cells[$"V{n}"].Text, dutyDenied1.Sum);
                    cells[$"K{n}"].Value = CellToInt(cells[$"K{n}"].Text, dutySatisfied2.Count);
                    cells[$"L{n}"].Value = CellToDec(cells[$"L{n}"].Text, dutySatisfied2.Sum);
                    cells[$"W{n}"].Value = CellToInt(cells[$"W{n}"].Text, dutyDenied2.Count);
                    cells[$"X{n}"].Value = CellToDec(cells[$"X{n}"].Text, dutyDenied2.Sum);
                    cells[$"M{n}"].Value = CellToInt(cells[$"M{n}"].Text, dutySatisfied3.Count);
                    cells[$"N{n}"].Value = CellToDec(cells[$"N{n}"].Text, dutySatisfied3.Sum);
                    cells[$"Y{n}"].Value = CellToInt(cells[$"Y{n}"].Text, dutyDenied3.Count);
                    cells[$"Z{n}"].Value = CellToDec(cells[$"Z{n}"].Text, dutyDenied3.Sum);
                    cells[$"O{n}"].Value = CellToInt(cells[$"O{n}"].Text, dutySatisfied4.Count);
                    cells[$"P{n}"].Value = CellToDec(cells[$"P{n}"].Text, dutySatisfied4.Sum);
                    cells[$"AA{n}"].Value = CellToInt(cells[$"AA{n}"].Text, dutyDenied4.Count);
                    cells[$"AB{n}"].Value = CellToDec(cells[$"AB{n}"].Text, dutyDenied4.Sum);
                }
            }
            if (servicesSatisfied1.Count > 0 || servicesDenied1.Count > 0 || servicesSatisfied2.Count > 0 || servicesDenied2.Count > 0 || servicesSatisfied3.Count > 0 || servicesDenied3.Count > 0 || servicesSatisfied4.Count > 0 || servicesDenied4.Count > 0)
            {
                var cat = template.Name.Equals(fileTemplateNameIn) ? "25.2" : "";
                var n = (from cell in worksheet.Cells["A:A"] where cell.Text.Equals(cat) select cell)?.Last().End.Row;
                if (n != null)
                {
                    var cells = worksheet.Cells;
                    cells[$"I{n}"].Value = CellToInt(cells[$"I{n}"].Text, servicesSatisfied1.Count);
                    cells[$"J{n}"].Value = CellToDec(cells[$"J{n}"].Text, servicesSatisfied1.Sum);
                    cells[$"U{n}"].Value = CellToInt(cells[$"U{n}"].Text, servicesDenied1.Count);
                    cells[$"V{n}"].Value = CellToDec(cells[$"V{n}"].Text, servicesDenied1.Sum);
                    cells[$"K{n}"].Value = CellToInt(cells[$"K{n}"].Text, servicesSatisfied2.Count);
                    cells[$"L{n}"].Value = CellToDec(cells[$"L{n}"].Text, servicesSatisfied2.Sum);
                    cells[$"W{n}"].Value = CellToInt(cells[$"W{n}"].Text, servicesDenied2.Count);
                    cells[$"X{n}"].Value = CellToDec(cells[$"X{n}"].Text, servicesDenied2.Sum);
                    cells[$"M{n}"].Value = CellToInt(cells[$"M{n}"].Text, servicesSatisfied3.Count);
                    cells[$"N{n}"].Value = CellToDec(cells[$"N{n}"].Text, servicesSatisfied3.Sum);
                    cells[$"Y{n}"].Value = CellToInt(cells[$"Y{n}"].Text, servicesDenied3.Count);
                    cells[$"Z{n}"].Value = CellToDec(cells[$"Z{n}"].Text, servicesDenied3.Sum);
                    cells[$"O{n}"].Value = CellToInt(cells[$"O{n}"].Text, servicesSatisfied4.Count);
                    cells[$"P{n}"].Value = CellToDec(cells[$"P{n}"].Text, servicesSatisfied4.Sum);
                    cells[$"AA{n}"].Value = CellToInt(cells[$"AA{n}"].Text, servicesDenied4.Count);
                    cells[$"AB{n}"].Value = CellToDec(cells[$"AB{n}"].Text, servicesDenied4.Sum);
                }
            }
            if (costSatisfied1.Count > 0 || costDenied1.Count > 0 || costSatisfied2.Count > 0 || costDenied2.Count > 0 || costSatisfied3.Count > 0 || costDenied3.Count > 0 || costSatisfied4.Count > 0 || costDenied4.Count > 0)
            {
                var cat = template.Name.Equals(fileTemplateNameIn) ? "25.3" : "20.2";
                var n = (from cell in worksheet.Cells["A:A"] where cell.Text.Equals(cat) select cell)?.Last().End.Row;
                if (n != null)
                {
                    var cells = worksheet.Cells;
                    cells[$"I{n}"].Value = CellToInt(cells[$"I{n}"].Text, costSatisfied1.Count);
                    cells[$"J{n}"].Value = CellToDec(cells[$"J{n}"].Text, costSatisfied1.Sum);
                    cells[$"U{n}"].Value = CellToInt(cells[$"U{n}"].Text, costDenied1.Count);
                    cells[$"V{n}"].Value = CellToDec(cells[$"V{n}"].Text, costDenied1.Sum);
                    cells[$"K{n}"].Value = CellToInt(cells[$"K{n}"].Text, costSatisfied2.Count);
                    cells[$"L{n}"].Value = CellToDec(cells[$"L{n}"].Text, costSatisfied2.Sum);
                    cells[$"W{n}"].Value = CellToInt(cells[$"W{n}"].Text, costDenied2.Count);
                    cells[$"X{n}"].Value = CellToDec(cells[$"X{n}"].Text, costDenied2.Sum);
                    cells[$"M{n}"].Value = CellToInt(cells[$"M{n}"].Text, costSatisfied3.Count);
                    cells[$"N{n}"].Value = CellToDec(cells[$"N{n}"].Text, costSatisfied3.Sum);
                    cells[$"Y{n}"].Value = CellToInt(cells[$"Y{n}"].Text, costDenied3.Count);
                    cells[$"Z{n}"].Value = CellToDec(cells[$"Z{n}"].Text, costDenied3.Sum);
                    cells[$"O{n}"].Value = CellToInt(cells[$"O{n}"].Text, costSatisfied4.Count);
                    cells[$"P{n}"].Value = CellToDec(cells[$"P{n}"].Text, costSatisfied4.Sum);
                    cells[$"AA{n}"].Value = CellToInt(cells[$"AA{n}"].Text, costDenied4.Count);
                    cells[$"AB{n}"].Value = CellToDec(cells[$"AB{n}"].Text, costDenied4.Sum);
                }
            }

            if (dutySatisfied11.Count > 0 || dutyDenied11.Count > 0 || dutySatisfied22.Count > 0 || dutyDenied22.Count > 0 || dutySatisfied33.Count > 0 || dutyDenied33.Count > 0 || dutySatisfied44.Count > 0 || dutyDenied44.Sum > 0)
            {
                var cat = template.Name.Equals(fileTemplateNameIn) ? "25.4" : "20.3";
                var n = (from cell in worksheet.Cells["A:A"] where cell.Text.Equals(cat) select cell)?.Last().End.Row;
                if (n != null)
                {
                    var cells = worksheet.Cells;
                    cells[$"I{n}"].Value = CellToInt(cells[$"I{n}"].Text, dutySatisfied11.Count);
                    cells[$"J{n}"].Value = CellToDec(cells[$"J{n}"].Text, dutySatisfied11.Sum);
                    cells[$"U{n}"].Value = CellToInt(cells[$"U{n}"].Text, dutyDenied11.Count);
                    cells[$"V{n}"].Value = CellToDec(cells[$"V{n}"].Text, dutyDenied11.Sum);
                    cells[$"K{n}"].Value = CellToInt(cells[$"K{n}"].Text, dutySatisfied22.Count);
                    cells[$"L{n}"].Value = CellToDec(cells[$"L{n}"].Text, dutySatisfied22.Sum);
                    cells[$"W{n}"].Value = CellToInt(cells[$"W{n}"].Text, dutyDenied22.Count);
                    cells[$"X{n}"].Value = CellToDec(cells[$"X{n}"].Text, dutyDenied22.Sum);
                    cells[$"M{n}"].Value = CellToInt(cells[$"M{n}"].Text, dutySatisfied33.Count);
                    cells[$"N{n}"].Value = CellToDec(cells[$"N{n}"].Text, dutySatisfied33.Sum);
                    cells[$"Y{n}"].Value = CellToInt(cells[$"Y{n}"].Text, dutyDenied33.Count);
                    cells[$"Z{n}"].Value = CellToDec(cells[$"Z{n}"].Text, dutyDenied33.Sum);
                    cells[$"O{n}"].Value = CellToInt(cells[$"O{n}"].Text, dutySatisfied44.Count);
                    cells[$"P{n}"].Value = CellToDec(cells[$"P{n}"].Text, dutySatisfied44.Sum);
                    cells[$"AA{n}"].Value = CellToInt(cells[$"AA{n}"].Text, dutyDenied44.Count);
                    cells[$"AB{n}"].Value = CellToDec(cells[$"AB{n}"].Text, dutyDenied44.Sum);
                }
            }

            if (servicesSatisfied11.Count > 0 || servicesDenied11.Count > 0 || servicesSatisfied22.Count > 0 ||
                servicesDenied22.Count > 0 || servicesSatisfied33.Count > 0 || servicesDenied33.Count > 0 ||
                servicesSatisfied44.Count > 0 || servicesDenied44.Count > 0)
            {
                var cat = template.Name.Equals(fileTemplateNameIn) ? "25.5" : "";
                var n = (from cell in worksheet.Cells["A:A"] where cell.Text.Equals(cat) select cell)?.Last().End
                    .Row;
                if (n != null)
                {
                    var cells = worksheet.Cells;
                    cells[$"I{n}"].Value = CellToInt(cells[$"I{n}"].Text, servicesSatisfied11.Count);
                    cells[$"J{n}"].Value = CellToDec(cells[$"J{n}"].Text, servicesSatisfied11.Sum);
                    cells[$"U{n}"].Value = CellToInt(cells[$"U{n}"].Text, servicesDenied11.Count);
                    cells[$"V{n}"].Value = CellToDec(cells[$"V{n}"].Text, servicesDenied11.Sum);
                    cells[$"K{n}"].Value = CellToInt(cells[$"K{n}"].Text, servicesSatisfied22.Count);
                    cells[$"L{n}"].Value = CellToDec(cells[$"L{n}"].Text, servicesSatisfied22.Sum);
                    cells[$"W{n}"].Value = CellToInt(cells[$"W{n}"].Text, servicesDenied22.Count);
                    cells[$"X{n}"].Value = CellToDec(cells[$"X{n}"].Text, servicesDenied22.Sum);
                    cells[$"M{n}"].Value = CellToInt(cells[$"M{n}"].Text, servicesSatisfied33.Count);
                    cells[$"N{n}"].Value = CellToDec(cells[$"N{n}"].Text, servicesSatisfied33.Sum);
                    cells[$"Y{n}"].Value = CellToInt(cells[$"Y{n}"].Text, servicesDenied33.Count);
                    cells[$"Z{n}"].Value = CellToDec(cells[$"Z{n}"].Text, servicesDenied33.Sum);
                    cells[$"O{n}"].Value = CellToInt(cells[$"O{n}"].Text, servicesSatisfied44.Count);
                    cells[$"P{n}"].Value = CellToDec(cells[$"P{n}"].Text, servicesSatisfied44.Sum);
                    cells[$"AA{n}"].Value = CellToInt(cells[$"AA{n}"].Text, servicesDenied44.Count);
                    cells[$"AB{n}"].Value = CellToDec(cells[$"AB{n}"].Text, servicesDenied44.Sum);
                }
            }

            if (costSatisfied11.Count > 0 || costDenied11.Count > 0 || costSatisfied22.Count > 0 ||
                costDenied22.Count > 0 || costSatisfied33.Count > 0 || costDenied33.Count > 0)
            {
                var cat = template.Name.Equals(fileTemplateNameIn) ? "25.5" : "20.4";
                var n = (from cell in worksheet.Cells["A:A"] where cell.Text.Equals(cat) select cell)?.Last().End
                    .Row;
                if (n != null)
                {
                    var cells = worksheet.Cells;
                    cells[$"I{n}"].Value = CellToInt(cells[$"I{n}"].Text, costSatisfied11.Count);
                    cells[$"J{n}"].Value = CellToDec(cells[$"J{n}"].Text, costSatisfied11.Sum);
                    cells[$"U{n}"].Value = CellToInt(cells[$"U{n}"].Text, costDenied11.Count);
                    cells[$"V{n}"].Value = CellToDec(cells[$"V{n}"].Text, costDenied11.Sum);
                    cells[$"K{n}"].Value = CellToInt(cells[$"K{n}"].Text, costSatisfied22.Count);
                    cells[$"L{n}"].Value = CellToDec(cells[$"L{n}"].Text, costSatisfied22.Sum);
                    cells[$"W{n}"].Value = CellToInt(cells[$"W{n}"].Text, costDenied22.Count);
                    cells[$"X{n}"].Value = CellToDec(cells[$"X{n}"].Text, costDenied22.Sum);
                    cells[$"M{n}"].Value = CellToInt(cells[$"M{n}"].Text, costSatisfied33.Count);
                    cells[$"N{n}"].Value = CellToDec(cells[$"N{n}"].Text, costSatisfied33.Sum);
                    cells[$"Y{n}"].Value = CellToInt(cells[$"Y{n}"].Text, costDenied33.Count);
                    cells[$"Z{n}"].Value = CellToDec(cells[$"Z{n}"].Text, costDenied33.Sum);
                    cells[$"O{n}"].Value = CellToInt(cells[$"O{n}"].Text, costSatisfied44.Count);
                    cells[$"P{n}"].Value = CellToDec(cells[$"P{n}"].Text, costSatisfied44.Sum);
                    cells[$"AA{n}"].Value = CellToInt(cells[$"AA{n}"].Text, costDenied44.Count);
                    cells[$"AB{n}"].Value = CellToDec(cells[$"AB{n}"].Text, costDenied44.Sum);
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