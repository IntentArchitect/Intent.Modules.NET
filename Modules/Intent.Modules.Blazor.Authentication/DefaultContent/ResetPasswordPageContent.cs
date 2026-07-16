using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AuthServiceInterface;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using System.Linq;

namespace Intent.Modules.Blazor.Authentication.DefaultContent
{
    /// <summary>
    /// Default (first-generation only) content for the ResetPassword page, seeded onto the modelled
    /// <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old dedicated ResetPasswordTemplate/ResetPasswordCodeBehindTemplate pair.
    /// </summary>
    internal static class ResetPasswordPageContent
    {
        public static string BuildRazorContent(RazorComponentTemplate template)
        {
            var authServiceInterfaceTemplate = template.ExecutionContext.FindTemplateInstance(AuthServiceInterfaceTemplate.TemplateId);
            var authServiceInterfaceBuilder = authServiceInterfaceTemplate as ICSharpFileBuilderTemplate;

            var authService = template.GetAuthServiceInterfaceTemplateName();
            var redirectManager = template.GetIdentityRedirectManagerTemplateName();
            var isMudBlazor = template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor");

            return isMudBlazor
                ? BuildMudBlazorContent(authService, redirectManager, authServiceInterfaceBuilder.Namespace ?? "")
                : BuildBootstrapContent(authService, redirectManager, authServiceInterfaceBuilder.Namespace ?? "");
        }

