using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using System.Text;
using Meshwark.Models;

namespace Meshwark.Service
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, string logoPath);
        Task SendWelcomeEmailAsync(string toEmail);
    }

    public class DriverEmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<DriverEmailService> _logger;

        public DriverEmailService(IOptions<EmailSettings> emailSettings, ILogger<DriverEmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, string logoPath)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SmtpUsername, "Meshwark - مشورك"),
                    Subject = subject,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
                {
                    var logoContentId = "companyLogo";
                    var linkedResource = new LinkedResource(logoPath)
                    {
                        ContentId = logoContentId,
                        TransferEncoding = System.Net.Mime.TransferEncoding.Base64,
                        ContentType = new System.Net.Mime.ContentType("image/png")
                    };

                    var alternateView = AlternateView.CreateAlternateViewFromString(body, Encoding.UTF8, "text/html");
                    alternateView.LinkedResources.Add(linkedResource);
                    mailMessage.AlternateViews.Add(alternateView);
                }
                else
                {
                    mailMessage.Body = body;
                }

                using var smtpClient = new SmtpClient
                {
                    Host = _emailSettings.SmtpHost,
                    Port = _emailSettings.SmtpPort,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = _emailSettings.EnableSsl,
                    Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword),
                    Timeout = _emailSettings.TimeoutSeconds * 1000
                };

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}");
                throw;
            }
        }

        public async Task SendWelcomeEmailAsync(string toEmail)
        {
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo.png");
            var subject = "مرحباً بك في مشورك - طلب التسجيل قيد المراجعة";
            var body = CreateArabicEmailTemplate(!string.IsNullOrEmpty(logoPath) ? "companyLogo" : null);

            await SendEmailAsync(toEmail, subject, body, logoPath);
        }

        private string CreateArabicEmailTemplate(string logoContentId)
        {
            var logoHtml = !string.IsNullOrEmpty(logoContentId)
                ? $@"<img src='cid:{logoContentId}' alt='Meshwark Logo' style='max-height: 60px; width: auto;'/>"
                : "";

            return $@"
<!DOCTYPE html>
<html lang='ar' dir='rtl'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{ font-family: 'Noto Sans Arabic', sans-serif; line-height: 1.6; color: #333; background: #f5f5f5; }}
        .email-container {{ max-width: 600px; margin: auto; background: white; padding: 20px; border-radius: 10px; }}
        .email-header {{ background: linear-gradient(135deg, #1a237e, #3949ab); color: white; text-align: center; padding: 15px; }}
        .email-content {{ margin-top: 20px; }}
        .footer {{ margin-top: 20px; text-align: center; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='email-header'>
            {logoHtml}
            <h1>مرحباً بك في مشورك</h1>
        </div>
        <div class='email-content'>
            <p>شكراً لتسجيلك. طلبك قيد المراجعة وسيتم التواصل معك قريباً.</p>
        </div>
        <div class='footer'>
            <p>© {DateTime.Now.Year} مشورك. جميع الحقوق محفوظة</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
