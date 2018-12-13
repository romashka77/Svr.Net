using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Svr.Core.Entities
{
    /// <summary>
    /// Предмет иска
    /// </summary>
    public class SubjectClaim : BaseEntityCode
    {
        /// <summary>
        /// Группа исков
        /// </summary>
        [Required(ErrorMessage = ErrorStringEmpty)]
        public long GroupClaimId { get; set; }
        [Display(Name = "Группа исков")]
        [ForeignKey("GroupClaimId")]
        public virtual GroupClaim GroupClaim { get; set; }  //навигационное свойство

        public override string ToString() => "Предмет иска";
    }
}
