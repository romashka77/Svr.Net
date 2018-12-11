//using MailKit.Net.Smtp;
//using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Svr.Core.Interfaces;

namespace Svr.Infrastructure.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public /*async*/ Task SendEmailAsync(string email, string subject, string message)
        {
            //var emailMessage = new MimeMessage();
            //emailMessage.From.Add(new MailboxAddress("Администрация сайта Svr", "romashka_77@mail.ru"));
            //emailMessage.To.Add(new MailboxAddress("", email));
            //emailMessage.Subject = subject;
            //emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            //{
            //    Text = message
            //};
            //using (var client = new SmtpClient())
            //{
            //    //   await client.ConnectAsync("smtp.mail.ru", 2525, true);//465
            //    await client.ConnectAsync("s079.079.pfr.ru");
            //    await client.AuthenticateAsync("079-0824", "1234");
            //    await client.SendAsync(emailMessage);

            //    await client.DisconnectAsync(true);
            //}
            return Task.CompletedTask;
        }
    }
}
