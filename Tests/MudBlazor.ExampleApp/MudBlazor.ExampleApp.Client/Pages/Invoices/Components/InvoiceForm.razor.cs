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
        private bool _onCancelClickedProcessing = false;
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
            try
            {
                _onSaveClickedProcessing = true;
                await _form!.Validate();
                if (!_form.IsValid)
                {
                    return;
                }
                await SaveClicked.InvokeAsync();
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
            finally
            {
                _onSaveClickedProcessing = false;
            }
        }

        private async Task OnCancelClicked()
        {
            try
            {
                _onCancelClickedProcessing = true;
                await CancelClicked.InvokeAsync();
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
            finally
            {
                _onCancelClickedProcessing = false;
            }
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

        private void OnStateAutoSelectedChanged()
        {
        }

        [IntentIgnore]
        private async Task<IEnumerable<string>> OnStateAutoAsync(string value, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(value))
                return _states.ToList();

            return _states.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        [IntentIgnore]
        private string[] _states =
        {
        "Alabama", "Alaska", "Arizona", "Arkansas", "California",
        "Colorado", "Connecticut", "Delaware", "Florida", "Georgia",
        "Hawaii", "Idaho", "Illinois", "Indiana", "Iowa", "Kansas",
        "Kentucky", "Louisiana", "Maine", "Maryland", "Massachusetts",
        "Michigan", "Minnesota", "Mississippi", "Missouri", "Montana",
        "Nebraska", "Nevada", "New Hampshire", "New Jersey", "New Mexico",
        "New York", "North Carolina", "North Dakota", "Ohio", "Oklahoma",
        "Oregon", "Pennsylvania", "Rhode Island", "South Carolina", "South Dakota",
        "Tennessee", "Texas", "Utah", "Vermont", "Virginia",
        "Washington", "West Virginia", "Wisconsin", "Wyoming" };


    }
}