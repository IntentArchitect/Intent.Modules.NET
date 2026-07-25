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
    /// Default (first-generation only) content for the ConfirmEmailChange page, seeded onto the
    /// modelled <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static content/Components(MudBlazor)/Account/Pages/ConfirmEmailChange pair.
    /// </summary>
    internal static class ConfirmEmailChangePageContent
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
            """;

        private static string BuildMudBlazorContent(string identityClass, string redirectManager)
        {
            return $$"""
                @using System.Text
                @using Microsoft.AspNetCore.Identity
                @using Microsoft.AspNetCore.WebUtilities

                @inject UserManager<{{identityClass}}> UserManager
                @inject SignInManager<{{identityClass}}> SignInManager
                @inject {{redirectManager}} RedirectManager

                <MudPaper Class="pa-4 mb-4 ux-gradient-primary"
                    Elevation="0">
                    <MudText Typo="Typo.h4"
                        Class="text-white font-weight-bold mb-2">
                        <MudIcon Icon="@Icons.Material.Filled.MarkEmailRead"
                            Class="mr-2" />
                        Confirm email change
                    </MudText>
                    <MudText Typo="Typo.body1"
                        Class="text-white opacity-90">
                        We are verifying your updated email address and applying the change.
                    </MudText>
                </MudPaper>

                <MudCard Class="ux-fade-in-up auth-form-shell"
                    Style="animation-delay: 0.1s"
                    Outlined="true">
                    <MudCardContent>
                        <MudText Typo="Typo.h5"
                            Class="mb-3">
                            Email change status
                        </MudText>
                        <StatusMessage Message="@message" />
                    </MudCardContent>
                </MudCard>
                """;
        }

        private static string BuildBootstrapContent(string identityClass, string redirectManager)
        {
            return $$"""
                @using System.Text
                @using Microsoft.AspNetCore.Identity
                @using Microsoft.AspNetCore.WebUtilities

                @inject UserManager<{{identityClass}}> UserManager
                @inject SignInManager<{{identityClass}}> SignInManager
                @inject {{redirectManager}} RedirectManager

                <AccountHero Icon="check-circle"
                    Title="Confirm email change"
                    Subtitle="Updating your email address." />

                <div class="ux-form-narrow">
                    <section>
                        <StatusMessage Message="@message" />
                    </section>
                </div>
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
            code.AddField("string?", "message");

            code.AddProperty(code.Template.UseType("Microsoft.AspNetCore.Http.HttpContext?"), "HttpContext", p => p.WithInitialValue("default!").AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.CascadingParameterAttribute").RemoveSuffix("Attribute")));

            code.AddProperty("string?", "UserId", p => p.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromQueryAttribute").RemoveSuffix("Attribute")));
            code.AddProperty("string?", "Email", p => p.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromQueryAttribute").RemoveSuffix("Attribute")));
            code.AddProperty("string?", "Code", p => p.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromQueryAttribute").RemoveSuffix("Attribute")));

            // either get the existing method or add one
            ICSharpClassMethodDeclaration onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            if (onInitializedAsync is null)
            {
                code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnInitializedAsync");
                onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            }

            onInitializedAsync.Async().Protected().Override();

            onInitializedAsync.AddIfStatement("UserId is null || Email is null || Code is null", @if =>
            {
                @if.AddStatement("RedirectManager.RedirectToWithStatus(\"Account/Login\", \"Error: Invalid email change confirmation link.\", HttpContext);");
            });

            onInitializedAsync.AddAssignmentStatement("var user", new CSharpStatement("await UserManager.FindByIdAsync(UserId);"));
            onInitializedAsync.AddIfStatement("user is null", @if =>
            {
                @if.AddStatement("message = \"Unable to find user with Id '{userId}'\";");
                @if.AddStatement("return;");
            });

            onInitializedAsync.AddAssignmentStatement("var code", new CSharpStatement($"{code.Template.UseType("System.Text.Encoding")}.UTF8.GetString({code.Template.UseType("Microsoft.AspNetCore.WebUtilities.WebEncoders")}.Base64UrlDecode(Code));"));
            onInitializedAsync.AddAssignmentStatement("var result", new CSharpStatement("await UserManager.ChangeEmailAsync(user, Email, code);"));
            onInitializedAsync.AddIfStatement("!result.Succeeded", @if =>
            {
                @if.AddStatement("message = \"Error changing email.\";");
                @if.AddStatement("return;");
            });

            onInitializedAsync.AddAssignmentStatement("var setUserNameResult", new CSharpStatement("await UserManager.SetUserNameAsync(user, Email);"));
            onInitializedAsync.AddIfStatement("!setUserNameResult.Succeeded", @if =>
            {
                @if.AddStatement("message = \"Error changing user name.\";");
                @if.AddStatement("return;");
            });

            onInitializedAsync.AddStatement("await SignInManager.RefreshSignInAsync(user);");
            onInitializedAsync.AddStatement("message = \"Thank you for confirming your email change.\";");
        }
    }
}
