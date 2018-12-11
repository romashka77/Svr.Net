using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    /// <summary>
    /// Регион (область)
    /// </summary>
    public class Region : BaseEntityCode
    {
        /// <summary>
        /// Колекция районов
        /// </summary>
        [Display(Name = "Районы")]
        public virtual ICollection<District> Districts { get; set; }
        public Region() { Districts = new List<District>(); }
        public override string ToString() => "Регион";
    }
}
