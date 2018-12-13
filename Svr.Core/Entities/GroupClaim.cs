using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Svr.Core.Entities
{
    /// <summary>
    /// Группа исков
    /// </summary>
    public class GroupClaim : BaseEntityCode
    {
        /// <summary>
        /// Категория споров
        /// </summary>
        [Required(ErrorMessage = ErrorStringEmpty)]
        public long CategoryDisputeId { get; set; }
        [Display(Name = "Категория споров")]
        //[ForeignKey("CategoryDisputeId")]
        public virtual CategoryDispute CategoryDispute { get; set; }  //навигационное свойство

        /// <summary>
        /// Предметы исков
        /// </summary>
        [Display(Name = "Предметы исков")]
        public virtual ICollection<SubjectClaim> SubjectClaims { get; set; }

        public GroupClaim() { SubjectClaims = new List<SubjectClaim>(); }
        public override string ToString() => "Группа исков";
    }
}
