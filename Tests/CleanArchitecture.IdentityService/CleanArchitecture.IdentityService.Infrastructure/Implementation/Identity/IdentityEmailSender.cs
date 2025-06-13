using System.Net;
using System.Net.Mail;
using CleanArchitecture.IdentityService.Domain.Entities;
using CleanArchitecture.IdentityService.Infrastructure.Options;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IdentityService.IdentityEmailSender", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Infrastructure.Implementation.Identity
{
    public class IdentityEmailSender : CleanArchitecture.IdentityService.Application.Interfaces.IIdentityEmailSender
    {
        private readonly EmailSenderOptions _options;
        private readonly ILogger<IdentityEmailSender> _logger;

        public IdentityEmailSender(IOptions<EmailSenderOptions> options, ILogger<IdentityEmailSender> logger)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task SendConfirmationLinkAsync(ApplicationIdentityUser user, string email, string confirmationLink)
        {
            string subject = "Confirm your email";
            string body = $"Hello {user.UserName},\n\nPlease confirm your email by clicking this link:\n{confirmationLink}";
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPasswordResetCodeAsync(ApplicationIdentityUser user, string email, string resetCode)
        {
            string subject = "Reset your password - Code";
            string body = $"Hello {user.UserName},\n\nUse this code to reset your password:\n{resetCode}";
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPasswordResetLinkAsync(ApplicationIdentityUser user, string email, string resetLink)
        {
            string subject = "Reset your password";
            string body = $"Hello {user.UserName},\n\nClick this link to reset your password:\n{resetLink}";
            await SendEmailAsync(email, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using var client = new SmtpClient(_options.SmtpHost, _options.SmtpPort)
            {
                Credentials = new NetworkCredential(_options.Username, _options.Password),
                EnableSsl = _options.UseSsl
            };

            var message = new MailMessage
            {
                From = new MailAddress(_options.SenderEmail, _options.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            message.To.Add(toEmail);

            try
            {
                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent to {Email} with subject '{Subject}'", toEmail, subject);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                throw;
            }
        }
    }
}