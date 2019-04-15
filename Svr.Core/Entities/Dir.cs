using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    public class Dir : BaseEntityName
    {
        public long DirNameId { get; set; }
        [Display(Name = "Наименование справочника")]
        public virtual DirName DirName { get; set; }  // навигационное свойство

        [Display(Name = "Стороны процесса")]
        public virtual ICollection<Applicant> Applicants { get; set; } = new List<Applicant>();

        public override string ToString() => "Справочник";
    }
}
