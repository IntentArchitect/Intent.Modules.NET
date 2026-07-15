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
    /// Default (first-generation only) content for the Manage/ResetAuthenticator page, seeded onto
    /// the modelled <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static content/Components(MudBlazor)/Account/Pages/Manage/ResetAuthenticator pair.
    /// </summary>
    internal static class ManageResetAuthenticatorPageContent
    {
        public static string BuildRazorContent(RazorComponentTemplate template)
        {
            var identityClass = IdentityHelperExtensions.GetIdentityUserClass(template);
            var userAccessor = template.GetIdentityUserAccessorTemplateName();
            var redirectManager = template.GetIdentityRedirectManagerTemplateName();
            var isMudBlazor = template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor");

            return isMudBlazor
                ? BuildMudBlazorContent(identityClass, userAccessor, redirectManager)
                : BuildBootstrapContent(identityClass, userAccessor, redirectManager);
        }

        private static string BuildMudBlazorContent(string identityClass, string userAccessor, string redirectManager)
        {
            return $$"""
                @using Microsoft.AspNetCore.Identity

                @inject UserManager<{{identityClass}}> UserManager
                @inject SignInManager<{{identityClass}}> SignInManager
                @inject {{userAccessor}} UserAccessor
                @inject {{redirectManager}} RedirectManager
                @inject ILogger<ResetAuthenticator> Logger

                <MudPaper Class="pa-4 mb-4 ux-gradient-primary" Elevation="0">
                    <MudText Typo="Typo.h4" Class="text-white font-weight-bold mb-2">
                        <MudIcon Icon="@Icons.Material.Filled.RestartAlt" Class="mr-2" />
                        Reset authenticator key
                    </MudText>
                    <MudText Typo="Typo.body1" Class="text-white opacity-90">
                        Reset the key used by your authenticator app and reconfigure your 2FA setup.
                    </MudText>
                </MudPaper>

                <StatusMessage />
                <MudCard Class="ux-fade-in-up auth-form-shell" Style="animation-delay: 0.1s" Outlined="true">
                    <MudCardContent>
                        <MudText Typo="Typo.h5" Class="mb-3">Reset authenticator key</MudText>
                        <MudAlert Severity="Severity.Warning" Class="mb-4">
                            <MudText Typo="Typo.body1"><strong>If you reset your authenticator key your authenticator app will not work until you reconfigure it.</strong></MudText>
                            <MudText Typo="Typo.body1">This process disables 2FA until you verify your authenticator app. If you do not complete your authenticator app configuration you may lose access to your account.</MudText>
                        </MudAlert>
                        <form @formname="reset-authenticator" @onsubmit="OnSubmitAsync" method="post">
                            <AntiforgeryToken />
                            <MudStack Row="true" Justify="Justify.FlexEnd">
                                <MudButton ButtonType="ButtonType.Submit" Variant="Variant.Filled" Color="Color.Error" StartIcon="@Icons.Material.Filled.RestartAlt">Reset authenticator key</MudButton>
                            </MudStack>
                        </form>
                    </MudCardContent>
                </MudCard>

                <style>
                    .auth-form-shell {
                        max-width: 720px;
                        box-shadow: var(--shadow-2);
                        border-radius: var(--radius-xl);
                    }
                </style>
                """;
        }

        private static string BuildBootstrapContent(string identityClass, string userAccessor, string redirectManager)
        {
            return $$"""
                @using Microsoft.AspNetCore.Identity

                @inject UserManager<{{identityClass}}> UserManager
                @inject SignInManager<{{identityClass}}> SignInManager
                @inject {{userAccessor}} UserAccessor
                @inject {{redirectManager}} RedirectManager
                @inject ILogger<ResetAuthenticator> Logger

                <StatusMessage />
                <h3>Reset authenticator key</h3>
                <div class="ux-callout ux-callout-warning" role="alert">
                    <span class="ux-callout-icon"><UxIcon Name="alert" /></span>
                    <div class="ux-callout-body">
                        <p><strong>If you reset your authenticator key your authenticator app will not work until you reconfigure it.</strong></p>
                        <p>
                            This process disables 2FA until you verify your authenticator app.
                            If you do not complete your authenticator app configuration you may lose access to your account.
                        </p>
                    </div>
                </div>

                <form @formname="reset-authenticator" @onsubmit="OnSubmitAsync" method="post">
                    <AntiforgeryToken />
                    <button class="btn btn-outline-danger" type="submit"><UxIcon Name="key" /> Reset authenticator key</button>
                </form>
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
            code.AddProperty(code.Template.UseType("Microsoft.AspNetCore.Http.HttpContext"), "HttpContext", p => p.WithInitialValue("default!").AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.CascadingParameterAttribute").RemoveSuffix("Attribute")));

            code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnSubmitAsync", onSubmitAsync =>
            {
                onSubmitAsync.Private().Async();

                onSubmitAsync.AddAssignmentStatement("var user", new CSharpStatement("await UserAccessor.GetRequiredUserAsync(HttpContext);"));
                onSubmitAsync.AddStatement("await UserManager.SetTwoFactorEnabledAsync(user, false);");
                onSubmitAsync.AddStatement("await UserManager.ResetAuthenticatorKeyAsync(user);");
                onSubmitAsync.AddAssignmentStatement("var userId", new CSharpStatement("await UserManager.GetUserIdAsync(user);"));
                onSubmitAsync.AddStatement("Logger.LogInformation(\"User with ID '{UserId}' has reset their authentication app key.\", userId);");
                onSubmitAsync.AddStatement("await SignInManager.RefreshSignInAsync(user);");
                onSubmitAsync.AddStatement("RedirectManager.RedirectToWithStatus(\"Account/Manage/EnableAuthenticator\", \"Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.\", HttpContext);");
            });
        }
    }
}
