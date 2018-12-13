using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Svr.Core.Entities
{
    /// <summary>
    /// Базовая сущьность с наименованием, описанием и кодом
    /// </summary>
    public abstract class BaseEntityCode : BaseEntityDescription
    {
        /// <summary>
        /// Код 079
        /// </summary>
        [Required(ErrorMessage = ErrorStringEmpty)]
        //[MaxLength(10, ErrorMessage= ErrorStringMaxLength)]
        [Display(Name = "Код", Prompt = "Введите код")]
        public string Code { get; set; }
        public override string ToString() => "Базовая сущьность с наименованием, описанием и кодом";
    }
}
