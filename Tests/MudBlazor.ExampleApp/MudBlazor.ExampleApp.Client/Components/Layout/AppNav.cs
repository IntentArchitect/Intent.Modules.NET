using Intent.RoslynWeaver.Attributes;
using MudBlazor;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Components.MudBlazor.AppNav", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Components.Layout
{
    /// <summary>
    /// Single source of the app navigation items, shared by the interactive MainLayout drawer
    /// and the static-SSR ManageLayout drawer (both render them via the presentational NavLinks
    /// component). Keeps the nav identical across shells without duplicating the data.
    /// </summary>
    public static class AppNav
    {
        public static IReadOnlyList<NavLinks.NavItem> Items { get; } = [
            new("Manage Invoices", "/invoices", Icons.Material.Filled.Receipt),
            new("Manage Customers", "/customers", Icons.Material.Filled.Person),
            new("Manage Products", "/products", Icons.Material.Filled.ShoppingCart),
            new("Products", "/products", Icons.Material.Filled.AddCircle),
            new("Group1", "/"),
            new("Paged Invoice List", "/invoices/paged-invoice-table"),
            new("Admin", "")
        ];
    }
}