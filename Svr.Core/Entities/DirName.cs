using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    public class DirName: BaseEntityName
    {
        [Display(Name = "Справочник")]
        public virtual ICollection<Dir> Dirs { get; set; } = new List<Dir>();

        public override string ToString() => "Названия справочников";
    }
}
