namespace CA.Ticketing.Common.Setup
{
    public class EmailSettings
    {
        public string SenderEmail { get; set; }

        public string SenderDisplayName { get; set; }

        public string SmtpUsername { get; set; }

        public string SmtpPassword { get; set; }

        public string SmtpUrl { get; set; }

        public int SmtpPort { get; set; }

        public bool UseSSL { get; set; }
    }
}
