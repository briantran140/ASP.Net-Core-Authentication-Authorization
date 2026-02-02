using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using WebApp.Settings;

namespace WebApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSetting _smtpSetting;

        public EmailService(IOptions<SmtpSetting> smtpSetting)
        {
            this._smtpSetting = smtpSetting.Value;
        }

        public async Task SendAsync(string from, string to, string subject, string body)
        {
            var message = new MailMessage(from, to, subject, body);

            using (var emailClient = new SmtpClient(this._smtpSetting.Host, this._smtpSetting.Port))
            {
                emailClient.Credentials = new NetworkCredential(this._smtpSetting.User, this._smtpSetting.Password);

                await emailClient.SendMailAsync(message);
            }
        }
    }
}
