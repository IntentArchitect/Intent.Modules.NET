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
    /// Default (first-generation only) content for the Manage/DeletePersonalData page, seeded onto
    /// the modelled <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static content/Components(MudBlazor)/Account/Pages/Manage/DeletePersonalData pair.
    /// </summary>
    internal static class ManageDeletePersonalDataPageContent
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
            """;

        private static string BuildMudBlazorContent(string identityClass, string userAccessor, string redirectManager)
        {
            return $$"""
                @using Microsoft.AspNetCore.Authorization
                @using Microsoft.AspNetCore.Identity
                @inject UserManager<{{identityClass}}> UserManager
                @inject SignInManager<{{identityClass}}> SignInManager
                @inject {{userAccessor}} UserAccessor
                @inject {{redirectManager}} RedirectManager
                @inject ILogger<DeletePersonalData> Logger

                <MudPaper Class="pa-4 mb-4 ux-gradient-primary"
                    Elevation="0">
                    <MudText Typo="Typo.h4"
                        Class="text-white font-weight-bold mb-2">
                        <MudIcon Icon="@Icons.Material.Filled.DeleteForever"
                            Class="mr-2" />
                        Delete personal data
                    </MudText>
                    <MudText Typo="Typo.body1"
                        Class="text-white opacity-90">
                        Permanently remove your account and all associated personal data.
                    </MudText>
                </MudPaper>

                <StatusMessage Message="@message" />

                <MudAlert Severity="Severity.Warning"
                    Class="mb-4">
                    Deleting this data will permanently remove your account, and this cannot be recovered.
                </MudAlert>

                <MudCard Class="ux-fade-in-up auth-form-shell"
                    Style="animation-delay: 0.1s"
                    Outlined="true">
                    <MudCardContent>
                        <EditForm Model="Input"
                            FormName="delete-user"
                            OnValidSubmit="OnValidSubmitAsync"
                            method="post">
                            <DataAnnotationsValidator />
                            <ValidationSummary class="text-danger"
                                role="alert" />

                            <MudGrid>
                                <MudItem xs="12">
                                    <MudText Typo="Typo.h5">Confirm account deletion</MudText>
                                    <MudText Typo="Typo.body2"
                                        Class="mb-4">
                                        Review this action carefully before continuing.
                                    </MudText>
                                </MudItem>
                                @if (requirePassword)
                                {
                                    <MudItem xs="12">
                                        <MudTextField T="string"
                                            @bind-Value="Input.Password"
                                            Label="Password"
                                            Variant="Variant.Outlined"
                                            Adornment="Adornment.Start"
                                            AdornmentIcon="@Icons.Material.Filled.Lock"
                                            InputType="InputType.Password"
                                            Immediate="true"
                                            For="@(() => Input.Password)" />
                                        <ValidationMessage For="() => Input.Password"
                                            class="text-danger" />
                                    </MudItem>
                                }
                                <MudItem xs="12">
                                    <MudStack Row="true"
                                        Justify="Justify.FlexEnd">
                                        <MudButton ButtonType="ButtonType.Submit"
                                            Variant="Variant.Filled"
                                            Color="Color.Error"
                                            StartIcon="@Icons.Material.Filled.DeleteForever">
                                            Delete data and close my account
                                        </MudButton>
                                    </MudStack>
                                </MudItem>
                            </MudGrid>
                        </EditForm>
                    </MudCardContent>
                </MudCard>
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
                @inject ILogger<DeletePersonalData> Logger

                <StatusMessage Message="@message" />

                <div class="ux-section-head">
                    <h3>Confirm account deletion</h3>
                    <p class="ux-section-subtitle">Review this action carefully before continuing.</p>
                </div>

                <div class="ux-callout ux-callout-warning"
                    role="alert">
                    <span class="ux-callout-icon"><UxIcon Name="alert" /></span>
                    <div class="ux-callout-body">
                        <strong>Deleting this data will permanently remove your account, and this cannot be recovered.</strong>
                    </div>
                </div>

                <EditForm Model="Input"
                    FormName="delete-user"
                    OnValidSubmit="OnValidSubmitAsync"
                    method="post">
                    <DataAnnotationsValidator />
                    <ValidationSummary class="text-danger"
                        role="alert" />
                    @if (requirePassword)
                    {
                        <UxField Label="Password"
                            Icon="lock"
                            For="password">
                            <InputText id="password"
                                type="password"
                                @bind-Value="Input.Password"
                                class="ux-input"
                                autocomplete="current-password"
                                aria-required="true"
                                placeholder="Enter your password" />
                        </UxField>
                        <ValidationMessage For="() => Input.Password"
                            class="text-danger" />
                    }
                    <button class="btn btn-outline-danger"
                        type="submit">
                        <UxIcon Name="trash" />
                        Delete data and close my account
                    </button>
                </EditForm>
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
            var identityClass = code.Template.ExecutionContext.InstalledModules.Any(im => im.ModuleId == "Intent.AspNetCore.Identity") ?
                IdentityHelperExtensions.GetIdentityUserClass(code.Template) :
                "ApplicationUser";

            code.AddField("string?", "message");
            code.AddField(identityClass, "user", f => f.WithAssignment(new CSharpStatement("default!")));
            code.AddField("bool", "requirePassword");

            code.AddProperty(code.Template.UseType("Microsoft.AspNetCore.Http.HttpContext?"), "HttpContext", p => p.WithInitialValue("default!").AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.CascadingParameterAttribute").RemoveSuffix("Attribute")));
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
            onInitializedAsync.AddAssignmentStatement("requirePassword", new CSharpStatement("await UserManager.HasPasswordAsync(user);"));

            code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnValidSubmitAsync", onValidSubmitAsync =>
            {
                onValidSubmitAsync.Private().Async();

                onValidSubmitAsync.AddIfStatement("requirePassword && !await UserManager.CheckPasswordAsync(user, Input.Password)", @if =>
                {
                    @if.AddStatement("message = \"Error: Incorrect password.\";");
                    @if.AddStatement("return;");
                });

                onValidSubmitAsync.AddAssignmentStatement("var result", new CSharpStatement("await UserManager.DeleteAsync(user);"));
                onValidSubmitAsync.AddIfStatement("!result.Succeeded", @if =>
                {
                    @if.AddStatement("throw new InvalidOperationException(\"Unexpected error occurred deleting user.\");");
                });

                onValidSubmitAsync.AddStatement("await SignInManager.SignOutAsync();");
                onValidSubmitAsync.AddAssignmentStatement("var userId", new CSharpStatement("await UserManager.GetUserIdAsync(user);"));
                onValidSubmitAsync.AddStatement("Logger.LogInformation(\"User with ID '{UserId}' deleted themselves.\", userId);");
                onValidSubmitAsync.AddStatement("RedirectManager.RedirectToCurrentPage();");
            });

            code.AddClass("InputModel", inputModel =>
            {
                inputModel.Private().Sealed();

                inputModel.AddProperty("string", "Password", p =>
                {
                    code.Template.AddUsing("System.ComponentModel.DataAnnotations");

                    p.AddAttribute("DataType(DataType.Password)");
                    p.WithInitialValue("\"\"");
                });
            });
        }
    }
}