        private static string BuildMudBlazorContent(string authService, string redirectManager, string authServiceNamespace)
        {
            return $$"""
                @using {{authServiceNamespace}}
                @inject {{authService}} AuthService
                @inject {{redirectManager}} RedirectManager

                <MudPaper Class="pa-4 mb-4 ux-gradient-primary"
                          Elevation="0">
                    <MudText Typo="Typo.h4"
                             Class="text-white font-weight-bold mb-2">
                        <MudIcon Icon="@Icons.Material.Filled.Password"
                                 Class="mr-2" />
                        Reset password
                    </MudText>
                    <MudText Typo="Typo.body1"
                             Class="text-white opacity-90">
                        Enter your email address and choose a new password.
                    </MudText>
                </MudPaper>

                <MudGrid Spacing="3">
                    <MudItem xs="12"
                             md="7"
                             lg="6">
                        <MudCard Class="ux-fade-in-up"
                                 Style="animation-delay: 0.1s">
                            <MudCardContent>
                                <StatusMessage Message="@Message" />
                                <EditForm Model="Input"
                                          FormName="reset-password"
                                          OnValidSubmit="OnValidSubmitAsync"
                                          method="post">
                                    <DataAnnotationsValidator />
                                    <MudGrid>
                                        <MudItem xs="12">
                                            <MudText Typo="Typo.h5">Reset your password</MudText>
                                            <MudText Typo="Typo.body2"
                                                     Class="mb-2">
                                                Enter your email address and your new password below.
                                            </MudText>
                                            <ValidationSummary class="text-danger"
                                                               role="alert" />
                                        </MudItem>
                                        <MudItem xs="12">
                                            <div class="reset-password-input-field">
                                                <label class="reset-password-input-label"
                                                       for="email">
                                                    Email
                                                </label>
                                                <div class="reset-password-input-shell">
                                                    <MudIcon Icon="@Icons.Material.Filled.Email"
                                                             Class="reset-password-input-icon" />
                                                    <InputText id="email"
                                                               class="reset-password-input-control"
                                                               @bind-Value="Input.Email"
                                                               autocomplete="username"
                                                               aria-required="true"
                                                               placeholder="name@example.com"
                                                               type="email" />
                                                </div>
                                                <ValidationMessage class="text-danger"
                                                                   For="() => Input.Email" />
                                            </div>
                                        </MudItem>
                                        <MudItem xs="12">
                                            <div class="reset-password-input-field">
                                                <label class="reset-password-input-label"
                                                       for="password">
                                                    Password
                                                </label>
                                                <div class="reset-password-input-shell">
                                                    <MudIcon Icon="@Icons.Material.Filled.Lock"
                                                             Class="reset-password-input-icon" />
                                                    <InputText id="password"
                                                               class="reset-password-input-control"
                                                               @bind-Value="Input.Password"
                                                               autocomplete="new-password"
                                                               aria-required="true"
                                                               placeholder="Enter your new password"
                                                               type="password" />
                                                </div>
                                                <ValidationMessage class="text-danger"
                                                                   For="() => Input.Password" />
                                            </div>
                                        </MudItem>
                                        <MudItem xs="12">
                                            <div class="reset-password-input-field">
                                                <label class="reset-password-input-label"
                                                       for="confirm-password">
                                                    Confirm password
                                                </label>
                                                <div class="reset-password-input-shell">
                                                    <MudIcon Icon="@Icons.Material.Filled.LockReset"
                                                             Class="reset-password-input-icon" />
                                                    <InputText id="confirm-password"
                                                               class="reset-password-input-control"
                                                               @bind-Value="Input.ConfirmPassword"
                                                               autocomplete="new-password"
                                                               aria-required="true"
                                                               placeholder="Confirm your new password"
                                                               type="password" />
                                                </div>
                                                <ValidationMessage class="text-danger"
                                                                   For="() => Input.ConfirmPassword" />
                                            </div>
                                        </MudItem>
                                        <MudItem xs="12">
                                            <InputText id="code"
                                                       class="d-none"
                                                       @bind-Value="Input.Code"
                                                       type="hidden" />
                                        </MudItem>
                                        <MudItem xs="12">
                                            <MudStack Row="true"
                                                      Spacing="2"
                                                      Justify="Justify.FlexEnd"
                                                      AlignItems="AlignItems.Center">
                                                <MudButton ButtonType="ButtonType.Submit"
                                                           Color="Color.Primary"
                                                           Variant="Variant.Filled"
                                                           FullWidth="true"
                                                           StartIcon="@Icons.Material.Filled.SaveAs">
                                                    Reset password
                                                </MudButton>
                                            </MudStack>
                                        </MudItem>
                                        <MudItem xs="12">
                                            <MudStack Spacing="1">
                                                <MudLink Href="Account/Login">Back to log in</MudLink>
                                            </MudStack>
                                        </MudItem>
                                    </MudGrid>
                                </EditForm>
                            </MudCardContent>
                        </MudCard>
                    </MudItem>
                </MudGrid>

                <style>
                    .reset-password-input-field {
                        display: flex;
                        flex-direction: column;
                        gap: var(--space-2);
                    }

                    .reset-password-input-label {
                        color: var(--text);
                        font-size: var(--type-label-lg);
                        font-weight: 500;
                    }

                    .reset-password-input-shell {
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

                    .reset-password-input-shell:focus-within {
                        border-color: var(--primary);
                        box-shadow: 0 0 0 3px var(--primary-subtle), 0 0 12px var(--primary-glow);
                    }

                    .reset-password-input-icon {
                        color: var(--text-muted);
                        flex-shrink: 0;
                    }

                    .reset-password-input-control {
                        width: 100%;
                        min-height: 42px;
                        color: var(--text);
                        background: transparent;
                        border: none;
                        outline: none;
                    }

                    .reset-password-input-control::placeholder {
                        color: var(--text-muted);
                    }
                </style>
                """;
        }

