namespace Meshwark.Models
{
    public class EmailSettings
    {

        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpHost { get; set; } = "mail.meshwark.com";
        public int SmtpPort { get; set; } = 8889;
        public bool EnableSsl { get; set; } = false;
        public int TimeoutSeconds { get; set; } = 60;
    }
}

