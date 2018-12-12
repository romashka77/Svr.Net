using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Svr.Core.Entities
{
    /// <summary>
    /// Базовая сущность c наименованием
    /// </summary>
    public abstract class BaseEntityName : BaseEntity
    {
        /// <summary>
        /// Наименование
        /// </summary>
        [Required(ErrorMessage = ErrorStringEmpty)]
        [MaxLength(100, ErrorMessage = ErrorStringMaxLength)]
        [Display(Name = "Наименование", Prompt = "Введите наименование")]
        public string Name { get; set; }
        public override string ToString() => "Базовая сущность c наименованием";
    }
}
