using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor.ExampleApp.Client.HttpClients;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Customers;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.Invoices.Components
{
    public partial class InvoiceForm
    {
        private MudForm _form;
        private bool _onSaveClickedProcessing = false;
        [Parameter]
        public InvoiceModel Model { get; set; } = new();
        [Parameter]
        public string HeaderText { get; set; }
        public List<CustomerLookupDto> CustomerOptions { get; set; } = [];
        public List<ProductDto> ProductOptions { get; set; } = [];
        [Parameter]
        public EventCallback SaveClicked { get; set; }
        [Parameter]
        public EventCallback CancelClicked { get; set; }
        [Inject]
        public ICustomersService CustomersService { get; set; } = default!;
        [Inject]
        public IProductsService ProductsService { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                CustomerOptions = await CustomersService.GetCustomersLookupAsync();
                ProductOptions = await ProductsService.GetProductsLookupAsync();
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
            StateHasChanged();
        }

        private async Task OnSaveClicked()
        {
            await _form!.Validate();
            if (!_form.IsValid)
            {
                return;
            }
        }

        private void OnCancelClicked()
        {
        }

        private void AddLineClicked()
        {
            Model.OrderLines.Add(new());
        }

        private void OnDeleteLineClick(InvoiceLineModel orderLine)
        {
            Model.OrderLines.Remove(orderLine);
        }

        private void OnCustomerSelectedChanged()
        {
        }

        private async Task<List<CustomerLookupDto>> OnAutoCompleteSearchFunction(string value, CancellationToken c)
        {
            return new List<CustomerLookupDto> { new CustomerLookupDto() { Id = Guid.NewGuid(), Name = "Value1" }, new CustomerLookupDto() { Id = Guid.NewGuid(), Name = "Value2" } };
        }
    }
}