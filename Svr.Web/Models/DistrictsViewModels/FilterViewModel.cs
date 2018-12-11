using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Svr.Core.Entities;

namespace Svr.Web.Models.DistrictsViewModels
{
    public class FilterViewModel
    {
        #region конструктор
        public FilterViewModel(List<Region> regions, long? region, string name)
        {
            // устанавливаем начальный элемент, который позволит выбрать всех
            regions.Insert(0, new Region { Name = "Все", Id = 0 });
            Regions = new SelectList(regions, "Id", "Name", region);
            SelectedRegion = region;
            SelectedName = name;
        }
        #endregion
        public SelectList Regions { get; private set; } // список регионов
        public long? SelectedRegion { get; private set; }   // выбранный регион
        public string SelectedName { get; private set; }    // введенное имя района
    }
}
