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
    /// Default (first-generation only) content for the shared ManageNavMenu component, seeded onto
    /// the modelled <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static content/Components(MudBlazor)/Account/Shared/ManageNavMenu pair.
    /// Identity-only (the stereotype's page-tagging script never creates it under JWT/OIDC).
    /// </summary>
    internal static class ManageNavMenuContent
    {
        public static string BuildRazorContent(RazorComponentTemplate template)
        {
            var identityClass = template.ExecutionContext.InstalledModules.Any(im => im.ModuleId == "Intent.AspNetCore.Identity") ?
                IdentityHelperExtensions.GetIdentityUserClass(template) :
                "ApplicationUser";
            var isMudBlazor = template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor");

            return isMudBlazor
                ? BuildMudBlazorContent(identityClass)
                : BuildBootstrapContent(identityClass);
        }

        private static string BuildMudBlazorContent(string identityClass)
        {
            return $$"""
                @using Microsoft.AspNetCore.Identity

                @inject SignInManager<{{identityClass}}> SignInManager

                <MudNavMenu>
                    <MudNavLink Href="Account/Manage"
                                Match="NavLinkMatch.All">
                        Profile
                    </MudNavLink>
                    <MudNavLink Href="Account/Manage/Email">Email</MudNavLink>
                    <MudNavLink Href="Account/Manage/ChangePassword">Password</MudNavLink>
                    @if (hasExternalLogins)
                    {
                        <MudNavLink Href="Account/Manage/ExternalLogins">External logins</MudNavLink>
                    }
                    <MudNavLink Href="Account/Manage/TwoFactorAuthentication">Two-factor authentication</MudNavLink>
                    <MudNavLink Href="Account/Manage/PersonalData">Personal data</MudNavLink>
                </MudNavMenu>
                """;
        }

        private static string BuildBootstrapContent(string identityClass)
        {
            return $$"""
                @using Microsoft.AspNetCore.Identity

                @inject SignInManager<{{identityClass}}> SignInManager

                <ul class="nav nav-pills flex-column">
                    <li class="nav-item">
                        <NavLink class="nav-link"
                                 href="Account/Manage"
                                 Match="NavLinkMatch.All">
                            Profile
                        </NavLink>
                    </li>
                    <li class="nav-item">
                        <NavLink class="nav-link"
                                 href="Account/Manage/Email">
                            Email
                        </NavLink>
                    </li>
                    <li class="nav-item">
                        <NavLink class="nav-link"
                                 href="Account/Manage/ChangePassword">
                            Password
                        </NavLink>
                    </li>
                    @if (hasExternalLogins)
                    {
                        <li class="nav-item">
                            <NavLink class="nav-link"
                                     href="Account/Manage/ExternalLogins">
                                External logins
                            </NavLink>
                        </li>
                    }
                    <li class="nav-item">
                        <NavLink class="nav-link"
                                 href="Account/Manage/TwoFactorAuthentication">
                            Two-factor authentication
                        </NavLink>
                    </li>
                    <li class="nav-item">
                        <NavLink class="nav-link"
                                 href="Account/Manage/PersonalData">
                            Personal data
                        </NavLink>
                    </li>
                </ul>

                <style>
                    .nav {
                        display: flex;
                        flex-direction: column;
                        gap: var(--space-1);
                        padding: 0;
                        margin: 0;
                        list-style: none;
                    }

                    .nav ::deep .nav-link {
                        display: block;
                        padding: var(--space-2) var(--space-3);
                        color: var(--text-muted);
                        text-decoration: none;
                        border: var(--border-width) solid transparent;
                        border-radius: var(--radius-md);
                        transition: color var(--dur-med) var(--ease-out),
                                    background-color var(--dur-med) var(--ease-out),
                                    border-color var(--dur-med) var(--ease-out);
                    }

                    .nav ::deep .nav-link:hover {
                        color: var(--text);
                        background: var(--surface-2);
                        border-color: var(--border);
                    }

                    .nav ::deep .nav-link.active {
                        color: var(--primary);
                        font-weight: 500;
                        background: color-mix(in srgb, var(--primary) 12%, transparent);
                        border-color: transparent;
                    }
                </style>
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
            code.AddField("bool", "hasExternalLogins");

            // either get the existing method or add one
            ICSharpClassMethodDeclaration onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            if (onInitializedAsync is null)
            {
                code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnInitializedAsync");
                onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            }

            onInitializedAsync.Async().Protected().Override();
            onInitializedAsync.AddAssignmentStatement("hasExternalLogins", new CSharpStatement("(await SignInManager.GetExternalAuthenticationSchemesAsync()).Any();"));
        }
    }
}
