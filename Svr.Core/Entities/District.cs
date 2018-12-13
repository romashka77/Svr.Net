using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

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
        [ForeignKey("RegionId")]
        [Display(Name = "Регион")]
        public virtual Region Region { get; set; }  // навигационное свойство
        public override string ToString() => "Район";
        /// <summary>
        /// Люди из района
        /// </summary>
        //public virtual ICollection<Man> Men { get; set; }

    }
}
