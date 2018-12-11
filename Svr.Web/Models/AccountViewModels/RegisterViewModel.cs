using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        const string str = "Пожалуйста, заполните поле: {0}";
        [Required(ErrorMessage = str)]
        [EmailAddress(ErrorMessage = "Пожалуйста, проверте {0}")]
        [Display(Name = "E-mail", Description = "Email Адресс", Prompt = "Введите E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = str)]
        [StringLength(100)]
        [Display(Name = "Имя", Prompt = "Введите имя пользователя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = str)]
        [StringLength(100)]
        [Display(Name = "Фамилия", Prompt = "Введите фамилию пользователя")]
        public string LastName { get; set; }

        [StringLength(100)]
        [Display(Name = "Отчество", Prompt = "Введите отчество пользователя")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = str)]
        [DataType(DataType.Date)]
        [Display(Name = "Дата рождения")]
        public DateTime DateofBirth { get; set; }

        [Required(ErrorMessage = str)]
        [StringLength(100, ErrorMessage = "Длина {0} должна быть не менее {2} и не более {1} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль", Prompt = "Введите пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля", Prompt = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }


    }
}
