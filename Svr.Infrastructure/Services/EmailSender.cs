//using MailKit.Net.Smtp;
//using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Svr.Core.Interfaces;
using System.Net.Mail;
using System.Net;

namespace Svr.Infrastructure.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("s079.079.pfr.ru")
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("079-0824", "1234")
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress("079-0824@079.pfr.ru")
            };
            mailMessage.To.Add(email);
            mailMessage.Subject = subject;
            mailMessage.Body = message;
            return client.SendMailAsync(mailMessage);
        }
    }
}
