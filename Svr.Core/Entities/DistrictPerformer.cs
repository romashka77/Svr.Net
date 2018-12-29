using System;
using System.Collections.Generic;
using System.Text;

namespace Svr.Core.Entities
{
    /// <summary>
    /// связь сногие ко многим
    /// </summary>
    public class DistrictPerformer
    {
        public int PerformerId { get; set; }
        public virtual Performer Performer { get; set; }

        public long DistrictId { get; set; }
        public virtual District District { get; set; }

    }
}
