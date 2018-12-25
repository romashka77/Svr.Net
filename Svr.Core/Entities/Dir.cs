using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Svr.Core.Entities
{
    public class Dir : BaseEntityName
    {
        public long DirNameId { get; set; }
        [Display(Name = "Наименование справочника")]
        public virtual DirName DirName { get; set; }  // навигационное свойство
        public override string ToString() => "Справочник";
    }
}
