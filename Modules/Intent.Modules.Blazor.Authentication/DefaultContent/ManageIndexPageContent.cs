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
    /// Default (first-generation only) content for the Manage/Index page, seeded onto the modelled
    /// <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static content/Components(MudBlazor)/Account/Pages/Manage/Index pair.
    /// </summary>
    internal static class ManageIndexPageContent
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
                <MudIcon Icon="@Icons.Material.Filled.AccountCircle"
                Class="mr-2" />
                Profile
                </MudText>
                <MudText Typo="Typo.body1"
                Class="text-white opacity-90">
                Manage your personal account details.
                </MudText>
                </MudPaper>

                <StatusMessage />

                <MudCard Class="ux-fade-in-up auth-form-shell"
                Style="animation-delay: 0.1s"
                Outlined="true">
                <MudCardContent>
                <EditForm Model="Input"
                FormName="profile"
                OnValidSubmit="OnValidSubmitAsync"
                method="post">
                <DataAnnotationsValidator />
                <ValidationSummary class="text-danger"
                role="alert" />

                <MudGrid>
                <MudItem xs="12">
                <MudText Typo="Typo.h5">Profile details</MudText>
                <MudText Typo="Typo.body2"
                Class="mb-4">
                Update the information associated with your account.
                </MudText>
                </MudItem>
                <MudItem xs="12">
                <MudTextField T="string"
                Value="@username"
                Label="Username"
                Variant="Variant.Outlined"
                Adornment="Adornment.Start"
                AdornmentIcon="@Icons.Material.Filled.Badge"
                Disabled="true" />
                </MudItem>
                <MudItem xs="12">
                <MudTextField T="string"
                @bind-Value="Input.PhoneNumber"
                Label="Phone number"
                Placeholder="Please enter your phone number."
                Variant="Variant.Outlined"
                Adornment="Adornment.Start"
                AdornmentIcon="@Icons.Material.Filled.Phone"
                Immediate="true"
                For="@(() => Input.PhoneNumber)" />
                <ValidationMessage For="() => Input.PhoneNumber"
                class="text-danger" />
                </MudItem>
                <MudItem xs="12">
                <MudStack Row="true"
                Justify="Justify.FlexEnd">
                <MudButton ButtonType="ButtonType.Submit"
                Variant="Variant.Filled"
                Color="Color.Primary"
                StartIcon="@Icons.Material.Filled.Save">
                Save
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

                <div class="ux-section-head">
                <h3>Profile details</h3>
                <p class="ux-section-subtitle">Update the information associated with your account.</p>
                </div>
                <StatusMessage />

                <EditForm Model="Input"
                FormName="profile"
                OnValidSubmit="OnValidSubmitAsync"
                method="post">
                <DataAnnotationsValidator />
                <ValidationSummary class="text-danger"
                role="alert" />
                <UxField Label="Username"
                Icon="user"
                For="username">
                <input id="username"
                type="text"
                value="@username"
                class="ux-input"
                placeholder="Your username"
                disabled />
                </UxField>
                <UxField Label="Phone number"
                Icon="phone"
                For="phone-number">
                <InputText id="phone-number"
                @bind-Value="Input.PhoneNumber"
                class="ux-input"
                placeholder="Your phone number" />
                </UxField>
                <ValidationMessage For="() => Input.PhoneNumber"
                class="text-danger" />
                <button type="submit"
                class="btn btn-primary">
                <UxIcon Name="save" />
                Save
                </button>
                </EditForm>
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
            var identityClass = code.Template.ExecutionContext.InstalledModules.Any(im => im.ModuleId == "Intent.AspNetCore.Identity") ?
                IdentityHelperExtensions.GetIdentityUserClass(code.Template) :
                "ApplicationUser";

            code.AddField(identityClass, "user", f => f.WithAssignment(new CSharpStatement("default!")));
            code.AddField("string?", "username");
            code.AddField("string?", "phoneNumber");

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
            onInitializedAsync.AddAssignmentStatement("username", new CSharpStatement("await UserManager.GetUserNameAsync(user);"));
            onInitializedAsync.AddAssignmentStatement("phoneNumber", new CSharpStatement("await UserManager.GetPhoneNumberAsync(user);"));
            onInitializedAsync.AddStatement("Input.PhoneNumber ??= phoneNumber;");

            code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnValidSubmitAsync", onValidSubmitAsync =>
            {
                onValidSubmitAsync.Private().Async();

                onValidSubmitAsync.AddIfStatement("Input.PhoneNumber != phoneNumber", @if =>
                {
                    @if.AddAssignmentStatement("var setPhoneResult", new CSharpStatement("await UserManager.SetPhoneNumberAsync(user, Input.PhoneNumber);"));
                    @if.AddIfStatement("!setPhoneResult.Succeeded", innerIf =>
                    {
                        innerIf.AddStatement("RedirectManager.RedirectToCurrentPageWithStatus(\"Error: Failed to set phone number.\", HttpContext);");
                    });
                });

                onValidSubmitAsync.AddStatement("await SignInManager.RefreshSignInAsync(user);");
                onValidSubmitAsync.AddStatement("RedirectManager.RedirectToCurrentPageWithStatus(\"Your profile has been updated\", HttpContext);");
            });

            code.AddClass("InputModel", inputModel =>
            {
                inputModel.Private().Sealed();

                inputModel.AddProperty("string?", "PhoneNumber", p =>
                {
                    p.AddAttribute(code.Template.UseType("System.ComponentModel.DataAnnotations.PhoneAttribute").RemoveSuffix("Attribute"));
                    p.AddAttribute("Display(Name = \"Phone number\")");
                });
            });
        }
    }
}
