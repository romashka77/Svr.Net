using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Svr.Core.Entities
{
    /// <summary>
    /// Группа исков
    /// </summary>
    class GroupClaim: BaseEntityCode
    {
        /// <summary>
        /// Предметы исков
        /// </summary>
        [Display(Name = "Предметы исков")]
        public virtual ICollection<SubjectClaim> SubjectClaims { get; set; }

        public GroupClaim() { SubjectClaims = new List<SubjectClaim>(); }
        public override string ToString() => "Группа исков";
    }
}
