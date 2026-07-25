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
    /// Default (first-generation only) content for the Manage/Disable2fa page, seeded onto the
    /// modelled <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static content/Components(MudBlazor)/Account/Pages/Manage/Disable2fa pair.
    /// </summary>
    internal static class ManageDisable2faPageContent
    {
        public static string BuildRazorContent(RazorComponentTemplate template)
        {
            var identityClass = template.ExecutionContext.InstalledModules.Any(im => im.ModuleId == "Intent.AspNetCore.Identity") ?
                IdentityHelperExtensions.GetIdentityUserClass(template) :
                "ApplicationUser";
            var userAccessor = template.GetIdentityUserAccessorTemplateName();
            var redirectManager = template.GetIdentityRedirectManagerTemplateName();
            var isMudBlazor = template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor");

            return isMudBlazor
                ? BuildMudBlazorContent(identityClass, userAccessor, redirectManager)
                : BuildBootstrapContent(identityClass, userAccessor, redirectManager);
        }

        public static string? BuildStyleContent(RazorComponentTemplate template)
        {
            var isMudBlazor = template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor");
            return isMudBlazor ? MudBlazorStyle : null;
        }

        private const string MudBlazorStyle = """
            .auth-form-shell {
                max-width: 720px;
                box-shadow: var(--shadow-2);
                border-radius: var(--radius-xl);
            }
            """;

        private static string BuildMudBlazorContent(string identityClass, string userAccessor, string redirectManager)
        {
            return $$"""
                @using Microsoft.AspNetCore.Authorization
                @using Microsoft.AspNetCore.Identity
                @inject UserManager<{{identityClass}}> UserManager
                @inject {{userAccessor}} UserAccessor
                @inject {{redirectManager}} RedirectManager
                @inject ILogger<Disable2fa> Logger

                <MudPaper Class="pa-4 mb-4 ux-gradient-primary"
                    Elevation="0">
                    <MudText Typo="Typo.h4"
                        Class="text-white font-weight-bold mb-2">
                        <MudIcon Icon="@Icons.Material.Filled.GppBad"
                            Class="mr-2" />
                        Disable two-factor authentication
                    </MudText>
                    <MudText Typo="Typo.body1"
                        Class="text-white opacity-90">
                        Turn off 2FA for your account if you no longer wish to use it.
                    </MudText>
                </MudPaper>

                <StatusMessage />

                <MudCard Class="ux-fade-in-up auth-form-shell"
                    Style="animation-delay: 0.1s"
                    Outlined="true">
                    <MudCardContent>
                        <MudText Typo="Typo.h5"
                            Class="mb-3">
                            Disable two-factor authentication (2FA)
                        </MudText>

                        <MudAlert Severity="Severity.Warning"
                            Class="mb-4">
                            <MudText Typo="Typo.body1"><strong>This action only disables 2FA.</strong></MudText>
                            <MudText Typo="Typo.body1">Disabling 2FA does not change the keys used in authenticator apps. If you wish to change the key used in an authenticator app you should <MudLink Href="Account/Manage/ResetAuthenticator">reset your authenticator keys.</MudLink></MudText>
                        </MudAlert>

                        <form @formname="disable-2fa"
                            @onsubmit="OnSubmitAsync"
                            method="post">
                            <AntiforgeryToken />
                            <MudStack Row="true"
                                Justify="Justify.FlexEnd">
                                <MudButton ButtonType="ButtonType.Submit"
                                    Variant="Variant.Filled"
                                    Color="Color.Error"
                                    StartIcon="@Icons.Material.Filled.GppBad">
                                    Disable 2FA
                                </MudButton>
                            </MudStack>
                        </form>
                    </MudCardContent>
                </MudCard>

                """;
        }

        private static string BuildBootstrapContent(string identityClass, string userAccessor, string redirectManager)
        {
            return $$"""
                @using Microsoft.AspNetCore.Identity

                @inject UserManager<{{identityClass}}> UserManager
                @inject {{userAccessor}} UserAccessor
                @inject {{redirectManager}} RedirectManager
                @inject ILogger<Disable2fa> Logger

                <StatusMessage />
                <h3>Disable two-factor authentication (2FA)</h3>

                <div class="ux-callout ux-callout-warning">
                    <p><strong>This action only disables 2FA.</strong></p>
                    <p>
                        Disabling 2FA does not change the keys used in authenticator apps. If you wish to change the key
                        used in an authenticator app you should <a href="Account/Manage/ResetAuthenticator">reset your authenticator keys.</a>
                    </p>
                </div>

                <form @formname="disable-2fa"
                    @onsubmit="OnSubmitAsync"
                    method="post">
                    <AntiforgeryToken />
                    <button class="btn btn-danger"
                        type="submit">
                        <UxIcon Name="shield-off" /> Disable 2FA
                    </button>
                </form>
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
            var identityClass = code.Template.ExecutionContext.InstalledModules.Any(im => im.ModuleId == "Intent.AspNetCore.Identity") ?
                IdentityHelperExtensions.GetIdentityUserClass(code.Template) :
                "ApplicationUser";

            code.AddField(identityClass, "user", f => f.WithAssignment(new CSharpStatement("default!")));

            code.AddProperty(code.Template.UseType("Microsoft.AspNetCore.Http.HttpContext?"), "HttpContext", p => p.WithInitialValue("default!").AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.CascadingParameterAttribute").RemoveSuffix("Attribute")));

            // either get the existing method or add one
            ICSharpClassMethodDeclaration onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            if (onInitializedAsync is null)
            {
                code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnInitializedAsync");
                onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            }

            onInitializedAsync.Async().Protected().Override();
            onInitializedAsync.AddAssignmentStatement("user", new CSharpStatement("await UserAccessor.GetRequiredUserAsync(HttpContext);"));
            onInitializedAsync.AddIfStatement($"{code.Template.UseType("Microsoft.AspNetCore.Http.HttpMethods")}.IsGet(HttpContext.Request.Method) && !await UserManager.GetTwoFactorEnabledAsync(user)", @if =>
            {
                @if.AddStatement("throw new InvalidOperationException(\"Cannot disable 2FA for user as it's not currently enabled.\");");
            });

            code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnSubmitAsync", onSubmitAsync =>
            {
                onSubmitAsync.Private().Async();

                onSubmitAsync.AddAssignmentStatement("var disable2faResult", new CSharpStatement("await UserManager.SetTwoFactorEnabledAsync(user, false);"));
                onSubmitAsync.AddIfStatement("!disable2faResult.Succeeded", @if =>
                {
                    @if.AddStatement("throw new InvalidOperationException(\"Unexpected error occurred disabling 2FA.\");");
                });

                onSubmitAsync.AddAssignmentStatement("var userId", new CSharpStatement("await UserManager.GetUserIdAsync(user);"));
                onSubmitAsync.AddStatement("Logger.LogInformation(\"User with ID '{UserId}' has disabled 2fa.\", userId);");
                onSubmitAsync.AddStatement("RedirectManager.RedirectToWithStatus(\"Account/Manage/TwoFactorAuthentication\", \"2fa has been disabled. You can reenable 2fa when you setup an authenticator app\", HttpContext);");
            });
        }
    }
}
