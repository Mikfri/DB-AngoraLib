using DB_AngoraLib.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.EmailService
{
    public class EmailServices : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailServices(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlMessage)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailSettings.FromEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart("html") { Text = htmlMessage };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, true);
            await smtp.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }


}
