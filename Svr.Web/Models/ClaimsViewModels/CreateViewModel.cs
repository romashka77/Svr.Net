using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Models.ClaimsViewModels
{
    public class CreateViewModel: BaseEntityDescription
    {
        [Display(Name = "Регион")]
        [Required(ErrorMessage = ErrorStringEmpty)]
        public long RegionId { get; set; }
        [Display(Name = "Район")]
        [Required(ErrorMessage = ErrorStringEmpty)]
        public long DistrictId { get; set; }
        [Display(Name = "Дата регистрации")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = ErrorStringEmpty)]
        public DateTime DateReg { get; set; }
        public override string ToString() => "Иск";
    }
}
