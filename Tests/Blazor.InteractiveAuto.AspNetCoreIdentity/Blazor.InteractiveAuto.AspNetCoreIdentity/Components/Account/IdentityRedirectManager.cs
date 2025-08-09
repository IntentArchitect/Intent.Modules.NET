using System.Diagnostics.CodeAnalysis;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.IdentityRedirectManagerTemplate", Version = "1.0")]

namespace Blazor.InteractiveAuto.AspNetCoreIdentity.Components.Account
{
    internal sealed class IdentityRedirectManager
    {
        public const string StatusCookieName = "Identity.StatusMessage";
        private readonly NavigationManager _navigationManager;
        private static readonly CookieBuilder StatusCookieBuilder = new() { SameSite = SameSiteMode.Strict, HttpOnly = true, IsEssential = true, MaxAge = TimeSpan.FromSeconds(5) };

        public IdentityRedirectManager(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        [DoesNotReturn]
        public void RedirectTo(string? uri)
        {
            uri ??= "";

            if (!Uri.IsWellFormedUriString(uri, UriKind.Relative))
            {
                uri = _navigationManager.ToBaseRelativePath(uri);
            }
            _navigationManager.NavigateTo(uri);
            throw new InvalidOperationException($"{nameof(IdentityRedirectManager)} can only be used during static rendering.");
        }

        [DoesNotReturn]
        public void RedirectTo(string? uri, Dictionary<string, object?> queryParameters)
        {
            var uriWithoutQuery = _navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
            var newUri = _navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
            RedirectTo(newUri);
        }

        [DoesNotReturn]
        public void RedirectToWithStatus(string? uri, string message, HttpContext context)
        {
            context.Response.Cookies.Append(StatusCookieName, message, StatusCookieBuilder.Build(context));
            RedirectTo(uri);
        }

        [DoesNotReturn]
        public void RedirectToCurrentPage()
        {
            RedirectTo(_navigationManager.ToAbsoluteUri(_navigationManager.Uri).GetLeftPart(UriPartial.Path));
        }

        [DoesNotReturn]
        public void RedirectToCurrentPageWithStatus(string message, HttpContext context)
        {
            RedirectToWithStatus(_navigationManager.ToAbsoluteUri(_navigationManager.Uri).GetLeftPart(UriPartial.Path), message, context);
        }
    }
}