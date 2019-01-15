using System;
using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    /// <summary>
    /// Заседание
    /// </summary>
    public class Meeting : BaseEntityDescription
    {
        public long ClaimId { get; set; }
        [Display(Name = "Иск")]
        public virtual Claim Claim { get; set; }
        [Display(Name = "Номер")]
        public byte Number { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Дата")]
        public DateTime? Date { get; set; }
        [DataType(DataType.Time)]
        [Display(Name = "Время")]
        public DateTime? Time { get; set; }
        public override string ToString() => "Заседание";
    }
}
