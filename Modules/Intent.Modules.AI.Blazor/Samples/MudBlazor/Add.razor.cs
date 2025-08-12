using BlazorServer.Sample.App.Application.Customers;
using BlazorServer.Sample.App.Application.Customers.CreateCustomer;
using BlazorServer.Sample.App.Domain.Entities;
using BlazorServer.Sample.App.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace BlazorServer.Sample.App.Api.Components.Pages.Customers
{
    public partial class Add
    {
        public CreateCustomerModel Model { get; set; } = new();
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        private MudForm? _form;
        private bool _saving;
        private bool _billingSameAsDelivery = true;

        [Inject]
        public IScopedMediator Mediator { get; set; } = default!;
        [Inject] public ISnackbar Snackbar { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            // Ensure both address objects are not null for binding
            Model.DeliveryAddress ??= new CreateCustomerCommandDeliveryAddressDto();
            Model.BillingAddress ??= new CreateCustomerCommandBillingAddressDto();
        }

        private async Task SaveAsync()
        {
            if (_form is null) return;

            await _form.Validate();
            if (!_form.IsValid)
            {
                Snackbar.Add("Please fix validation errors before saving.", Severity.Warning);
                return;
            }

            _saving = true;
            try
            {

                if (_billingSameAsDelivery)
                {
                    Model.DeliveryAddress = null;
                }

                await CreateCustomer();

                Snackbar.Add("Customer created successfully.", Severity.Success);
                NavigationManager.NavigateTo("/customers/");
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Failed to save customer: {ex.Message}", Severity.Error);
            }
            finally
            {
                _saving = false;
            }
        }

        private void Cancel() => NavigationManager.NavigateTo("/customers/");

        private async Task CreateCustomer()
        {
            try
            {
                await Mediator.Send(new CreateCustomerCommand(
                    name: Model.Name,
                    surname: Model.Surname,
                    email: Model.Email,
                    billingAddress: Model.BillingAddress,
                    deliveryAddress: Model.DeliveryAddress));
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }

        public class CreateCustomerModel
        {
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Email { get; set; }
            public CreateCustomerCommandBillingAddressDto? BillingAddress { get; set; }
            public CreateCustomerCommandDeliveryAddressDto DeliveryAddress { get; set; }
        }

        private IEnumerable<string> ValidateAll(object _)
        {
            // Extra cross-field rules beyond per-control Required/Validation
            if (!_billingSameAsDelivery)
            {
                if (string.IsNullOrWhiteSpace(Model.BillingAddress.Line1))
                    yield return "Billing address line 1 is required.";
                if (string.IsNullOrWhiteSpace(Model.BillingAddress.City))
                    yield return "Billing city is required.";
                if (string.IsNullOrWhiteSpace(Model.BillingAddress.Postal))
                    yield return "Billing postal code is required.";
            }
        }

    }
}