using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Models.RegionsViewModels
{
    public class FilterViewModel
    {
        #region конструктор
        public FilterViewModel(string searchString)
        {
            // устанавливаем начальный элемент, который позволит выбрать всех
            SearchString = searchString;
        }
        #endregion
        public string SearchString { get; private set; }    // строка поиска
    }
}
