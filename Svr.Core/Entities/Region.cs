using System.Collections.Generic;
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
        public virtual ICollection<District> Districts { get; set; } = new List<District>();

        /// <summary>
        /// Колекция исполнителей
        /// </summary>
        [Display(Name = "Исполнители")]
        public virtual ICollection<Performer> Performers { get; set; } = new List<Performer>();

        public override string ToString() => "Регион";
    }
}
