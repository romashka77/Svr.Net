using System;
using System.Collections.Generic;
using System.Text;

namespace Svr.Core.Entities
{
    /// <summary>
    /// Исполнитель
    /// </summary>
    public class Performer: BaseEntityName
    {
        /// <summary>
        /// Колекция районов
        /// </summary>
        public virtual ICollection<DistrictPerformer> DistrictPerformers { get; set; } 
        public Performer()
        {
            DistrictPerformers = new List<DistrictPerformer>();
        }

        public override string ToString() => "Исполнитель";
    }
}
