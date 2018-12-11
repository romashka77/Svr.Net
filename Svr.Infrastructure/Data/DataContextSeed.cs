using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;

namespace Svr.Infrastructure.Data
{
    public static class DataContextSeed
    {
        private static Region region;
        public static async Task SeedAsync(DataContext dataContext/*, ILoggerFactory loggerFactory, int? retry = 0*/)
        {
            //int retryForAvailability = retry.Value;
            //try
            {
                if (!dataContext.Regions.Any())
                {
                    dataContext.Regions.AddRange(GetPreconfiguredRegions());
                    if (!dataContext.Districts.Any())
                    {
                        dataContext.Districts.AddRange(GetPreconfiguredDistricts());
                        await dataContext.SaveChangesAsync();
                        for (int j = 0; j < 50; j++)
                        {
                            region = new Region { Code = $"{j}", Name = $"Московская область {j}", Description = $"Каталог ОПФР по Московской области {j}" };
                            await dataContext.SaveChangesAsync();
                            for (int i = 0; i < 50; i++)
                            {
                                dataContext.Districts.Add(new District { Code = $"{i}", Name = $"Московский {i}", Description = $"ОПФР по Московскому району {i}", Region = region });
                                await dataContext.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
            // catch (Exception ex)
            {
                //if (retryForAvailability < 10)
                {
                    //retryForAvailability++;
                    //var log = loggerFactory.CreateLogger<DataContextSeed>();
                    //log.LogError(ex.Message);
                    //await SeedAsync(dataContext, loggerFactory, retryForAvailability);
                }
            }
        }

        private static IEnumerable<Region> GetPreconfiguredRegions()
        {
            region = new Region { Code = "079", Name = "Тамбовская область", Description = "Каталог ОПФР по Тамбовской области" };
            return new List<Region>() { region };
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

        //private static IEnumerable<Directory> GetPreconfiguredDirectories()
        //{
        //    return new List<Directory>()
        //    {
        //        new Directory { Name = "Принятые меры и формы погашения", Description="Описание справочника" },
        //                new Directory { Name = "Подразделения ПФР", Description="Описание справочника" },
        //                new Directory { Name = "Виды пенсий", Description="Описание справочника" },
        //                new Directory { Name = "Органы, осуществляющие выплату пенсии", Description="Описание справочника" },
        //                new Directory { Name = "Специалисты, осуществляющие приём", Description="Описание справочника" },
        //                new Directory { Name = "Счета и причины возникновения переплат", Description="Описание справочника"
        //                }
        //    };
        //}
    }
}
