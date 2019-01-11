using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Svr.Core.Entities
{
    /// <summary>
    /// Иск
    /// </summary>
    public class Claim : BaseEntityCode
    {
        public long RegionId { get; set; }
        [Display(Name = "Регион")]
        public virtual Region Region { get; set; }

        public long DistrictId { get; set; }
        [Display(Name = "Район")]
        public virtual District District { get; set; }

        [Display(Name = "Дата регистрации")]
        [DataType(DataType.Date)]
        public DateTime? DateReg { get; set; }

        [Display(Name = "Дата принятия иска")]
        [DataType(DataType.Date)]
        public DateTime? DateIn { get; set; }

        public long? CategoryDisputeId { get; set; }
        [Display(Name = "Категория споров")]
        public virtual CategoryDispute CategoryDispute { get; set; }

        public long? GroupClaimId { get; set; }
        [Display(Name = "Группа исков")]
        public virtual GroupClaim GroupClaim { get; set; }

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
        public decimal? Sum { get; set; }

        public long? PlaintiffId { get; set; }
        [Display(Name = "Истец")]
        [ForeignKey("PlaintiffId")]
        public Applicant Plaintiff { get; set; }

        public long? RespondentId { get; set; }
        [Display(Name = "Ответчик")]
        [ForeignKey("RespondentId")]
        public Applicant Respondent { get; set; }

        public long? Person3rdId { get; set; }
        [Display(Name = "3-е лицо")]
        [ForeignKey("Person3rdId")]
        public Applicant Person3rd { get; set; }
        //-----------
        /// <summary>
        /// Колекция инстанций
        /// </summary>
        [Display(Name = "Инстанции")]
        public virtual ICollection<Instance> Instances { get; set; }
        
        public override string ToString() => "Иск";
        public Claim()
        {
            Instances = new List<Instance>();
        }
    }
}
