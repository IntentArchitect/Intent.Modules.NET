using System.Threading.Tasks;
using Blazor.InteractiveServer.AspNetCoreIdentity.Data;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.IdentityUserAccessorTemplate", Version = "1.0")]

namespace Blazor.InteractiveServer.AspNetCoreIdentity.Components.Account
{
    internal sealed class IdentityUserAccessor
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IdentityRedirectManager _redirectManager;

        public IdentityUserAccessor(UserManager<ApplicationUser> userManager, IdentityRedirectManager redirectManager)
        {
            _userManager = userManager;
            _redirectManager = redirectManager;
        }

        public async Task<ApplicationUser> GetRequiredUserAsync(HttpContext context)
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