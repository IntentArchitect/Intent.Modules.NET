using System.ComponentModel.DataAnnotations;
using BlazorServer.Sample.App.Application.Customers;
using BlazorServer.Sample.App.Application.Customers.GetCustomerById;
using BlazorServer.Sample.App.Application.Customers.UpdateCustomer;
using BlazorServer.Sample.App.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace BlazorServer.Sample.App.Api.Components.Pages.Customers
{
    public partial class Edit
    {
        [Parameter]
        public Guid CustomerId { get; set; }
        public UpdateCustomerModel? Model { get; set; }
        [Inject]
        public IScopedMediator Mediator { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        private MudForm? _form;
        private bool _saving;
        private bool _billingSameAsDelivery = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadCustomerById(CustomerId);
            if (Model is not null)
            {
                Model.DeliveryAddress ??= new UpdateCustomerCommandDeliveryAddressDto();
                Model.BillingAddress ??= new UpdateCustomerCommandBillingAddressDto();

                if (Model.BillingAddress is not null && Model.DeliveryAddress is not null &&
                    Model.BillingAddress.Line1 == Model.DeliveryAddress.Line1 &&
                    Model.BillingAddress.City == Model.DeliveryAddress.City &&
                    Model.BillingAddress.Postal == Model.DeliveryAddress.Postal)
                {
                    _billingSameAsDelivery = true;
                }
                else
                {
                    _billingSameAsDelivery = false;
                }
            }
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
                    Model.BillingAddress = new UpdateCustomerCommandBillingAddressDto
                    {
                        Id = Model.DeliveryAddress.Id,
                        Line1 = Model.DeliveryAddress.Line1,
                        Line2 = Model.DeliveryAddress.Line2,
                        City = Model.DeliveryAddress.City,
                        Postal = Model.DeliveryAddress.Postal
                    };
                }

                await UpdateCustomer();
                Snackbar.Add("Customer updated successfully.", Severity.Success);
                NavigationManager.NavigateTo("/customers-example/");
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

        private IEnumerable<string> ValidateAll(object _)
        {
            if (!_billingSameAsDelivery)
            {
                if (Model.BillingAddress is null)
                {
                    yield return "Billing address is required.";
                }
                else
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

        [IntentIgnore]
        private async Task UpdateCustomer()
        {
            try
            {
                await Mediator.Send(new UpdateCustomerCommand(
                    id: Model.Id,
                    name: Model.Name,
                    surname: Model.Surname,
                    email: Model.Email,
                    billingAddress: _billingSameAsDelivery ? Model.BillingAddress : Model.BillingAddress,
                    deliveryAddress: Model.DeliveryAddress));
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }

        private async Task LoadCustomerById(Guid id)
        {
            try
            {
                var customerDto = await Mediator.Send(new GetCustomerByIdQuery(
                    id: id));
                Model = new UpdateCustomerModel
                {
                    Id = customerDto.Id,
                    Name = customerDto.Name,
                    Surname = customerDto.Surname,
                    Email = customerDto.Email,
                    BillingAddress = customerDto.BillingAddress is not null
                        ? new UpdateCustomerCommandBillingAddressDto
                        {
                            Id = customerDto.BillingAddress.Id,
                            Line1 = customerDto.BillingAddress.Line1,
                            Line2 = customerDto.BillingAddress?.Line2,
                            City = customerDto.BillingAddress.City,
                            Postal = customerDto.BillingAddress.Postal
                        }
                        : null,
                    DeliveryAddress = new UpdateCustomerCommandDeliveryAddressDto
                    {
                        Id = customerDto.DeliveryAddress.Id,
                        Line1 = customerDto.DeliveryAddress.Line1,
                        Line2 = customerDto.DeliveryAddress.Line2,
                        City = customerDto.DeliveryAddress.City,
                        Postal = customerDto.DeliveryAddress.Postal
                    }
                };
                // [IntentIgnore]
                if (Model.BillingAddress == null)
                    Model.BillingAddress = new UpdateCustomerCommandBillingAddressDto();
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }

        public class UpdateCustomerModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Email { get; set; }
            public UpdateCustomerCommandBillingAddressDto? BillingAddress { get; set; }
            public UpdateCustomerCommandDeliveryAddressDto DeliveryAddress { get; set; }
        }
    }
}
