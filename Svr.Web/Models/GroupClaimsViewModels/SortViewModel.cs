using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Models.GroupClaimsViewModels
{
    public class SortViewModel
    {
        public SortState NameSort { get; private set; } // значение для сортировки по имени
        public SortState CodeSort { get; private set; }    // значение для сортировки по коду
        public SortState CategoryDisputeSort { get; private set; }    // значение для сортировки по категории диспутов
        public SortState Current { get; private set; }     // текущее значение сортировки
        public bool Up { get; set; }  // Сортировка по возрастанию или убыванию

        public SortViewModel(SortState sortOrder)
        {
            // значения по умолчанию
            NameSort = SortState.NameAsc;
            CodeSort = SortState.CodeAsc;
            CategoryDisputeSort = SortState.CategoryDisputeAsc;
            Up = true;

            if (sortOrder == SortState.CodeDesc || sortOrder == SortState.NameDesc || sortOrder == SortState.CategoryDisputeDesc)
            {
                Up = false;
            }
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    Current = NameSort = SortState.NameAsc;
                    break;
                case SortState.CodeAsc:
                    Current = CodeSort = SortState.CodeDesc;
                    break;
                case SortState.CodeDesc:
                    Current = CodeSort = SortState.CodeAsc;
                    break;
                case SortState.CategoryDisputeAsc:
                    Current = CategoryDisputeSort = SortState.CategoryDisputeDesc;
                    break;
                case SortState.CategoryDisputeDesc:
                    Current = CategoryDisputeSort = SortState.CategoryDisputeAsc;
                    break;
                default:
                    Current = NameSort = SortState.NameDesc;
                    break;
            }
            NameSort = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            CodeSort = sortOrder == SortState.CodeAsc ? SortState.CodeDesc : SortState.CodeAsc;
            CategoryDisputeSort = sortOrder == SortState.CategoryDisputeAsc ? SortState.CategoryDisputeDesc : SortState.CategoryDisputeAsc;
            Current = sortOrder;
        }
    }
}