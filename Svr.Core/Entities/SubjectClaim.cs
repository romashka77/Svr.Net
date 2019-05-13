using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    /// <inheritdoc />
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
        //[ForeignKey("GroupClaimId")]
        public virtual GroupClaim GroupClaim { get; set; }  //навигационное свойство

        //[Display(Name = "Предметы исков")]
        //public virtual ICollection<Claim> Claims { get; set; }

        //public SubjectClaim()
        //{
        //    Claims = new List<Claim>();
        //}
        public override string ToString() => "Предмет иска";
    }
}
