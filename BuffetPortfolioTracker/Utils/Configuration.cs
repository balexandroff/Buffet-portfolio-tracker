namespace BuffetPortfolioTracker.Utils
{
    public class Configuration
    {
        public const string AppSettings = "AppSettings";
        public string PortfolioUrl { get; set; }
        public string JsonStoragePath { get; set; }
        public string FileName { get; set; }
        public string FileNamePerDate { get; set; }
        public string MailerSenderEmail { get; set; }
        public string MailerSenderPassword { get; set; }
        public string MailerSenderName { get; set; }
        public string MailerReceiverEmail { get; set; }
    }
}
