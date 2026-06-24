using Intent.RoslynWeaver.Attributes;
using MudBlazor;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Components.MudBlazor.AppNav", Version = "1.0")]

namespace Blazor.InteractiveWebAssembly.Jwt.Client.Components.Layout
{
    /// <summary>
    /// Single source of the app navigation items, shared by the interactive MainLayout drawer
    /// and the static-SSR ManageLayout drawer (both render them via the presentational NavLinks
    /// component). Keeps the nav identical across shells without duplicating the data.
    /// </summary>
    public static class AppNav
    {
        public static IReadOnlyList<NavLinks.NavItem> Items { get; } = [
            new("Add Menu Options...", $"/example-page/{"Navigation Page"}", Icons.Material.Filled.AddCircle)
        ];
    }
}