        private static string BuildBootstrapContent(string authService, string redirectManager, string authServiceNamespace)
        {
            return $$"""
                @using {{authServiceNamespace}}
                @inject {{authService}} AuthService
                @inject {{redirectManager}} RedirectManager

                <AccountHero Icon="lock"
                             Title="Reset your password"
                             Subtitle="Choose a new password for your account." />

                <div class="ux-form-narrow">
                    <section>
                        <EditForm Model="Input"
                                  FormName="reset-password"
                                  OnValidSubmit="OnValidSubmitAsync"
                                  method="post">
                            <DataAnnotationsValidator />
                            <ValidationSummary class="text-danger"
                                               role="alert" />
                            <UxField Label="Email"
                                     Icon="mail"
                                     For="email">
                                <InputText id="email"
                                           class="ux-input"
                                           @bind-Value="Input.Email"
                                           autocomplete="username"
                                           aria-required="true"
                                           placeholder="name@example.com" />
                            </UxField>
                            <ValidationMessage class="text-danger"
                                               For="() => Input.Email" />
                            <UxField Label="Password"
                                     Icon="lock"
                                     For="password">
                                <InputText id="password"
                                           class="ux-input"
                                           type="password"
                                           @bind-Value="Input.Password"
                                           autocomplete="new-password"
                                           aria-required="true"
                                           placeholder="Enter your new password" />
                            </UxField>
                            <ValidationMessage class="text-danger"
                                               For="() => Input.Password" />
                            <UxField Label="Confirm password"
                                     Icon="lock"
                                     For="confirm-password">
                                <InputText id="confirm-password"
                                           class="ux-input"
                                           type="password"
                                           @bind-Value="Input.ConfirmPassword"
                                           autocomplete="new-password"
                                           aria-required="true"
                                           placeholder="Confirm your new password" />
                            </UxField>
                            <ValidationMessage class="text-danger"
                                               For="() => Input.ConfirmPassword" />
                            <button class="w-100 btn btn-primary"
                                    type="submit">
                                <UxIcon Name="lock" />
                                Reset password
                            </button>
                        </EditForm>
                    </section>
                </div>
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
            code.AddField($"System.Collections.Generic.IEnumerable<{code.Template.UseType("Microsoft.AspNetCore.Identity.IdentityError")}>?", "identityErrors");

            code.AddProperty("InputModel", "Input", input =>
            {
                input.Private();
                // If you are getting a build warning (BL0008) here, the simple standard MSFT solution is applied in the static content files instead — a C# weaver issue prevents this initializer (new() -> default!) from migrating cleanly for pre-existing codebases.
                input.WithInitialValue("new()");
                input.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromFormAttribute").RemoveSuffix("Attribute"));
            });

            code.AddProperty("string?", "Code", input =>
            {
                input.Private();
                input.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromQueryAttribute").RemoveSuffix("Attribute"));
            });

            code.AddProperty("string?", "Message", p => p.Private().WithoutSetter().Getter.WithExpressionImplementation("identityErrors is null ? null : $\"Error: {string.Join(\", \", identityErrors.Select(error => error.Description))}\""));

            code.AddMethod("void", "OnInitialized", onValidSubmitAsync =>
            {
                onValidSubmitAsync.Protected().Override();

                onValidSubmitAsync.AddIfStatement("Code is null", @if =>
                {
                    @if.AddStatement("RedirectManager.RedirectTo(\"Account/InvalidPasswordReset\");");
                });

                onValidSubmitAsync.AddStatement($"Input.Code = {code.Template.UseType("System.Text.Encoding")}.UTF8.GetString({code.Template.UseType("Microsoft.AspNetCore.WebUtilities.WebEncoders")}.Base64UrlDecode(Code));");
            });

            code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnValidSubmitAsync", onValidSubmitAsync =>
            {
                onValidSubmitAsync.Private().Async();

                onValidSubmitAsync.AddStatement("await AuthService.ResetPassword(Input.Email, Input.Code, Input.Password);");
            });

            code.AddClass("InputModel", inputModel =>
            {
                inputModel.Private().Sealed();

                inputModel.AddProperty("string", "Email", email =>
                {
                    email.AddAttribute(code.Template.UseType("System.ComponentModel.DataAnnotations.RequiredAttribute").RemoveSuffix("Attribute"));
                    email.AddAttribute(code.Template.UseType("System.ComponentModel.DataAnnotations.EmailAddressAttribute").RemoveSuffix("Attribute"));
                    email.WithInitialValue("\"\"");
                });

                inputModel.AddProperty("string", "Password", email =>
                {
                    email.AddAttribute(code.Template.UseType("System.ComponentModel.DataAnnotations.RequiredAttribute").RemoveSuffix("Attribute"));
                    email.AddAttribute("StringLength(100, ErrorMessage = \"The {0} must be at least {2} and at max {1} characters long.\", MinimumLength = 6)");
                    email.AddAttribute("DataType(DataType.Password)");
                    email.WithInitialValue("\"\"");
                });

                inputModel.AddProperty("string", "ConfirmPassword", email =>
                {
                    email.AddAttribute("DataType(DataType.Password)");
                    email.AddAttribute("Display(Name = \"Confirm password\")");
                    email.AddAttribute("Compare(\"Password\", ErrorMessage = \"The password and confirmation password do not match.\")");
                    email.WithInitialValue("\"\"");
                });

                inputModel.AddProperty("string", "Code", email =>
                {
                    email.AddAttribute(code.Template.UseType("System.ComponentModel.DataAnnotations.RequiredAttribute").RemoveSuffix("Attribute"));
                    email.WithInitialValue("\"\"");
                });
            });
        }
    }
}
