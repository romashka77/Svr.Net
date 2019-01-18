using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{

    public class FileEntity : BaseEntity
    {
        public long ClaimId { get; set; }
        [Display(Name = "Иск")]
        public virtual Claim Claim { get; set; }

        [Display(Name = "Наименование", Prompt = "Введите наименование")]
        [MaxLength(100, ErrorMessage = ErrorStringMaxLength)]
        public string Name { get; set; }

        [Display(Name = "Описание", Prompt = "Введите описание")]
        public string Description { get; set; }

        [Display(Name = "Имя файла")]
        public string Path { get; set; }

        public override string ToString() => "Файл";
    }
}
