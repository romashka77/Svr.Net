using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Svr.Core.Entities
{
    public abstract class BaseEntityDescription : BaseEntityName
    {
        /// <summary>
        /// Описание
        /// </summary>
        [Display(Name = "Описание", Prompt = "Введите описание")]
        public string Description { get; set; }
        public override string ToString() => "Базовая сущьность с наименованием и описанием";
    }
}
