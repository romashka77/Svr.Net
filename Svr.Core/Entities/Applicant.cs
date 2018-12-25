using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Svr.Core.Entities
{
    public class Applicant: BaseEntityDescription
    {
        /// <summary>
        /// Тип контрагента
        /// </summary>
        [Required(ErrorMessage = ErrorStringEmpty)]
        public long TypeApplicantID { get; set; }
        [Display(Name = "Тип контрагента")]
        public virtual Dir Dir { get; set; }

        [Display(Name = "Полное наименование", Prompt = "Введите полное наименование")]
        string FullName { get; set; }
        [Display(Name = "Краткое наименование", Prompt = "Введите краткое наименование")]
        string ShortName { get; set; }
        public override string ToString() => "Сторона процесса";

    }
}
