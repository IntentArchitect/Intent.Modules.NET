using Intent.Modules.Blazor.Authentication.FactoryExtensions;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Templates;
using System.Linq;

namespace Intent.Modules.Blazor.Authentication.DefaultContent
{
    /// <summary>
    /// Default (first-generation only) content for the RegisterConfirmation page, seeded onto the
    /// modelled <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old dedicated RegisterConfirmationTemplate/RegisterConfirmationCodeBehindTemplate pair.
    /// Resolves the concrete ASP.NET Core Identity user class via <see cref="IdentityHelperExtensions.GetIdentityUserClass"/>
    /// for the raw <c>UserManager&lt;T&gt;</c>/<c>IEmailSender&lt;T&gt;</c> injections this page needs
    /// (the old T4 <c>&lt;#= IdentityClass #&gt;</c> placeholder).
    /// </summary>
    internal static class RegisterConfirmationPageContent
    {
        public static string BuildRazorContent(RazorComponentTemplate template)
        {
            var identityUserClass = IdentityHelperExtensions.GetIdentityUserClass(template);
            var redirectManager = template.GetIdentityRedirectManagerTemplateName();
            var isMudBlazor = template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor");

            return isMudBlazor
                ? BuildMudBlazorContent(identityUserClass, redirectManager)
                : BuildBootstrapContent(identityUserClass, redirectManager);
        }

        private static string BuildMudBlazorContent(string identityUserClass, string redirectManager)
        {
            return $$"""
                @using System.Text
                @using Microsoft.AspNetCore.Identity
                @using Microsoft.AspNetCore.WebUtilities

                @inject UserManager<{{identityUserClass}}> UserManager
                @inject IEmailSender<{{identityUserClass}}> EmailSender
                @inject NavigationManager NavigationManager
                @inject {{redirectManager}} RedirectManager

                <MudPaper Class="pa-4 mb-4 ux-gradient-primary" Elevation="0">
                    <MudText Typo="Typo.h4" Class="text-white font-weight-bold mb-2">
                        <MudIcon Icon="@Icons.Material.Filled.MarkEmailUnread" Class="mr-2" />
                        Register confirmation
                    </MudText>
                    <MudText Typo="Typo.body1" Class="text-white opacity-90">
                        Confirm your email address to activate your account.
                    </MudText>
                </MudPaper>

                <MudCard Class="ux-fade-in-up auth-form-shell" Style="animation-delay: 0.1s" Outlined="true">
                    <MudCardContent>
                        <MudText Typo="Typo.h5" Class="mb-3">Confirmation status</MudText>
                        <StatusMessage Message="@statusMessage" />

                        @if (emailConfirmationLink is not null)
                        {
                            <MudText Typo="Typo.body1">
                                This app does not currently have a real email sender registered, see <a href="https://aka.ms/aspaccountconf">these docs</a> for how to configure a real email sender.
                                Normally this would be emailed: <MudLink Href="@emailConfirmationLink">Click here to confirm your account</MudLink>
                            </MudText>
                        }
                        else
                        {
                            <MudText Typo="Typo.body1">Please check your email to confirm your account.</MudText>
                        }
                    </MudCardContent>
                </MudCard>
                """;
        }

        private static string BuildBootstrapContent(string identityUserClass, string redirectManager)
        {
            return $$"""
                @using System.Text
                @using Microsoft.AspNetCore.Identity
                @using Microsoft.AspNetCore.WebUtilities

                @inject UserManager<{{identityUserClass}}> UserManager
                @inject IEmailSender<{{identityUserClass}}> EmailSender
                @inject NavigationManager NavigationManager
                @inject {{redirectManager}} RedirectManager

                <AccountHero Icon="mail-check"
                             Title="Check your email"
                             Subtitle="One more step to activate your account." />
                <div class="ux-form-narrow">
                    <section>
                        <StatusMessage Message="@statusMessage" />
                        @if (emailConfirmationLink is not null)
                        {
                            <p>
                                This app does not currently have a real email sender registered, see <a href="https://aka.ms/aspaccountconf">these docs</a> for how to configure a real email sender.
                                Normally this would be emailed: <a href="@emailConfirmationLink">Click here to confirm your account</a>
                            </p>
                        }
                        else
                        {
                            <p>Please check your email to confirm your account.</p>
                        }
                    </section>
                </div>
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
            code.AddField("string?", "emailConfirmationLink", c => c.Private());
            code.AddField("string?", "statusMessage", c => c.Private());

            code.AddProperty(code.Template.UseType("Microsoft.AspNetCore.Http.HttpContext"), "HttpContext", httpContext => httpContext.WithInitialValue("default!").AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.CascadingParameterAttribute").RemoveSuffix("Attribute")));

            code.AddProperty("string?", "Email", email => email.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromQueryAttribute").RemoveSuffix("Attribute")));
            code.AddProperty("string?", "ReturnUrl", returnUrl => returnUrl.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromQueryAttribute").RemoveSuffix("Attribute")));

            code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnInitializedAsync", onInitializedAsync =>
            {
                onInitializedAsync.Protected().Async().Override();

                onInitializedAsync.AddIfStatement("Email is null", @if =>
                {
                    @if.AddStatement("RedirectManager.RedirectTo(\"\");");
                });

                onInitializedAsync.AddStatement("var user = await UserManager.FindByEmailAsync(Email);");

                onInitializedAsync.AddIfStatement("user is null", @if =>
                {
                    @if.AddStatement($"HttpContext.Response.StatusCode = {code.Template.UseType("Microsoft.AspNetCore.Http.StatusCodes")}.Status404NotFound;");
                    @if.AddStatement("statusMessage = \"Error finding user for unspecified email\";");
                });
                onInitializedAsync.AddElseStatement(@else =>
                {
                    @else.AddIfStatement($"EmailSender is {code.Template.GetIdentityNoOpEmailSenderTemplateName()}", elseIf =>
                    {
                        elseIf.AddStatement("var userId = await UserManager.GetUserIdAsync(user);");
                        elseIf.AddStatement("var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);");
                        elseIf.AddStatement($"code = {code.Template.UseType("Microsoft.AspNetCore.WebUtilities.WebEncoders")}.Base64UrlEncode({code.Template.UseType("System.Text.Encoding")}.UTF8.GetBytes(code));");
                        elseIf.AddStatement($"emailConfirmationLink = NavigationManager.GetUriWithQueryParameters(NavigationManager.ToAbsoluteUri(\"Account/ConfirmEmail\").AbsoluteUri, new {code.Template.UseType("System.Collections.Generic.Dictionary<string, object?>")} {{ [\"userId\"] = userId, [\"code\"] = code, [\"returnUrl\"] = ReturnUrl }});");
                    });
                });
            });
        }
    }
}
