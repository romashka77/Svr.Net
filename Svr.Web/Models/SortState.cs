using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Models
{
    public enum SortState
    {
        NameAsc,    // по имени по возрастанию
        NameDesc,   // по имени по убыванию
        CodeAsc,    // по коду по возрастанию
        CodeDesc,    // по коду по убыванию
        RegionAsc,    // по региону по возрастанию
        RegionDesc    // по региону по убыванию
    }
}
