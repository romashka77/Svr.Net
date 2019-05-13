using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Svr.Core.Entities
{
    public class Instance : BaseEntityDescription
    {
        [Display(Name = "Номер инстанции")]
        public byte Number { get; set; }

        public long ClaimId { get; set; }
        [Display(Name = "Иск")]
        public virtual Claim Claim { get; set; }

        [Display(Name = "Дата передачи")]
        [DataType(DataType.Date)]
        public DateTime? DateTransfer { get; set; }

        public long? CourtDecisionId { get; set; }
        [Display(Name = "Решение суда")]
        [ForeignKey("CourtDecisionId")]
        public Dir CourtDecision { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Дата решения")]
        public DateTime? DateCourtDecision { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Дата получения решения")]
        public DateTime? DateInCourtDecision { get; set; }

        [Display(Name = "Сумма отказано")]
        [Column(TypeName = "money")]
        public decimal? SumDenied { get; set; }

        [Display(Name = "Сумма удовлетворено")]
        [Column(TypeName = "money")]
        public decimal? SumSatisfied { get; set; }

        //[Display(Name = "Уплачено добровольно")]
        //[Column(TypeName = "money")]
        //public decimal? PaidVoluntarily { get; set; }

        [Display(Name = "Гос.пошлина удов.")]
        [Column(TypeName = "money")]
        public decimal? DutySatisfied { get; set; }

        [Display(Name = "Гос.пошлина отк.")]
        [Column(TypeName = "money")]
        public decimal? DutyDenied { get; set; }

        [Display(Name = "Услуги пред.удов.")]
        [Column(TypeName = "money")]
        public decimal? ServicesSatisfied { get; set; }

        [Display(Name = "Услуги пред.отк.")]
        [Column(TypeName = "money")]
        public decimal? ServicesDenied { get; set; }

        [Display(Name = "Суд.издер.удов.")]
        [Column(TypeName = "money")]
        // ReSharper disable once IdentifierTypo
        public decimal? СostSatisfied { get; set; }

        [Display(Name = "Суд.издер.отк.")]
        [Column(TypeName = "money")]
        // ReSharper disable once IdentifierTypo
        public decimal? СostDenied { get; set; }

        [Display(Name = "Упл.гос.пошлина")]
        [Column(TypeName = "money")]
        public decimal? DutyPaid { get; set; }

        public override string ToString() => "Инстанция";
    }
}
