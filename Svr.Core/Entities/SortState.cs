﻿
namespace Svr.Core.Entities
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
        LordAsc,    // по владельцу по возрастанию
        LordDesc,    // по владельцу по убыванию
        OwnerAsc,    // по владельцу по возрастанию
        OwnerDesc    // по владельцу по убыванию

    }
}
