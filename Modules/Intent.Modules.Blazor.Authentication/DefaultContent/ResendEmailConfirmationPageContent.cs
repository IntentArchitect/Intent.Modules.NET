using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AuthServiceInterface;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Blazor.Templates.Templates.Client;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using System.Linq;

namespace Intent.Modules.Blazor.Authentication.DefaultContent
{
    /// <summary>
    /// Default (first-generation only) content for the ResendEmailConfirmation page, seeded onto the
    /// modelled <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old dedicated ResendEmailConfirmationTemplate/ResendEmailConfirmationCodeBehindTemplate pair.
    /// </summary>
    internal static class ResendEmailConfirmationPageContent
    {
        public static string BuildRazorContent(RazorComponentTemplateBase<ComponentModel> template)
        {
            var authServiceInterfaceTemplate = template.ExecutionContext.FindTemplateInstance(AuthServiceInterfaceTemplate.TemplateId);
            var authServiceInterfaceBuilder = authServiceInterfaceTemplate as ICSharpFileBuilderTemplate;

            var authService = template.GetAuthServiceInterfaceTemplateName();
            var isMudBlazor = template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor");

            return isMudBlazor
                ? BuildMudBlazorContent(authService, authServiceInterfaceBuilder.Namespace ?? "")
                : BuildBootstrapContent(authService, authServiceInterfaceBuilder.Namespace ?? "");
        }

        public static string? BuildStyleContent(RazorComponentTemplateBase<ComponentModel> template)
        {
            var isMudBlazor = template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor");
            return isMudBlazor ? MudBlazorStyle : null;
        }

        private const string MudBlazorStyle = """
            .resend-email-input-field {
                display: flex;
                flex-direction: column;
                gap: var(--space-2);
            }

            .resend-email-input-label {
                color: var(--text);
                font-size: var(--type-label-lg);
                font-weight: 500;
            }

            .resend-email-input-shell {
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

            .resend-email-input-shell:focus-within {
                border-color: var(--primary);
                box-shadow: 0 0 0 3px var(--primary-subtle), 0 0 12px var(--primary-glow);
            }

            ::deep .resend-email-input-icon {
                color: var(--text-muted);
                flex-shrink: 0;
            }

            ::deep .resend-email-input-control {
                width: 100%;
                min-height: 42px;
                color: var(--text);
                background: transparent;
                border: none;
                outline: none;
            }

            ::deep .resend-email-input-control::placeholder {
                color: var(--text-muted);
            }
            """;

        private static string BuildMudBlazorContent(string authService, string authServiceNamespace)
        {
            return $$"""
                @using {{authServiceNamespace}}
                @inject {{authService}} AuthService

                <MudPaper Class="pa-4 mb-4 ux-gradient-primary"
                    Elevation="0">
                    <MudText Typo="Typo.h4"
                        Class="text-white font-weight-bold mb-2">
                        <MudIcon Icon="@Icons.Material.Filled.Mail"
                            Class="mr-2" />
                        Resend email confirmation
                    </MudText>
                    <MudText Typo="Typo.body1"
                        Class="text-white opacity-90">
                        Enter your email address and we will send you another confirmation email.
                    </MudText>
                </MudPaper>

                <MudGrid Spacing="3">
                    <MudItem xs="12"
                        md="7"
                        lg="6">
                        <MudCard Class="ux-fade-in-up"
                            Style="animation-delay: 0.1s">
                            <MudCardContent>
                                <StatusMessage Message="@message" />
                                <EditForm Model="Input"
                                    FormName="resend-email-confirmation"
                                    OnValidSubmit="OnValidSubmitAsync"
                                    method="post">
                                    <DataAnnotationsValidator />
                                    <MudGrid>
                                        <MudItem xs="12">
                                            <MudText Typo="Typo.h5">Send another confirmation email</MudText>
                                            <MudText Typo="Typo.body2"
                                                Class="mb-2">
                                                Enter the email address you used when registering.
                                            </MudText>
                                            <ValidationSummary class="text-danger"
                                                role="alert" />
                                        </MudItem>
                                        <MudItem xs="12">
                                            <div class="resend-email-input-field">
                                                <label class="resend-email-input-label"
                                                    for="email">
                                                    Email
                                                </label>
                                                <div class="resend-email-input-shell">
                                                    <MudIcon Icon="@Icons.Material.Filled.Email"
                                                        Class="resend-email-input-icon" />
                                                    <InputText id="email"
                                                        class="resend-email-input-control"
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
                                            <MudStack Row="true"
                                                Spacing="2"
                                                Justify="Justify.FlexEnd"
                                                AlignItems="AlignItems.Center">
                                                <MudButton ButtonType="ButtonType.Submit"
                                                    Color="Color.Primary"
                                                    Variant="Variant.Filled"
                                                    FullWidth="true"
                                                    StartIcon="@Icons.Material.Filled.Send">
                                                    Resend
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
                """;
        }

        private static string BuildBootstrapContent(string authService, string authServiceNamespace)
        {
            return $$"""
                @using {{authServiceNamespace}}
                @inject {{authService}} AuthService

                <AccountHero Icon="mail"
                    Title="Resend email confirmation"
                    Subtitle="Enter your email to receive a new confirmation link." />

                <div class="ux-form-narrow">
                    <section>
                        <div class="ux-section-head">
                            <h2>Send another confirmation email</h2>
                            <p class="ux-section-subtitle">Enter the email address you used when registering.</p>
                        </div>
                        <EditForm Model="Input"
                            FormName="resend-email-confirmation"
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
                            <button class="w-100 btn btn-primary"
                                type="submit">
                                <UxIcon Name="mail" />
                                Resend
                            </button>
                            <div class="ux-account-links">
                                <a href="Account/Login">Back to log in</a>
                            </div>
                        </EditForm>
                    </section>
                </div>
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
            code.AddField("string?", "message");

            code.AddProperty("InputModel", "Input", input =>
            {
                input.Private();
                // If you are getting a build warning (BL0008) here, the simple standard MSFT solution is applied in the static content files instead — a C# weaver issue prevents this initializer (new() -> default!) from migrating cleanly for pre-existing codebases.
                input.WithInitialValue("new()");
                input.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromFormAttribute").RemoveSuffix("Attribute"));
            });

            code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnValidSubmitAsync", onValidSubmitAsync =>
            {
                onValidSubmitAsync.Private().Async();

                onValidSubmitAsync.AddStatement("await AuthService.ResendEmailConfirmation(Input.Email);");
                onValidSubmitAsync.AddStatement("message = \"Verification email sent. Please check your email.\";");
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
            });
        }
    }
}
