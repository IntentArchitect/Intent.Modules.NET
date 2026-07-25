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
    /// Default (first-generation only) content for the Manage/SetPassword page, seeded onto the
    /// modelled <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static content/Components(MudBlazor)/Account/Pages/Manage/SetPassword pair.
    /// </summary>
    internal static class ManageSetPasswordPageContent
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

                <MudPaper Class="pa-4 mb-4 ux-gradient-primary"
                    Elevation="0">
                    <MudText Typo="Typo.h4"
                        Class="text-white font-weight-bold mb-2">
                        <MudIcon Icon="@Icons.Material.Filled.Password"
                            Class="mr-2" />
                        Set password
                    </MudText>
                    <MudText Typo="Typo.body1"
                        Class="text-white opacity-90">
                        Add a local password so you can sign in without an external provider.
                    </MudText>
                </MudPaper>

                <StatusMessage Message="@message" />
                <MudAlert Severity="Severity.Info"
                    Class="mb-4">
                    You do not have a local username/password for this site. Add a local
                    account so you can log in without an external login.
                </MudAlert>

                <MudCard Class="ux-fade-in-up auth-form-shell"
                    Style="animation-delay: 0.1s"
                    Outlined="true">
                    <MudCardContent>
                        <EditForm Model="Input"
                            FormName="set-password"
                            OnValidSubmit="OnValidSubmitAsync"
                            method="post">
                            <DataAnnotationsValidator />
                            <ValidationSummary class="text-danger"
                                role="alert" />

                            <MudGrid>
                                <MudItem xs="12">
                                    <MudText Typo="Typo.h5">Set your password</MudText>
                                    <MudText Typo="Typo.body2"
                                        Class="mb-4">
                                        Choose a password for future sign-ins.
                                    </MudText>
                                </MudItem>
                                <MudItem xs="12">
                                    <MudTextField T="string"
                                        @bind-Value="Input.NewPassword"
                                        Label="New password"
                                        Variant="Variant.Outlined"
                                        Adornment="Adornment.Start"
                                        AdornmentIcon="@Icons.Material.Filled.Lock"
                                        InputType="InputType.Password"
                                        Immediate="true"
                                        For="@(() => Input.NewPassword)" />
                                    <ValidationMessage For="() => Input.NewPassword"
                                        class="text-danger" />
                                </MudItem>
                                <MudItem xs="12">
                                    <MudTextField T="string"
                                        @bind-Value="Input.ConfirmPassword"
                                        Label="Confirm password"
                                        Variant="Variant.Outlined"
                                        Adornment="Adornment.Start"
                                        AdornmentIcon="@Icons.Material.Filled.LockReset"
                                        InputType="InputType.Password"
                                        Immediate="true"
                                        For="@(() => Input.ConfirmPassword)" />
                                    <ValidationMessage For="() => Input.ConfirmPassword"
                                        class="text-danger" />
                                </MudItem>
                                <MudItem xs="12">
                                    <MudStack Row="true"
                                        Justify="Justify.FlexEnd">
                                        <MudButton ButtonType="ButtonType.Submit"
                                            Variant="Variant.Filled"
                                            Color="Color.Primary"
                                            StartIcon="@Icons.Material.Filled.Save">
                                            Set password
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

                <h3>Set your password</h3>
                <StatusMessage Message="@message" />
                <p class="ux-section-subtitle">
                    You do not have a local username/password for this site. Add a local
                    account so you can log in without an external login.
                </p>
                <EditForm Model="Input"
                    FormName="set-password"
                    OnValidSubmit="OnValidSubmitAsync"
                    method="post">
                    <DataAnnotationsValidator />
                    <ValidationSummary class="text-danger"
                        role="alert" />
                    <UxField Label="New password"
                        Icon="lock"
                        For="new-password">
                        <InputText id="new-password"
                            type="password"
                            @bind-Value="Input.NewPassword"
                            class="ux-input"
                            autocomplete="new-password"
                            placeholder="Enter a new password" />
                    </UxField>
                    <ValidationMessage For="() => Input.NewPassword"
                        class="text-danger" />
                    <UxField Label="Confirm password"
                        Icon="lock"
                        For="confirm-password">
                        <InputText id="confirm-password"
                            type="password"
                            @bind-Value="Input.ConfirmPassword"
                            class="ux-input"
                            autocomplete="new-password"
                            placeholder="Confirm your new password" />
                    </UxField>
                    <ValidationMessage For="() => Input.ConfirmPassword"
                        class="text-danger" />
                    <button type="submit"
                        class="btn btn-primary">
                        <UxIcon Name="lock" />
                        Set password
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
            onInitializedAsync.AddAssignmentStatement("var hasPassword", new CSharpStatement("await UserManager.HasPasswordAsync(user);"));
            onInitializedAsync.AddIfStatement("hasPassword", @if =>
            {
                @if.AddStatement("RedirectManager.RedirectTo(\"Account/Manage/ChangePassword\");");
            });

            code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnValidSubmitAsync", onValidSubmitAsync =>
            {
                onValidSubmitAsync.Private().Async();

                onValidSubmitAsync.AddAssignmentStatement("var addPasswordResult", new CSharpStatement("await UserManager.AddPasswordAsync(user, Input.NewPassword!);"));
                onValidSubmitAsync.AddIfStatement("!addPasswordResult.Succeeded", @if =>
                {
                    @if.AddStatement("message = $\"Error: {string.Join(\",\", addPasswordResult.Errors.Select(error => error.Description))}\";");
                    @if.AddStatement("return;");
                });

                onValidSubmitAsync.AddStatement("await SignInManager.RefreshSignInAsync(user);");
                onValidSubmitAsync.AddStatement("RedirectManager.RedirectToCurrentPageWithStatus(\"Your password has been set.\", HttpContext);");
            });

            code.AddClass("InputModel", inputModel =>
            {
                inputModel.Private().Sealed();

                inputModel.AddProperty("string?", "NewPassword", p =>
                {
                    p.AddAttribute(code.Template.UseType("System.ComponentModel.DataAnnotations.RequiredAttribute").RemoveSuffix("Attribute"));
                    p.AddAttribute("StringLength(100, ErrorMessage = \"The {0} must be at least {2} and at max {1} characters long.\", MinimumLength = 6)");
                    p.AddAttribute("DataType(DataType.Password)");
                    p.AddAttribute("Display(Name = \"New password\")");
                });

                inputModel.AddProperty("string?", "ConfirmPassword", p =>
                {
                    p.AddAttribute("DataType(DataType.Password)");
                    p.AddAttribute("Display(Name = \"Confirm new password\")");
                    p.AddAttribute("Compare(\"NewPassword\", ErrorMessage = \"The new password and confirmation password do not match.\")");
                });
            });
        }
    }
}
