using System;
using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    /// <summary>
    /// Базовый класс для сущностей
    /// </summary>
    public abstract class BaseEntity
    {
        protected const string ErrorStringEmpty ="Пожалуйста, заполните поле: {0}";
        protected const string ErrorStringMaxLength = "Максимальная длина поля: {0} не более {1} символов";
        /// <summary>
        /// Возвращает или задает идентификатор сущности
        /// </summary>
        [Key]
        public long Id { get; set; }
        /// <summary>
        /// Дата и время создания
        /// </summary>
        [Display(Name = "Дата создания")]
        public DateTime CreatedOnUtc { get; set; }
        /// <summary>
        /// Дата и время обновления
        /// </summary>
        [Display(Name = "Дата изменения")]
        public DateTime UpdatedOnUtc { get; set; }
        //[NotMapped]//чтобы не создавался столбец в таблице.
        public override string ToString()=> "Базовая сущность";
        // настройка каскадного удаления https://metanit.com/sharp/entityframeworkcore/3.2.php
    }
}
