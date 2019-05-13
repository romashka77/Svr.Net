using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Svr.Core.Entities
{
    /// <inheritdoc />
    /// <summary>
    /// Иск
    /// </summary>
    public class Claim : BaseEntity
    {
        public long RegionId { get; set; }
        [Display(Name = "Регион")]
        public virtual Region Region { get; set; }

        public long DistrictId { get; set; }
        [Display(Name = "Район")]
        public virtual District District { get; set; }

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
        public long? CategoryDisputeId { get; set; }
        [Display(Name = "Категория споров")]
        public virtual CategoryDispute CategoryDispute { get; set; }

        [Display(Name = "Группа исков")]
        public long? GroupClaimId { get; set; }
        [Display(Name = "Группа исков")]
        public virtual GroupClaim GroupClaim { get; set; }

        [Display(Name = "Предмет иска")]
        public long? SubjectClaimId { get; set; }
        [Display(Name = "Предмет иска")]
        public virtual SubjectClaim SubjectClaim { get; set; }

        public long? СourtId { get; set; }
        [Display(Name = "Суд")]
        public virtual Dir Сourt { get; set; }

        public long? PerformerId { get; set; }
        [Display(Name = "Исполнитель")]
        public virtual Performer Performer { get; set; }

        [Display(Name = "Сумма иска")]
        [Column(TypeName = "money")]
        public decimal? Sum { get; set; }

        public long? PlaintiffId { get; set; }
        [Display(Name = "Истец")]
        [ForeignKey("PlaintiffId")]
        public Applicant Plaintiff { get; set; }

        public long? RespondentId { get; set; }
        [Display(Name = "Ответчик")]
        [ForeignKey("RespondentId")]
        public Applicant Respondent { get; set; }

        // ReSharper disable once InconsistentNaming
        public long? Person3rdId { get; set; }
        [Display(Name = "3-е лицо")]
        [ForeignKey("Person3rdId")]
        // ReSharper disable once InconsistentNaming
        public Applicant Person3rd { get; set; }

        [Display(Name = "Дата вступления в законную силу")]
        [DataType(DataType.Date)]
        public DateTime? DateForce { get; set; }

        [Display(Name = "Итоговое решение суда")]
        public string FinalDecision { get; set; }

        /// <summary>
        /// Коллекция инстанций
        /// </summary>
        [Display(Name = "Инстанции")]
        public virtual ICollection<Instance> Instances { get; set; } = new List<Instance>();

        /// <summary>
        /// Коллекция заседаний
        /// </summary>
        [Display(Name = "График заседаний")]
        public virtual ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();

        /// <summary>
        /// Коллекция файлов
        /// </summary>
        [Display(Name = "Документы по иску")]
        public virtual ICollection<FileEntity> FileEntities { get; set; } = new List<FileEntity>();

        public override string ToString() => "Иск";
    }
}
