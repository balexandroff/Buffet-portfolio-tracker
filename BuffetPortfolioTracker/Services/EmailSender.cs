using BuffetPortfolioTracker.Interfaces;
using BuffetPortfolioTracker.Utils;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Mail;

namespace BuffetPortfolioTracker.Services
{
    public class EmailSender : IEmailSender
    {
        public Configuration _configuration { get; set; }
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<Configuration> configuration, ILogger<EmailSender> logger)
        {
            _configuration = configuration.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(
            string email,
            string subject,
            string message)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(_configuration.MailerSenderEmail);
                    mail.To.Add(_configuration.MailerReceiverEmail);
                    mail.Subject = subject;
                    mail.Body = message;
                    mail.IsBodyHtml = true;
                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential(_configuration.MailerSenderEmail, _configuration.MailerSenderPassword);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to send email");
            }
        }
    }
}
