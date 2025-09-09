using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Identity;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.IdentityUserAccessorTemplate", Version = "1.0")]

namespace BlazorServerTests.Api.Components.Account
{
    internal sealed class IdentityUserAccessor
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IdentityRedirectManager _redirectManager;

        public IdentityUserAccessor(UserManager<IdentityUser> userManager, IdentityRedirectManager redirectManager)
        {
            _userManager = userManager;
            _redirectManager = redirectManager;
        }

        public async Task<IdentityUser> GetRequiredUserAsync(HttpContext context)
        {
            var user = await _userManager.GetUserAsync(context.User);

            if (user is null)
            {
                _redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{_userManager.GetUserId(context.User)}'.", context);
            }
            return user;
        }
    }
}