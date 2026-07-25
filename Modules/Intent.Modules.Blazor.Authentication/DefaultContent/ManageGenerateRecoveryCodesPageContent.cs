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
    /// Default (first-generation only) content for the Manage/GenerateRecoveryCodes page, seeded
    /// onto the modelled <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static content/Components(MudBlazor)/Account/Pages/Manage/GenerateRecoveryCodes pair.
    /// </summary>
    internal static class ManageGenerateRecoveryCodesPageContent
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
                @inject ILogger<GenerateRecoveryCodes> Logger

                @if (recoveryCodes is not null)
                {
                    <ShowRecoveryCodes RecoveryCodes="recoveryCodes.ToArray()"
                        StatusMessage="@message" />
                }
                else
                {
                    <MudPaper Class="pa-4 mb-4 ux-gradient-primary"
                        Elevation="0">
                        <MudText Typo="Typo.h4"
                            Class="text-white font-weight-bold mb-2">
                            <MudIcon Icon="@Icons.Material.Filled.Key"
                                Class="mr-2" />
                            Generate recovery codes
                        </MudText>
                        <MudText Typo="Typo.body1"
                            Class="text-white opacity-90">
                            Create a new set of recovery codes for your two-factor authentication setup.
                        </MudText>
                    </MudPaper>

                    <MudCard Class="ux-fade-in-up auth-form-shell"
                        Style="animation-delay: 0.1s"
                        Outlined="true">
                        <MudCardContent>
                            <MudText Typo="Typo.h5"
                                Class="mb-3">
                                Generate two-factor authentication (2FA) recovery codes
                            </MudText>
                            <MudAlert Severity="Severity.Warning"
                                Class="mb-4">
                                <MudText Typo="Typo.body1"><strong>Put these codes in a safe place.</strong></MudText>
                                <MudText Typo="Typo.body1">If you lose your device and don't have the recovery codes you will lose access to your account.</MudText>
                                <MudText Typo="Typo.body1">Generating new recovery codes does not change the keys used in authenticator apps. If you wish to change the key used in an authenticator app you should <MudLink Href="Account/Manage/ResetAuthenticator">reset your authenticator keys.</MudLink></MudText>
                            </MudAlert>
                            <form @formname="generate-recovery-codes"
                                @onsubmit="OnSubmitAsync"
                                method="post">
                                <AntiforgeryToken />
                                <MudStack Row="true"
                                    Justify="Justify.FlexEnd">
                                    <MudButton ButtonType="ButtonType.Submit"
                                        Variant="Variant.Filled"
                                        Color="Color.Error"
                                        StartIcon="@Icons.Material.Filled.Key">
                                        Generate recovery codes
                                    </MudButton>
                                </MudStack>
                            </form>
                        </MudCardContent>
                    </MudCard>
                }

                """;
        }

        private static string BuildBootstrapContent(string identityClass, string userAccessor, string redirectManager)
        {
            return $$"""
                @using Microsoft.AspNetCore.Identity

                @inject UserManager<{{identityClass}}> UserManager
                @inject {{userAccessor}} UserAccessor
                @inject {{redirectManager}} RedirectManager
                @inject ILogger<GenerateRecoveryCodes> Logger

                @if (recoveryCodes is not null)
                {
                    <ShowRecoveryCodes RecoveryCodes="recoveryCodes.ToArray()"
                        StatusMessage="@message" />
                }
                else
                {
                    <h3>Generate two-factor authentication (2FA) recovery codes</h3>
                    <div class="ux-callout ux-callout-warning">
                        <p><strong>Put these codes in a safe place.</strong></p>
                        <p>If you lose your device and don't have the recovery codes you will lose access to your account.</p>
                        <p>
                            Generating new recovery codes does not change the keys used in authenticator apps. If you wish to change the key
                            used in an authenticator app you should <a href="Account/Manage/ResetAuthenticator">reset your authenticator keys.</a>
                        </p>
                    </div>
                    <form @formname="generate-recovery-codes"
                        @onsubmit="OnSubmitAsync"
                        method="post">
                        <AntiforgeryToken />
                        <button class="btn btn-danger"
                            type="submit">
                            <UxIcon Name="key" /> Generate recovery codes
                        </button>
                    </form>
                }
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
            var identityClass = code.Template.ExecutionContext.InstalledModules.Any(im => im.ModuleId == "Intent.AspNetCore.Identity") ?
                IdentityHelperExtensions.GetIdentityUserClass(code.Template) :
                "ApplicationUser";

            code.AddField("string?", "message");
            code.AddField(identityClass, "user", f => f.WithAssignment(new CSharpStatement("default!")));
            code.AddField("IEnumerable<string>?", "recoveryCodes");

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
            onInitializedAsync.AddAssignmentStatement("var isTwoFactorEnabled", new CSharpStatement("await UserManager.GetTwoFactorEnabledAsync(user);"));
            onInitializedAsync.AddIfStatement("!isTwoFactorEnabled", @if =>
            {
                @if.AddStatement("throw new InvalidOperationException(\"Cannot generate recovery codes for user because they do not have 2FA enabled.\");");
            });

            code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnSubmitAsync", onSubmitAsync =>
            {
                onSubmitAsync.Private().Async();

                onSubmitAsync.AddAssignmentStatement("var userId", new CSharpStatement("await UserManager.GetUserIdAsync(user);"));
                onSubmitAsync.AddAssignmentStatement("recoveryCodes", new CSharpStatement("await UserManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);"));
                onSubmitAsync.AddStatement("message = \"You have generated new recovery codes.\";");
                onSubmitAsync.AddStatement("Logger.LogInformation(\"User with ID '{UserId}' has generated new 2FA recovery codes.\", userId);");
            });
        }
    }
}
