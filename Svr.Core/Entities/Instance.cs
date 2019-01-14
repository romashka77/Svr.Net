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

        [Display(Name = "Дата решения")]
        public DateTime? DateCourtDecision { get; set; }

        [Display(Name = "Дата получения решения")]
        public DateTime? DateInCourtDecision { get; set; }

        [Display(Name = "Сумма отказано")]
        public decimal? SumDenied { get; set; }

        [Display(Name = "Сумма удовлетворено")]
        public decimal? SumSatisfied { get; set; }

        [Display(Name = "Уплачено добровольно")]
        public decimal? PaidVoluntarily { get; set; }

        [Display(Name = "Гос.пошлина удов.")]
        public decimal? DutySatisfied { get; set; }

        [Display(Name = "Гос.пошлина отк.")]
        public decimal? DutyDenied { get; set; }

        [Display(Name = "Услуги пред.удов.")]
        public decimal? ServicesSatisfied { get; set; }

        [Display(Name = "Услуги пред.отк.")]
        public decimal? ServicesDenied { get; set; }

        [Display(Name = "Суд.издер.удов.")]
        public decimal? СostSatisfied { get; set; }

        [Display(Name = "Суд.издер.отк.")]
        public decimal? СostDenied { get; set; }

        [Display(Name = "Упл.гос.пошлина")]
        public decimal? DutyPaid { get; set; }

        //shortage-недостача
        [Display(Name = "Период с")]
        public DateTime? DateSShortage { get; set; }
        [Display(Name = "Период по")]
        public DateTime? DateToShortage { get; set; }
        [Display(Name = "Страх.часть ПФР")]
        public decimal? InsurancePartPFRShortage { get; set; }
        [Display(Name = "Накоп.часть ПФР")]
        public decimal? FundedPartPFRShortage { get; set; }
        [Display(Name = "В ФФОМС")]
        public decimal? FFOMSShortage { get; set; }
        [Display(Name = "В ТФОМС")]
        public decimal? TFOMSShortage { get; set; }

        //fine - пени
        [Display(Name = "Период с")]
        public DateTime? DateSFine { get; set; }
        [Display(Name = "Период по")]
        public DateTime? DateToFine { get; set; }
        [Display(Name = "Страх.часть ПФР")]
        public decimal? InsurancePartPFRFine { get; set; }
        [Display(Name = "Накоп.часть ПФР")]
        public decimal? FundedPartPFRFine { get; set; }
        [Display(Name = "В ФФОМС")]
        public decimal? FFOMSFine { get; set; }
        [Display(Name = "В ТФОМС")]
        public decimal? TFOMSFine { get; set; }

        //penalty - штраф
        [Display(Name = "Период с")]
        public DateTime? DateSPenalty { get; set; }
        [Display(Name = "Период по")]
        public DateTime? DateToPenalty { get; set; }
        [Display(Name = "Сумма штрафа")]
        public decimal? SumPenalty { get; set; }

        public override string ToString() => "Инстанция";
    }
}
