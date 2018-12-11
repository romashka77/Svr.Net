using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Models.DistrictsViewModels
{
    public class IndexViewModel : EditViewModel
    {
        [Display(Name = "Дата создания")]
        public DateTime CreatedOnUtc { get; set; }
        [Display(Name = "Дата изменения")]
        public DateTime UpdatedOnUtc { get; set; }
    }
}
