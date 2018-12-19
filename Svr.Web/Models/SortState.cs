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
        DescriptionAsc,    // по возрастанию
        DescriptionDesc,    // по убыванию
        CreatedOnUtcAsc,
        CreatedOnUtcDesc,
        UpdatedOnUtcAsc,
        UpdatedOnUtcDesc,
        OwnerAsc,    // по владельцу по возрастанию
        OwnerDesc    // по владельцу по убыванию

    }
}
