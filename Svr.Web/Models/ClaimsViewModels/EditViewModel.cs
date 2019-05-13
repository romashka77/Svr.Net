using Svr.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Svr.Web.Models.ClaimsViewModels
{
    /// <summary>
    /// Иск
    /// </summary>
    public class EditViewModel : BaseEntity
    {
        [Display(Name = "Регион")]
        public long RegionId { get; set; }

        [Display(Name = "Район")]
        public long DistrictId { get; set; }

        [MaxLength(100, ErrorMessage = ErrorStringMaxLength)]
        [Display(Name = "Рег.номер", Prompt = "Введите регистрационный номер")]
        public string Code { get; set; }

        [MaxLength(50, ErrorMessage = ErrorStringMaxLength)]
        [Display(Name = "№ дела", Prompt = "Введите № дела")]
        [Required(ErrorMessage = ErrorStringEmpty)]
        public string Name { get; set; }

        [Display(Name = "Описание", Prompt = "Введите описание")]
        public string Description { get; set; }

        [Display(Name = "Дата регистрации")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = ErrorStringEmpty)]
        public DateTime DateReg { get; set; }

        [Display(Name = "Дата принятия иска")]
        [DataType(DataType.Date)]
        public DateTime? DateIn { get; set; }

        [Display(Name = "Категория споров")]
        [Required(ErrorMessage = ErrorStringEmpty)]
        public long? CategoryDisputeId { get; set; }
        
        [Display(Name = "Группа исков")]
        [Required(ErrorMessage = ErrorStringEmpty)]
        public long? GroupClaimId { get; set; }
        
        [Display(Name = "Предмет иска")]
        [Required(ErrorMessage = ErrorStringEmpty)]
        public long? SubjectClaimId { get; set; }
        
        public long? СourtId { get; set; }
        [Display(Name = "Суд")]
        public virtual Dir Сourt { get; set; }

        [Display(Name = "Исполнитель")]
        public long? PerformerId { get; set; }
        
        [Display(Name = "Сумма иска")]
        public decimal? Sum { get; set; }

        [Display(Name = "Истец")]
        public long? PlaintiffId { get; set; }

        [Display(Name = "Ответчик")]
        public long? RespondentId { get; set; }

        [Display(Name = "3-е лицо")]
        // ReSharper disable once InconsistentNaming
        public long? Person3rdId { get; set; }

        [Display(Name = "Дата вступления в законную силу")]
        [DataType(DataType.Date)]
        public DateTime? DateForce { get; set; }

        [Display(Name = "Итоговое решение суда")]
        public string FinalDecision { get; set; }

        public override string ToString() => "Иск";
        public string StatusMessage { get; set; }
    }
}
