using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Svr.Core.Entities
{
    /// <summary>
    /// Исполнитель
    /// </summary>
    public class Performer: BaseEntityDescription
    {
        /// <summary>
        /// Регион (область)
        /// </summary>
        [Required(ErrorMessage = ErrorStringEmpty)]
        public long RegionId { get; set; }
        //[ForeignKey("RegionId")]
        [Display(Name = "Регион")]
        public virtual Region Region { get; set; }

        /// <summary>
        /// Колекция районов
        /// </summary>
        [Display(Name = "Районы")]
        public virtual ICollection<DistrictPerformer> DistrictPerformers { get; set; } 
        public Performer()
        {
            DistrictPerformers = new List<DistrictPerformer>();
        }

        public override string ToString() => "Исполнитель";
    }
}
