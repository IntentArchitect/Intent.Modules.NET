using System.Text;
using System.Text.Encodings.Web;
using BlazorNoMudBlazor.Api.Components.Account;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.AspNetCoreIdentityAuthServiceConcreteTemplate", Version = "1.0")]

namespace BlazorNoMudBlazor.Api.Common
{
    internal class AspNetCoreIdentityAuthServiceConcrete : IAuthService
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly NavigationManager _navigationManager;
        private readonly IdentityRedirectManager _redirectManager;
        private readonly IEmailSender<IdentityUser> _emailSender;

        public AspNetCoreIdentityAuthServiceConcrete(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            NavigationManager navigationManager,
            IdentityRedirectManager redirectManager,
            IEmailSender<IdentityUser> emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _navigationManager = navigationManager;
            _redirectManager = redirectManager;
            _emailSender = emailSender;
        }

        public async Task<string> ConfirmEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return $"Error loading user with ID {userId}";
            }
            else
            {
                var result = await _userManager.ConfirmEmailAsync(user, code);
                return result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";
            }
        }

        public async Task ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return;
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = _navigationManager.GetUriWithQueryParameters(_navigationManager.ToAbsoluteUri("Account/ResetPassword").AbsoluteUri, new Dictionary<string, object?> { ["code"] = code });
            await _emailSender.SendPasswordResetLinkAsync(user, email, HtmlEncoder.Default.Encode(callbackUrl));
        }

        public async Task Login(string username, string password, bool rememberMe, string returnUrl)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, rememberMe, false);

            if (result.Succeeded)
            {
                _redirectManager.RedirectTo(returnUrl);
            }
            else if (result.RequiresTwoFactor)
            {
                _redirectManager.RedirectTo("Account/LoginWith2fa", new() { ["returnUrl"] = returnUrl, ["rememberMe"] = rememberMe });
            }
            else if (result.IsLockedOut)
            {
                _redirectManager.RedirectTo("Account/Lockout");
            }
            else
            {
                throw new Exception("Error: Invalid login attempt.");
            }
        }

        public async Task Register(string email, string password, string returnUrl)
        {
            var user = CreateUser();
            user.Id = Guid.NewGuid().ToString();
            await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
            var emailStore = GetEmailStore();
            await emailStore.SetEmailAsync(user, email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                throw new Exception("Could not register user");
            }
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = _navigationManager.GetUriWithQueryParameters(_navigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri, new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code, ["returnUrl"] = returnUrl });
            await _emailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(callbackUrl));

            if (_userManager.Options.SignIn.RequireConfirmedAccount)
            {
                _redirectManager.RedirectTo("Account/RegisterConfirmation", new() { ["email"] = email, ["returnUrl"] = returnUrl });
            }
            else
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                _redirectManager.RedirectTo(returnUrl);
            }
        }

        public async Task ResendEmailConfirmation(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return;
            }
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = _navigationManager.GetUriWithQueryParameters(_navigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri, new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code });
            await _emailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(callbackUrl));
        }

        public async Task ResetPassword(string email, string code, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                _redirectManager.RedirectTo("Account/ResetPasswordConfirmation");
            }
            var result = await _userManager.ResetPasswordAsync(user, code, password);

            if (result.Succeeded)
            {
                _redirectManager.RedirectTo("Account/ResetPasswordConfirmation");
            }
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>(); ;
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " + $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor.");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}