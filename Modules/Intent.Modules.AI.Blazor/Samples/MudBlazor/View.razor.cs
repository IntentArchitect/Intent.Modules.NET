using BlazorServer.Sample.App.Application.Customers;
using BlazorServer.Sample.App.Application.Customers.GetCustomerById;
using BlazorServer.Sample.App.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace BlazorServer.Sample.App.Api.Components.Pages.Customers
{
    public partial class View
    {
        [Parameter]
        public Guid CustomerId { get; set; }
        public CustomerDto? Model { get; set; }
        [Inject]
        public IScopedMediator Mediator { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        private bool _billingSameAsDelivery = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadCustomerById(CustomerId);
        }

        private void EditCustomer() => NavigationManager.NavigateTo($"/customers/edit/{CustomerId}");

        private async Task LoadCustomerById(Guid id)
        {
            try
            {
                Model = await Mediator.Send(new GetCustomerByIdQuery(
                    id: id));
                // [IntentIgnore]
                if (Model.BillingAddress is null)
                {
                    _billingSameAsDelivery = true;
                    Model.BillingAddress = new CustomerAddressDto();
                }
                else
                {
                    _billingSameAsDelivery = false;
                }

            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }

        private void Cancel()
        {
            NavigationManager.NavigateTo("/customers/");
        }
    }
}