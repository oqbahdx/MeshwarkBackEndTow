using Meshwark.Service;
using Microsoft.AspNetCore.Mvc;

public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailController> _logger;

    public EmailController(IEmailService emailService, ILogger<EmailController> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// Sends a modern designed email notifying user about account review status.
    /// </summary>
    /// <param name="toEmail">Recipient email address.</param>
    /// <returns>Success or failure message.</returns>
    [HttpPost("send-review-notification")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendAccountReviewEmail([FromBody] string toEmail)
    {
        try
        {
            if (string.IsNullOrEmpty(toEmail))
                return BadRequest(new { message = "Invalid email address" });

            const string subject = "حسابك قيد المراجعة - مشوارك";
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo.png");
            var body = CreateAccountReviewTemplate("Meshwark Logo", logoPath);

            _logger.LogInformation($"Sending account review notification to {toEmail}");
            await _emailService.SendEmailAsync(toEmail, subject, body, logoPath);

            return Ok(new { message = "Review notification sent successfully!" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending account review notification");
            return StatusCode(500, new
            {
                message = "Email sending failed",
                details = ex.Message,
                innerException = ex.InnerException?.Message
            });
        }
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private string CreateAccountReviewTemplate(string logoAltText, string logoPath)
    {
        var logoHtml = !string.IsNullOrEmpty(logoPath)
            ? $@"<img src='cid:companyLogo' alt='{logoAltText}' style='max-height: 80px; margin-bottom: 20px;'/>"
            : "";

        return $@"
<!DOCTYPE html>
<html lang='ar' dir='rtl'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>حسابك قيد المراجعة - مشوارك</title>
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Cairo:wght@400;600;700&display=swap');
        
        body, html {{
            margin: 0;
            padding: 0;
            font-family: 'Cairo', sans-serif;
            background-color: #f7f9fc;
            color: #2d3748;
            text-align: right;
            direction: rtl;
        }}
        .email-container {{
            max-width: 600px;
            margin: 40px auto;
            background: #ffffff;
            border-radius: 16px;
            overflow: hidden;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
        }}
        .email-header {{
            background: linear-gradient(135deg, #2c5282, #4299e1);
            padding: 30px;
            text-align: center;
        }}
        .email-header img {{
            max-height: 80px;
            margin-bottom: 20px;
        }}
        .status-badge {{
            background: rgba(255, 255, 255, 0.1);
            color: #ffffff;
            padding: 8px 16px;
            border-radius: 50px;
            font-size: 14px;
            display: inline-block;
            margin-top: 10px;
        }}
        .email-body {{
            padding: 40px;
            text-align: right;
        }}
        .email-body h1 {{
            font-size: 28px;
            color: #2d3748;
            margin-bottom: 24px;
            font-weight: 700;
        }}
        .email-body p {{
            font-size: 16px;
            line-height: 1.8;
            margin: 16px 0;
            color: #4a5568;
        }}
        .review-steps {{
            background: #f8fafc;
            border-radius: 12px;
            padding: 24px;
            margin: 24px 0;
        }}
        .review-steps h2 {{
            color: #2c5282;
            font-size: 20px;
            margin-bottom: 16px;
        }}
        .review-steps ul {{
            list-style: none;
            padding: 0;
            margin: 0;
        }}
        .review-steps li {{
            margin: 12px 0;
            padding-right: 24px;
            position: relative;
        }}
        .review-steps li:before {{
            content: '•';
            color: #4299e1;
            font-weight: bold;
            position: absolute;
            right: 0;
        }}
        .support-section {{
            margin-top: 32px;
            padding-top: 24px;
            border-top: 1px solid #e2e8f0;
        }}
        .contact-button {{
            display: inline-block;
            background: #4299e1;
            color: white;
            padding: 12px 24px;
            border-radius: 8px;
            text-decoration: none;
            margin-top: 16px;
            font-weight: 600;
        }}
        .email-footer {{
            background: #f8fafc;
            padding: 24px;
            text-align: center;
            font-size: 14px;
            color: #718096;
            border-top: 1px solid #e2e8f0;
        }}
        @media only screen and (max-width: 480px) {{
            .email-container {{
                margin: 0;
                border-radius: 0;
            }}
            .email-body {{
                padding: 24px;
            }}
            .review-steps {{
                padding: 16px;
            }}
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='email-header'>
            {logoHtml}
            <div class='status-badge'>قيد المراجعة</div>
        </div>
        <div class='email-body'>
            <h1>شكراً لتسجيلك في مشوارك</h1>
            <p>
                نود إعلامك بأن حسابك حالياً قيد المراجعة من قبل فريقنا المختص. نحن نقوم بهذه الخطوة لضمان جودة الخدمة وأمان جميع مستخدمينا.
            </p>
            
            <div class='review-steps'>
                <h2>ماذا يحدث الآن؟</h2>
                <ul>
                    <li>يقوم فريقنا بمراجعة المعلومات المقدمة</li>
                    <li>سيتم التحقق من صحة الوثائق المرفقة</li>
                    <li>ستصلك رسالة تأكيد فور اكتمال المراجعة</li>
                    <li>عادةً ما تستغرق العملية 24-48 ساعة عمل</li>
                </ul>
            </div>

            <div class='support-section'>
                <p>هل لديك أي استفسارات؟ فريق الدعم جاهز للمساعدة</p>
                <a href='mailto:support@meshwark.com' class='contact-button'>تواصل مع الدعم</a>
            </div>
        </div>
        <div class='email-footer'>
            <p>© {DateTime.Now.Year} مشوارك. جميع الحقوق محفوظة.</p>
            <p>هذه رسالة آلية، يرجى عدم الرد عليها</p>
        </div>
    </div>
</body>
</html>";
    }

    /// <summary>
    /// Sends approval notification for driver accounts with safety tips and guidelines.
    /// </summary>
    [HttpPost("send-driver-approval")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendDriverApprovalEmail([FromBody] string toEmail)
    {
        try
        {
            if (string.IsNullOrEmpty(toEmail) || !IsValidEmail(toEmail))
                return BadRequest(new { message = "Invalid email address" });

            const string subject = "تم تفعيل حسابك كسائق! - مشوارك";
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo.png");
            var body = CreateDriverApprovalTemplate("Meshwark Logo", logoPath);

            _logger.LogInformation($"Sending driver approval notification to {toEmail}");
            await _emailService.SendEmailAsync(toEmail, subject, body, logoPath);

            return Ok(new { message = "Driver approval notification sent successfully!" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending driver approval notification");
            return StatusCode(500, new { message = "Email sending failed", details = ex.Message });
        }
    }

 

    private string CreateDriverApprovalTemplate(string logoAltText, string logoPath)
    {
        var logoHtml = !string.IsNullOrEmpty(logoPath)
            ? $@"<img src='cid:companyLogo' alt='{logoAltText}' style='max-height: 80px; margin-bottom: 20px;'/>"
            : "";

        return $@"
<!DOCTYPE html>
<html lang='ar' dir='rtl'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>تم تفعيل حسابك كسائق! - مشوارك</title>
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Cairo:wght@400;600;700&display=swap');
        
        body, html {{
            margin: 0;
            padding: 0;
            font-family: 'Cairo', sans-serif;
            background-color: #f7f9fc;
            color: #2d3748;
            text-align: right;
            direction: rtl;
        }}
        .email-container {{
            max-width: 600px;
            margin: 40px auto;
            background: #ffffff;
            border-radius: 16px;
            overflow: hidden;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
        }}
        .email-header {{
            background: linear-gradient(135deg, #1e40af, #3b82f6);
            padding: 30px;
            text-align: center;
            color: white;
        }}
        .email-header img {{
            max-height: 80px;
            margin-bottom: 20px;
        }}
        .status-badge {{
            background: rgba(255, 255, 255, 0.2);
            color: #ffffff;
            padding: 8px 16px;
            border-radius: 50px;
            font-size: 14px;
            display: inline-block;
            margin-top: 10px;
        }}
        .email-body {{
            padding: 40px;
            text-align: right;
        }}
        .email-body h1 {{
            font-size: 28px;
            color: #1e40af;
            margin-bottom: 24px;
            font-weight: 700;
        }}
        .email-body p {{
            font-size: 16px;
            line-height: 1.8;
            margin: 16px 0;
            color: #4a5568;
        }}
        .commission-notice {{
            background: #eff6ff;
            border-right: 4px solid #3b82f6;
            padding: 20px;
            margin: 24px 0;
            border-radius: 8px;
        }}
        .tips-section {{
            background: #f8fafc;
            border-radius: 12px;
            padding: 24px;
            margin: 24px 0;
        }}
        .tips-section h2 {{
            color: #1e40af;
            font-size: 20px;
            margin-bottom: 16px;
        }}
        .safety-tips {{
            display: grid;
            grid-template-columns: repeat(2, 1fr);
            gap: 20px;
            margin: 20px 0;
        }}
        .tip-card {{
            background: white;
            border: 1px solid #e2e8f0;
            border-radius: 8px;
            padding: 16px;
        }}
        .tip-card h3 {{
            color: #1e40af;
            font-size: 16px;
            margin-bottom: 8px;
            display: flex;
            align-items: center;
        }}
        .tip-card h3::before {{
            content: '🔔';
            margin-left: 8px;
        }}
        .earning-info {{
            background: #f0f9ff;
            border-radius: 12px;
            padding: 24px;
            margin: 24px 0;
        }}
        .getting-started {{
            background: #f0f7ff;
            border-radius: 12px;
            padding: 24px;
            margin: 24px 0;
        }}
        .getting-started ol {{
            padding-right: 20px;
        }}
        .getting-started li {{
            margin: 12px 0;
            color: #4a5568;
        }}
        .support-section {{
            margin-top: 32px;
            text-align: center;
        }}
        .cta-button {{
            display: inline-block;
            background: #3b82f6;
            color: white;
            padding: 14px 28px;
            border-radius: 8px;
            text-decoration: none;
            font-weight: 600;
        }}
        .email-footer {{
            background: #f8fafc;
            padding: 24px;
            text-align: center;
            font-size: 14px;
            color: #718096;
            border-top: 1px solid #e2e8f0;
        }}
        @media only screen and (max-width: 480px) {{
            .email-container {{
                margin: 0;
                border-radius: 0;
            }}
            .email-body {{
                padding: 24px;
            }}
            .safety-tips {{
                grid-template-columns: 1fr;
            }}
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='email-header'>
            {logoHtml}
            <div class='status-badge'>تم تفعيل حساب السائق ✓</div>
        </div>
        <div class='email-body'>
            <h1>مرحباً بك في عائلة مشوارك للسائقين! 🚗</h1>
            <p>
                تهانينا! تم تفعيل حسابك كسائق في تطبيق مشوارك. نحن سعداء بانضمامك إلى فريقنا.
            </p>

            <div class='commission-notice'>
                <h3>معلومات العمولة 💰</h3>
                <p>عمولة الشركة 12% من قيمة كل رحلة. هذه العمولة تشمل:</p>
                <ul>
                    <li>خدمات التطبيق المتكاملة</li>
                    <li>الدعم الفني على مدار الساعة</li>
                    <li>التأمين الأساسي للرحلات</li>
                    <li>خدمات العملاء</li>
                </ul>
            </div>

            <div class='tips-section'>
                <h2>نصائح السلامة المهمة 🛡️</h2>
                <div class='safety-tips'>
                    <div class='tip-card'>
                        <h3>فحص المركبة</h3>
                        <p>تأكد من سلامة الإطارات والفرامل وجميع أنظمة السيارة قبل بدء العمل</p>
                    </div>
                    <div class='tip-card'>
                        <h3>الوقود</h3>
                        <p>احرص على أن يكون خزان الوقود ممتلئاً دائماً لتجنب أي تأخير</p>
                    </div>
                    <div class='tip-card'>
                        <h3>النظافة</h3>
                        <p>حافظ على نظافة سيارتك من الداخل والخارج لتقديم خدمة مميزة</p>
                    </div>
                    <div class='tip-card'>
                        <h3>القيادة الآمنة</h3>
                        <p>التزم بقواعد المرور وحدود السرعة لسلامتك وسلامة الركاب</p>
                    </div>
                </div>
            </div>

            <div class='getting-started'>
                <h2>كيف تبدأ العمل؟ 🚀</h2>
                <ol>
                    <li>افتح تطبيق السائق</li>
                    <li>اضغط على زر 'متاح للعمل'</li>
                    <li>انتظر طلبات الركاب في منطقتك</li>
                    <li>اقبل الرحلة واتبع توجيهات التطبيق</li>
                </ol>
            </div>

            <div class='earning-info'>
                <h2>معلومات مهمة عن الأرباح 📊</h2>
                <ul>
                    <li>يتم تحويل الأرباح أسبوعياً إلى حسابك البنكي</li>
                    <li>يمكنك متابعة أرباحك اليومية من خلال التطبيق</li>
                    <li>توجد مكافآت إضافية للسائقين المتميزين</li>
                    <li>فرص لزيادة الدخل خلال ساعات الذروة</li>
                </ul>
            </div>

            <div class='support-section'>
                <p>نحن هنا لمساعدتك 24/7</p>
                <p>رقم الدعم الفني: XXXX-XXX-XXX</p>
                <a href='https://meshwark.com/driver/start' class='cta-button'>ابدأ العمل الآن</a>
            </div>
        </div>
        <div class='email-footer'>
            <p>© {DateTime.Now.Year} مشوارك. جميع الحقوق محفوظة.</p>
            <p>للتواصل مع الدعم الفني: support@meshwark.com</p>
        </div>
    </div>
</body>
</html>";
    }

    private string CreateRiderWelcomeTemplate(string logoAltText, string logoPath)
    {
        var logoHtml = !string.IsNullOrEmpty(logoPath)
            ? $@"<img src='cid:companyLogo' alt='{logoAltText}' style='max-height: 80px; margin-bottom: 20px;'/>"
            : "";

        return $@"
<!DOCTYPE html>
<html lang='ar' dir='rtl'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>أهلاً بك في مشوارك! - رحلتك تبدأ الآن</title>
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Cairo:wght@400;600;700&display=swap');
        
        body, html {{
            margin: 0;
            padding: 0;
            font-family: 'Cairo', sans-serif;
            background-color: #f7f9fc;
            color: #2d3748;
            text-align: right;
            direction: rtl;
        }}
        .email-container {{
            max-width: 600px;
            margin: 40px auto;
            background: #ffffff;
            border-radius: 16px;
            overflow: hidden;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
        }}
        .email-header {{
            background: linear-gradient(135deg, #1e40af, #3b82f6);
            padding: 30px;
            text-align: center;
            color: white;
        }}
        .email-body {{
            padding: 40px;
            text-align: right;
        }}
        .email-body h1 {{
            font-size: 28px;
            color: #1e40af;
            margin-bottom: 24px;
            font-weight: 700;
        }}
        .app-features {{
            background: #f0f7ff;
            border-radius: 12px;
            padding: 24px;
            margin: 24px 0;
        }}
        .feature-list {{
            display: grid;
            grid-template-columns: repeat(2, 1fr);
            gap: 20px;
        }}
        .feature-card {{
            background: white;
            border: 1px solid #e2e8f0;
            border-radius: 8px;
            padding: 16px;
        }}
        .feature-card h3 {{
            color: #1e40af;
            margin-bottom: 8px;
            display: flex;
            align-items: center;
        }}
        .cta-button {{
            display: inline-block;
            background: #3b82f6;
            color: white;
            padding: 14px 28px;
            border-radius: 8px;
            text-decoration: none;
            font-weight: 600;
            margin-top: 20px;
        }}
        .email-footer {{
            background: #f8fafc;
            padding: 24px;
            text-align: center;
            font-size: 14px;
            color: #718096;
            border-top: 1px solid #e2e8f0;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='email-header'>
            {logoHtml}
        </div>
        <div class='email-body'>
            <h1>مرحباً بك في مشوارك! 🚗</h1>
            <p>
                أهلاً بك في عائلة مشوارك. استعد لرحلات سلسة وآمنة في أي وقت وأي مكان.
            </p>

            <div class='app-features'>
                <h2>مميزات التطبيق 🌟</h2>
                <div class='feature-list'>
                    <div class='feature-card'>
                        <h3>حجز سريع</h3>
                        <p>احجز رحلتك بضغطة زر</p>
                    </div>
                    <div class='feature-card'>
                        <h3>تتبع الرحلة</h3>
                        <p>تابع موقع سائقك لحظياً</p>
                    </div>
                    <div class='feature-card'>
                        <h3>دفع آمن</h3>
                        <p>خيارات دفع متعددة ومؤمنة</p>
                    </div>
                    <div class='feature-card'>
                        <h3>دعم 24/7</h3>
                        <p>مساعدة فورية عند الحاجة</p>
                    </div>
                </div>
            </div>

            <div class='support-section'>
                <p>نحن هنا لمساعدتك دائماً</p>
                <a href='https://meshwark.com/ride' class='cta-button'>ابدأ رحلتك الآن</a>
            </div>
        </div>
        <div class='email-footer'>
            <p>© {DateTime.Now.Year} مشوارك. جميع الحقوق محفوظة.</p>
            <p>للتواصل: support@meshwark.com</p>
        </div>
    </div>
</body>
</html>";
    }

    // Corresponding method to send rider welcome email
    [HttpPost("send-rider-welcome")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendRiderWelcomeEmail([FromBody] string toEmail)
    {
        try
        {
            if (string.IsNullOrEmpty(toEmail) || !IsValidEmail(toEmail))
                return BadRequest(new { message = "Invalid email address" });

            const string subject = "أهلاً بك في مشوارك! - رحلتك تبدأ الآن";
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo.png");
            var body = CreateRiderWelcomeTemplate("Meshwark Logo", logoPath);

            _logger.LogInformation($"Sending rider welcome notification to {toEmail}");
            await _emailService.SendEmailAsync(toEmail, subject, body, logoPath);

            return Ok(new { message = "Rider welcome notification sent successfully!" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending rider welcome notification");
            return StatusCode(500, new { message = "Email sending failed", details = ex.Message });
        }
    }
}

