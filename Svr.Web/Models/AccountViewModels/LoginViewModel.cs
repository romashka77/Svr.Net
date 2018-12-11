using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Models.AccountViewModels
{
    public class LoginViewModel
    {
        const string str = "Пожалуйста, заполните поле: {0}";
        [EmailAddress(ErrorMessage = "Пожалуйста, проверте {0}")]
        [Required(ErrorMessage = str)]
        [Display(Name = "E-mail", Description = "Email Адресс", Prompt = "Введите E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = str)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль", Prompt = "Введите пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }
    }
}
