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
    /// Default (first-generation only) content for the shared ExternalLoginPicker component, seeded
    /// onto the modelled <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static content/Components(MudBlazor)/Account/Shared/ExternalLoginPicker pair.
    /// Identity-only (the stereotype's page-tagging script never creates it under JWT/OIDC).
    /// </summary>
    internal static class ExternalLoginPickerContent
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

        private static string BuildMudBlazorContent(string identityClass, string redirectManager)
        {
            return $$"""
                @using Microsoft.AspNetCore.Identity

                @inject SignInManager<{{identityClass}}> SignInManager
                @inject {{redirectManager}} RedirectManager

                @if (externalLogins.Length == 0)
                {
                    <MudText Typo="Typo.body1">
                        There are no external authentication services configured. See this <MudLink Href="https://go.microsoft.com/fwlink/?LinkID=532715" Typo="Typo.body1">article</MudLink> about setting up this ASP.NET application to support logging in via external services.
                    </MudText>
                }
                else
                {
                    <form class="form-horizontal"
                        action="Account/PerformExternalLogin"
                        method="post">
                        <AntiforgeryToken />
                        <input type="hidden"
                            name="ReturnUrl"
                            value="@ReturnUrl" />
                        <div class="external-login-buttons">
                            @foreach (var provider in externalLogins)
                            {
                                <MudButton ButtonType="ButtonType.Submit"
                                    Variant="Variant.Outlined"
                                    Color="Color.Primary"
                                    Name="provider"
                                    Value="@provider.Name"
                                    Title="@($"Log in using your {provider.DisplayName} account")">
                                    @provider.DisplayName
                                </MudButton>
                            }
                        </div>
                    </form>
                }
                """;
        }

        private static string BuildBootstrapContent(string identityClass, string redirectManager)
        {
            return $$"""
                @using Microsoft.AspNetCore.Identity

                @inject SignInManager<{{identityClass}}> SignInManager
                @inject {{redirectManager}} RedirectManager

                @if (externalLogins.Length == 0)
                {
                    <div>
                        <p>
                            There are no external authentication services configured. See this <a href="https://go.microsoft.com/fwlink/?LinkID=532715">article
                            about setting up this ASP.NET application to support logging in via external services</a>.
                        </p>
                    </div>
                }
                else
                {
                    <form class="form-horizontal"
                        action="Account/PerformExternalLogin"
                        method="post">
                        <div>
                            <AntiforgeryToken />
                            <input type="hidden"
                                name="ReturnUrl"
                                value="@ReturnUrl" />
                            <p>
                                @foreach (var provider in externalLogins)
                                {
                                    <button type="submit"
                                        class="btn btn-primary"
                                        name="provider"
                                        value="@provider.Name"
                                        title="Log in using your @provider.DisplayName account">
                                        @provider.DisplayName
                                    </button>
                                }
                            </p>
                        </div>
                    </form>
                }
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
            code.AddField($"{code.Template.UseType("Microsoft.AspNetCore.Authentication.AuthenticationScheme")}[]", "externalLogins", f => f.WithAssignment(new CSharpStatement("[]")));

            code.AddProperty("string?", "ReturnUrl", p => p.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromQueryAttribute").RemoveSuffix("Attribute")));

            // either get the existing method or add one
            ICSharpClassMethodDeclaration onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            if (onInitializedAsync is null)
            {
                code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnInitializedAsync");
                onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            }

            onInitializedAsync.Async().Protected().Override();
            onInitializedAsync.AddAssignmentStatement("externalLogins", new CSharpStatement("(await SignInManager.GetExternalAuthenticationSchemesAsync()).ToArray();"));
        }
    }
}
