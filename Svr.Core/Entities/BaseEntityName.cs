using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    /// <summary>
    /// Базовая сущность c наименованием
    /// </summary>
    public abstract class BaseEntityName : BaseEntity
    {
        private string name;
        /// <summary>
        /// Наименование
        /// </summary>
        [Required(ErrorMessage = ErrorStringEmpty)]
        //[MaxLength(250, ErrorMessage = ErrorStringMaxLength)]
        [Display(Name = "Наименование", Prompt = "Введите наименование")]
        [MaxLength(100, ErrorMessage = ErrorStringMaxLength)]
        public string Name { get { return name; } set { name = value.Substring(0, 99); } }
        public override string ToString() => "Базовая сущность c наименованием";
    }
}
