using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Svr.Core.Entities;

namespace Svr.Web.Models.GroupClaimsViewModels
{
    public class FilterViewModel
    {
        #region конструктор
        public FilterViewModel(List<CategoryDispute> categoryDisputes, long? categoryDispute, string name)
        {
            // устанавливаем начальный элемент, который позволит выбрать всех
            categoryDisputes.Insert(0, new CategoryDispute { Name = "Все", Id = 0 });
            CategoryDisputes = new SelectList(categoryDisputes, "Id", "Name", categoryDispute);
            SelectedCategoryDispute = categoryDispute;
            SelectedName = name;
        }
        #endregion
        public SelectList CategoryDisputes { get; private set; } // список категорий диспута
        public long? SelectedCategoryDispute { get; private set; }   // выбранная регион категория диспута
        public string SelectedName { get; private set; }    // введенное имя группы исков
    }
}
