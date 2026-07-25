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
    /// Default (first-generation only) content for the ExternalLogin page, seeded onto the modelled
    /// <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static content/Components(MudBlazor)/Account/Pages/ExternalLogin pair.
    /// </summary>
    internal static class ExternalLoginPageContent
    {
        public static string BuildRazorContent(RazorComponentTemplate template)
        {
            var identityClass = template.ExecutionContext.InstalledModules.Any(im => im.ModuleId == "Intent.AspNetCore.Identity") ?
                IdentityHelperExtensions.GetIdentityUserClass(template) :
                "ApplicationUser";
            var redirectManager = template.GetIdentityRedirectManagerTemplateName();
            var isMudBlazor = template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor");

            return isMudBlazor
                ? BuildMudBlazorContent(identityClass, redirectManager)
                : BuildBootstrapContent(identityClass, redirectManager);
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

            .account-input-field {
            display: flex;
            flex-direction: column;
            gap: var(--space-2);
            }

            .account-input-label {
            color: var(--text);
            font-size: var(--type-label-lg);
            font-weight: 500;
            }

            .account-input-shell {
            display: flex;
            align-items: center;
            gap: var(--space-2);
            min-height: 44px;
            padding: 0 0.875rem;
            background: var(--surface-2);
            border: 1px solid var(--border);
            border-radius: var(--radius-sm);
            box-shadow: var(--shadow-1);
            }

            .account-input-shell:focus-within {
            border-color: var(--primary);
            box-shadow: 0 0 0 3px var(--primary-subtle), 0 0 12px var(--primary-glow);
            }

            ::deep .account-input-icon {
            color: var(--text-muted);
            flex-shrink: 0;
            }

            ::deep .account-input-control {
            width: 100%;
            min-height: 42px;
            color: var(--text);
            background: transparent;
            border: none;
            outline: none;
            }

            ::deep .account-input-control::placeholder {
            color: var(--text-muted);
            }
            """;

        private static string BuildMudBlazorContent(string identityClass, string redirectManager)
        {
            return $$"""
                @using System.Security.Claims
                @using System.Text
                @using System.Text.Encodings.Web
                @using Microsoft.AspNetCore.Identity
                @using Microsoft.AspNetCore.WebUtilities

                @inject SignInManager<{{identityClass}}> SignInManager
                @inject UserManager<{{identityClass}}> UserManager
                @inject IUserStore<{{identityClass}}> UserStore
                @inject IEmailSender<{{identityClass}}> EmailSender
                @inject {{redirectManager}} RedirectManager
                @inject ILogger<ExternalLogin> Logger

                <MudPaper Class="pa-4 mb-4 ux-gradient-primary"
                Elevation="0">
                <MudText Typo="Typo.h4"
                Class="text-white font-weight-bold mb-2">
                <MudIcon Icon="@Icons.Material.Filled.PersonAddAlt1"
                Class="mr-2" />
                Complete registration
                </MudText>
                <MudText Typo="Typo.body1"
                Class="text-white opacity-90">
                Link your @ProviderDisplayName account and finish creating your profile.
                </MudText>
                </MudPaper>

                <StatusMessage Message="@message" />
                <MudAlert Severity="Severity.Info"
                Class="mb-4">
                You've successfully authenticated with <strong>@ProviderDisplayName</strong>.
                Please enter an email address for this site below and click the Register button to finish
                logging in.
                </MudAlert>

                <MudCard Class="ux-fade-in-up auth-form-shell"
                Style="animation-delay: 0.1s"
                Outlined="true">
                <MudCardContent>
                <EditForm Model="Input"
                OnValidSubmit="OnValidSubmitAsync"
                FormName="confirmation"
                method="post">
                <DataAnnotationsValidator />
                <ValidationSummary class="text-danger"
                role="alert" />

                <MudGrid>
                <MudItem xs="12">
                <MudText Typo="Typo.h5">Associate your @ProviderDisplayName account</MudText>
                <MudText Typo="Typo.body2"
                Class="mb-4">
                Enter an email address to complete registration.
                </MudText>
                </MudItem>
                <MudItem xs="12">
                <div class="account-input-field">
                <label class="account-input-label"
                for="email">
                Email
                </label>
                <div class="account-input-shell">
                <MudIcon Icon="@Icons.Material.Filled.Email"
                Class="account-input-icon" />
                <InputText id="email"
                class="account-input-control"
                @bind-Value="Input.Email"
                autocomplete="email"
                aria-required="true"
                placeholder="Please enter your email."
                type="email" />
                </div>
                <ValidationMessage For="() => Input.Email"
                class="text-danger" />
                </div>
                </MudItem>
                <MudItem xs="12">
                <MudStack Row="true"
                Justify="Justify.FlexEnd">
                <MudButton ButtonType="ButtonType.Submit"
                Variant="Variant.Filled"
                Color="Color.Primary"
                StartIcon="@Icons.Material.Filled.HowToReg">
                Register
                </MudButton>
                </MudStack>
                </MudItem>
                </MudGrid>
                </EditForm>
                </MudCardContent>
                </MudCard>
                """;
        }

        private static string BuildBootstrapContent(string identityClass, string redirectManager)
        {
            return $$"""
                @using System.Security.Claims
                @using System.Text
                @using System.Text.Encodings.Web
                @using Microsoft.AspNetCore.Identity
                @using Microsoft.AspNetCore.WebUtilities

                @inject SignInManager<{{identityClass}}> SignInManager
                @inject UserManager<{{identityClass}}> UserManager
                @inject IUserStore<{{identityClass}}> UserStore
                @inject IEmailSender<{{identityClass}}> EmailSender
                @inject {{redirectManager}} RedirectManager
                @inject ILogger<ExternalLogin> Logger

                <AccountHero Icon="user-plus"
                Title="Almost there"
                Subtitle="@($"Associate your {ProviderDisplayName} account.")" />

                <div class="ux-form-narrow">
                <section>
                <StatusMessage Message="@message" />
                <p class="ux-section-subtitle">
                You've successfully authenticated with <strong>@ProviderDisplayName</strong>. Enter an email address for this site and select Register to finish.
                </p>
                <EditForm Model="Input"
                OnValidSubmit="OnValidSubmitAsync"
                FormName="confirmation"
                method="post">
                <DataAnnotationsValidator />
                <ValidationSummary class="text-danger"
                role="alert" />
                <UxField Label="Email"
                Icon="mail"
                For="email">
                <InputText id="email"
                @bind-Value="Input.Email"
                class="ux-input"
                autocomplete="email"
                placeholder="name@example.com" />
                </UxField>
                <ValidationMessage For="() => Input.Email"
                class="text-danger" />
                <button type="submit"
                class="w-100 btn btn-primary">
                <UxIcon Name="user-plus" />
                Register
                </button>
                </EditForm>
                </section>
                </div>
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
            var identityClass = code.Template.ExecutionContext.InstalledModules.Any(im => im.ModuleId == "Intent.AspNetCore.Identity") ?
                IdentityHelperExtensions.GetIdentityUserClass(code.Template) :
                "ApplicationUser";

            code.AddField("string", "LoginCallbackAction", f => f.Public("\"LoginCallback\"").Constant());
            code.AddField("string?", "message");
            code.AddField(code.Template.UseType("Microsoft.AspNetCore.Identity.ExternalLoginInfo"), "externalLoginInfo", f => f.WithAssignment(new CSharpStatement("default!")));

            code.AddProperty(code.Template.UseType("Microsoft.AspNetCore.Http.HttpContext?"), "HttpContext", p => p.WithInitialValue("default!").AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.CascadingParameterAttribute").RemoveSuffix("Attribute")));
            code.AddProperty("InputModel", "Input", p =>
            {
                p.Private();
                p.WithInitialValue("default!");
                p.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromFormAttribute").RemoveSuffix("Attribute"));
            });
            code.AddProperty("string?", "RemoteError", p => p.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromQueryAttribute").RemoveSuffix("Attribute")));
            code.AddProperty("string?", "ReturnUrl", p => p.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromQueryAttribute").RemoveSuffix("Attribute")));
            code.AddProperty("string?", "Action", p => p.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromQueryAttribute").RemoveSuffix("Attribute")));

            code.AddProperty("string?", "ProviderDisplayName", p => p.Private().WithoutSetter().Getter.WithExpressionImplementation("externalLoginInfo.ProviderDisplayName"));

            // either get the existing method or add one
            ICSharpClassMethodDeclaration onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            if (onInitializedAsync is null)
            {
                code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnInitializedAsync");
                onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            }

            onInitializedAsync.Async().Protected().Override();

            onInitializedAsync.AddStatement("Input ??= new();");
            onInitializedAsync.AddIfStatement("RemoteError is not null", @if =>
            {
                @if.AddStatement("RedirectManager.RedirectToWithStatus(\"Account/Login\", $\"Error from external provider: {RemoteError}\", HttpContext);");
            });

            onInitializedAsync.AddAssignmentStatement("var info", new CSharpStatement("await SignInManager.GetExternalLoginInfoAsync();"));
            onInitializedAsync.AddIfStatement("info is null", @if =>
            {
                @if.AddStatement("RedirectManager.RedirectToWithStatus(\"Account/Login\", \"Error loading external login information.\", HttpContext);");
            });

            onInitializedAsync.AddStatement("externalLoginInfo = info;");

            onInitializedAsync.AddIfStatement($"{code.Template.UseType("Microsoft.AspNetCore.Http.HttpMethods")}.IsGet(HttpContext.Request.Method)", @if =>
            {
                @if.AddIfStatement("Action == LoginCallbackAction", innerIf =>
                {
                    innerIf.AddStatement("await OnLoginCallbackAsync();");
                    innerIf.AddStatement("return;");
                });

                @if.AddStatement("// We should only reach this page via the login callback, so redirect back to");
                @if.AddStatement("// the login page if we get here some other way.");
                @if.AddStatement("RedirectManager.RedirectTo(\"Account/Login\");");
            });

            code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnLoginCallbackAsync", onLoginCallbackAsync =>
            {
                onLoginCallbackAsync.Private().Async();

                onLoginCallbackAsync.AddStatement("// Sign in the user with this external login provider if the user already has a login.");
                onLoginCallbackAsync.AddAssignmentStatement("var result", new CSharpStatement("await SignInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, isPersistent: false, bypassTwoFactor: true);"));

                onLoginCallbackAsync.AddIfStatement("result.Succeeded", @if =>
                {
                    @if.AddStatement("Logger.LogInformation(\"{Name} logged in with {LoginProvider} provider.\", externalLoginInfo.Principal.Identity?.Name, externalLoginInfo.LoginProvider);");
                    @if.AddStatement("RedirectManager.RedirectTo(ReturnUrl);");
                }).AddElseIfStatement("result.IsLockedOut", eIf =>
                {
                    eIf.AddStatement("RedirectManager.RedirectTo(\"Account/Lockout\");");
                });

                onLoginCallbackAsync.AddStatement("// If the user does not have an account, then ask the user to create an account.");
                onLoginCallbackAsync.AddIfStatement($"externalLoginInfo.Principal.HasClaim(c => c.Type == {code.Template.UseType("System.Security.Claims.ClaimTypes")}.Email)", @if =>
                {
                    @if.AddStatement("Input.Email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email) ?? \"\";");
                });
            });

            code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnValidSubmitAsync", onValidSubmitAsync =>
            {
                onValidSubmitAsync.Private().Async();

                onValidSubmitAsync.AddAssignmentStatement("var emailStore", new CSharpStatement("GetEmailStore();"));
                onValidSubmitAsync.AddAssignmentStatement("var user", new CSharpStatement("CreateUser();"));

                onValidSubmitAsync.AddStatement("await UserStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);");
                onValidSubmitAsync.AddStatement("await emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);");
                onValidSubmitAsync.AddAssignmentStatement("var result", new CSharpStatement("await UserManager.CreateAsync(user);"));

                onValidSubmitAsync.AddIfStatement("result.Succeeded", @if =>
                {
                    @if.AddAssignmentStatement("result", new CSharpStatement("await UserManager.AddLoginAsync(user, externalLoginInfo);"));

                    @if.AddIfStatement("result.Succeeded", innerIf =>
                    {
                        innerIf.AddStatement("Logger.LogInformation(\"User created an account using {Name} provider.\", externalLoginInfo.LoginProvider);");
                        innerIf.AddAssignmentStatement("var userId", new CSharpStatement("await UserManager.GetUserIdAsync(user);"));
                        innerIf.AddAssignmentStatement("var code", new CSharpStatement("await UserManager.GenerateEmailConfirmationTokenAsync(user);"));
                        innerIf.AddAssignmentStatement("code", new CSharpStatement($"{code.Template.UseType("Microsoft.AspNetCore.WebUtilities.WebEncoders")}.Base64UrlEncode({code.Template.UseType("System.Text.Encoding")}.UTF8.GetBytes(code));"));
                        innerIf.AddAssignmentStatement("var callbackUrl", new CSharpStatement("NavigationManager.GetUriWithQueryParameters(NavigationManager.ToAbsoluteUri(\"Account/ConfirmEmail\").AbsoluteUri, new Dictionary<string, object?> { [\"userId\"] = userId, [\"code\"] = code });"));
                        innerIf.AddStatement($"await EmailSender.SendConfirmationLinkAsync(user, Input.Email, {code.Template.UseType("System.Text.Encodings.Web.HtmlEncoder")}.Default.Encode(callbackUrl));");

                        innerIf.AddStatement("// If account confirmation is required, we need to show the link if we don't have a real email sender");
                        innerIf.AddIfStatement("UserManager.Options.SignIn.RequireConfirmedAccount", requireConfirm =>
                        {
                            requireConfirm.AddStatement("RedirectManager.RedirectTo(\"Account/RegisterConfirmation\", new() { [\"email\"] = Input.Email });");
                        });

                        innerIf.AddStatement("await SignInManager.SignInAsync(user, isPersistent: false, externalLoginInfo.LoginProvider);");
                        innerIf.AddStatement("RedirectManager.RedirectTo(ReturnUrl);");
                    });
                });

                onValidSubmitAsync.AddStatement("message = $\"Error: {string.Join(\",\", result.Errors.Select(error => error.Description))}\";");
            });

            code.AddMethod(identityClass, "CreateUser", createUser =>
            {
                createUser.Private();
                createUser.AddTryBlock(@try =>
                {
                    @try.AddReturn($"Activator.CreateInstance<{identityClass}>();");
                }).AddCatchBlock(@catch =>
                {
                    @catch.AddStatement($"throw new InvalidOperationException($\"Can't create an instance of '{{nameof({identityClass})}}'. \" + $\"Ensure that '{{nameof({identityClass})}}' is not an abstract class and has a parameterless constructor\");");
                });
            });

            code.AddMethod($"IUserEmailStore<{identityClass}>", "GetEmailStore", getEmailStore =>
            {
                getEmailStore.Private();
                getEmailStore.AddIfStatement("!UserManager.SupportsUserEmail", @if =>
                {
                    @if.AddStatement("throw new NotSupportedException(\"The default UI requires a user store with email support.\");");
                });
                getEmailStore.AddStatement($"return (IUserEmailStore<{identityClass}>)UserStore;");
            });

            code.AddClass("InputModel", inputModel =>
            {
                inputModel.Private().Sealed();

                inputModel.AddProperty("string", "Email", p =>
                {
                    p.AddAttribute(code.Template.UseType("System.ComponentModel.DataAnnotations.RequiredAttribute").RemoveSuffix("Attribute"));
                    p.AddAttribute(code.Template.UseType("System.ComponentModel.DataAnnotations.EmailAddressAttribute").RemoveSuffix("Attribute"));
                    p.WithInitialValue("\"\"");
                });
            });
        }
    }
}
