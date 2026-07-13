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
    /// Default (first-generation only) content for the Login page, seeded onto the modelled
    /// <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old dedicated LoginTemplate/LoginCodeBehindTemplate pair.
    /// </summary>
    internal static class LoginPageContent
    {
        public static string BuildRazorContent(RazorComponentTemplate template)
        {
            var authService = template.GetAuthServiceInterfaceTemplateName();
            var isAspnetcoreIdentity = template.ExecutionContext.GetSettings().GetBlazor().Authentication().IsAspnetcoreIdentity();
            var isMudBlazor = template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor");

            return isMudBlazor
                ? BuildMudBlazorContent(authService, isAspnetcoreIdentity)
                : BuildBootstrapContent(authService, isAspnetcoreIdentity);
        }

        private static string BuildMudBlazorContent(string authService, bool isAspnetcoreIdentity)
        {
            var externalLoginCard = isAspnetcoreIdentity
                ? """
                    <MudItem xs="12" md="5" lg="6">
                        <MudCard Class="ux-fade-in-up" Style="animation-delay: 0.2s">
                            <MudCardContent>
                                <MudText Typo="Typo.h6">Use another service to log in</MudText>
                                <MudText Typo="Typo.body2" Class="mb-4">Choose an external provider to authenticate.</MudText>
                                <ExternalLoginPicker />
                            </MudCardContent>
                        </MudCard>
                    </MudItem>
                """
                : string.Empty;

            var head = $$"""
                @inject {{authService}} AuthService
                @inject NavigationManager NavigationManager

                <MudPaper Class="pa-4 mb-4 ux-gradient-primary" Elevation="0">
                    <MudText Typo="Typo.h4" Class="text-white font-weight-bold mb-2">
                        <MudIcon Icon="@Icons.Material.Filled.LockOpen" Class="mr-2" />
                        Welcome back
                    </MudText>
                    <MudText Typo="Typo.body1" Class="text-white opacity-90">Sign in with your local account to continue.</MudText>
                </MudPaper>

                <MudGrid Spacing="3">
                    <MudItem xs="12" md="7" lg="6">
                        <MudCard Class="ux-fade-in-up" Style="animation-delay: 0.1s">
                            <MudCardContent>
                                <StatusMessage Message="@errorMessage" />
                                <EditForm Model="Input" FormName="login" OnValidSubmit="LoginUser" method="post">
                                    <DataAnnotationsValidator />
                                    <MudGrid>
                                        <MudItem xs="12">
                                            <MudText Typo="Typo.h5">Use a local account to log in</MudText>
                                            <MudText Typo="Typo.body2" Class="mb-2">Enter your credentials below.</MudText>
                                            <ValidationSummary class="text-danger" role="alert" />
                                        </MudItem>
                                        <MudItem xs="12">
                                            <div class="login-input-field">
                                                <label class="login-input-label" for="email">Email</label>
                                                <div class="login-input-shell">
                                                    <MudIcon Icon="@Icons.Material.Filled.Email" Class="login-input-icon" />
                                                    <InputText id="email" class="login-input-control" @bind-Value="Input.Email" autocomplete="username" aria-required="true" placeholder="name@example.com" type="email" />
                                                </div>
                                                <ValidationMessage class="text-danger" For="() => Input.Email" />
                                            </div>
                                        </MudItem>
                                        <MudItem xs="12">
                                            <div class="login-input-field">
                                                <label class="login-input-label" for="password">Password</label>
                                                <div class="login-input-shell">
                                                    <MudIcon Icon="@Icons.Material.Filled.Lock" Class="login-input-icon" />
                                                    <InputText id="password" class="login-input-control" @bind-Value="Input.Password" autocomplete="current-password" aria-required="true" placeholder="Enter your password" type="password" />
                                                </div>
                                                <ValidationMessage class="text-danger" For="() => Input.Password" />
                                            </div>
                                        </MudItem>
                                        <MudItem xs="12">
                                            <div class="login-checkbox-field">
                                                <InputCheckbox id="rememberMe" class="login-checkbox-control" @bind-Value="Input.RememberMe" />
                                                <label class="login-checkbox-label" for="rememberMe">Remember me</label>
                                            </div>
                                        </MudItem>
                                        <MudItem xs="12">
                                            <MudStack Row="true" Spacing="2" Justify="Justify.FlexEnd" AlignItems="AlignItems.Center">
                                                <MudButton ButtonType="ButtonType.Submit" Color="Color.Primary" Variant="Variant.Filled" FullWidth="true" StartIcon="@Icons.Material.Filled.Login">Log in</MudButton>
                                            </MudStack>
                                        </MudItem>
                                        <MudItem xs="12">
                                            <MudStack Spacing="1">
                                                <MudLink Href="Account/ForgotPassword">Forgot your password?</MudLink>
                                                <MudLink Href="@(NavigationManager.GetUriWithQueryParameters("Account/Register", new Dictionary<string, object?> { ["ReturnUrl"] = ReturnUrl }))">Register as a new user</MudLink>
                                                <MudLink Href="Account/ResendEmailConfirmation">Resend email confirmation</MudLink>
                                            </MudStack>
                                        </MudItem>
                                    </MudGrid>
                                </EditForm>
                            </MudCardContent>
                        </MudCard>
                    </MudItem>
                """;

            const string tail = """

                </MudGrid>

                <style>
                    .login-input-field {
                        display: flex;
                        flex-direction: column;
                        gap: var(--space-2);
                    }

                    .login-input-label,
                    .login-checkbox-label {
                        color: var(--text);
                        font-size: var(--type-label-lg);
                        font-weight: 500;
                    }

                    .login-input-shell {
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

                    .login-input-shell:focus-within {
                        border-color: var(--primary);
                        box-shadow: 0 0 0 3px var(--primary-subtle), 0 0 12px var(--primary-glow);
                    }

                    .login-input-icon {
                        color: var(--text-muted);
                        flex-shrink: 0;
                    }

                    .login-input-control {
                        width: 100%;
                        min-height: 42px;
                        color: var(--text);
                        background: transparent;
                        border: none;
                        outline: none;
                    }

                    .login-input-control::placeholder {
                        color: var(--text-muted);
                    }

                    .login-checkbox-field {
                        display: flex;
                        align-items: center;
                        gap: var(--space-2);
                    }

                    .login-checkbox-control {
                        width: 1rem;
                        height: 1rem;
                        accent-color: var(--primary);
                        flex-shrink: 0;
                    }
                </style>
                """;

            return head + externalLoginCard + tail;
        }

        private static string BuildBootstrapContent(string authService, bool isAspnetcoreIdentity)
        {
            var gridClass = isAspnetcoreIdentity ? "ux-form-grid" : "ux-form-grid ux-form-narrow";

            var externalLoginSection = isAspnetcoreIdentity
                ? """
                    <div class="ux-form-col">
                        <section>
                            <div class="ux-section-head">
                                <h3>Use another service to log in</h3>
                                <p class="ux-section-subtitle">Choose an external provider to authenticate.</p>
                            </div>
                            <ExternalLoginPicker />
                        </section>
                    </div>
                """
                : string.Empty;

            var head = $$"""
                @inject {{authService}} AuthService
                @inject NavigationManager NavigationManager

                <AccountHero Icon="lock-open" Title="Welcome back" Subtitle="Sign in with your local account to continue." />

                <div class="{{gridClass}}">
                    <div class="ux-form-col">
                        <section>
                            <StatusMessage Message="@errorMessage" />
                            <EditForm Model="Input" FormName="login" OnValidSubmit="LoginUser" method="post">
                                <DataAnnotationsValidator />
                                <div class="ux-section-head">
                                    <h2>Use a local account to log in</h2>
                                    <p class="ux-section-subtitle">Enter your credentials below.</p>
                                </div>
                                <ValidationSummary class="text-danger" role="alert" />
                                <UxField Label="Email" Icon="mail" For="email">
                                    <InputText id="email" class="ux-input" @bind-Value="Input.Email" autocomplete="username" aria-required="true" placeholder="name@example.com" />
                                </UxField>
                                <ValidationMessage class="text-danger" For="() => Input.Email" />
                                <UxField Label="Password" Icon="lock" For="password">
                                    <InputText id="password" class="ux-input" type="password" @bind-Value="Input.Password" autocomplete="current-password" aria-required="true" placeholder="Enter your password" />
                                </UxField>
                                <ValidationMessage class="text-danger" For="() => Input.Password" />
                                <div class="form-check">
                                    <InputCheckbox id="rememberMe" class="form-check-input" @bind-Value="Input.RememberMe" />
                                    <label class="form-check-label" for="rememberMe">Remember me</label>
                                </div>
                                <button class="w-100 btn btn-primary" type="submit">
                                    <UxIcon Name="log-in" />
                                    Log in
                                </button>
                                <div class="ux-account-links">
                                    <a href="Account/ForgotPassword">Forgot your password?</a>
                                    <a href="@(NavigationManager.GetUriWithQueryParameters("Account/Register", new Dictionary<string, object?> { ["ReturnUrl"] = ReturnUrl }))">Register as a new user</a>
                                    <a href="Account/ResendEmailConfirmation">Resend email confirmation</a>
                                </div>
                            </EditForm>
                        </section>
                    </div>
                """;

            const string tail = """

                </div>
                """;

            return head + externalLoginSection + tail;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
            code.AddField("string?", "errorMessage");

            code.AddProperty(code.Template.UseType("Microsoft.AspNetCore.Http.HttpContext"), "HttpContext", input =>
            {
                input.Private();
                input.WithInitialValue("default!");
                input.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.CascadingParameterAttribute").RemoveSuffix("Attribute"));
            });
            code.AddProperty("InputModel", "Input", input =>
            {
                input.Private();
                // If you are getting a build warning (BL0008) here, the simple standard MSFT solution is applied in the static content files instead — a C# weaver issue prevents this initializer (new() -> default!) from migrating cleanly for pre-existing codebases.
                input.WithInitialValue("new()");
                input.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromFormAttribute").RemoveSuffix("Attribute"));
            });
            code.AddProperty("string?", "ReturnUrl", input =>
            {
                input.Private();
                input.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromQueryAttribute").RemoveSuffix("Attribute"));
            });


            code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "LoginUser", onValidSubmitAsync =>
            {
                onValidSubmitAsync.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Mvc.IgnoreAntiforgeryTokenAttribute").RemoveSuffix("Attribute"));
                onValidSubmitAsync.Async();

                onValidSubmitAsync.AddStatement("await AuthService.Login(Input.Email, Input.Password, Input.RememberMe, ReturnUrl);");
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
                    email.AddAttribute("DataType(DataType.Password)");
                    email.WithInitialValue("\"\"");
                });

                inputModel.AddProperty("bool", "RememberMe", email =>
                {
                    email.AddAttribute("Display(Name = \"Remember me?\")");
                });
            });
        }
    }
}
