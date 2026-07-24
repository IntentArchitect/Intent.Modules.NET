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
    /// Default (first-generation only) content for the Manage/ExternalLogins page, seeded onto the
    /// modelled <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static content/Components(MudBlazor)/Account/Pages/Manage/ExternalLogins pair.
    /// </summary>
    internal static class ManageExternalLoginsPageContent
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

        public static string BuildStyleContent(RazorComponentTemplate template)
        {
            var isMudBlazor = template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor");
            return isMudBlazor ? MudBlazorStyle : BootstrapStyle;
        }

        private const string MudBlazorStyle = """
            .external-login-buttons {
            display: flex;
            flex-wrap: wrap;
            gap: var(--space-2);
            }
            """;

        private const string BootstrapStyle = """
            .form-horizontal {
            display: flex;
            flex-direction: column;
            gap: var(--space-3);
            }

            .form-horizontal p {
            display: flex;
            flex-wrap: wrap;
            gap: var(--space-2);
            }
            """;

        private static string BuildMudBlazorContent(string identityClass, string userAccessor, string redirectManager)
        {
            return $$"""
                @using Microsoft.AspNetCore.Authentication
                @using Microsoft.AspNetCore.Identity

                @inject UserManager<{{identityClass}}> UserManager
                @inject SignInManager<{{identityClass}}> SignInManager
                @inject {{userAccessor}} UserAccessor
                @inject IUserStore<{{identityClass}}> UserStore
                @inject {{redirectManager}} RedirectManager

                <MudPaper Class="pa-4 mb-4 ux-gradient-primary"
                Elevation="0">
                <MudText Typo="Typo.h4"
                Class="text-white font-weight-bold mb-2">
                <MudIcon Icon="@Icons.Material.Filled.Link"
                Class="mr-2" />
                External logins
                </MudText>
                <MudText Typo="Typo.body1"
                Class="text-white opacity-90">
                Manage the external identity providers linked to your account.
                </MudText>
                </MudPaper>

                <StatusMessage />
                @if (currentLogins?.Count > 0)
                {
                <MudCard Class="ux-fade-in-up mb-4"
                Style="animation-delay: 0.1s"
                Outlined="true">
                <MudCardContent>
                <MudText Typo="Typo.h5"
                Class="mb-3">
                Registered logins
                </MudText>
                <MudTable Items="currentLogins"
                Hover="true"
                Dense="true"
                Class="mb-3">
                <HeaderContent>
                <MudTh>Provider</MudTh>
                <MudTh>Actions</MudTh>
                </HeaderContent>
                <RowTemplate>
                <MudTd DataLabel="Provider">@context.ProviderDisplayName</MudTd>
                <MudTd DataLabel="Actions">
                @if (showRemoveButton)
                {
                <form @formname="@($"remove-login-{context.LoginProvider}")"
                @onsubmit="OnSubmitAsync"
                method="post">
                <AntiforgeryToken />
                <input type="hidden"
                name="@nameof(LoginProvider)"
                value="@context.LoginProvider" />
                <input type="hidden"
                name="@nameof(ProviderKey)"
                value="@context.ProviderKey" />
                <MudButton ButtonType="ButtonType.Submit"
                Variant="Variant.Outlined"
                Color="Color.Error"
                Size="Size.Small"
                Title="@($"Remove this {context.ProviderDisplayName} login from your account")">
                Remove
                </MudButton>
                </form>
                }
                </MudTd>
                </RowTemplate>
                </MudTable>
                </MudCardContent>
                </MudCard>
                }
                @if (otherLogins?.Count > 0)
                {
                <MudCard Class="ux-fade-in-up"
                Style="animation-delay: 0.2s"
                Outlined="true">
                <MudCardContent>
                <MudText Typo="Typo.h6"
                Class="mb-3">
                Add another service to log in
                </MudText>
                <MudText Typo="Typo.body2"
                Class="mb-4">
                Connect another external provider for sign-in convenience.
                </MudText>
                <form action="Account/Manage/LinkExternalLogin"
                method="post">
                <AntiforgeryToken />
                <div class="external-login-buttons">
                @foreach (var provider in otherLogins)
                {
                <MudButton ButtonType="ButtonType.Submit"
                Variant="Variant.Outlined"
                Color="Color.Primary"
                Name="Provider"
                Value="@provider.Name"
                Title="@($"Log in using your {provider.DisplayName} account")">
                @provider.DisplayName
                </MudButton>
                }
                </div>
                </form>
                </MudCardContent>
                </MudCard>
                }
                """;
        }

        private static string BuildBootstrapContent(string identityClass, string userAccessor, string redirectManager)
        {
            return $$"""
                @using Microsoft.AspNetCore.Authentication
                @using Microsoft.AspNetCore.Identity

                @inject UserManager<{{identityClass}}> UserManager
                @inject SignInManager<{{identityClass}}> SignInManager
                @inject {{userAccessor}} UserAccessor
                @inject IUserStore<{{identityClass}}> UserStore
                @inject {{redirectManager}} RedirectManager

                <StatusMessage />
                @if (currentLogins?.Count > 0)
                {
                <h3>Registered logins</h3>
                <table class="table">
                <tbody>
                @foreach (var login in currentLogins)
                {
                <tr>
                <td>@login.ProviderDisplayName</td>
                <td>
                @if (showRemoveButton)
                {
                <form @formname="@($"remove-login-{login.LoginProvider}")"
                @onsubmit="OnSubmitAsync"
                method="post">
                <AntiforgeryToken />
                <input type="hidden"
                name="@nameof(LoginProvider)"
                value="@login.LoginProvider" />
                <input type="hidden"
                name="@nameof(ProviderKey)"
                value="@login.ProviderKey" />
                <button type="submit"
                class="btn btn-danger"
                title="Remove this @login.ProviderDisplayName login from your account">
                <UxIcon Name="trash" /> Remove
                </button>
                </form>
                }
                else
                {
                @: &nbsp;
                }
                </td>
                </tr>
                }
                </tbody>
                </table>
                }
                @if (otherLogins?.Count > 0)
                {
                <h4>Add another service to log in</h4>
                <form class="form-horizontal"
                action="Account/Manage/LinkExternalLogin"
                method="post">
                <AntiforgeryToken />
                <div class="ux-button-row">
                @foreach (var provider in otherLogins)
                {
                <button type="submit"
                class="btn btn-primary"
                name="Provider"
                value="@provider.Name"
                title="Log in using your @provider.DisplayName account">
                @provider.DisplayName
                </button>
                }
                </div>
                </form>
                }
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
            var identityClass = code.Template.ExecutionContext.InstalledModules.Any(im => im.ModuleId == "Intent.AspNetCore.Identity") ?
                IdentityHelperExtensions.GetIdentityUserClass(code.Template) :
                "ApplicationUser";

            code.AddField("string", "LinkLoginCallbackAction", f => f.Public("\"LinkLoginCallback\"").Constant());
            code.AddField(identityClass, "user", f => f.WithAssignment(new CSharpStatement("default!")));
            code.AddField($"IList<{code.Template.UseType("Microsoft.AspNetCore.Identity.UserLoginInfo")}>?", "currentLogins");
            code.AddField($"IList<{code.Template.UseType("Microsoft.AspNetCore.Authentication.AuthenticationScheme")}>?", "otherLogins");
            code.AddField("bool", "showRemoveButton");

            code.AddProperty(code.Template.UseType("Microsoft.AspNetCore.Http.HttpContext?"), "HttpContext", p => p.WithInitialValue("default!").AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.CascadingParameterAttribute").RemoveSuffix("Attribute")));
            code.AddProperty("string?", "LoginProvider", p => p.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromFormAttribute").RemoveSuffix("Attribute")));
            code.AddProperty("string?", "ProviderKey", p => p.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromFormAttribute").RemoveSuffix("Attribute")));
            code.AddProperty("string?", "Action", p => p.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromQueryAttribute").RemoveSuffix("Attribute")));

            // either get the existing method or add one
            ICSharpClassMethodDeclaration onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            if (onInitializedAsync is null)
            {
                code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnInitializedAsync");
                onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            }

            onInitializedAsync.Async().Protected().Override();
            onInitializedAsync.AddAssignmentStatement("user", new CSharpStatement("await UserAccessor.GetRequiredUserAsync(HttpContext);"));
            onInitializedAsync.AddAssignmentStatement("currentLogins", new CSharpStatement("await UserManager.GetLoginsAsync(user);"));
            onInitializedAsync.AddAssignmentStatement("otherLogins", new CSharpStatement("(await SignInManager.GetExternalAuthenticationSchemesAsync()).Where(auth => currentLogins.All(ul => auth.Name != ul.LoginProvider)).ToList();"));

            onInitializedAsync.AddAssignmentStatement("string? passwordHash", new CSharpStatement("null;"));
            onInitializedAsync.AddIfStatement($"UserStore is IUserPasswordStore<{identityClass}> userPasswordStore", @if =>
            {
                @if.AddAssignmentStatement("passwordHash", new CSharpStatement("await userPasswordStore.GetPasswordHashAsync(user, HttpContext.RequestAborted);"));
            });

            onInitializedAsync.AddAssignmentStatement("showRemoveButton", new CSharpStatement("passwordHash is not null || currentLogins.Count > 1;"));

            onInitializedAsync.AddIfStatement($"{code.Template.UseType("Microsoft.AspNetCore.Http.HttpMethods")}.IsGet(HttpContext.Request.Method) && Action == LinkLoginCallbackAction", @if =>
            {
                @if.AddStatement("await OnGetLinkLoginCallbackAsync();");
            });

            code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnSubmitAsync", onSubmitAsync =>
            {
                onSubmitAsync.Private().Async();

                onSubmitAsync.AddAssignmentStatement("var result", new CSharpStatement("await UserManager.RemoveLoginAsync(user, LoginProvider!, ProviderKey!);"));
                onSubmitAsync.AddIfStatement("!result.Succeeded", @if =>
                {
                    @if.AddStatement("RedirectManager.RedirectToCurrentPageWithStatus(\"Error: The external login was not removed.\", HttpContext);");
                });

                onSubmitAsync.AddStatement("await SignInManager.RefreshSignInAsync(user);");
                onSubmitAsync.AddStatement("RedirectManager.RedirectToCurrentPageWithStatus(\"The external login was removed.\", HttpContext);");
            });

            code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnGetLinkLoginCallbackAsync", onGetLinkLoginCallbackAsync =>
            {
                onGetLinkLoginCallbackAsync.Private().Async();

                onGetLinkLoginCallbackAsync.AddAssignmentStatement("var userId", new CSharpStatement("await UserManager.GetUserIdAsync(user);"));
                onGetLinkLoginCallbackAsync.AddAssignmentStatement("var info", new CSharpStatement("await SignInManager.GetExternalLoginInfoAsync(userId);"));
                onGetLinkLoginCallbackAsync.AddIfStatement("info is null", @if =>
                {
                    @if.AddStatement("RedirectManager.RedirectToCurrentPageWithStatus(\"Error: Could not load external login info.\", HttpContext);");
                });

                onGetLinkLoginCallbackAsync.AddAssignmentStatement("var result", new CSharpStatement("await UserManager.AddLoginAsync(user, info);"));
                onGetLinkLoginCallbackAsync.AddIfStatement("!result.Succeeded", @if =>
                {
                    @if.AddStatement("RedirectManager.RedirectToCurrentPageWithStatus(\"Error: The external login was not added. External logins can only be associated with one account.\", HttpContext);");
                });

                onGetLinkLoginCallbackAsync.AddStatement("// Clear the existing external cookie to ensure a clean login process");
                onGetLinkLoginCallbackAsync.AddStatement($"await HttpContext.SignOutAsync({code.Template.UseType("Microsoft.AspNetCore.Identity.IdentityConstants")}.ExternalScheme);");

                onGetLinkLoginCallbackAsync.AddStatement("RedirectManager.RedirectToCurrentPageWithStatus(\"The external login was added.\", HttpContext);");
            });
        }
    }
}
