using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Svr.Core.Entities
{
    public class BaseFinance : BaseEntity
    {
        /// <summary>
        /// Сумма
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Sum { get; set; }
        /// <summary>
        /// Дата
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        [Display(Name = "Описание")]
        public string Description { get; set; }
    }
}
