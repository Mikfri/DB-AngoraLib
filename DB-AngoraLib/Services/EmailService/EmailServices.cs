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
        private readonly Settings_Email _emailSettings;

        public EmailServices(IOptions<Settings_Email> emailSettings)
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
            //await smtp.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, true); // uden SSL
            await smtp.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls); // med SSL
            await smtp.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }


    //public class EmailServices : IEmailService    // Alternative implementation
    //{
    //    private readonly IConfiguration _configuration;

    //    public EmailService(IConfiguration configuration)
    //    {
    //        _configuration = configuration;
    //    }

    //    public async Task SendEmailAsync(string to, string subject, string htmlMessage)
    //    {
    //        var emailSettings = _configuration.GetSection("EmailSettings");
    //        var smtpHost = emailSettings["SmtpHost"];
    //        var smtpPort = Convert.ToInt32(emailSettings["SmtpPort"]);
    //        var fromEmail = emailSettings["FromEmail"];
    //        var smtpUser = emailSettings["SmtpUser"];
    //        var smtpPass = emailSettings["SmtpPass"];

    //        var email = new MimeMessage();
    //        email.From.Add(MailboxAddress.Parse(fromEmail));
    //        email.To.Add(MailboxAddress.Parse(to));
    //        email.Subject = subject;
    //        email.Body = new TextPart("html") { Text = htmlMessage };

    //        using var smtp = new SmtpClient();
    //        await smtp.ConnectAsync(smtpHost, smtpPort, true);
    //        await smtp.AuthenticateAsync(smtpUser, smtpPass);
    //        await smtp.SendAsync(email);
    //        await smtp.DisconnectAsync(true);
    //    }
    //}



}
