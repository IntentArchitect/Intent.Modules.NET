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
    /// Default (first-generation only) content for the LoginWithRecoveryCode page, seeded onto the
    /// modelled <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static content/Components(MudBlazor)/Account/Pages/LoginWithRecoveryCode pair.
    /// </summary>
    internal static class LoginWithRecoveryCodePageContent
    {
        public static string BuildRazorContent(RazorComponentTemplate template)
        {
            var identityClass = IdentityHelperExtensions.GetIdentityUserClass(template);
            var redirectManager = template.GetIdentityRedirectManagerTemplateName();
            var isMudBlazor = template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor");

            return isMudBlazor
                ? BuildMudBlazorContent(identityClass, redirectManager)
                : BuildBootstrapContent(identityClass, redirectManager);
        }

        private static string BuildMudBlazorContent(string identityClass, string redirectManager)
        {
            return $$"""
                @using Microsoft.AspNetCore.Identity

                @inject SignInManager<{{identityClass}}> SignInManager
                @inject UserManager<{{identityClass}}> UserManager
                @inject {{redirectManager}} RedirectManager
                @inject ILogger<LoginWithRecoveryCode> Logger

                <MudPaper Class="pa-4 mb-4 ux-gradient-primary"
                          Elevation="0">
                    <MudText Typo="Typo.h4"
                             Class="text-white font-weight-bold mb-2">
                        <MudIcon Icon="@Icons.Material.Filled.Key"
                                 Class="mr-2" />
                        Recovery code verification
                    </MudText>
                    <MudText Typo="Typo.body1"
                             Class="text-white opacity-90">
                        Use a recovery code to complete sign-in when your authenticator app is unavailable.
                    </MudText>
                </MudPaper>

                <StatusMessage Message="@message" />
                <MudText Typo="Typo.body1"
                         Class="mb-4">
                    You have requested to log in with a recovery code. This login will not be remembered until you provide
                    an authenticator app code at log in or disable 2FA and log in again.
                </MudText>

                <MudCard Class="ux-fade-in-up auth-form-shell"
                         Style="animation-delay: 0.1s"
                         Outlined="true">
                    <MudCardContent>
                        <EditForm Model="Input"
                                  FormName="login-with-recovery-code"
                                  OnValidSubmit="OnValidSubmitAsync"
                                  method="post">
                            <DataAnnotationsValidator />
                            <ValidationSummary class="text-danger"
                                               role="alert" />

                            <MudGrid>
                                <MudItem xs="12">
                                    <MudText Typo="Typo.h5">Recovery code</MudText>
                                    <MudText Typo="Typo.body2"
                                             Class="mb-4">
                                        Enter one of your saved recovery codes to continue.
                                    </MudText>
                                </MudItem>
                                <MudItem xs="12">
                                    <div class="account-input-field">
                                        <label class="account-input-label"
                                               for="recovery-code">
                                            Recovery code
                                        </label>
                                        <div class="account-input-shell">
                                            <MudIcon Icon="@Icons.Material.Filled.VpnKey"
                                                     Class="account-input-icon" />
                                            <InputText id="recovery-code"
                                                       class="account-input-control"
                                                       @bind-Value="Input.RecoveryCode"
                                                       autocomplete="off"
                                                       aria-required="true"
                                                       placeholder="Recovery code" />
                                        </div>
                                        <ValidationMessage For="() => Input.RecoveryCode"
                                                           class="text-danger" />
                                    </div>
                                </MudItem>
                                <MudItem xs="12">
                                    <MudStack Row="true"
                                              Justify="Justify.FlexEnd">
                                        <MudButton ButtonType="ButtonType.Submit"
                                                   Variant="Variant.Filled"
                                                   Color="Color.Primary"
                                                   StartIcon="@Icons.Material.Filled.Login">
                                            Log in
                                        </MudButton>
                                    </MudStack>
                                </MudItem>
                            </MudGrid>
                        </EditForm>
                    </MudCardContent>
                </MudCard>

                <style>
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

                    .account-input-icon {
                        color: var(--text-muted);
                        flex-shrink: 0;
                    }

                    .account-input-control {
                        width: 100%;
                        min-height: 42px;
                        color: var(--text);
                        background: transparent;
                        border: none;
                        outline: none;
                    }

                    .account-input-control::placeholder {
                        color: var(--text-muted);
                    }
                </style>
                """;
        }

        private static string BuildBootstrapContent(string identityClass, string redirectManager)
        {
            return $$"""
                @using Microsoft.AspNetCore.Identity

                @inject SignInManager<{{identityClass}}> SignInManager
                @inject UserManager<{{identityClass}}> UserManager
                @inject {{redirectManager}} RedirectManager
                @inject ILogger<LoginWithRecoveryCode> Logger

                <AccountHero Icon="key"
                             Title="Recovery code verification"
                             Subtitle="Log in using one of your saved recovery codes." />

                <div class="ux-form-narrow">
                    <section>
                        <StatusMessage Message="@message" />
                        <p class="ux-section-subtitle">This login won't be remembered until you provide an authenticator code or disable 2FA.</p>
                        <EditForm Model="Input"
                                  FormName="login-with-recovery-code"
                                  OnValidSubmit="OnValidSubmitAsync"
                                  method="post">
                            <DataAnnotationsValidator />
                            <ValidationSummary class="text-danger"
                                               role="alert" />
                            <UxField Label="Recovery code"
                                     Icon="key"
                                     For="recovery-code">
                                <InputText id="recovery-code"
                                           @bind-Value="Input.RecoveryCode"
                                           class="ux-input"
                                           autocomplete="off"
                                           placeholder="Enter a recovery code" />
                            </UxField>
                            <ValidationMessage For="() => Input.RecoveryCode"
                                               class="text-danger" />
                            <button type="submit"
                                    class="w-100 btn btn-primary">
                                <UxIcon Name="log-in" />
                                Log in
                            </button>
                        </EditForm>
                    </section>
                </div>
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
            var identityClass = IdentityHelperExtensions.GetIdentityUserClass(code.Template);

            code.AddField("string?", "message");
            code.AddField(identityClass, "user", f => f.WithAssignment(new CSharpStatement("default!")));

            code.AddProperty("InputModel", "Input", p =>
            {
                p.Private();
                p.WithInitialValue("default!");
                p.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromFormAttribute").RemoveSuffix("Attribute"));
            });
            code.AddProperty("string?", "ReturnUrl", p => p.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromQueryAttribute").RemoveSuffix("Attribute")));

            // either get the existing method or add one
            ICSharpClassMethodDeclaration onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            if (onInitializedAsync is null)
            {
                code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnInitializedAsync");
                onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            }

            onInitializedAsync.Async().Protected().Override();
            onInitializedAsync.AddStatement("Input ??= new();");
            onInitializedAsync.AddStatement("// Ensure the user has gone through the username & password screen first");
            onInitializedAsync.AddStatement("user = await SignInManager.GetTwoFactorAuthenticationUserAsync() ?? throw new InvalidOperationException(\"Unable to load two-factor authentication user.\");");

            code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnValidSubmitAsync", onValidSubmitAsync =>
            {
                onValidSubmitAsync.Private().Async();

                onValidSubmitAsync.AddAssignmentStatement("var recoveryCode", new CSharpStatement("Input.RecoveryCode.Replace(\" \", string.Empty);"));
                onValidSubmitAsync.AddAssignmentStatement("var result", new CSharpStatement("await SignInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);"));
                onValidSubmitAsync.AddAssignmentStatement("var userId", new CSharpStatement("await UserManager.GetUserIdAsync(user);"));

                onValidSubmitAsync.AddIfStatement("result.Succeeded", @if =>
                {
                    @if.AddStatement("Logger.LogInformation(\"User with ID '{UserId}' logged in with a recovery code.\", userId);");
                    @if.AddStatement("RedirectManager.RedirectTo(ReturnUrl);");
                }).AddElseIfStatement("result.IsLockedOut", eIf =>
                {
                    eIf.AddStatement("Logger.LogWarning(\"User account locked out.\");");
                    eIf.AddStatement("RedirectManager.RedirectTo(\"Account/Lockout\");");
                }).AddElseStatement(@else =>
                {
                    @else.AddStatement("Logger.LogWarning(\"Invalid recovery code entered for user with ID '{UserId}' \", userId);");
                    @else.AddStatement("message = \"Error: Invalid recovery code entered.\";");
                });
            });

            code.AddClass("InputModel", inputModel =>
            {
                inputModel.Private().Sealed();

                inputModel.AddProperty("string", "RecoveryCode", p =>
                {
                    p.AddAttribute(code.Template.UseType("System.ComponentModel.DataAnnotations.RequiredAttribute").RemoveSuffix("Attribute"));
                    p.AddAttribute("DataType(DataType.Text)");
                    p.AddAttribute("Display(Name = \"Recovery Code\")");
                    p.WithInitialValue("\"\"");
                });
            });
        }
    }
}
