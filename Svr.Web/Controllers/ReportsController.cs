﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Infrastructure.Extensions;
using Svr.Infrastructure.Identity;
using Svr.Web.Extensions;
using Svr.Web.Models;
using Svr.Web.Models.ReportsViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

//using OfficeOpenXml.Table;

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
        private const string ReportsFolder = "Reports";
        private const string TemplatesFolder = "Templates";
        private const string FileTemplateNameOut = "0901.xlsx"; //"Template1.xlsx";
        private const string FileTemplateNameIn = "0902.xlsx";
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
            //if (string.IsNullOrEmpty(owner))
            //{
            //    if (string.IsNullOrEmpty(lord))
            //    {
            //        var user = await userManager.FindByNameAsync(User.Identity.Name);
            //        owner = user.DistrictId.ToString();
            //        lord = "1";
            //    }
            //}
            if (string.IsNullOrEmpty(owner) && (string.IsNullOrEmpty(lord)))
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                if (user != null)
                {
                    owner = user.DistrictId.ToString();
                }
            }

            var path = await GetPath(lord.ToLong(), owner.ToLong());

            var dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            IEnumerable<FileInfo> list = dirInfo.GetFiles();
            //фильтрация
            if (!string.IsNullOrEmpty(searchString))
            {
                list = list.Where(d =>
                    d.Name.ToUpper().Contains(searchString.ToUpper()) ||
                    d.Extension.ToUpper().Contains(searchString.ToUpper()));
            }
            // сортировка
            list = list.Sort(sortOrder);

            // определение страниц
            // ReSharper disable once PossibleMultipleEnumeration
            var count = list.Count();
            // ReSharper disable once PossibleMultipleEnumeration
            var itemsOnPage = list.Skip((page - 1) * itemsPage).Take(itemsPage).ToList();
            var indexModel = new IndexViewModel
            {
                ItemViewModels = itemsOnPage.Select(i => new ItemViewModel
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
            if (string.IsNullOrEmpty(owner) && string.IsNullOrEmpty(lord))
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                owner = user.DistrictId.ToString();
            }
            byte[] reportBytes;
            using (var package = await CreateExcelPackage(owner, dateS, datePo, category))
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
            if (string.IsNullOrEmpty(owner) && string.IsNullOrEmpty(lord))
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                owner = user.DistrictId.ToString();
            }
            var path = await GetPath(lord.ToLong(), owner.ToLong());
            //byte[] reportBytes;
            using (var package = await CreateExcelPackage(owner, dateS, datePo, category))
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
                StatusMessage = "Ошибка: Выберите категорию.";
                return null;
            }

            if ((await categoryDisputeRepository.GetByIdAsync(category.ToLong())).Name.ToUpper()
                .Equals("Входящие".ToUpper()))
                fileTemplateName = FileTemplateNameIn;
            else if ((await categoryDisputeRepository.GetByIdAsync(category.ToLong())).Name.ToUpper()
                .Equals("Исходящие".ToUpper()))
                fileTemplateName = FileTemplateNameOut;
            else
            {
                StatusMessage = "Ошибка: Категория не определена.";
                return null;
            }
            var fileInfo = new FileInfo(Path.Combine(hostingEnvironment.WebRootPath, TemplatesFolder, fileTemplateName));
            if (fileInfo.Exists) return fileInfo;
            StatusMessage = $"Ошибка: Файл Excel-шаблона {fileTemplateName} отсутствует.";
            return null;
        }

        private void GetSumInstances(List<Instance> instances, Rec satisfied,
            Rec denied, Rec end, Rec no, Record duty, Record services, Record cost, Rec dutyPaid)
        {
            foreach (var item in instances)
            {
                if (item.CourtDecision == null) continue;
                if (item.CourtDecision.Name.ToUpper().Equals("Удовлетворено (частично)".ToUpper()))
                {
                    satisfied.Count++;
                    satisfied.Sum += item?.SumSatisfied ?? 0;
                    denied.Sum += item?.SumDenied ?? 0;
                }
                else if (item.CourtDecision.Name.ToUpper().Equals("Отказано".ToUpper()))
                {
                    denied.Count++;
                    denied.Sum += item?.SumDenied ?? 0;
                }
                else if (item.CourtDecision.Name.ToUpper().Equals("Прекращено".ToUpper()))
                {
                    end.Count++;
                    end.Sum += item?.Claim?.Sum ?? 0;
                }
                else if (item.CourtDecision.Name.ToUpper().Equals("Оставлено без рассмотрения".ToUpper()))
                {
                    no.Count++;
                    no.Sum += item?.Claim?.Sum ?? 0;
                }

                if (item?.DutySatisfied != null && item.DutySatisfied > 0)
                {
                    duty.Satisfied.Count++;
                    duty.Satisfied.Sum = duty.Satisfied.Sum + (item?.DutySatisfied ?? 0);
                }

                if (item?.DutyDenied != null && item?.DutyDenied > 0)
                {
                    duty.Denied.Count++;
                    duty.Denied.Sum = duty.Denied.Sum + (item?.DutyDenied ?? 0);
                }

                if (item?.ServicesSatisfied != null && item?.ServicesSatisfied > 0)
                {
                    services.Satisfied.Count++;
                    services.Satisfied.Sum = services.Satisfied.Sum + (item?.ServicesSatisfied ?? 0);
                }

                if (item?.ServicesDenied != null && item?.ServicesDenied > 0)
                {
                    services.Denied.Count++;
                    services.Denied.Sum = services.Denied.Sum + (item?.ServicesDenied ?? 0);
                }

                if (item?.СostSatisfied != null && item?.СostSatisfied > 0)
                {
                    cost.Satisfied.Count++;
                    cost.Satisfied.Sum = cost.Satisfied.Sum + (item?.СostSatisfied ?? 0);
                }

                if (item?.СostDenied != null && item?.СostDenied > 0)
                {
                    cost.Denied.Count++;
                    cost.Denied.Sum = cost.Denied.Sum + (item?.СostDenied ?? 0);
                }

                //-----------------
                if (item?.DutyPaid != null && item?.DutyPaid > 0)
                {
                    dutyPaid.Count++;
                    dutyPaid.Sum = dutyPaid.Sum + (item?.DutyPaid ?? 0);
                }
            }
            //return result;
        }

        private static int CellToInt(string text, int count)
        {
            return int.TryParse(text, out var t) ? count + t : count;
        }

        private static decimal CellToDec(string text, decimal count)
        {
            return decimal.TryParse(text, out var t) ? count + t : count;
        }

        private class Rec
        {
            //количество
            public int Count { get; set; }
            //сумма
            public decimal Sum { get; set; }
        }

        private class Record
        {
            //удовлетворено
            public Rec Satisfied { get; }
            //отказано
            public Rec Denied { get; }

            public Record()
            {
                Satisfied = new Rec();
                Denied = new Rec();
            }
        }

        private static List<Rec> InitialRec(int count = 11)
        {
            var res = new List<Rec>(count);
            for (var i = 0; i < count; i++)
                res.Add(new Rec());
            return res;
        }

        private static List<Record> InitialRecord(int count = 9)
        {
            var res = new List<Record>(count);
            for (var i = 0; i < count; i++)
                res.Add(new Record());
            return res;
        }

        private static int GetNumRow(ExcelWorksheet worksheet, string cat = "")
        {
            var res = 0;
            var rage = from cell in worksheet.Cells["A:A"] where cell.Text.Equals(cat) select cell;
            var enumerable = rage.ToList();
            if (enumerable.Any())
                res = enumerable.Last().End.Row;
            return res;
        }
        private static void SetCells2(ExcelWorksheet worksheet, IReadOnlyList<Rec> record,string cat = "")
        {
            if (string.IsNullOrEmpty(cat) || (record.Sum(rec => rec.Count) == 0)) return;
            var n = GetNumRow(worksheet, cat);
            if (n == 0) return;
            var cells = worksheet.Cells;
            cells[$"C{n}"].Value = CellToInt(cells[$"C{n}"].Text, record[0].Count);
            cells[$"D{n}"].Value = CellToDec(cells[$"D{n}"].Text, record[0].Sum);
            cells[$"I{n}"].Value = CellToInt(cells[$"I{n}"].Text, record[1].Count);
            cells[$"J{n}"].Value = CellToDec(cells[$"J{n}"].Text, record[1].Sum);
            cells[$"U{n}"].Value = CellToInt(cells[$"U{n}"].Text, record[2].Count);
            cells[$"V{n}"].Value = CellToDec(cells[$"V{n}"].Text, record[2].Sum);
            cells[$"K{n}"].Value = CellToInt(cells[$"K{n}"].Text, record[3].Count);
            cells[$"L{n}"].Value = CellToDec(cells[$"L{n}"].Text, record[3].Sum);
            cells[$"W{n}"].Value = CellToInt(cells[$"W{n}"].Text, record[4].Count);
            cells[$"X{n}"].Value = CellToDec(cells[$"X{n}"].Text, record[4].Sum);
            cells[$"M{n}"].Value = CellToInt(cells[$"M{n}"].Text, record[5].Count);
            cells[$"N{n}"].Value = CellToDec(cells[$"N{n}"].Text, record[5].Sum);
            cells[$"Y{n}"].Value = CellToInt(cells[$"Y{n}"].Text, record[6].Count);
            cells[$"Z{n}"].Value = CellToDec(cells[$"Z{n}"].Text, record[6].Sum);
            cells[$"O{n}"].Value = CellToInt(cells[$"O{n}"].Text, record[7].Count);
            cells[$"P{n}"].Value = CellToDec(cells[$"P{n}"].Text, record[7].Sum);
            cells[$"AA{n}"].Value = CellToInt(cells[$"AA{n}"].Text, record[8].Count);
            cells[$"AB{n}"].Value = CellToDec(cells[$"AB{n}"].Text, record[8].Sum);
            cells[$"AE{n}"].Value = CellToInt(cells[$"AE{n}"].Text, record[9].Count);
            cells[$"AF{n}"].Value = CellToDec(cells[$"AF{n}"].Text, record[9].Sum);
            cells[$"AG{n}"].Value = CellToInt(cells[$"AG{n}"].Text, record[10].Count);
            cells[$"AH{n}"].Value = CellToDec(cells[$"AH{n}"].Text, record[10].Sum);
            var regex = new Regex(@"(\d+|\.[^.]*)$");//(@"\.[^.]*$");
            // ReSharper disable once TailRecursiveCall
            SetCells2(worksheet, record, regex.Replace(cat, string.Empty, 1));
        }

        private static void SetCells(ExcelWorksheet worksheet, IReadOnlyList<Record> record, string cat = "", byte d = 0)
        {
            if (record[1 + d].Satisfied.Count <= 0 && record[1 + d].Denied.Count <= 0 &&
                record[3 + d].Satisfied.Count <= 0 && record[3 + d].Denied.Count <= 0 &&
                record[5 + d].Satisfied.Count <= 0 && record[5 + d].Denied.Count <= 0 &&
                record[7 + d].Satisfied.Count <= 0 && record[7 + d].Denied.Count <= 0) return;
            var n = GetNumRow(worksheet, cat);
            if (n == 0) return;
            var cells = worksheet.Cells;
            cells[$"I{n}"].Value = CellToInt(cells[$"I{n}"].Text, record[1 + d].Satisfied.Count);
            cells[$"J{n}"].Value = CellToDec(cells[$"J{n}"].Text, record[1 + d].Satisfied.Sum);
            cells[$"U{n}"].Value = CellToInt(cells[$"U{n}"].Text, record[1 + d].Denied.Count);
            cells[$"V{n}"].Value = CellToDec(cells[$"V{n}"].Text, record[1 + d].Denied.Sum);
            cells[$"K{n}"].Value = CellToInt(cells[$"K{n}"].Text, record[3 + d].Satisfied.Count);
            cells[$"L{n}"].Value = CellToDec(cells[$"L{n}"].Text, record[3 + d].Satisfied.Sum);
            cells[$"W{n}"].Value = CellToInt(cells[$"W{n}"].Text, record[3 + d].Denied.Count);
            cells[$"X{n}"].Value = CellToDec(cells[$"X{n}"].Text, record[3 + d].Denied.Sum);
            cells[$"M{n}"].Value = CellToInt(cells[$"M{n}"].Text, record[5 + d].Satisfied.Count);
            cells[$"N{n}"].Value = CellToDec(cells[$"N{n}"].Text, record[5 + d].Satisfied.Sum);
            cells[$"Y{n}"].Value = CellToInt(cells[$"Y{n}"].Text, record[5 + d].Denied.Count);
            cells[$"Z{n}"].Value = CellToDec(cells[$"Z{n}"].Text, record[5 + d].Denied.Sum);
            cells[$"O{n}"].Value = CellToInt(cells[$"O{n}"].Text, record[7 + d].Satisfied.Count);
            cells[$"P{n}"].Value = CellToDec(cells[$"P{n}"].Text, record[7 + d].Satisfied.Sum);
            cells[$"AA{n}"].Value = CellToInt(cells[$"AA{n}"].Text, record[7 + d].Denied.Count);
            cells[$"AB{n}"].Value = CellToDec(cells[$"AB{n}"].Text, record[7 + d].Denied.Sum);
        }

        private async Task<ExcelPackage> CreateExcelPackage(string owner = null,
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
            //Гос.пошлина
            var duty = InitialRecord();
            //Услуги пред.
            var services = InitialRecord();
            //Суд.издержки
            var cost = InitialRecord();


            var dutyPaid = new Rec();

            foreach (var groupClaim in groupClaims)
            {
                byte flg = 0;
                long? groupClaimCode = groupClaim.Code.ToLong();
                if ((template.Name.Equals(FileTemplateNameIn) && (groupClaimCode > 0) && (groupClaimCode < 5)) || (template.Name.Equals(FileTemplateNameOut) && (groupClaimCode > 0) && (groupClaimCode < 5)))
                {
                    flg = 1;
                }
                else if ((template.Name.Equals(FileTemplateNameIn) && (groupClaimCode > 4) && (groupClaimCode < 25)) || (template.Name.Equals(FileTemplateNameOut) && (groupClaimCode > 4) && (groupClaimCode < 18)))
                {
                    flg = 2;
                }

                /// Предметы иска
                var subjectClaims = groupClaim.SubjectClaims.OrderBy(a => a.Code);
                foreach (var subjectClaim in subjectClaims)
                {
                    try
                    {
                        var groupRecord = InitialRec();
                        var claims = claimRepository.List(new ClaimSpecificationReport(owner.ToLong())).Where(c => c.SubjectClaimId == subjectClaim.Id);
                        var n = (from cell in worksheet.Cells["A:A"] where cell.Text.Equals(subjectClaim.Code) select cell)
                            ?.Last().End.Row;
                        if (n == null) continue;
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
                            groupRecord[0].Count += count;
                            var sum = await claims.SumAsync(c => c.Sum);
                            if (sum != null)
                            {
                                groupRecord[0].Sum += (sum ?? 0);
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
                        var instances1 = await instances.Where(i => i.Number == 1).AsNoTracking().ToListAsync();
                        if (instances1.Count > 0)
                        {
                            var k = 0;
                            if (flg == 1)
                                k = 1;
                            else if (flg == 2)
                                k = 2;
                            GetSumInstances(instances1, groupRecord[1], groupRecord[2], groupRecord[9], groupRecord[10], duty[k], services[k], cost[k], dutyPaid);
                        }
                        var instances2 = await instances.Where(i => i.Number == 2).AsNoTracking().ToListAsync();
                        if (instances2.Count > 0)
                        {
                            var k = 0;
                            if (flg == 1)
                                k = 3;
                            else if (flg == 2)
                                k = 4;
                            GetSumInstances(instances2, groupRecord[3], groupRecord[4], groupRecord[9], groupRecord[10], duty[k], services[k], cost[k], dutyPaid);
                        }
                        var instances3 = await instances.Where(i => i.Number == 3).AsNoTracking().ToListAsync();
                        if (instances3.Count > 0)
                        {
                            var k = 0;
                            if (flg == 1)
                                k = 5;
                            else if (flg == 2)
                                k = 6;
                            GetSumInstances(instances3, groupRecord[5], groupRecord[6], groupRecord[9], groupRecord[10], duty[k], services[k], cost[k], dutyPaid);
                        }
                        var instances4 = await instances.Where(i => i.Number == 4).AsNoTracking().ToListAsync();
                        if (instances4.Count > 0)
                        {
                            var k = 0;
                            if (flg == 1)
                                k = 7;
                            else if (flg == 2)
                                k = 8;
                            GetSumInstances(instances4, groupRecord[7], groupRecord[8], groupRecord[9], groupRecord[10], duty[k], services[k], cost[k], dutyPaid);
                        }
                        SetCells2(worksheet, groupRecord, subjectClaim.Code);
                        //groupRecord = null;
                    }
                    catch (Exception e)
                    {
                        logger.LogError($"{e.Message}  subjectClaim.Code = {subjectClaim.Code}");
                        //Console.WriteLine($"{e.Message}  subjectClaim.Code = {subjectClaim.Code}");
                        throw;
                    }
                }
            }
            SetCells(worksheet, duty, (template.Name.Equals(FileTemplateNameIn) ? "25.1" : "20.1"));
            SetCells(worksheet, services, (template.Name.Equals(FileTemplateNameIn) ? "25.2" : ""));
            SetCells(worksheet, cost, (template.Name.Equals(FileTemplateNameIn) ? "25.3" : "20.2"));
            SetCells(worksheet, duty, (template.Name.Equals(FileTemplateNameIn) ? "25.4" : "20.3"), 1);
            SetCells(worksheet, services, (template.Name.Equals(FileTemplateNameIn) ? "25.5" : ""), 1);
            SetCells(worksheet, cost, (template.Name.Equals(FileTemplateNameIn) ? "25.5" : "20.4"), 1);
            return package;
        }

        private string GetFileName(DateTime? dateS, DateTime? datePo)
        {
            return $"{dateS?.ToString("yyyy.MM.dd")}-{datePo?.ToString("yyyy.MM.dd")} {template.Name}";
        }

        private async Task<string> GetPath(long? lord, long? owner)
        {
            var path = Path.Combine(hostingEnvironment.WebRootPath, ReportsFolder);
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