using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Svr.Core.Entities
{
    /// <summary>
    /// Сторона процесса
    /// </summary>
    public class Applicant : BaseEntityDescription
    {
        /// <summary>
        /// Тип контрагента
        /// </summary>
        //[Required(ErrorMessage = ErrorStringEmpty)]
        [ForeignKey("TypeApplicant"), Column(Order = 0)]
        public long? TypeApplicantId { get; set; }
        [ForeignKey("Opf"), Column(Order = 1)]
        public long? OpfId { get; set; }

        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateTime? Born { get; set; }

        [Display(Name = "Полное наименование", Prompt = "Введите полное наименование")]
        public string FullName { get; set; }
        [Display(Name = "Адрес", Prompt = "Введите адрес")]
        public string Address { get; set; }

        [Display(Name = "Адрес банка", Prompt = "Введите адрес")]
        public string AddressBank { get; set; }

        [Display(Name = "ИНН", Prompt = "Введите ИНН")]
        public string Inn { get; set; }

        [Display(Name = "Тип контрагента")]
        public virtual Dir TypeApplicant { get; set; }
        [Display(Name = "ОПФ")]
        public virtual Dir Opf { get; set; }

        public override string ToString() => "Сторона процесса";
    }
}
