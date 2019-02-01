using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Models.ClaimsViewModels
{
    public class CreateViewModel: BaseEntity
    {
        [MaxLength(50, ErrorMessage = ErrorStringMaxLength)]
        [Display(Name = "№ дела", Prompt = "Введите № дела")]
        [Required(ErrorMessage = ErrorStringEmpty)]
        public string Name { get; set; }
        [Display(Name = "Описание", Prompt = "Введите описание")]
        public string Description { get; set; }
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
