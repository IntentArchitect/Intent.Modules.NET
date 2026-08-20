using Intent.RoslynWeaver.Attributes;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Products;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.PageCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Components.Pages.Products
{
    public partial class MyProduct
    {
        public ProductDto Model { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
        }
    }
}