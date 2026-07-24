using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using System.Linq;

namespace Intent.Modules.Blazor.Authentication.DefaultContent
{
    /// <summary>
    /// Default (first-generation only) content for the shared AppUserMenu component, seeded onto the
    /// modelled <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static content/Components(MudBlazor)/Account/Shared/AppUserMenu pair.
    /// Identity-only. Pure markup — the original had no code-behind, so <see cref="BuildCodeBehind"/>
    /// is a no-op.
    /// </summary>
    internal static class AppUserMenuContent
    {
        public static string BuildRazorContent(RazorComponentTemplate template)
        {
            // The shared layout lives in the server project for InteractiveServer, but in the .Client
            // project for InteractiveAuto / InteractiveWebAssembly — mirrors the old static content's
            // Replacements() computation of "LayoutNamespace" so @using Components.Layout still resolves.
            var layoutRoot = template.GetNamespace().Replace("Components.Account.Shared", "");
            var layoutNamespace = template.ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveServer()
                ? layoutRoot
                : $"{layoutRoot.TrimEnd('.')}.Client.";

            var isMudBlazor = template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor");

            return isMudBlazor
                ? BuildMudBlazorContent(layoutNamespace)
                : BuildBootstrapContent(layoutNamespace);
        }

        public static string BuildStyleContent(RazorComponentTemplate template) => Style;

        private const string Style = """
            .ux-user-menu-item {
            display: flex;
            align-items: center;
            justify-content: flex-start;
            gap: var(--space-3);
            width: 100%;
            padding: var(--space-2) var(--space-4);
            background: none;
            border: 0;
            font: inherit;
            font-size: var(--type-body-sm);
            text-align: left;
            text-decoration: none;
            color: var(--text);
            cursor: pointer;
            }

            .ux-user-menu-item:hover {
            background-color: var(--surface-2);
            }

            .ux-user-menu-logout-form {
            margin: 0;
            }
            """;

        private static string BuildMudBlazorContent(string layoutNamespace)
        {
            return $$"""
                @using {{layoutNamespace}}Components.Layout

                <UserMenu>
                <Trigger>
                <MudIcon Icon="@Icons.Material.Filled.MoreVert" />
                </Trigger>
                <ChildContent>
                <a class="ux-user-menu-item"
                href="Account/Manage">
                <MudIcon Icon="@Icons.Material.Filled.Person"
                Size="Size.Small" />
                <span>Profile</span>
                </a>
                <a class="ux-user-menu-item"
                href="Account/Manage">
                <MudIcon Icon="@Icons.Material.Filled.ManageAccounts"
                Size="Size.Small" />
                <span>My Account</span>
                </a>
                <form class="ux-user-menu-logout-form"
                action="Account/Logout"
                method="post">
                <AntiforgeryToken />
                <input type="hidden"
                name="returnUrl"
                value="" />
                <button type="submit"
                class="ux-user-menu-item">
                <MudIcon Icon="@Icons.Material.Filled.Logout"
                Size="Size.Small" />
                <span>Logout</span>
                </button>
                </form>
                </ChildContent>
                </UserMenu>
                """;
        }

        private static string BuildBootstrapContent(string layoutNamespace)
        {
            return $$"""
                @using {{layoutNamespace}}Components.Layout

                <UserMenu>
                <Trigger>
                <UxIcon Name="more-vertical" />
                </Trigger>
                <ChildContent>
                <a class="ux-user-menu-item"
                href="Account/Manage">
                <UxIcon Name="user" />
                <span>Profile</span>
                </a>
                <a class="ux-user-menu-item"
                href="Account/Manage">
                <UxIcon Name="settings" />
                <span>My Account</span>
                </a>
                <form class="ux-user-menu-logout-form"
                action="Account/Logout"
                method="post">
                <AntiforgeryToken />
                <input type="hidden"
                name="returnUrl"
                value="" />
                <button type="submit"
                class="ux-user-menu-item">
                <UxIcon Name="log-out" />
                <span>Logout</span>
                </button>
                </form>
                </ChildContent>
                </UserMenu>
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
        }
    }
}
