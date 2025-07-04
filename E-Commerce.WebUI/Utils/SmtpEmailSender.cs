using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;

namespace E_Commerce.WebUI.Utils
{
    public class SmtpEmailSender : ICustomEmailSender
    {
        private readonly IConfiguration _config;

        public SmtpEmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var smtpClient = new SmtpClient
            {
                Host = _config["EmailSettings:Host"],
                Port = int.Parse(_config["EmailSettings:Port"]),
                EnableSsl = bool.Parse(_config["EmailSettings:EnableSsl"]),
                Credentials = new NetworkCredential(
                    _config["EmailSettings:Username"],
                    _config["EmailSettings:Password"]
                )
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["EmailSettings:Username"], _config["EmailSettings:DisplayName"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
