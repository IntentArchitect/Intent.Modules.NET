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
    /// Default (first-generation only) content for the Register page, seeded onto the modelled
    /// <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old dedicated RegisterTemplate/RegisterCodeBehindTemplate pair.
    /// </summary>
    internal static class RegisterPageContent
    {
        public static string BuildRazorContent(RazorComponentTemplate template)
        {
            var isAspnetcoreIdentity = template.ExecutionContext.GetSettings().GetBlazor().Authentication().IsAspnetcoreIdentity();
            var isMudBlazor = template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor");

            return isMudBlazor
                ? BuildMudBlazorContent(isAspnetcoreIdentity)
                : BuildBootstrapContent(isAspnetcoreIdentity);
        }

        private static string BuildMudBlazorContent(bool isAspnetcoreIdentity)
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

            const string head = """
                <MudPaper Class="pa-4 mb-4 ux-gradient-primary" Elevation="0">
                    <MudText Typo="Typo.h4" Class="text-white font-weight-bold mb-2">
                        <MudIcon Icon="@Icons.Material.Filled.PersonAdd" Class="mr-2" />
                        Create your account
                    </MudText>
                    <MudText Typo="Typo.body1" Class="text-white opacity-90">Register with your email address to continue.</MudText>
                </MudPaper>

                <MudGrid Spacing="3">
                    <MudItem xs="12" md="7" lg="6">
                        <MudCard Class="ux-fade-in-up" Style="animation-delay: 0.1s">
                            <MudCardContent>
                                <StatusMessage Message="@Message" />
                                <EditForm Model="Input" FormName="register" OnValidSubmit="RegisterUser" method="post" asp-route-returnUrl="@ReturnUrl">
                                    <DataAnnotationsValidator />
                                    <MudGrid>
                                        <MudItem xs="12">
                                            <MudText Typo="Typo.h5">Create a new account</MudText>
                                            <MudText Typo="Typo.body2" Class="mb-2">Enter your details below.</MudText>
                                            <ValidationSummary class="text-danger" role="alert" />
                                        </MudItem>
                                        <MudItem xs="12">
                                            <div class="register-input-field">
                                                <label class="register-input-label" for="email">Email</label>
                                                <div class="register-input-shell">
                                                    <MudIcon Icon="@Icons.Material.Filled.Email" Class="register-input-icon" />
                                                    <InputText id="email" class="register-input-control" @bind-Value="Input.Email" autocomplete="username" aria-required="true" placeholder="name@example.com" type="email" />
                                                </div>
                                                <ValidationMessage class="text-danger" For="() => Input.Email" />
                                            </div>
                                        </MudItem>
                                        <MudItem xs="12">
                                            <div class="register-input-field">
                                                <label class="register-input-label" for="password">Password</label>
                                                <div class="register-input-shell">
                                                    <MudIcon Icon="@Icons.Material.Filled.Lock" Class="register-input-icon" />
                                                    <InputText id="password" class="register-input-control" @bind-Value="Input.Password" autocomplete="new-password" aria-required="true" placeholder="Enter your password" type="password" />
                                                </div>
                                                <ValidationMessage class="text-danger" For="() => Input.Password" />
                                            </div>
                                        </MudItem>
                                        <MudItem xs="12">
                                            <div class="register-input-field">
                                                <label class="register-input-label" for="confirm-password">Confirm Password</label>
                                                <div class="register-input-shell">
                                                    <MudIcon Icon="@Icons.Material.Filled.LockReset" Class="register-input-icon" />
                                                    <InputText id="confirm-password" class="register-input-control" @bind-Value="Input.ConfirmPassword" autocomplete="new-password" aria-required="true" placeholder="Confirm your password" type="password" />
                                                </div>
                                                <ValidationMessage class="text-danger" For="() => Input.ConfirmPassword" />
                                            </div>
                                        </MudItem>
                                        <MudItem xs="12">
                                            <MudStack Row="true" Spacing="2" Justify="Justify.FlexEnd" AlignItems="AlignItems.Center">
                                                <MudButton ButtonType="ButtonType.Submit" Color="Color.Primary" Variant="Variant.Filled" FullWidth="true" StartIcon="@Icons.Material.Filled.HowToReg">Register</MudButton>
                                            </MudStack>
                                        </MudItem>
                                        <MudItem xs="12">
                                            <MudStack Spacing="1">
                                                <MudLink Href="Account/Login">Already have an account? Log in</MudLink>
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
                    .register-input-field {
                        display: flex;
                        flex-direction: column;
                        gap: var(--space-2);
                    }

                    .register-input-label {
                        color: var(--text);
                        font-size: var(--type-label-lg);
                        font-weight: 500;
                    }

                    .register-input-shell {
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

                    .register-input-shell:focus-within {
                        border-color: var(--primary);
                        box-shadow: 0 0 0 3px var(--primary-subtle), 0 0 12px var(--primary-glow);
                    }

                    .register-input-icon {
                        color: var(--text-muted);
                        flex-shrink: 0;
                    }

                    .register-input-control {
                        width: 100%;
                        min-height: 42px;
                        color: var(--text);
                        background: transparent;
                        border: none;
                        outline: none;
                    }

                    .register-input-control::placeholder {
                        color: var(--text-muted);
                    }
                </style>
                """;

            return head + externalLoginCard + tail;
        }

        private static string BuildBootstrapContent(bool isAspnetcoreIdentity)
        {
            var gridClass = isAspnetcoreIdentity ? "ux-form-grid" : "ux-form-grid ux-form-narrow";

            var externalLoginSection = isAspnetcoreIdentity
                ? """
                    <div class="ux-form-col">
                        <section>
                            <div class="ux-section-head">
                                <h3>Use another service to register</h3>
                                <p class="ux-section-subtitle">Sign up with an external provider.</p>
                            </div>
                            <ExternalLoginPicker />
                        </section>
                    </div>
                """
                : string.Empty;

            var head = $$"""
                <AccountHero Icon="user-plus" Title="Create your account" Subtitle="Get started with a new local account." />

                <div class="{{gridClass}}">
                    <div class="ux-form-col">
                        <section>
                            <StatusMessage Message="@Message" />
                            <EditForm Model="Input" FormName="register" OnValidSubmit="RegisterUser" method="post" asp-route-returnUrl="@ReturnUrl">
                                <DataAnnotationsValidator />
                                <div class="ux-section-head">
                                    <h2>Create a new account</h2>
                                    <p class="ux-section-subtitle">It only takes a moment.</p>
                                </div>
                                <ValidationSummary class="text-danger" role="alert" />
                                <UxField Label="Email" Icon="mail" For="email">
                                    <InputText id="email" class="ux-input" @bind-Value="Input.Email" autocomplete="username" aria-required="true" placeholder="name@example.com" />
                                </UxField>
                                <ValidationMessage class="text-danger" For="() => Input.Email" />
                                <UxField Label="Password" Icon="lock" For="password">
                                    <InputText id="password" class="ux-input" type="password" @bind-Value="Input.Password" autocomplete="new-password" aria-required="true" placeholder="Create a password" />
                                </UxField>
                                <ValidationMessage class="text-danger" For="() => Input.Password" />
                                <UxField Label="Confirm Password" Icon="lock" For="confirm-password">
                                    <InputText id="confirm-password" class="ux-input" type="password" @bind-Value="Input.ConfirmPassword" autocomplete="new-password" aria-required="true" placeholder="Confirm your password" />
                                </UxField>
                                <ValidationMessage class="text-danger" For="() => Input.ConfirmPassword" />
                                <button class="w-100 btn btn-primary" type="submit">
                                    <UxIcon Name="user-plus" />
                                    Register
                                </button>
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
            code.AddField($"System.Collections.Generic.IEnumerable<{code.Template.UseType("Microsoft.AspNetCore.Identity.IdentityError")}>?", "identityErrors");

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

            code.AddProperty("string?", "Message", p => p.Private().WithoutSetter().Getter.WithExpressionImplementation("identityErrors is null ? null : $\"Error: {string.Join(\", \", identityErrors.Select(error => error.Description))}\""));

            code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "RegisterUser", onValidSubmitAsync =>
            {
                onValidSubmitAsync.Async();

                onValidSubmitAsync.AddStatement("await AuthService.Register(Input.Email, Input.Password, ReturnUrl);");
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
            });
        }
    }
}
