using Microsoft.AspNetCore.Mvc.Rendering;
using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Models
{
    public class FilterViewModel
    {
        #region конструктор
        public FilterViewModel(string searchString, string owner = null, IEnumerable<SelectListItem> owners = null, string lord = null, IEnumerable<SelectListItem> lords = null, DateTime? date = null)
        {
            // устанавливаем начальный элемент, который позволит выбрать всех
            //owners.Insert(new  { Name = "Все", Id = 0 });
            //Owners = owners.Select(a => new SelectListItem { Text=a.}); new SelectList(owners, "Id", "Name", owner);
            Lords = lords;
            Owners = owners;
            SelectedLord = lord;
            SelectedOwner = owner;
            SearchString = searchString;
            Date = date;
        }
        #endregion
        public IEnumerable<SelectListItem> Lords { get; private set; } // список владельцев владельцев
        public IEnumerable<SelectListItem> Owners { get; private set; } // список владельцев
        public string SelectedLord { get; set; }   // выбранный владелец владельцев
        public string SelectedOwner { get; set; }   // выбранный владелец
        [Display(Name = "Поиск:")] 
        public string SearchString { get; private set; }    // строка поиска
        [DataType(DataType.Date)]
        [Display(Name = "Дата")]
        public DateTime? Date { get; private set; }
    }
}
