using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Svr.Core.Entities
{
    public class DirName: BaseEntityName
    {
        [Display(Name = "Справочник")]
        public virtual ICollection<Dir> Dirs { get; set; }

        public DirName() { Dirs = new List<Dir>(); }
        public override string ToString() => "Названия справочников";
    }
}
