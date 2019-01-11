using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Svr.Core.Entities
{
    public class Instance:BaseEntityDescription
    {
        public long ClaimId { get; set; }
        [Display(Name = "Иск")]
        public virtual Claim Claim { get; set; }

        [Display(Name = "Дата передачи")]
        [DataType(DataType.Date)]
        public DateTime? DateTransfer { get; set; }

        [Display(Name = "Решение суда")]
        public string CourtDecision { get; set; }

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
        
    }
}
