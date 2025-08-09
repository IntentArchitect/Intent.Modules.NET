using System.Security.Claims;
using System.Text.Json;
using Blazor.InteractiveServer.AspNetCoreIdentity.Components.Account.Pages;
using Blazor.InteractiveServer.AspNetCoreIdentity.Components.Account.Pages.Manage;
using Blazor.InteractiveServer.AspNetCoreIdentity.Data;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.IdentityComponentsEndpointRouteBuilderExtensionsTemplate", Version = "1.0")]

namespace Blazor.InteractiveServer.AspNetCoreIdentity.Components.Account
{
    internal static class IdentityComponentsEndpointRouteBuilderExtensions
    {
        public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
        {
            ArgumentNullException.ThrowIfNull(endpoints);
            var accountGroup = endpoints.MapGroup("/Account");
            accountGroup.MapPost("/PerformExternalLogin", (
                                        HttpContext context,
                                        [FromServices] SignInManager<ApplicationUser> signInManager,
                                        [FromForm] string provider,
                                        [FromForm] string returnUrl) =>
                                    {
                                        IEnumerable<KeyValuePair<string, StringValues>> query = [
                                            new("ReturnUrl", returnUrl),
                                            new("Action", ExternalLogin.LoginCallbackAction)];

                                        var redirectUrl = UriHelper.BuildRelative(
                                            context.Request.PathBase,
                                            "/Account/ExternalLogin",
                                            QueryString.Create(query));

                                        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
                                        return TypedResults.Challenge(properties, [provider]);
                                    });
            accountGroup.MapPost("/Logout", async (
                                        ClaimsPrincipal user,
                                        SignInManager<ApplicationUser> signInManager,
                                        [FromForm] string returnUrl) =>
                                        {
                                            await signInManager.SignOutAsync();
                                            return TypedResults.LocalRedirect($"~/{returnUrl}");
                                        });
            accountGroup.MapGet("/Logout", async (SignInManager<ApplicationUser> signInManager, string? returnUrl) =>
                                    {
                                        await signInManager.SignOutAsync();
                                        return Results.Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
                                    });
            var manageGroup = accountGroup.MapGroup("/Manage").RequireAuthorization();
            manageGroup.MapPost("/LinkExternalLogin", async (
                                        HttpContext context,
                                        [FromServices] SignInManager<ApplicationUser> signInManager,
                                        [FromForm] string provider) =>
                                        {
                                            // Clear the existing external cookie to ensure a clean login process
                                            await context.SignOutAsync(IdentityConstants.ExternalScheme);

                                            var redirectUrl = UriHelper.BuildRelative(
                                                context.Request.PathBase,
                                                "/Account/Manage/ExternalLogins",
                                                QueryString.Create("Action", ExternalLogins.LinkLoginCallbackAction));

                                            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, signInManager.UserManager.GetUserId(context.User));
                                            return TypedResults.Challenge(properties, [provider]);
                                        });
            var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var downloadLogger = loggerFactory.CreateLogger("DownloadPersonalData");
            manageGroup.MapPost("/DownloadPersonalData", async (
                                        HttpContext context,
                                        [FromServices] UserManager<ApplicationUser> userManager,
                                        [FromServices] AuthenticationStateProvider authenticationStateProvider) =>
                                        {
                                            var user = await userManager.GetUserAsync(context.User);
                                            if (user is null)
                                            {
                                                return Results.NotFound($"Unable to load user with ID '{userManager.GetUserId(context.User)}'.");
                                            }

                                            var userId = await userManager.GetUserIdAsync(user);
                                            downloadLogger.LogInformation("User with ID '{UserId}' asked for their personal data.", userId);

                                            // Only include personal data for download
                                            var personalData = new Dictionary<string, string>();
                                            var personalDataProps = typeof(ApplicationUser).GetProperties().Where(
                                                prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
                                            foreach (var p in personalDataProps)
                                            {
                                                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
                                            }

                                            var logins = await userManager.GetLoginsAsync(user);
                                            foreach (var l in logins)
                                            {
                                                personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
                                            }

                                            personalData.Add("Authenticator Key", (await userManager.GetAuthenticatorKeyAsync(user))!);
                                            var fileBytes = JsonSerializer.SerializeToUtf8Bytes(personalData);

                                            context.Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");
                                            return TypedResults.File(fileBytes, contentType: "application/json", fileDownloadName: "PersonalData.json");
                                        });
            return accountGroup;
        }
    }
}