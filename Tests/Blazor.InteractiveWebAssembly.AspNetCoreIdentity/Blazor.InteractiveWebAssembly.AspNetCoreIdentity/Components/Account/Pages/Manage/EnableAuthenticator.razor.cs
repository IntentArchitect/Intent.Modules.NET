using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Blazor.InteractiveWebAssembly.AspNetCoreIdentity.Data;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveWebAssembly.AspNetCoreIdentity.Components.Account.Pages.Manage
{
    public partial class EnableAuthenticator
    {
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        private string? message;
        private ApplicationUser user = default!;
        private string? sharedKey;
        private string? authenticatorUri;
        private IEnumerable<string>? recoveryCodes;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            user = await UserAccessor.GetRequiredUserAsync(HttpContext);

            await LoadSharedKeyAndQrCodeUriAsync(user);
        }

        private async Task OnValidSubmitAsync()
        {
            // Strip spaces and hyphens
            var verificationCode = Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await UserManager.VerifyTwoFactorTokenAsync(
                user, UserManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                message = "Error: Verification code is invalid.";
                return;
            }

            await UserManager.SetTwoFactorEnabledAsync(user, true);
            var userId = await UserManager.GetUserIdAsync(user);
            Logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", userId);

            message = "Your authenticator app has been verified.";

            if (await UserManager.CountRecoveryCodesAsync(user) == 0)
            {
                recoveryCodes = await UserManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            }
            else
            {
                RedirectManager.RedirectToWithStatus("Account/Manage/TwoFactorAuthentication", message, HttpContext);
            }
        }

        private async ValueTask LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user)
        {
            // Load the authenticator key & QR code URI to display on the form
            var unformattedKey = await UserManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await UserManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await UserManager.GetAuthenticatorKeyAsync(user);
            }

            sharedKey = FormatKey(unformattedKey!);

            var email = await UserManager.GetEmailAsync(user);
            authenticatorUri = GenerateQrCodeUri(email!, unformattedKey!);
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.AsSpan(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                AuthenticatorUriFormat,
                UrlEncoder.Encode("Microsoft.AspNetCore.Identity.UI"),
                UrlEncoder.Encode(email),
                unformattedKey);
        }

        private sealed class InputModel
        {
            [Required]
            [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Verification Code")]
            public string Code { get; set; } = "";
        }
    }
}
