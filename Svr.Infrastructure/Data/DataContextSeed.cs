using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Infrastructure.Data
{
    public static class DataContextSeed
    {
        private static Region region;
        private static CategoryDispute categoryDispute;
        private static GroupClaim groupClaim;
        private static DirName dirName;
        private static Dir dir;

        public static async Task SeedAsync(DataContext dataContext/*, ILoggerFactory loggerFactory, int? retry = 0*/)
        {
            //int retryForAvailability = retry.Value;
            try
            {
                if (!dataContext.Regions.Any())
                {
                    dataContext.Regions.AddRange(GetPreconfiguredRegions());
                    if (!dataContext.Districts.Any())
                    {
                        dataContext.Districts.AddRange(GetPreconfiguredDistricts());
                        await dataContext.SaveChangesAsync();
                    }
                }
                if (!dataContext.CategoryDisputes.Any())
                {
                    //Входящие
                    dataContext.CategoryDisputes.AddRange(GetPreconfiguredCategoryDisputeIn(dataContext));
                    //Исходящие
                    dataContext.CategoryDisputes.AddRange(GetPreconfiguredCategoryDisputeOut(dataContext));
                    await dataContext.SaveChangesAsync();
                }
                if (!dataContext.DirName.Any())
                {
                    //dataContext.DirName.AddRange(GetPreconfiguredDirNamePerformers(dataContext));
                    //await dataContext.SaveChangesAsync();

                    dataContext.DirName.AddRange(GetPreconfiguredDirNameOPF(dataContext));
                    await dataContext.SaveChangesAsync();
                    dataContext.DirName.AddRange(GetPreconfiguredDirNameCourt(dataContext));
                    await dataContext.SaveChangesAsync();
                    dataContext.DirName.AddRange(GetPreconfiguredDirNameTypeApplicant(dataContext));
                    await dataContext.SaveChangesAsync();

                    dataContext.DirName.AddRange(GetPreconfiguredDirNameCourtDecision1(dataContext));
                    await dataContext.SaveChangesAsync();
                    dataContext.DirName.AddRange(GetPreconfiguredDirNameCourtDecision2(dataContext));
                    await dataContext.SaveChangesAsync();
                    dataContext.DirName.AddRange(GetPreconfiguredDirNameCourtDecision3(dataContext));
                    await dataContext.SaveChangesAsync();
                    dataContext.DirName.AddRange(GetPreconfiguredDirNameCourtDecision4(dataContext));
                    await dataContext.SaveChangesAsync();
                }
                if (!dataContext.Performers.Any())
                {
                    dataContext.Performers.AddRange(GetPreconfiguredPerformers());
                    await dataContext.SaveChangesAsync();
                }
                if (!dataContext.DistrictPerformers.Any())
                {
                    dataContext.DistrictPerformers.Add(new DistrictPerformer { DistrictId = 1, PerformerId = 1 });
                    dataContext.DistrictPerformers.Add(new DistrictPerformer { DistrictId = 1, PerformerId = 2 });
                    await dataContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при заполнении справочников: {ex.Message}");
                //if (retryForAvailability < 10)
                {
                    throw;
                    //retryForAvailability++;
                    //var log = loggerFactory.CreateLogger<DataContextSeed>();
                    //log.LogError(ex.Message);
                    //await SeedAsync(dataContext, loggerFactory, retryForAvailability);
                }
            }
        }

        private static IEnumerable<CategoryDispute> GetPreconfiguredCategoryDisputeOut(DataContext dataContext)
        {
            categoryDispute = new CategoryDispute { Name = "Исходящие", Description = "Исходящие документы" };
            dataContext.CategoryDisputes.Add(categoryDispute);

            groupClaim = new GroupClaim { Name = "1", Description = "", Code = "1", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsOut(1));

            groupClaim = new GroupClaim { Name = "2", Description = "", Code = "2", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsOut(2));

            groupClaim = new GroupClaim { Name = "3", Description = "", Code = "3", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsOut(3));

            groupClaim = new GroupClaim { Name = "4", Description = "", Code = "4", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsOut(4));

            groupClaim = new GroupClaim { Name = "Взыскание по ДТП", Description = "", Code = "5", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsOut(5));

            groupClaim = new GroupClaim { Name = "Споры, возникающие в рамках исполнительного производства", Description = "", Code = "6", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsOut(6));

            groupClaim = new GroupClaim { Name = "Дела о банкротстве", Description = "", Code = "7", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsOut(7));

            groupClaim = new GroupClaim { Name = "8", Description = "", Code = "8", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsOut(8));

            groupClaim = new GroupClaim { Name = "9", Description = "", Code = "9", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsOut(9));

            groupClaim = new GroupClaim { Name = "10", Description = "", Code = "10", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsOut(10));

            groupClaim = new GroupClaim { Name = "11", Description = "", Code = "11", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsOut(11));

            groupClaim = new GroupClaim { Name = "12", Description = "", Code = "12", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsOut(12));

            groupClaim = new GroupClaim { Name = "13", Description = "", Code = "13", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsOut(13));

            groupClaim = new GroupClaim { Name = "14", Description = "", Code = "14", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsOut(14));

            groupClaim = new GroupClaim { Name = "15", Description = "", Code = "15", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsOut(15));

            groupClaim = new GroupClaim { Name = "16", Description = "", Code = "16", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsOut(16));

            groupClaim = new GroupClaim { Name = "17", Description = "", Code = "17", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsOut(17));

            groupClaim = new GroupClaim { Name = "18", Description = "", Code = "18", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsOut(18));

            groupClaim = new GroupClaim { Name = "19", Description = "", Code = "19", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsOut(19));

            return new List<CategoryDispute>() { categoryDispute };
        }

        private static IEnumerable<CategoryDispute> GetPreconfiguredCategoryDisputeIn(DataContext dataContext)
        {
            categoryDispute = new CategoryDispute { Name = "Входящие", Description = "Входящие документы" };
            dataContext.CategoryDisputes.Add(categoryDispute);
            groupClaim = new GroupClaim { Name = "Административные штрафы, в т.ч.", Description = "", Code = "1", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(1));

            groupClaim = new GroupClaim { Name = "Обжалования", Description = "", Code = "2", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(2));

            groupClaim = new GroupClaim { Name = "3", Description = "", Code = "3", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(3));

            groupClaim = new GroupClaim { Name = "4", Description = "", Code = "4", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(4));

            groupClaim = new GroupClaim { Name = "Взыскание по ДТП", Description = "", Code = "5", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(5));

            groupClaim = new GroupClaim { Name = "Споры, возникающие в рамках исполнительного производства", Description = "", Code = "6", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(6));

            groupClaim = new GroupClaim { Name = "Дела о банкротстве", Description = "", Code = "7", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(7));

            groupClaim = new GroupClaim { Name = "8", Description = "", Code = "8", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(8));

            groupClaim = new GroupClaim { Name = "9", Description = "", Code = "9", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(9));

            groupClaim = new GroupClaim { Name = "10", Description = "", Code = "10", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(10));

            groupClaim = new GroupClaim { Name = "11", Description = "", Code = "11", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(11));

            groupClaim = new GroupClaim { Name = "12", Description = "", Code = "12", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(12));

            groupClaim = new GroupClaim { Name = "13", Description = "", Code = "13", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(13));

            groupClaim = new GroupClaim { Name = "14", Description = "", Code = "14", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(14));

            groupClaim = new GroupClaim { Name = "15", Description = "", Code = "15", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(15));

            groupClaim = new GroupClaim { Name = "16", Description = "", Code = "16", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(16));

            groupClaim = new GroupClaim { Name = "17", Description = "", Code = "17", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(17));

            groupClaim = new GroupClaim { Name = "18", Description = "", Code = "18", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(18));

            groupClaim = new GroupClaim { Name = "19", Description = "", Code = "19", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(19));

            groupClaim = new GroupClaim { Name = "20", Description = "", Code = "20", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(20));

            groupClaim = new GroupClaim { Name = "21", Description = "", Code = "21", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(21));

            groupClaim = new GroupClaim { Name = "22", Description = "", Code = "22", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(22));

            groupClaim = new GroupClaim { Name = "23", Description = "", Code = "23", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(23));

            groupClaim = new GroupClaim { Name = "24", Description = "", Code = "24", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(24));

            groupClaim = new GroupClaim { Name = "25", Description = "", Code = "25", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(25));

            groupClaim = new GroupClaim { Name = "26", Description = "", Code = "26", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(26));

            groupClaim = new GroupClaim { Name = "27", Description = "", Code = "27", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(27));

            groupClaim = new GroupClaim { Name = "28", Description = "", Code = "28", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(28));

            groupClaim = new GroupClaim { Name = "29", Description = "", Code = "29", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(29));

            groupClaim = new GroupClaim { Name = "31", Description = "", Code = "31", CategoryDispute = categoryDispute };
            dataContext.GroupClaims.Add(groupClaim);
            dataContext.SubjectClaims.AddRange(GetPreconfiguredSubjectClaimsIn(31));

            return new List<CategoryDispute>() { categoryDispute };
        }
        private static IEnumerable<DirName> GetPreconfiguredDirNameTypeApplicant(DataContext dataContext)
        {
            dirName = new DirName { Name = "Тип контрагента" };
            dataContext.DirName.Add(dirName);
            dataContext.Dir.AddRange(GetPreconfiguredTypeApplicant(dataContext));

            var resault = new List<DirName> { };
            resault.Add(dirName);
            return resault;
        }
        private static IEnumerable<DirName> GetPreconfiguredDirNameOPF(DataContext dataContext)
        {
            dirName = new DirName { Name = "ОПФ" };
            dataContext.DirName.Add(dirName);
            dataContext.Dir.AddRange(GetPreconfiguredOPF());

            var resault = new List<DirName> { };
            resault.Add(dirName);
            return resault;
        }
        private static IEnumerable<DirName> GetPreconfiguredDirNameCourtDecision1(DataContext dataContext)
        {
            dirName = new DirName { Name = "Решения суда 1-ой инстанции" };
            dataContext.DirName.Add(dirName);
            dataContext.Dir.AddRange(GetPreconfiguredDecision1());

            var resault = new List<DirName> { };
            resault.Add(dirName);
            return resault;
        }
        private static IEnumerable<DirName> GetPreconfiguredDirNameCourtDecision2(DataContext dataContext)
        {
            dirName = new DirName { Name = "Решения суда 2-ой инстанции" };
            dataContext.DirName.Add(dirName);
            dataContext.Dir.AddRange(GetPreconfiguredDecision2());

            var resault = new List<DirName> { };
            resault.Add(dirName);
            return resault;
        }
        private static IEnumerable<DirName> GetPreconfiguredDirNameCourtDecision3(DataContext dataContext)
        {
            dirName = new DirName { Name = "Решения суда 3-ей инстанции" };
            dataContext.DirName.Add(dirName);
            dataContext.Dir.AddRange(GetPreconfiguredDecision3());

            var resault = new List<DirName> { };
            resault.Add(dirName);
            return resault;
        }
        private static IEnumerable<DirName> GetPreconfiguredDirNameCourtDecision4(DataContext dataContext)
        {
            dirName = new DirName { Name = "Решения суда 4-ой инстанции" };
            dataContext.DirName.Add(dirName);
            dataContext.Dir.AddRange(GetPreconfiguredDecision4());

            var resault = new List<DirName> { };
            resault.Add(dirName);
            return resault;
        }
        private static IEnumerable<DirName> GetPreconfiguredDirNameCourt(DataContext dataContext)
        {
            dirName = new DirName { Name = "Суд" };
            dataContext.DirName.Add(dirName);
            dataContext.Dir.AddRange(GetPreconfiguredCourt());

            var resault = new List<DirName> { };
            resault.Add(dirName);
            return resault;
        }
        private static IEnumerable<Dir> GetPreconfiguredCourt()
        {
            return new List<Dir>()
            {
                new Dir {Name="Арбитражный суд Тамбовской области", DirName=dirName },
                new Dir {Name="Бондарсвкий районный суд Тамбовской области", DirName=dirName },
                new Dir {Name="Гавриловский районный суд Тамбовской области", DirName=dirName },
                new Dir {Name="Жердевский районный суд Тамбовской области", DirName=dirName },
                new Dir {Name="Знаменский районный суд Тамбовской области", DirName=dirName },
                new Dir {Name="Инжавинский районный суд Тамбовской области", DirName=dirName },
                new Dir {Name="Кирсановский районный суд Тамбовской области", DirName=dirName },
                new Dir {Name="Котовский городской суд Тамбовской области", DirName=dirName },
                new Dir {Name="Ленинский районный суд г. Тамбова Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой суд г. Котовска Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой суд г. Мичуринска Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой суд Кирсановского района Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой суд Ленинского района г. Тамбова Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой суд Мичуринского района Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой суд Моршанского района Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой суд Октябрьского района г. Тамбова Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой суд по г. Уварово и Уваровскому району Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой суд Рассказовского района Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой суд Советского района г. Тамбова Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой суд Сосновкого района Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой суд судебного участка Пичаевского района Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой суд Тамбовского района Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой судья судебного участка  Мучкапского района Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой судья судебного участка Бондарского района Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой судья судебного участка Гавриловского района Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой судья судебного участка Жердевского района Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой судья судебного участка Знаменского района Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой судья судебного участка Инжавинского района Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой судья судебного участка Мордовского района Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой судья судебного участка Никифоровского района Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой судья судебного участка Первомайского района Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой судья судебного участка Петровского района Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой судья судебного участка Пичаевского района Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой судья судебного участка Ржаксинского района Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой судья судебного участка Сампурского района Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой судья судебного участка Староюрьевского района Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой судья судебного участка Токаревского района Тамбовской области", DirName=dirName },
                new Dir {Name="Мировой судья судебного участка Уметского района Тамбовской области", DirName=dirName },
                new Dir {Name="Мичуринский городской суд Тамбовской области", DirName=dirName },
                new Dir {Name="Мичуринский районный суд Тамбовской области", DirName=dirName },
                new Dir {Name="Мордовский районный суд Тамбовской области", DirName=dirName },
                new Dir {Name="Моршанский районный суд Тамбовской области", DirName=dirName },
                new Dir {Name="Мучкапский районный суд Тамбовской области", DirName=dirName },
                new Dir {Name="Никифоровский районный суд Тамбовской области", DirName=dirName },
                new Dir {Name="Октябрьский районный суд г. Тамбова Тамбовской области", DirName=dirName },
                new Dir {Name="Первомайский районный суд Тамбовской области", DirName=dirName },
                new Dir {Name="Петровский районный суд Тамбовской области", DirName=dirName },
                new Dir {Name="Пичаевский районный суд Тамбовской области", DirName=dirName },
                new Dir {Name="Рассказовский районный суд Тамбовской области", DirName=dirName },
                new Dir {Name="Ржаксинский районный суд Тамбовской области", DirName=dirName },
                new Dir {Name="Сампурский районный суд Тамбовской области", DirName=dirName },
                new Dir {Name="Советский районный суд г. Тамбова Тамбовской области", DirName=dirName },
                new Dir {Name="Сосновский районный суд Тамбовской области", DirName=dirName },
                new Dir {Name="Староюрьевский районный суд Тамбовской области", DirName=dirName },
                new Dir {Name="Тамбовский областной суд", DirName=dirName },
                new Dir {Name="Тамбовский районный суд Тамбовской области", DirName=dirName },
                new Dir {Name="Токаревский районный суд Тамбовской области", DirName=dirName },
                new Dir {Name="Уваровский районный суд Тамбовской области", DirName=dirName },
                new Dir {Name="Уметский районный суд Тамбовской области", DirName=dirName }
            };
        }

        private static IEnumerable<Dir> GetPreconfiguredTypeApplicant(DataContext dataContext)
        {
            var result = new List<Dir>();

            dir = new Dir { Name = "Физическое лицо", DirName = dirName };
            result.Add(dir);
            dataContext.Dir.Add(dir);
            dataContext.Applicant.AddRange(new List<Applicant>() { new Applicant { Name = "test", Description = "test", TypeApplicant = dir }, new Applicant { Name = "test1", Description = "test1", TypeApplicant = dir }
            });

            dir = new Dir { Name = "Юридическое лицо", DirName = dirName };
            dataContext.Dir.Add(dir);
            result.Add(dir);
            dataContext.Applicant.AddRange(new List<Applicant>() { new Applicant { Name = "test2", Description = "test2", TypeApplicant = dir }, new Applicant { Name = "test3", Description = "test3", TypeApplicant = dir }
            });

            return result;
            //{
            //    new Dir {Name="Физическое лицо", DirName=dirName },
            //    new Dir {Name="Юридическое лицо", DirName=dirName }
            //};
        }
        private static IEnumerable<Dir> GetPreconfiguredDecision1()
        {
            return new List<Dir>()
            {
                new Dir {Name ="Оставлено без рассмотрения (абз. 2 ст. 222 ГПК РФ)", DirName=dirName },
                new Dir {Name="Оставлено без рассмотрения (абз. 3 ст. 222 ГПК РФ)", DirName=dirName },
                new Dir {Name="Оставлено без рассмотрения (абз. 4 ст. 222 ГПК РФ)", DirName=dirName },
                new Dir {Name="Оставлено без рассмотрения (абз. 5 ст. 222 ГПК РФ)", DirName=dirName },
                new Dir {Name="Оставлено без рассмотрения (абз. 7 ст. 222 ГПК РФ)", DirName=dirName },
                new Dir {Name="Оставлено без рассмотрения (абз. 8 ст. 222 ГПК РФ)", DirName=dirName },
                new Dir {Name="Оставлено без рассмотрения (п. 1 ст. 148 АПК РФ)", DirName=dirName },
                new Dir {Name="Оставлено без рассмотрения (п. 2 ст. 148 АПК РФ)", DirName=dirName },
                new Dir {Name="Оставлено без рассмотрения (п. 3 ст. 148 АПК РФ)", DirName=dirName },
                new Dir {Name="Оставлено без рассмотрения (п. 4 ст. 148 АПК РФ)", DirName=dirName },
                new Dir {Name="Оставлено без рассмотрения (п. 7 ст. 148 АПК РФ)", DirName=dirName },
                new Dir {Name="Отказано", DirName=dirName },
                new Dir {Name="Прекращено (абз. 2 ст. 220 ГПК РФ)", DirName=dirName },
                new Dir {Name="Прекращено (абз. 3 ст. 220 ГПК РФ)", DirName=dirName },
                new Dir {Name="Прекращено (абз. 4 ст. 220 ГПК РФ)", DirName=dirName },
                new Dir {Name="Прекращено (абз. 7 ст. 220 ГПК РФ)", DirName=dirName },
                new Dir {Name="Прекращено (п. 2 ст. 150 АПК РФ)", DirName=dirName },
                new Dir {Name="Прекращено (п. 5 ст. 150 АПК РФ)", DirName=dirName },
                new Dir {Name="Прекращено (п. 6 ст. 150 АПК РФ)", DirName=dirName },
                new Dir {Name="Прекращено (п.1 ст. 150 АПК РФ)", DirName=dirName },
                new Dir {Name="Прекращено (п.4 ст. 150 АПК РФ)", DirName=dirName },
                new Dir {Name="Удовлетворено", DirName=dirName },
                new Dir {Name="Удовлетворено в части", DirName=dirName }
            };
        }
        private static IEnumerable<Dir> GetPreconfiguredDecision2()
        {
            return new List<Dir>()
            {
                new Dir {Name="Возвращение апелляционной жалобы", DirName=dirName },
                new Dir {Name="Оставлено без движения", DirName=dirName },
                new Dir {Name="Прекращено (абз. 2 ст. 220 ГПК РФ)", DirName=dirName },
                new Dir {Name="Прекращено (абз. 3 ст. 220 ГПК РФ)", DirName=dirName },
                new Dir {Name="Прекращено (абз. 4 ст. 220 ГПК РФ)", DirName=dirName },
                new Dir {Name="Прекращено (абз. 7 ст. 220 ГПК РФ)", DirName=dirName },
                new Dir {Name="Прекращено (п. 1 ст. 265 АПК РФ)", DirName=dirName },
                new Dir {Name="Прекращено (п. 2 ст. 265 АПК РФ)", DirName=dirName },
                new Dir {Name="Решение оставлено без изменения", DirName=dirName },
                new Dir {Name="Решение отменено  - вынесено новое решение", DirName=dirName },
                new Dir {Name="Решение отменено – направлено на новое рассмотрение", DirName=dirName },
                new Dir {Name="Решение отменено в части", DirName=dirName }
            };
        }
        private static IEnumerable<Dir> GetPreconfiguredDecision3()
        {
            return new List<Dir>()
            {
                new Dir {Name="Возвращение кассационной жалобы", DirName=dirName },
                new Dir {Name="Оставлено без движения", DirName=dirName },
                new Dir {Name="Прекращено (абз.2 ст. 220 ГПК РФ)", DirName=dirName },
                new Dir {Name="Прекращено (абз.3 ст. 220 ГПК РФ)", DirName=dirName },
                new Dir {Name="Прекращено (абз.4 ст. 220 ГПК РФ)", DirName=dirName },
                new Dir {Name="Прекращено (абз.7 ст. 220 ГПК РФ)", DirName=dirName },
                new Dir {Name="Прекращено (п.1 ст. 282 АПК РФ)", DirName=dirName },
                new Dir {Name="Решение оставлено без изменения", DirName=dirName },
                new Dir {Name="Решение отменено – вынесено новое решение", DirName=dirName },
                new Dir {Name="Решение отменено – направлено на новое рассмотрение", DirName=dirName },
                new Dir {Name="Решение отменено в части", DirName=dirName }
            };
        }
        private static IEnumerable<Dir> GetPreconfiguredDecision4()
        {
            return new List<Dir>()
            {
                new Dir {Name="Возвращение надзорной жалобы", DirName=dirName },
                new Dir {Name="Оставлено без движения", DirName=dirName },
                new Dir {Name="Отказ в передаче дела в Президиум ВАС РФ", DirName=dirName },
                new Dir {Name="Отказано в истребовании", DirName=dirName },
                new Dir {Name="Отказано в передаче для рассмотрения по существу", DirName=dirName },
                new Dir {Name="Прекращено (абз.2 ст. 220 ГПК РФ)", DirName=dirName },
                new Dir {Name="Прекращено (абз.3 ст. 220 ГПК РФ)", DirName=dirName },
                new Dir {Name="Прекращено (абз.4 ст. 220 ГПК РФ)", DirName=dirName },
                new Dir {Name="Прекращено (абз.7 ст. 220 ГПК РФ)", DirName=dirName },
                new Dir {Name="Решение оставлено без изменения", DirName=dirName },
                new Dir {Name="Решение отменено – вынесено новое решение", DirName=dirName },
                new Dir {Name="Решение отменено – направлено на новое рассмотрение", DirName=dirName },
                new Dir {Name="Решение отменено в части", DirName=dirName }
            };
        }
        private static IEnumerable<Dir> GetPreconfiguredOPF()
        {
            return new List<Dir>()
            {
                new Dir {Name="ЗАО", DirName=dirName },
                new Dir {Name="ООО", DirName=dirName },
                new Dir {Name="ОАО", DirName=dirName },
                new Dir {Name="АО", DirName=dirName },
                new Dir {Name="МУП", DirName=dirName },
                new Dir {Name="НАО", DirName=dirName },
                new Dir {Name="ОДО", DirName=dirName },
                new Dir {Name="ПАО", DirName=dirName },
                new Dir {Name="ФГУП", DirName=dirName },
                new Dir {Name="ДУП", DirName=dirName },
                new Dir {Name="ПТ", DirName=dirName },
                new Dir {Name="ПК", DirName=dirName },
                new Dir {Name="ТВ", DirName=dirName },
                new Dir {Name="ГУП", DirName=dirName },
                new Dir {Name="АНО", DirName=dirName },
                new Dir {Name="ГЖИ", DirName=dirName },
                new Dir {Name="ГК", DirName=dirName },
                new Dir {Name="ГОУ", DirName=dirName },
                new Dir {Name="ГУ", DirName=dirName },
                new Dir {Name="МДОУ", DirName=dirName },
                new Dir {Name="МОУ", DirName=dirName },
                new Dir {Name="ОГБУ", DirName=dirName },
                new Dir {Name="СО", DirName=dirName },
                new Dir {Name="ТОС", DirName=dirName },
                new Dir {Name="ТСЖ", DirName=dirName },
                new Dir {Name="ФГБУ", DirName=dirName },
                new Dir {Name="ФГУ", DirName=dirName },
                new Dir {Name="АБ", DirName=dirName },
                new Dir {Name="АС", DirName=dirName },
                new Dir {Name="АКХ", DirName=dirName },
                new Dir {Name="АФХ", DirName=dirName },
                new Dir {Name="ГК", DirName=dirName },
                new Dir {Name="КО", DirName=dirName },
                new Dir {Name="КА", DirName=dirName },
                new Dir {Name="МУ", DirName=dirName },
                new Dir {Name="НП", DirName=dirName },
                new Dir {Name="ОО", DirName=dirName },
                new Dir {Name="ОД", DirName=dirName },
                new Dir {Name="ОФ", DirName=dirName },
                new Dir {Name="ОВС", DirName=dirName },
                new Dir {Name="ОМН РФ", DirName=dirName },
                new Dir {Name="ООС", DirName=dirName },
                new Dir {Name="ПП", DirName=dirName },
                new Dir {Name="ПК", DirName=dirName },
                new Dir {Name="Профсоюз", DirName=dirName },
                new Dir {Name="РО", DirName=dirName },
                new Dir {Name="СОДНО", DirName=dirName },
                new Dir {Name="Фонд", DirName=dirName },
                new Dir {Name="ЧУ", DirName=dirName },
                new Dir {Name="Государственный орган", DirName=dirName }
            };
        }

        private static IEnumerable<Performer> GetPreconfiguredPerformers()
        {
            return new List<Performer>()
            {
                new Performer {Name="Белякина Маргарита Александровна" , Region = region },
                new Performer {Name="Волосевич Юлия Сергеевна", Region = region },
                new Performer {Name="Арзамасцева Елена Геннадьевна", Region = region },
                new Performer {Name="Галузинская Екатерина Владимировна", Region = region },
                new Performer {Name="Горшкова Ирина Геннадьевна", Region = region },
                new Performer {Name="Грабко Александр Сергеевич", Region = region },
                new Performer {Name="Гунько Анастасия Игоревна", Region = region },
                new Performer {Name="Данилова Виктория Викторовна", Region = region },
                new Performer {Name="Завражнева Ольга Анатольевна", Region = region },
                new Performer {Name="Киянова Татьяна Ивановна", Region = region },
                new Performer{Name="Колотуша Марта Анатольевна", Region = region },
                new Performer{Name="Корнеева Елена Владимировна", Region = region },
                new Performer{Name="Корнишина Лариса Анатольевна", Region = region },
                new Performer{Name="Коханов Дмитрий Павлович", Region = region },
                new Performer{Name="Курьянова Елена Николаевна", Region = region },
                new Performer{Name="Лихачева Елена Николаевна", Region = region },
                new Performer{Name="Ломовцева Татьяна Александровна", Region = region },
                new Performer{Name="Николаев Алексей Евгеньевич", Region = region },
                new Performer{Name="Панова Ольга Анатольевна", Region = region },
                new Performer{Name="Платицына Елена Геннадьевна", Region = region },
                new Performer{Name="Решетова Ирина Николаевна", Region = region },
                new Performer{Name="Рыбкина Ольга Анатольевна", Region = region },
                new Performer{Name="Рыжкова Юлия Владимировна", Region = region },
                new Performer{Name="Сапрыкина Анастасия Александровна", Region = region },
                new Performer{Name="Сиднева Галина Васильевна", Region = region },
                new Performer{Name="Суворин Андрей Владимирович", Region = region },
                new Performer{Name="Сычева Светлана Алексеевна", Region = region },
                new Performer{Name="Тарнопольская Елена Сергеевна", Region = region },
                new Performer{Name="Топорков Илья Николаевич", Region = region },
                new Performer{Name="Труба Антонина Александровна", Region = region },
                new Performer{Name="Фатахутдинов Денис Фаилевич", Region = region },
                new Performer{Name="Черкасова Мария Сергеевна", Region = region },
                new Performer{Name="Четверикова Елена Витальевна", Region = region },
                new Performer{Name="Чубарова Юлия Юрьевна", Region = region },
                new Performer{Name="Шишкова Елена Алексеевна", Region = region },
                new Performer{Name="Яблочкина Татьяна Юрьевна", Region = region }
            };
        }

        private static IEnumerable<Region> GetPreconfiguredRegions()
        {
            region = new Region { Code = "079", Name = "Тамбовская область", Description = "Каталог ОПФР по Тамбовской области" };
            return new List<Region>() { region, new Region { Code = "100", Name = "Московская область", Description = "Каталог ОПФР по Московской области" } };
        }

        private static IEnumerable<District> GetPreconfiguredDistricts()
        {
            return new List<District>() {
                new District {Code="001", Name = "Бондарский", Description = "ОПФР по Бондарскому району", Region = region },
                new District {Code="002",  Name = "Гавриловский", Description = "ОПФР по Гавриловскому району", Region = region },
                new District { Code="003", Name = "Жердевский", Description = "ОПФР по Жердевскому району", Region = region },
                new District { Code="004", Name = "Знаменский", Description = "ОПФР по Знаменскому району", Region = region },
                new District { Code="005", Name = "Инжавинский", Description = "ОПФР по Инжавинскому району", Region = region },
                new District { Code="006", Name = "Кирсановский", Description = "ОПФР по Кирсановскому району", Region = region },
                new District { Code="007", Name = "Мичуринский", Description = "ОПФР по Мичуринскому району", Region = region },
                new District { Code="008", Name = "Мордовский", Description = "ОПФР по Мордовскому району", Region = region },
                new District { Code="009", Name = "Моршанский", Description = "ОПФР по Моршанскому району", Region = region },
                new District { Code="010", Name = "Мучкапский", Description = "ОПФР по Мучкапскому району", Region = region },
                new District { Code="011", Name = "Никифоровский", Description = "ОПФР по Никифоровскому району", Region = region },
                new District { Code="012", Name = "Первомайский", Description = "ОПФР по Первомайскому району", Region = region },
                new District { Code="013", Name = "Петровский", Description = "ОПФР по Петровскому району", Region = region },
                new District { Code="014", Name = "Пичаевский", Description = "ОПФР по Пичаевскому району", Region = region },
                new District { Code="015", Name = "Рассказовский", Description = "ОПФР по Рассказовскому району", Region = region },
                new District { Code="016", Name = "Ржаксинский", Description = "ОПФР по Ржаксинскому району", Region = region },
                new District { Code="017", Name = "Сампурский", Description = "ОПФР по Сампурскому району", Region = region },
                new District { Code="018", Name = "Сосновский", Description = "ОПФР по Сосновскому району", Region = region },
                new District { Code="019", Name = "Староюрьевский", Description = "ОПФР по Староюрьевскому району", Region = region },
                new District { Code="020", Name = "Тамбовский", Description = "ОПФР по Тамбовскому району", Region = region },
                new District { Code="021", Name = "Токаревский", Description = "ОПФР по Токаревскому району", Region = region },
                new District { Code="022", Name = "Уваровский", Description = "ОПФР по Уваровскому району", Region = region },
                new District { Code="023", Name = "Уметский", Description = "ОПФР по Уметскому району", Region = region },
                new District { Code="024", Name = "Котовск", Description = "ОПФР по г.Котовску", Region = region },
                new District { Code="025", Name = "Тамбов", Description = "ОПФР по г.Тамбову", Region = region } };
        }

        private static IEnumerable<SubjectClaim> GetPreconfiguredSubjectClaimsOut(int cod)
        {
            switch (cod)
            {
                case 1:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "1.1", Name = "Взыскание недоимки по страховым взносам и пеней с организаций и индивидуальных предпринимателей (ч.4 ст.18, ч.5 ст.19)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "1.2", Name = "Взыскание штрафа за непредставление в установленный срок расчета по начисленным и уплаченным страховым взносам (ч. 1 ст. 46 )", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "1.3", Name = "Взыскание штрафа за несоблюдение порядка представления расчета по начисленным и уплаченным страховым взносам в орган контроля за уплатой страховых взносов в электронном виде (ч.2 ст.46 )", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "1.4", Name = "Взыскание штрафа за неуплату сумм страховых взносов (занижение базы для начисления) (ч.1 ст.47 )", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "1.5", Name = "Взыскание штрафа за неуплату сумм страховых взносов (занижение базы для начисления) (ч.2 ст.47 )", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "1.6", Name = "Взыскание штрафа за отказ или непредставление в орган контроля за уплатой страховых взносов документов, необходимых для осуществления контроля за уплатой страховых взносов (ст.48)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "1.7", Name = "О взыскании штрафа за несообщение банком сведений о счете плательщика страховых взносов (ст.49)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "1.8", Name = "О взыскании штрафа за непредставление банком справок (выписок) по операциям и счетам в орган контроля за уплатой страховых взносов (ст. 49.1)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "1.9", Name = "О взыскании штрафа за нарушение банком срока исполнения поручения о перечислении страховых взносов, пеней и штрафов (ст.50 )", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "1.10", Name = "О взыскании штрафа за неисполнение банком поручения о перечислении страховых взносов, пеней и штрафов (ст. 51)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "1.11", Name = "Взыскание страховых взносов с предприятий, признанных банкротами (о взыскании текущих платежей)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "1.12", Name = "Иные вопросы по уплате страховых взносов", GroupClaim = groupClaim, Description = "" }
                    };
                case 2:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "2.1", Name = "Взыскание финансовых санкций за непредставление в установленные сроки необходимых для осуществления индивидуального (персонифицированного) учета сведений", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "2.2", Name = "Взыскание финансовых санкций за представление страхователем неполных и (или) недостоверных сведений", GroupClaim = groupClaim, Description = "" }
                    };
                case 3:
                    return new List<SubjectClaim>
                    {   new SubjectClaim { Code = "3.1", Name = "Взыскание штрафа за несвоевременную регистрацию в ПФР (абз. 1 п.1 ст.27)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "3.2", Name = "Взыскание штрафа за несвоевременную регистрацию в ПФР (более 90 дней) (абз. 3 п.1 ст.27)", GroupClaim = groupClaim, Description = "" }
                    };
                case 4:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "4.1", Name = "О признании недействительными договоров (государственных контрактов) и применении последствий недействительности сделки", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "4.2", Name = "Взыскание стоимости товара, работ или услуг по договору  (государственному контракту)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "4.3", Name = "О взыскании долга и процентов за пользование чужими денежными средствами", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "4.4", Name = "Об обмене некачественного или некомплектного товара", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "4.5", Name = "О расторжении договора (государственного контракта) и взыскании неустойки, штрафа, пеней за неисполнение или ненадлежащее исполнение обязательств по договору", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "4.6", Name = "Иные вопросы по договорным отношениям", GroupClaim = groupClaim, Description = "" }
                    };
                case 5:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "5.1", Name = "Жалоба на действия (бездействия) арбитражного управляющего", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "5.2", Name = "Об оспаривании сделок должника", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "5.3", Name = "О включении в реестр требований кредиторов задолженности по страховым взносам, пеней и штрафов", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "5.4", Name = "Об очередности удовлетворения требований кредиторов", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "5.5", Name = "Иные споры по делам о банкротстве", GroupClaim = groupClaim, Description = "" }
                    };
                case 6:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "6", Name = "Взыскание излишне выплаченных сумм пенсий, дополнительного материального обеспечения и компенсационных выплат с организации, представившей недостоверные сведения", GroupClaim = groupClaim, Description = "" }
                    };
                case 7:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "7", Name = "Исполнительное производство", GroupClaim = groupClaim, Description = "" }
                    };
                case 8:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "8.1", Name = "Взыскание недоимки по страховым взносам, пеней и штрафов за счет имущества плательщика страховых взносов - физического лица, не являющегося индивидуальным предпринимателем (ст. 21 )", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "8.2", Name = "Взыскание недоимки по страховым взносам, а также пеней и штрафов с физического лица, утратившего статус индивидуального предпринимателя (ст.21)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "8.3", Name = "Взыскание штрафа за непредставление в установленные сроки расчета по начисленным и уплаченным страховым взносам  (ч. 1 ст. 46 )", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "8.4", Name = "Взыскание штрафа за отказ или непредставление в орган контроля за уплатой страховых взносов документов, необходимых для осуществления контроля за уплатой страховых взносов (ст.48)", GroupClaim = groupClaim, Description = "" }
                        };
                case 9:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "9.1", Name = "Взыскание штрафа за несвоевременную регистрацию в ПФР (абз. 1 п.1 ст.27 )", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "9.2", Name = "Взыскание штрафа за несвоевременную регистрацию в ПФР (более 90 дней) (абз. 2 п.1 ст.27 )", GroupClaim = groupClaim, Description = "" }
                      };
                case 10:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "10.1", Name = "Взыскание штрафа за нарушение срока регистрации (ст.15.32 КоАП РФ)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "10.2", Name = "Взыскание штрафа за нарушение срока представления сведений об открытии (закрытии) счета (ч.1 ст. 15.33 КоАП РФ)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "10.3", Name = "Взыскание штрафа за нарушение установленных законодательством Российской Федерации сроков представления расчета по начисленным и уплаченным страховым взносам (ч.2 ст. 15.33 КоАП РФ)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "10.4", Name = "Взыскание финансовых санкций за представление сведений, необходимых для осуществления контроля (ч.3 ст. 15.33 КоАП РФ)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "10.5", Name = "Взыскание штрафа за нарушение срока исполнения поручения о перечислении страховых взносов (ч.2 ст.15.10 КоАП РФ)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "10.6", Name = "Взыскание штрафа за представление негосударственным пенсионным фондом в Пенсионный фонд Российской Федерации недостоверных сведений в уведомлении о вновь заключенных договорах об обязательном пенсионном страховании, а также подложных заявлений застрахованных лиц о выборе страховщика по обязательному пенсионному страхованию и (или) договоров об обязательном пенсионном страховании, повлекшее неправомерное перечисление негосударственному пенсионному фонду средств пенсионных накоплений (ч.10.1 ст.15.29 КоАП РФ)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "10.7", Name = "Взыскание штрафа за повторное в течение года совершение административного правонарушения, предусмотренного частью 10.1 статьи 15.29 КоАП РФ (ч.10.2 ст.15.29 КоАП РФ)", GroupClaim = groupClaim, Description = "" }
                    };
                case 11:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "11.1", Name = "О внесении изменений в индивидуальные сведения персонифицированного учета застрахованного лица", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "11.2", Name = "Об обязании представить индивидуальные сведения о застрахованных лицах, а также расчет по начисленным и уплаченным страховым взносам", GroupClaim = groupClaim, Description = "" }
                    };
                case 12:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "12.1", Name = "Взыскание излишне выплаченных сумм пенсии,ЕДВ, ДМО, региональной социальной доплаты , иных выплат", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "12.2", Name = "Взыскание в регрессном порядке выплаченных сумм пенсии по инвалидности или по потере кормильца с лиц, виновных в причинении вреда здоровью или жизни кормильца", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "12.3", Name = "Взыскание незаконно полученной компенсации стоимости проезда к месту отдыха и обратно", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "12.4", Name = "Взыскание незаконно полученной компенсации расходов, связанных с переездом из районов Крайнего Севера и приравненных к ним местностей, лицам, являющимся получателями трудовых пенсий и (или) пенсий по государственному пенсионному обеспечению, и членам их семей", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "12.5", Name = "Взыскание выплаченных сумм пенсии с виновного лица по искам прокуроров в интересах ПФР", GroupClaim = groupClaim, Description = "" }
                    };
                case 13:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "13", Name = "Взыскание средств пенсионных накоплений в порядке регресса (п. 31 Правил, утвержденных постановлением Правительства РФ от 30.07.2014 № 711)", GroupClaim = groupClaim, Description = "" }
                    };
                case 14:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "14", Name = "Взыскание по ДТП", GroupClaim = groupClaim, Description = "" }
                     };
                case 15:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "15.1", Name = "О признании государственного сертификата на материнский (семейный) капитал недействительным ( ПФР истец)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.2", Name = "О взыскании средств материнского (семейного) капитала (ПФР истец)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.3", Name = "О взыскании незаконно полученных средств материнского (семейного) капитала в рамках рассмотрения уголовного дела (ПФР истец)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.4", Name = "Об исполнении обязательства оформить жилое помещение, приобретенное с использованием средств МСК в общую долевую собственность ( ПФР третье лицо)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.5", Name = "Иные", GroupClaim = groupClaim, Description = "" }
                    };
                case 16:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "16", Name = "Взыскание незаконно полученных сумм пенсий  и (или) иных социальных выплат в рамках рассмотрения уголовного дела,", GroupClaim = groupClaim, Description = "" }
                        };
                case 17:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "17", Name = "Иск пенсионера, работника или прокурора к работодателю об обязании уплатить страховые взносы, подать в ПФР сведения ПУ или расчет по начисленным и уплаченным страховым взносам, о взыскании суммы неполученной пенсии в связи с неуплатой страховых взносов, в т.ч. возмещении морального вреда (территориальный орган ПФР 3-е лицо )", GroupClaim = groupClaim, Description = "" }
                    };
                case 18:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "18", Name = "Иные", GroupClaim = groupClaim, Description = "" }
                    };
                case 19:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "19.1", Name = "Оплата госпошлины (арбитраж)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "19.2", Name = "Иные судебные издержки (арбитраж)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "19.3", Name = "Оплата госпошлины (общая юрисдикция)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "19.4", Name = "Иные судебные издержки (общая юрисдикция)", GroupClaim = groupClaim, Description = "" }
                    };
                default:
                    return null;
            }
        }
        private static IEnumerable<SubjectClaim> GetPreconfiguredSubjectClaimsIn(int cod)
        {
            switch (cod)
            {
                case 1:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Name = "О признании обязанности по уплате страховых взносов исполненной", Code = "1.1", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Name = "О зачете (возврате) излишне уплаченных (взысканных) страховых взносов, пеней и штрафов", Code = "1.2", GroupClaim = groupClaim, Description = "" }
                    };
                case 2:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Name = "О признании незаконным решения территориального органа ПФР о взыскании недоимки по страховым взносам, пеней  и штрафов", Code = "2.1", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Name = "О признании незаконным решения территориального органа ПФР о взыскании страховых взносов в фиксированных размерах", Code = "2.2", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Name = "Об обжаловании  решения территориального органа ПФР о привлечении к ответственности за нарушение законодательства об обязательном пенсионном страховании и законодательства о страховых взносах", Code = "2.3", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Name = "Об освобождении от уплаты страховых взносов на обязательное пенсионное и медицинское страхование в период   нахождения в отпуске по уходу за ребёнком до 1,5 лет.", Code = "2.4", GroupClaim = groupClaim, Description = "" }
                    };
                case 3:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Name = "О внесении изменений в индивидуальные сведения персонифицированного учета застрахованного лица", Code = "3.1", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Name = "Иные споры, возникающие при реализации Федерального закона от  01.04.1996  № 27-ФЗ", Code = "3.2", GroupClaim = groupClaim, Description = "" }
                    };
                case 4:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Name = "О признании недействительными договоров (гос.контрактов) и применении последствий недействительной сделки", Code = "4.1", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Name = "Взыскание стоимости товара, работ, услуг по договору (государственному контракту)", Code = "4.2", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Name = "О расторжении договора (гос.контракта) и взыскании неустойки, штрафа, пеней за неисполнение или ненадлежащее исполнение обязательств по договору", Code = "4.3", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Name = "Иные споры, возникающие из договоров (соглашений, государственных контрактов)", Code = "4.4", GroupClaim = groupClaim, Description = "" }
                    };
                case 5:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Name = "Взыскание по ДТП", Code = "5", GroupClaim = groupClaim, Description = "" }
                    };
                case 6:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Name = "Споры, возникающие в рамках исполнительного производства", Code = "6", GroupClaim = groupClaim, Description = "" }
                    };
                case 7:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Name = "Дела о банкротстве", Code = "7", GroupClaim = groupClaim, Description = "" }
                    };
                case 8:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Name = "Обжалование действий (бездействий), актов, решений территориальных органов ПФР и их должностных лиц в рамках ФЗ № 212-ФЗ", Code = "8.1", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Name = "Обжалование действий (бездействий), актов, решений территориальных органов ПФР и их должностных лиц  в рамках ФЗ № 167-ФЗ", Code = "8.2", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Name = "Обжалование действий (бездействий), актов, решений территориальных органов ПФР и их должностных лиц  в рамках ФЗ № 27-ФЗ", Code = "8.3", GroupClaim = groupClaim, Description = "" }
                    };
                case 9:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "9.1", Name = "О признании обязанности по уплате страховых взносов исполненной", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "9.2", Name = "О зачете(возврате) излишне уплаченных(взысканных) страховых взносов, пеней и штрафов", GroupClaim = groupClaim, Description = "" }
                    };
                case 10:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "10.1", Name = "О восстановлении на работе и оплате за время вынужденного прогула", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "10.2", Name = "О взыскании заработной платы", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "10.3", Name = "Об оспаривании дисциплинарного взыскания", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "10.4", Name = "О взыскании компенсации стоимости проезда к месту отдыха и обратно работников ПФР", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "10.5", Name = "Иные споры, возникающие из трудовых отношений", GroupClaim = groupClaim, Description = "" }
                    };
                case 11:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "11.1", Name = "О признании договора об обязательном пенсионном страховании недействительным", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "11.2", Name = "Споры по агентским договорам, заключенным с НПФ(территориальный орган ПФР – 3 - е лицо)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "11.3", Name = "Иные споры, возникающие при реализации Федерального закона от 07.05.1998 № 75 - ФЗ", GroupClaim = groupClaim, Description = "" }
                    };
                case 12:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "12", Name = "О признании права состоять на учёте в ПФР без присвоения страхового номера индивидуального лицевого счета", GroupClaim = groupClaim, Description = "" }
                    };
                case 13:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "13.1", Name = "О взыскании стоимости проезда к месту отдыха и обратно", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "13.2", Name = "О предоставлении проездных документов, обеспечивающих проезд пенсионера к месту отдыха и обратно", GroupClaim = groupClaim, Description = "" }
                    };
                case 14:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "14", Name = "О взыскании расходов, связанных с выездом из районов Крайнего Севера и приравненных к ним местностей   на новое место жительства(ст. 35 Закона РФ № 4520 - 1)", GroupClaim = groupClaim, Description = "" }
                    };
                case 15:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "15.1", Name = "Индексация и валоризация пенсий, в т.ч.", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.1.1", Name = "О взыскании индексации(компенсации) несвоевременно назначенной пенсии и несвоевременно произведённого перерасчёта пенсии", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.1.2.О", Name = "Взыскании индексации несвоевременно выплаченной пенсии", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.1.3", Name = "О взыскании убытков, причиненных неправильным исчислением пенсии", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.1.4", Name = "Спор о правильности индексации страховой пенсии", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.1.5", Name = "Спор о  правильности валоризации величины расчетного пенсионного капитала застрахованного лица, исчисленного  при оценке его пенсионных прав", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.1.6", Name = "Спор по  исчислению суммы расчетного пенсионного капитала застрахованного лица, с учетом которой исчисляется  размер страховой пенсии(страховой части  трудовой пенсии по старости)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.2", Name = "Специальный стаж(педагогический и медицинский состав), в т.ч.", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.2.1", Name = "Об отказе включить в стаж, дающий право на досрочное назначение страховой пенсии по старости педагогическим работникам,  периодов их работы в должностях и учреждениях, не предусмотренных соответствующими Списками", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.2.2", Name = "Об отказе включить в стаж, дающий право на досрочное назначение страховой пенсии по старости медицинским работникам,   периодов их работы в должностях и учреждениях, не предусмотренных соответствующими списками", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.2.3", Name = "Об отказе включить в стаж, дающий право на досрочное назначение страховой пенсии по старости педагогическим работникам,  периодов работы и иной деятельности(в т.ч.по ранее действующему законодательству)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.2.4", Name = "Об отказе включить в стаж, дающий право на досрочное назначение страховой пенсии по старости медицинским работникам,  периодов работы и иной деятельности(в т.ч.по ранее действующему законодательству)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.2.5", Name = "Об отказе исчислить медицинский стаж при работе в сельской местности и(или) поселках городского типа(рабочих поселках)  в соотношении один год, как год и три месяца", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.2.6", Name = "Об отказе исчислять стаж работы в должности хирурга, а также в иных должностях и учреждениях, предусмотренных в постановлении Правительства РФ от 29.10.2002 № 781, в соотношении один год, как год и шесть месяцев", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.2.7", Name = "Об отказе исчислять периоды нахождения в командировках, на курсах повышения квалификации, в учебных отпусках, протекавших   в периоды работы в должностях и учреждениях, предусмотренных в постановлении Правительства РФ от 29.10.2002 № 781 и по ранее действующему законодательству РФ, в соотношении один год, как год и шесть месяцев", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.3.1", Name = "Применение Списка №1 при назначении пенсии (пункт 1 части 1 статьи 30 ФЗ № 400-ФЗ)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.3.2", Name = "Применение Списка №2 при назначении пенсии(пункт 2 части 1 статьи 30 ФЗ № 400 - ФЗ)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.4.1", Name = "Назначение второй пенсии по ст. 29 Закона РФ № 1244 - 1", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.4.2", Name = "О снижении пенсионного возраста на дополнительную величину по ст. 32 - 37 Закона РФ № 1244 - 1", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.4.3", Name = "О снижении пенсионного возраста по п. 11 ст. 2  Закона РФ № 2 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.5", Name = "Вторая пенсия, компенсационные и ежемесячные выплаты трудоспособному лицу, доплаты к пенсии, в т.ч.", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.5.1", Name = "О праве на установление второй пенсии инвалидам военной травмы, УВОВ,ЖБЛ, родителям и вдовам военнослужащих", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.5.2", Name = "О назначении, перерасчете пенсии участникам ВОВ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.5.3", Name = "О взыскании сумм компенсационной и ежемесячной  выплаты неработающему трудоспособному лицу, осуществляющему   уход за нетрудоспособными гражданами", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.5.4", Name = "О праве на одновременное получение двух пенсий, за исключением категорий, указанных в графе 15.5.1", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.5.5", Name = "Споры о назначении, перерасчете и выплате доплаты к пенсии работникам угольной промышленности в соответствии с Федеральным законом от 10 мая 2010 № 84 - ФЗ О дополнительном социальном обеспечении отдельных категорий работников организаций угольной промышленности", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.5.6", Name = "О ежемесячной и компенсационной выплате неработающему трудоспособному лицу", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.1", Name = "О назначении страховой пенсии военным пенсионерам", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.2", Name = "О подтверждении и исчислении среднемесячного заработка в целях оценки пенсионных прав", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.3", Name = "О порядке исчисления, правил подсчета и порядке подтверждения страхового стажа в соответствии со статьями 13, 14 ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.4", Name = "О праве на досрочное назначение пенсии в соответствии с пунктом 3 части 1 статьи 30 ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.5", Name = "О праве на досрочное назначение пенсии в соответствии с пунктом 4 части 1 статьи 30 ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.6", Name = "О праве на досрочное назначение пенсии в соответствии с пунктом 5 части 1 статьи 30 ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.7", Name = "О праве на досрочное назначение пенсии в соответствии с пунктом 6 части 1 статьи 30 ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.8", Name = "О праве на досрочное назначение пенсии в соответствии с пунктом 7 части 1 статьи 30 ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.9", Name = "О праве на досрочное назначение пенсии в соответствии с пунктом 8 части 1 статьи 30 ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.10", Name = "О праве на досрочное назначение пенсии в соответствии с пунктом 9 части 1 статьи 30 ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.11", Name = "О праве на досрочное назначение пенсии в соответствии с пунктом 10 части 1 статьи 30 ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.12", Name = "О праве на досрочное назначение пенсии в соответствии с пунктом 11 части 1 статьи 30 ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.13", Name = "О праве на досрочное назначение пенсии в соответствии с пунктом 12 части 1 статьи 30 ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.14", Name = "О праве на досрочное назначение пенсии в соответствии с пунктом 1 части 1 статьи 32 ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.15", Name = "О праве на досрочное назначение пенсии женщинам, имеющим двух и более детей за работу в РКС / МКС в соответствии пунктом 2 части 1 статьи 32 ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.16", Name = "О праве на досрочное назначение пенсии инвалидам вследствие военной травмы в соответствии с пунктом 3 части 1 статьи 32   ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.17", Name = "О праве на досрочное назначение пенсии инвалидам по зрению в соответствии с пунктом 4 части 1 статьи 32 ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.18", Name = "О праве на досрочное назначение пенсии лилипутам и карликам в соответствии с пунктом 5 части 1 статьи 32 ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.19", Name = "О праве на досрочное назначение пенсии за работу в РКС/ МКС в соответствии с пунктом 6 части 1 статьи 32 ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.20", Name = "О праве на досрочное назначение пенсии за работу спасателем МЧС в соответствии с пунктом 16 части 1 статьи 30 ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.21", Name = "О праве на досрочное назначение пенсии за работу с осужденными в соответствии с пунктом 17 части 1 статьи 30 ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.22", Name = "О праве на досрочное назначение пенсии за работу пожарным в соответствии с пунктом 18 части 1 статьи 30 ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.23", Name = "О праве на досрочное назначение пенсии работникам театров в соответствии с пунктом 21 части 1 статьи 30 ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.24", Name = "О льготном порядке исчисления страхового стажа на соответствующих видах работ с учетом РКС / МКС(в 2 - кратном и 1, 5 - кратном размере)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.25", Name = "О праве на досрочное назначение пенсии оленеводам, рыбакам, охотникам - промысловикам в соответствии с пунктом 7 части 1 статьи 32 ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.26", Name = "Об исчислении  стажа(суммирование стажа на соответствующих видах работ и снижение возраста, дающего право   на страховую пенсию по старости, лицам, работавшим в районах Крайнего Севера и приравненных к ним местностях) часть 2 статьи 33 ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.27", Name = "О включении в стаж на соответствующих видах работ иных периодов, предусмотренных статьей 12 ФЗ № 400 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.28", Name = "Об установлении и выплате пенсии по СПК", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.29", Name = "О сроках назначения и  перерасчета пенсии в соответствии со статьями 22, 23 ФЗ № 400 - ФЗ, в том числе о назначении пенсии с момента первоначального обращения", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.30", Name = "О конвертации пенсионных прав застрахованных лиц с применением стажа на соответствующих видах работ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.31", Name = "О суммировании стажа работы, дающей право на досрочное назначение трудовой пенсии по старости в связи с особыми  условиями труда, предусмотренными абз.3 п.3 постановления Правительства РФ от 11.07.2002 г. № 516", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.32", Name = "Об отказе включения в общий трудовой стаж в целях оценки пенсионных прав застрахованных лиц нестраховых периодов", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.33", Name = "О применении районного коэффициента при конвертации пенсионных прав ЗЛ с учетом размера отношения заработков", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.34", Name = "О конвертации пенсионных прав лиц, с применением общего трудового стажа", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.35", Name = "Об отказе во включении в спец. стаж периодов нахождения в отпусках по уходу за ребенком", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.36", Name = "О конвертации пенсионных прав с применением сниженных требований к стажевому коэффициенту", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.37", Name = "Об установлении и выплате пенсии по инвалидности", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.38", Name = "Об установленииповышения  фиксированной выплаты к страховой  пенсии по старости и к страховой пенсии по инвалидности в связи с работой в РКС и МПКС", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.39", Name = "О назначении трудовой пенсии иностранному гражданину или лицу без гражданства, без вида на жительство", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.40", Name = "О включении в общий трудовой стаж периодов работы во время ВОВ до достижения возраста 16 лет по свидетельским показаниям", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.41", Name = "Об отказе включить в стаж, дающий право на досрочное назначение страховой пенсии по старости, периодов нахождения  в командировках, на курсах повышения квалификации, в учебных отпусках(в т.ч.по ранее действующему законодательству)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.42", Name = "Перерасчет пенсии по документам, не предусмотренным пенсионным законодательством", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.43", Name = "Об оспаривании удержаний из пенсий и других выплат", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.44", Name = "Обжалование решения территориальных органов ПФР об отказе в назначении пенсии в связи с недостоверным(откорректированным) представлением сведений индивидуального(персонифицированного) учёта", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.45", Name = "О включении в стаж на соответствующих видах работ периодов нахождения на военных сборах и донорские дни", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.46", Name = "О назначении(перерасчете) пенсии в соответствии с нормами международных договоров(соглашений) о сотрудничестве в области пенсионного обеспечения", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.6.47", Name = "Обжалование отказа произвести перерасчёт пенсии с учётом иждивенцев", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.7.1 ", Name = "Об обжаловании решений о приостановлении выплаты пенсии по доверенности сроком действия более 1 года в случае не подтверждения пенсионером факта регистрации по месту получения пенсии", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.7.2", Name = "О прекращении и восстановлении выплаты пенсии", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.7.3", Name = "О приостановлении и возобновлении выплаты пенсии", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.7.4", Name = "О выплате пенсии, неполученной пенсионером своевременно по вине пенсионера или территориального органа ПФР", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.7.5", Name = "О выплате сумм пенсий, неполученных пенсионером в связи со смертью, а также о включении суммы неполученной   пенсии в наследственную массу", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.7.6", Name = "О выплате пенсии гражданину, выехавшему на постоянное место жительства за пределы РФ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.7.7", Name = "О выплате пособия на погребение", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.7.8", Name = "О взыскании процентов с невыплаченной через кредитные организации пенсии", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.7.9", Name = "О выплате сумм начисленной пенсии в полном размере", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.8.1", Name = "Летный состав ГА(пункт 13 части 1 статьи 30 ФЗ № 400 - ФЗ)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.8.2", Name = "Управление полетами воздушных судов ГА(пункт 14 части 1 статьи 30 ФЗ № 400 - ФЗ)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.8.3", Name = "Инженерно - технический состав по обслуживанию воздушных судов ГА(пункт 15 части 1 статьи 30 ФЗ № 400 - ФЗ)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.8.4", Name = "Установление ежемесячной доплаты к пенсии ЧЛЭ(ФЗ № 155 - ФЗ от 27.11.2001)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.8.5", Name = "О включении в стаж для назначения пенсии летно-испытательному составу периодов военной службы в должностях летного состава, работы в должностях летного состава гражданской авиации и обучения в учебных заведениях", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.8.6", Name = "Об установлении страховой пенсии и пенсии за выслугу лет гражданам из числа работников летно-испытательного состава", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "15.9", Name = "Иные споры об обжаловании решений территориальных органов ПФР об отказе в назначении(перерасчете) пенсии", GroupClaim = groupClaim, Description = "" }
                    };
                case 16:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "16", Name = "Иски о назначении, перерасчете и выплате ЕДВ", GroupClaim = groupClaim, Description = "" }
                    };
                case 17:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "17.1", Name = "Об обжаловании решения об отказе в выдаче государственного сертификата на материнский(семейный) капитал(ч. 6 ст. 5  ФЗ № 256 - ФЗ)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "17.2", Name = "Об обжаловании решения об отказе в удовлетворении заявления о распоряжении средствами(частью средств) материнского(семейного) капитала(ст. 10 ФЗ № 256 - ФЗ)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "17.3", Name = "Об обжаловании решения об отказе в удовлетворении заявления о распоряжении  средствами(частью средств) материнского(семейного) капитала(ст. 11 ФЗ № 256 - ФЗ)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "17.4", Name = "Об обжаловании решения об отказе в удовлетворении заявления о распоряжении  средствами(частью средств) материнского(семейного) капитала(ст. 11.1 ФЗ № 256 - ФЗ)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "17.5", Name = "Об обжаловании решения об отказе в удовлетворении заявления о распоряжении  средствами(частью средств) материнского(семейного) капитала(ст. 12  ФЗ № 256 - ФЗ)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "17.6", Name = "О восстановлении права на дополнительные меры государственной поддержки и зачислении средств материнского(семейного)  капитала на лицевой счет владельца государственного сертификата", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "17.7", Name = "Иски прокурора в интересах Российской Федерации(территориальный орган ПФР – 3 - е лицо)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "17.8", Name = "О предоставлении единовременной выплаты за счет средств материнского (семейного) капитала", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "17.9", Name = "Иные споры, возникающие при реализации Федерального закона от 29.12.2006 г.№256 - ФЗ", GroupClaim = groupClaim, Description = "" },
                    };
                case 18:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "18.1", Name = "О назначении дополнительного материального обеспечения в соответствии с Указом Президента РФ от 23.08.2000 № 1563 «О неотложных мерах социальной поддержки специалистов ядерного оружейного комплекса РФ»", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "18.2", Name = "О назначении дополнительного ежемесячного материального обеспечения как вдове инвалида ВОВ, ДМО гражданам за выдающиеся достижения и особые заслуги перед РФ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "18.3", Name = "Иные споры по дополнительному материальному обеспечению", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "18.4", Name = "О порядке установления дополнительного материального обеспечения отдельным категориям граждан", GroupClaim = groupClaim, Description = "" },
                    };
                case 19:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "19.1", Name = "По вопросам, связанным с назначением пенсий по инвалидности военнослужащим, проходившим военную службу по призыву в качестве солдат, матросов, сержантов и старшин, и пенсии по СПК членам их семей", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "19.2", Name = "Об исчислении стажа государственной службы и среднемесячного заработка, из которого исчисляется размер пенсии федеральных государственных служащих", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "19.3", Name = "Об установлении пенсии по государственному обеспечению граждан из числа космонавтов и членам их семей", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "19.4", Name = "Иные споры, возникающие при реализации ФЗ № 166 - ФЗ", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "19.5", Name = "По вопросам, связанным с назначением и выплатой социальной пенсии", GroupClaim = groupClaim, Description = "" },
                    };
                case 20:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "20", Name = "Споры, возникающие в рамках исполнительного производства", GroupClaim = groupClaim, Description = "" },
                    };
                case 21:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "21", Name = "Об обжаловании действий(бездействий), актов, решений территориальных органов ПФР по назначению, пересмотру  размера, выплате и доставке федеральной социальной доплаты к пенсии", GroupClaim = groupClaim, Description = "" },
                    };
                case 22:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "22", Name = "Споры, связанные с выплатой накопительной  пенсии", GroupClaim = groupClaim, Description = "" },
                    };
                case 23:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "23", Name = "Споры, связанные с софинансированием средств пенсионных накоплений", GroupClaim = groupClaim, Description = "" },
                    };
                case 24:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "24", Name = "Споры, возникающие по вопросам применения законодательства РФ о противодействии коррупции", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "24.1", Name = "О привлечении к административной ответственности по ст.19.7 КоАП РФ(непредставление, представление в неполном объеме  или в искаженном виде сведений о доходах, об имуществе и обязательствах имущественного характера)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "24.2", Name = "О привлечении к административной ответственности по ст. 19.29 КоАП РФ(незаконное привлечение к трудовой деятельности либо к выполнению работ или оказанию услуг государственного или муниципального служащего либо бывшего государственного или муниципального служащего)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "24.3", Name = "О привлечении к административной ответственности по ст. 17.7 КоАП РФ(невыполнение законных требований прокурора, следователя, дознавателя или должностного лица, осуществляющего производство по делу об административном правонарушении)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "24.4", Name = "О расторжении трудового договора в связи с нарушением территориальным органом ПФР при трудоустройстве требований Федерального закона от 25.12.2008 № 273 - ФЗ О противодействии коррупции", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "24.5", Name = "О возложении обязанности устранить нарушения законодательства о противодействии коррупции, признании незаконным заключения трудового договора и расторжении трудового договора", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "24.6", Name = "О признании незаконными действий работников ПФР, осуществляющих предпринимательскую деятельность", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "24.7", Name = "Об обязании представить работодателю сведения о доходах об имуществе, обязательствах имущественного характера, а также  о доходах, об  имуществе и обязательствах имущественного характера своей супруги и несовершеннолетнего ребенка(территориальный орган ПФР -3 - е лицо)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "24.8", Name = "Иные споры, связанные с применением законодательства РФ о противодействии коррупции", GroupClaim = groupClaim, Description = "" },
                    };
                case 25:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "25", Name = "Иные споры по которым территориальные органы ПФР привлечены в качестве третьих лиц", GroupClaim = groupClaim, Description = "" },
                    };
                case 26:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "26.1", Name = "О нарушении срока регистрации запроса заявителя о предоставлении государственной услуги(п. 1 ст. 11.1 ФЗ № 210 - ФЗ)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "26.2", Name = "О нарушении срока предоставления государственной услуги(п. 2 ст. 11.1 ФЗ № 210 - ФЗ)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "26.3", Name = "О требовании у заявителя документов, не предусмотренных нормативными правовыми актами Российской Федерации, для предоставления государственной услуги(п. 3 ст. 11.1 ФЗ № 210 - ФЗ)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "26.4", Name = "Об отказе в приеме документов, предоставление которых предусмотрено нормативными правовыми актами Российской Федерации, для предоставления государственной услуги, у заявителя(п. 4 ст. 11.1 ФЗ № 210 - ФЗ)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "26.5", Name = "Об отказе в предоставлении государственной услуги, если основания отказа не предусмотрены федеральными законами   и принятыми в соответствии с ними иными нормативными правовыми актами Российской Федерации(п. 5 ст. 11.1 ФЗ № 210 - ФЗ)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "26.6", Name = "О затребовании с заявителя при предоставлении государственной услуги платы, не предусмотренной нормативными правовыми  актами Российской Федерации(п. 6 ст. 11.1 ФЗ № 210 - ФЗ)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "26.7", Name = "Об отказе в исправлении допущенных опечаток и ошибок в выданных в результате предоставления государственной услуги документах либо нарушение установленного срока таких исправлений(п. 7 ст. 11.1 ФЗ № 210 - ФЗ)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "26.8", Name = "Иные споры, связанные с применением законодательства РФ о предоставлении государственных услуг", GroupClaim = groupClaim, Description = "" },
                    };
                case 27:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "27", Name = "Иные", GroupClaim = groupClaim, Description = "" },
                    };
                case 28:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "28", Name = "О взыскании средств пенсионных накоплений(накопительной  пенсии) правопреемниками умерших  застрахованных лиц(восстановление срока для обращение за выплатой СПН)", GroupClaim = groupClaim, Description = "" },
                    };
                case 29:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "29", Name = "Об установлении юридических фактов(территориальные органы ПФР -заинтересованные лица)", GroupClaim = groupClaim, Description = "" },
                    };
                case 31:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "31", Name = "Оплата госпошлины(арбитраж)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "31.1", Name = "Оплата услуг представителя(арбитраж)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "31.2", Name = "Иные судебные издержки(арбитраж)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "31.3", Name = "Оплата госпошлины(общая юрисдикция)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "31.4", Name = "Оплата услуг представителя(общая юрисдикция)", GroupClaim = groupClaim, Description = "" },
                        new SubjectClaim { Code = "31.5", Name = "Иные судебные издержки(общая юрисдикция)", GroupClaim = groupClaim, Description = "" }
                    };
                default:
                    return null;
            }
        }
    }
}
