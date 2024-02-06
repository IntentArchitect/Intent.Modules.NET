using System;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Identity.AccountController.AccountEmailSenderInterface", Version = "1.0")]

namespace Application.Identity.AccountController.UserIdentity.Application.Account;

public interface IAccountEmailSender
{
    Task SendEmailConfirmationRequest(string email, string userId, string code);
    Task SendPasswordResetCode(string email, string userId, string resetCode);
}
