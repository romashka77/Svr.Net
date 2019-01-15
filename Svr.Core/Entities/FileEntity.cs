using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{

    public class FileEntity : BaseEntityDescription
    {
        public long ClaimId { get; set; }
        [Display(Name = "Иск")]
        public virtual Claim Claim { get; set; }

        [Display(Name = "Путь к файлу")]
        public string Path { get; set; }

        public override string ToString() => "Файл";
    }
}
