using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Svr.Infrastructure.Identity
{
    //Добавление данных профиля для пользователей приложения путем добавления свойств в класс ApplicationUser

    //    В ASP.NET Core Identity пользователь представлен классом IdentityUser из пространства имен Microsoft.AspNetCore.Identity.EntityFrameworkCore.Этот класс предоставляет базовую информацию о пользователе с помощью следующих свойств:

    //Id: уникальный идентификатор пользователя
    //UserName: ник пользователя
    //Email: электронный адрес пользователя
    //Logins: коллекция логинов, которые использовались пользователем для входа через сторонние сервисы (Google, Facebook и т.д.)
    //Claims: коллекция клеймов или дополнительных объектов, которые используются для авторизации пользователя
    //PasswordHash: хеш пароля.В базе данных напрямую не хранится пароль, а только его хеш.
    //Roles: набор ролей, к которым принадлежит пользователь
    //PhoneNumber: номер телефона
    //SecurityStamp: некоторое специальное значение, которое меняется при смене аутентификационных данных, например, пароля
    //AccessFailedCount: количество неудачных входов пользователя в систему
    //EmailConfirmed: подтвержден ли адрес электронной почты
    //PhoneNumberConfirmed: подтвержден ли номер телефона
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }//Фамилия

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }//Имя

        [MaxLength(100)]
        public string MiddleName { get; set; }//Отчество
        [Required]
        public DateTime DateofBirth { get; set; }//Дата рождения
        /// <summary>
        /// Дата и время создания
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
        /// <summary>
        /// Дата и время обновления
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }

    }
}
