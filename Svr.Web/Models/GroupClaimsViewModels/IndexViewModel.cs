using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Models.GroupClaimsViewModels
{
    public class IndexViewModel : StatusMessageViewModel
    {
        public IEnumerable<ItemViewModel> ItemViewModels { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
    }
}
