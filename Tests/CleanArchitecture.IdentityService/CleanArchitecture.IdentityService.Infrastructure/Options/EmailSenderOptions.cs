using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IdentityService.EmailSenderOptions", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Infrastructure.Options
{
    public class EmailSenderOptions
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }
    }
}