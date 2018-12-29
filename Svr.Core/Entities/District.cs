using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    /// <summary>
    /// Район
    /// </summary>
    public class District : BaseEntityCode
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
        /// Колекция исполнителей
        /// </summary>
        [Display(Name = "Исполнители")]
        public virtual ICollection<DistrictPerformer> DistrictPerformers { get; set; }

        public District()
        {
            DistrictPerformers = new List<DistrictPerformer>();
        }
        public override string ToString() => "Район";
    }
}
