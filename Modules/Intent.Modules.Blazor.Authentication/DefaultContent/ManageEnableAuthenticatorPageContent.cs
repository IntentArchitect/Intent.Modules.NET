using Intent.Modules.Blazor.Authentication.FactoryExtensions;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Templates;
using Intent.Templates;
using System.Linq;

namespace Intent.Modules.Blazor.Authentication.DefaultContent
{
    /// <summary>
    /// Default (first-generation only) content for the Manage/EnableAuthenticator page, seeded onto
    /// the modelled <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static content/Components(MudBlazor)/Account/Pages/Manage/EnableAuthenticator pair.
    /// </summary>
    internal static class ManageEnableAuthenticatorPageContent
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
            }

            .auth-form-shell ::deep .mud-input-control-input-container,
            .auth-form-shell ::deep .mud-input-slot {
            background: var(--surface-2);
            }

            .auth-form-shell ::deep .mud-input-outlined-border {
            border-color: var(--border);
            }

            .auth-form-shell ::deep .mud-input-label {
            color: var(--text-muted);
            }

            .auth-form-shell ::deep .mud-input-root.mud-input-outlined.mud-input-adorned-start:hover .mud-input-outlined-border,
            .auth-form-shell ::deep .mud-input-root.mud-input-outlined.mud-input-adorned-start.mud-input-focused .mud-input-outlined-border {
            border-color: var(--primary);
            }

            .auth-form-shell ::deep .mud-input-root.mud-input-outlined.mud-input-focused {
            box-shadow: 0 0 0 3px var(--primary-subtle), 0 0 12px var(--primary-glow);
            border-radius: var(--radius-sm);
            }

            .auth-steps {
            padding-left: var(--space-5);
            }

            .auth-steps li {
            margin-bottom: var(--space-4);
            }
            """;

        private static string BuildMudBlazorContent(string identityClass, string userAccessor, string redirectManager)
        {
            return $$"""
                @using System.Globalization
                @using System.Text
                @using System.Text.Encodings.Web
                @using Microsoft.AspNetCore.Identity

                @inject UserManager<{{identityClass}}> UserManager
                @inject {{userAccessor}} UserAccessor
                @inject UrlEncoder UrlEncoder
                @inject {{redirectManager}} RedirectManager
                @inject ILogger<EnableAuthenticator> Logger

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
                <MudIcon Icon="@Icons.Material.Filled.QrCode2"
                Class="mr-2" />
                Configure authenticator app
                </MudText>
                <MudText Typo="Typo.body1"
                Class="text-white opacity-90">
                Set up an authenticator app to strengthen your account security.
                </MudText>
                </MudPaper>

                <StatusMessage Message="@message" />
                <MudCard Class="ux-fade-in-up"
                Style="animation-delay: 0.1s"
                Outlined="true">
                <MudCardContent>
                <MudText Typo="Typo.h5"
                Class="mb-3">
                Configure authenticator app
                </MudText>
                <MudText Typo="Typo.body1"
                Class="mb-4">
                To use an authenticator app go through the following steps:
                </MudText>
                <ol class="auth-steps">
                <li>
                <MudText Typo="Typo.body1">
                Download a two-factor authenticator app like Microsoft Authenticator for
                <a href="https://go.microsoft.com/fwlink/?Linkid=825072">Android</a> and
                <a href="https://go.microsoft.com/fwlink/?Linkid=825073">iOS</a> or
                Google Authenticator for
                <a href="https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2&amp;hl=en">Android</a> and
                <a href="https://itunes.apple.com/us/app/google-authenticator/id388497605?mt=8">iOS</a>.
                </MudText>
                </li>
                <li>
                <MudText Typo="Typo.body1">Scan the QR Code or enter this key <code>@sharedKey</code> into your two factor authenticator app. Spaces and casing do not matter.</MudText>
                <MudAlert Severity="Severity.Info"
                Class="mb-3">
                Learn how to <a href="https://go.microsoft.com/fwlink/?Linkid=852423">enable QR code generation</a>.
                </MudAlert>
                <div data-url="@authenticatorUri"></div>
                </li>
                <li>
                <MudText Typo="Typo.body1"
                Class="mb-3">
                Once you have scanned the QR code or input the key above, your two factor authentication app will provide you with a unique code. Enter the code in the confirmation box below.
                </MudText>
                <div class="auth-form-shell">
                <EditForm Model="Input"
                FormName="send-code"
                OnValidSubmit="OnValidSubmitAsync"
                method="post">
                <DataAnnotationsValidator />
                <ValidationSummary class="text-danger"
                role="alert" />

                <MudGrid>
                <MudItem xs="12">
                <MudTextField T="string"
                @bind-Value="Input.Code"
                Label="Verification code"
                Placeholder="Please enter the code."
                Variant="Variant.Outlined"
                Adornment="Adornment.Start"
                AdornmentIcon="@Icons.Material.Filled.Password"
                Immediate="true"
                For="@(() => Input.Code)" />
                <ValidationMessage For="() => Input.Code"
                class="text-danger" />
                </MudItem>
                <MudItem xs="12">
                <MudStack Row="true"
                Justify="Justify.FlexEnd">
                <MudButton ButtonType="ButtonType.Submit"
                Variant="Variant.Filled"
                Color="Color.Primary"
                StartIcon="@Icons.Material.Filled.VerifiedUser">
                Verify
                </MudButton>
                </MudStack>
                </MudItem>
                </MudGrid>
                </EditForm>
                </div>
                </li>
                </ol>
                </MudCardContent>
                </MudCard>
                }

                """;
        }

        private static string BuildBootstrapContent(string identityClass, string userAccessor, string redirectManager)
        {
            return $$"""
                @using System.Globalization
                @using System.Text
                @using System.Text.Encodings.Web
                @using Microsoft.AspNetCore.Identity

                @inject UserManager<{{identityClass}}> UserManager
                @inject {{userAccessor}} UserAccessor
                @inject UrlEncoder UrlEncoder
                @inject {{redirectManager}} RedirectManager
                @inject ILogger<EnableAuthenticator> Logger

                @if (recoveryCodes is not null)
                {
                <ShowRecoveryCodes RecoveryCodes="recoveryCodes.ToArray()"
                StatusMessage="@message" />
                }
                else
                {
                <StatusMessage Message="@message" />
                <h3>Configure authenticator app</h3>
                <p>To use an authenticator app go through the following steps:</p>
                <ol class="ux-steps">
                <li>
                <p>
                Download a two-factor authenticator app like Microsoft Authenticator for
                <a href="https://go.microsoft.com/fwlink/?Linkid=825072">Android</a> and
                <a href="https://go.microsoft.com/fwlink/?Linkid=825073">iOS</a> or
                Google Authenticator for
                <a href="https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2&amp;hl=en">Android</a> and
                <a href="https://itunes.apple.com/us/app/google-authenticator/id388497605?mt=8">iOS</a>.
                </p>
                </li>
                <li>
                <p>Scan the QR Code or enter this key <kbd>@sharedKey</kbd> into your two factor authenticator app. Spaces and casing do not matter.</p>
                <div class="ux-callout ux-callout-info">Learn how to <a href="https://go.microsoft.com/fwlink/?Linkid=852423">enable QR code generation</a>.</div>
                <div></div>
                <div data-url="@authenticatorUri"></div>
                </li>
                <li>
                <p>
                Once you have scanned the QR code or input the key above, your two factor authentication app will provide you
                with a unique code. Enter the code in the confirmation box below.
                </p>
                <EditForm Model="Input"
                FormName="send-code"
                OnValidSubmit="OnValidSubmitAsync"
                method="post">
                <DataAnnotationsValidator />
                <UxField Label="Verification code"
                Icon="shield"
                For="code">
                <InputText id="code"
                @bind-Value="Input.Code"
                class="ux-input"
                autocomplete="off"
                placeholder="Enter the code" />
                </UxField>
                <ValidationMessage For="() => Input.Code"
                class="text-danger" />
                <button type="submit"
                class="btn btn-primary">
                <UxIcon Name="check-circle" /> Verify
                </button>
                <ValidationSummary class="text-danger"
                role="alert" />
                </EditForm>
                </li>
                </ol>
                }
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
            var identityClass = code.Template.ExecutionContext.InstalledModules.Any(im => im.ModuleId == "Intent.AspNetCore.Identity") ?
                IdentityHelperExtensions.GetIdentityUserClass(code.Template) :
                "ApplicationUser";

            code.AddField("string", "AuthenticatorUriFormat", f => f.PrivateConstant("\"otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6\""));
            code.AddField("string?", "message");
            code.AddField(identityClass, "user", f => f.WithAssignment(new CSharpStatement("default!")));
            code.AddField("string?", "sharedKey");
            code.AddField("string?", "authenticatorUri");
            code.AddField("IEnumerable<string>?", "recoveryCodes");

            code.AddProperty(code.Template.UseType("Microsoft.AspNetCore.Http.HttpContext"), "HttpContext", p => p.WithInitialValue("default!").AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.CascadingParameterAttribute").RemoveSuffix("Attribute")));
            code.AddProperty("InputModel", "Input", p =>
            {
                p.Private();
                p.WithInitialValue("default!");
                p.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromFormAttribute").RemoveSuffix("Attribute"));
            });

            // either get the existing method or add one
            ICSharpClassMethodDeclaration onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            if (onInitializedAsync is null)
            {
                code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnInitializedAsync");
                onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            }

            onInitializedAsync.Async().Protected().Override();
            onInitializedAsync.AddStatement("Input ??= new();");
            onInitializedAsync.AddAssignmentStatement("user", new CSharpStatement("await UserAccessor.GetRequiredUserAsync(HttpContext);"));
            onInitializedAsync.AddStatement("await LoadSharedKeyAndQrCodeUriAsync(user);");

            code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnValidSubmitAsync", onValidSubmitAsync =>
            {
                onValidSubmitAsync.Private().Async();

                onValidSubmitAsync.AddStatement("// Strip spaces and hyphens");
                onValidSubmitAsync.AddAssignmentStatement("var verificationCode", new CSharpStatement("Input.Code.Replace(\" \", string.Empty).Replace(\"-\", string.Empty);"));
                onValidSubmitAsync.AddAssignmentStatement("var is2faTokenValid", new CSharpStatement("await UserManager.VerifyTwoFactorTokenAsync(user, UserManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);"));

                onValidSubmitAsync.AddIfStatement("!is2faTokenValid", @if =>
                {
                    @if.AddStatement("message = \"Error: Verification code is invalid.\";");
                    @if.AddStatement("return;");
                });

                onValidSubmitAsync.AddStatement("await UserManager.SetTwoFactorEnabledAsync(user, true);");
                onValidSubmitAsync.AddAssignmentStatement("var userId", new CSharpStatement("await UserManager.GetUserIdAsync(user);"));
                onValidSubmitAsync.AddStatement("Logger.LogInformation(\"User with ID '{UserId}' has enabled 2FA with an authenticator app.\", userId);");
                onValidSubmitAsync.AddStatement("message = \"Your authenticator app has been verified.\";");

                onValidSubmitAsync.AddIfStatement("await UserManager.CountRecoveryCodesAsync(user) == 0", @if =>
                {
                    @if.AddAssignmentStatement("recoveryCodes", new CSharpStatement("await UserManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);"));
                }).AddElseStatement(@else =>
                {
                    @else.AddStatement("RedirectManager.RedirectToWithStatus(\"Account/Manage/TwoFactorAuthentication\", message, HttpContext);");
                });
            });

            code.AddMethod(code.Template.UseType("System.Threading.Tasks.ValueTask"), "LoadSharedKeyAndQrCodeUriAsync", loadSharedKeyAndQrCodeUriAsync =>
            {
                loadSharedKeyAndQrCodeUriAsync.Private().Async();
                loadSharedKeyAndQrCodeUriAsync.AddParameter(identityClass, "user");

                loadSharedKeyAndQrCodeUriAsync.AddStatement("// Load the authenticator key & QR code URI to display on the form");
                loadSharedKeyAndQrCodeUriAsync.AddAssignmentStatement("var unformattedKey", new CSharpStatement("await UserManager.GetAuthenticatorKeyAsync(user);"));
                loadSharedKeyAndQrCodeUriAsync.AddIfStatement("string.IsNullOrEmpty(unformattedKey)", @if =>
                {
                    @if.AddStatement("await UserManager.ResetAuthenticatorKeyAsync(user);");
                    @if.AddAssignmentStatement("unformattedKey", new CSharpStatement("await UserManager.GetAuthenticatorKeyAsync(user);"));
                });

                loadSharedKeyAndQrCodeUriAsync.AddAssignmentStatement("sharedKey", new CSharpStatement("FormatKey(unformattedKey!);"));
                loadSharedKeyAndQrCodeUriAsync.AddAssignmentStatement("var email", new CSharpStatement("await UserManager.GetEmailAsync(user);"));
                loadSharedKeyAndQrCodeUriAsync.AddAssignmentStatement("authenticatorUri", new CSharpStatement("GenerateQrCodeUri(email!, unformattedKey!);"));
            });

            code.AddMethod("string", "FormatKey", formatKey =>
            {
                formatKey.Private();
                formatKey.AddParameter("string", "unformattedKey");

                formatKey.AddAssignmentStatement($"var result", new CSharpStatement($"new {code.Template.UseType("System.Text.StringBuilder")}();"));
                formatKey.AddAssignmentStatement("int currentPosition", new CSharpStatement("0;"));
                formatKey.AddStatement("while (currentPosition + 4 < unformattedKey.Length)\n{\n    result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');\n    currentPosition += 4;\n}");
                formatKey.AddIfStatement("currentPosition < unformattedKey.Length", @if =>
                {
                    @if.AddStatement("result.Append(unformattedKey.AsSpan(currentPosition));");
                });

                formatKey.AddStatement("return result.ToString().ToLowerInvariant();");
            });

            code.AddMethod("string", "GenerateQrCodeUri", generateQrCodeUri =>
            {
                generateQrCodeUri.Private();
                generateQrCodeUri.AddParameter("string", "email");
                generateQrCodeUri.AddParameter("string", "unformattedKey");

                generateQrCodeUri.AddStatement($"return string.Format({code.Template.UseType("System.Globalization.CultureInfo")}.InvariantCulture, AuthenticatorUriFormat, UrlEncoder.Encode(\"Microsoft.AspNetCore.Identity.UI\"), UrlEncoder.Encode(email), unformattedKey);");
            });

            code.AddClass("InputModel", inputModel =>
            {
                inputModel.Private().Sealed();

                inputModel.AddProperty("string", "Code", p =>
                {
                    p.AddAttribute(code.Template.UseType("System.ComponentModel.DataAnnotations.RequiredAttribute").RemoveSuffix("Attribute"));
                    p.AddAttribute("StringLength(7, ErrorMessage = \"The {0} must be at least {2} and at max {1} characters long.\", MinimumLength = 6)");
                    p.AddAttribute("DataType(DataType.Text)");
                    p.AddAttribute("Display(Name = \"Verification Code\")");
                    p.WithInitialValue("\"\"");
                });
            });
        }
    }
}
