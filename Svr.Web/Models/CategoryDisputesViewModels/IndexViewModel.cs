using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Models.CategoryDisputesViewModels
{
    public class IndexViewModel : StatusMessageViewModel
    {
        public IEnumerable<ItemViewModel> CategoryDisputeItems { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
    }
}
