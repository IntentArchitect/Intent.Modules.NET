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
    /// Default (first-generation only) content for the Manage/TwoFactorAuthentication page, seeded
    /// onto the modelled <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static content/Components(MudBlazor)/Account/Pages/Manage/TwoFactorAuthentication pair.
    /// </summary>
    internal static class ManageTwoFactorAuthenticationPageContent
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

            .two-fa-actions {
            display: flex;
            flex-wrap: wrap;
            gap: var(--space-3);
            align-items: center;
            }

            .two-fa-inline-form {
            display: inline-block;
            }
            """;

        private static string BuildMudBlazorContent(string identityClass, string userAccessor, string redirectManager)
        {
            return $$"""
                @using Microsoft.AspNetCore.Http.Features
                @using Microsoft.AspNetCore.Identity

                @inject UserManager<{{identityClass}}> UserManager
                @inject SignInManager<{{identityClass}}> SignInManager
                @inject {{userAccessor}} UserAccessor
                @inject {{redirectManager}} RedirectManager

                <MudPaper Class="pa-4 mb-4 ux-gradient-primary"
                Elevation="0">
                <MudText Typo="Typo.h4"
                Class="text-white font-weight-bold mb-2">
                <MudIcon Icon="@Icons.Material.Filled.Security"
                Class="mr-2" />
                Two-factor authentication
                </MudText>
                <MudText Typo="Typo.body1"
                Class="text-white opacity-90">
                Manage your 2FA settings, recovery codes, and authenticator app.
                </MudText>
                </MudPaper>

                <StatusMessage />
                <MudCard Class="ux-fade-in-up auth-form-shell"
                Style="animation-delay: 0.1s"
                Outlined="true">
                <MudCardContent>
                <MudText Typo="Typo.h5"
                Class="mb-3">
                Two-factor authentication (2FA)
                </MudText>
                @if (canTrack)
                {
                if (is2faEnabled)
                {
                if (recoveryCodesLeft == 0)
                {
                <MudAlert Severity="Severity.Error"
                Class="mb-3">
                <strong>You have no recovery codes left.</strong>
                <p>You must <MudLink Href="Account/Manage/GenerateRecoveryCodes">generate a new set of recovery codes</MudLink> before you can log in with a recovery code.</p>
                </MudAlert>
                }
                else if (recoveryCodesLeft == 1)
                {
                <MudAlert Severity="Severity.Error"
                Class="mb-3">
                <strong>You have 1 recovery code left.</strong>
                <p>You can <MudLink Href="Account/Manage/GenerateRecoveryCodes">generate a new set of recovery codes</MudLink>.</p>
                </MudAlert>
                }
                else if (recoveryCodesLeft <= 3)
                {
                <MudAlert Severity="Severity.Warning"
                Class="mb-3">
                <strong>You have @recoveryCodesLeft recovery codes left.</strong>
                <p>You should <MudLink Href="Account/Manage/GenerateRecoveryCodes">generate a new set of recovery codes</MudLink>.</p>
                </MudAlert>
                }

                <div class="two-fa-actions mb-4">
                @if (isMachineRemembered)
                {
                <form class="two-fa-inline-form"
                @formname="forget-browser"
                @onsubmit="OnSubmitForgetBrowserAsync"
                method="post">
                <AntiforgeryToken />
                <MudButton ButtonType="ButtonType.Submit"
                Variant="Variant.Outlined"
                Color="Color.Primary">
                Forget this browser
                </MudButton>
                </form>
                }
                <MudButton Href="Account/Manage/Disable2fa"
                Variant="Variant.Outlined"
                Color="Color.Primary">
                Disable 2FA
                </MudButton>
                <MudButton Href="Account/Manage/GenerateRecoveryCodes"
                Variant="Variant.Outlined"
                Color="Color.Primary">
                Reset recovery codes
                </MudButton>
                </div>
                }

                <div class="two-fa-actions mt-3">
                @if (!hasAuthenticator)
                {
                <MudButton Href="Account/Manage/EnableAuthenticator"
                Variant="Variant.Filled"
                Color="Color.Primary">
                Add authenticator app
                </MudButton>
                }
                else
                {
                <MudButton Href="Account/Manage/EnableAuthenticator"
                Variant="Variant.Filled"
                Color="Color.Primary">
                Set up authenticator app
                </MudButton>
                <MudButton Href="Account/Manage/ResetAuthenticator"
                Variant="Variant.Outlined"
                Color="Color.Primary">
                Reset authenticator app
                </MudButton>
                }
                </div>
                }
                else
                {
                <MudAlert Severity="Severity.Error"
                Class="mb-3">
                <strong>Privacy and cookie policy have not been accepted.</strong>
                <p>You must accept the policy before you can enable two factor authentication.</p>
                </MudAlert>
                }
                </MudCardContent>
                </MudCard>

                """;
        }

        private static string BuildBootstrapContent(string identityClass, string userAccessor, string redirectManager)
        {
            return $$"""
                @using Microsoft.AspNetCore.Http.Features
                @using Microsoft.AspNetCore.Identity

                @inject UserManager<{{identityClass}}> UserManager
                @inject SignInManager<{{identityClass}}> SignInManager
                @inject {{userAccessor}} UserAccessor
                @inject {{redirectManager}} RedirectManager

                <StatusMessage />
                <h3>Two-factor authentication (2FA)</h3>
                @if (canTrack)
                {
                if (is2faEnabled)
                {
                if (recoveryCodesLeft == 0)
                {
                <div class="ux-callout ux-callout-danger">
                <strong>You have no recovery codes left.</strong>
                <p>You must <a href="Account/Manage/GenerateRecoveryCodes">generate a new set of recovery codes</a> before you can log in with a recovery code.</p>
                </div>
                }
                else if (recoveryCodesLeft == 1)
                {
                <div class="ux-callout ux-callout-danger">
                <strong>You have 1 recovery code left.</strong>
                <p>You can <a href="Account/Manage/GenerateRecoveryCodes">generate a new set of recovery codes</a>.</p>
                </div>
                }
                else if (recoveryCodesLeft <= 3)
                {
                <div class="ux-callout ux-callout-warning">
                <strong>You have @recoveryCodesLeft recovery codes left.</strong>
                <p>You should <a href="Account/Manage/GenerateRecoveryCodes">generate a new set of recovery codes</a>.</p>
                </div>
                }

                <div class="ux-button-row">
                @if (isMachineRemembered)
                {
                <form @formname="forget-browser"
                @onsubmit="OnSubmitForgetBrowserAsync"
                method="post">
                <AntiforgeryToken />
                <button type="submit"
                class="btn btn-primary">
                Forget this browser
                </button>
                </form>
                }
                <a href="Account/Manage/Disable2fa"
                class="btn btn-danger">
                <UxIcon Name="shield-off" /> Disable 2FA
                </a>
                <a href="Account/Manage/GenerateRecoveryCodes"
                class="btn btn-primary">
                <UxIcon Name="key" /> Reset recovery codes
                </a>
                </div>
                }

                <h4>Authenticator app</h4>
                <div class="ux-button-row">
                @if (!hasAuthenticator)
                {
                <a href="Account/Manage/EnableAuthenticator"
                class="btn btn-primary">
                <UxIcon Name="shield" /> Add authenticator app
                </a>
                }
                else
                {
                <a href="Account/Manage/EnableAuthenticator"
                class="btn btn-primary">
                <UxIcon Name="shield" /> Set up authenticator app
                </a>
                <a href="Account/Manage/ResetAuthenticator"
                class="btn btn-primary">
                <UxIcon Name="key" /> Reset authenticator app
                </a>
                }
                </div>
                }
                else
                {
                <div class="ux-callout ux-callout-danger">
                <strong>Privacy and cookie policy have not been accepted.</strong>
                <p>You must accept the policy before you can enable two factor authentication.</p>
                </div>
                }
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
            code.AddField("bool", "canTrack");
            code.AddField("bool", "hasAuthenticator");
            code.AddField("int", "recoveryCodesLeft");
            code.AddField("bool", "is2faEnabled");
            code.AddField("bool", "isMachineRemembered");

            code.AddProperty(code.Template.UseType("Microsoft.AspNetCore.Http.HttpContext"), "HttpContext", p => p.WithInitialValue("default!").AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.CascadingParameterAttribute").RemoveSuffix("Attribute")));

            // either get the existing method or add one
            ICSharpClassMethodDeclaration onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            if (onInitializedAsync is null)
            {
                code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnInitializedAsync");
                onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            }

            onInitializedAsync.Async().Protected().Override();
            onInitializedAsync.AddAssignmentStatement("var user", new CSharpStatement("await UserAccessor.GetRequiredUserAsync(HttpContext);"));
            onInitializedAsync.AddAssignmentStatement("canTrack", new CSharpStatement($"HttpContext.Features.Get<{code.Template.UseType("Microsoft.AspNetCore.Http.Features.ITrackingConsentFeature")}>()?.CanTrack ?? true;"));
            onInitializedAsync.AddAssignmentStatement("hasAuthenticator", new CSharpStatement("await UserManager.GetAuthenticatorKeyAsync(user) is not null;"));
            onInitializedAsync.AddAssignmentStatement("is2faEnabled", new CSharpStatement("await UserManager.GetTwoFactorEnabledAsync(user);"));
            onInitializedAsync.AddAssignmentStatement("isMachineRemembered", new CSharpStatement("await SignInManager.IsTwoFactorClientRememberedAsync(user);"));
            onInitializedAsync.AddAssignmentStatement("recoveryCodesLeft", new CSharpStatement("await UserManager.CountRecoveryCodesAsync(user);"));

            code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnSubmitForgetBrowserAsync", onSubmitForgetBrowserAsync =>
            {
                onSubmitForgetBrowserAsync.Private().Async();

                onSubmitForgetBrowserAsync.AddStatement("await SignInManager.ForgetTwoFactorClientAsync();");
                onSubmitForgetBrowserAsync.AddStatement("RedirectManager.RedirectToCurrentPageWithStatus(\"The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.\", HttpContext);");
            });
        }
    }
}
