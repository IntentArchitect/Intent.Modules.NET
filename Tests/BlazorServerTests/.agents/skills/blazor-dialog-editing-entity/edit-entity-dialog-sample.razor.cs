using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using UI.AI.Samples.Application.Customers;
using UI.AI.Samples.Application.Customers.GetCustomerById;
using UI.AI.Samples.Application.Customers.UpdateCustomer;
using UI.AI.Samples.Domain;
using UI.AI.Samples.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace UI.AI.Samples.Api.Components.Pages.Dialogs.Customers
{
    public partial class EditCustomerDialog
    {
        [Parameter]
        public Guid CustomerId { get; set; }
        public UpdateCustomerModel? Model { get; set; }
        [Inject]
        public IScopedMediator Mediator { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        [CascadingParameter]
        public IMudDialogInstance Dialog { get; set; }

        private MudForm? _form;
        private bool _isLoading = false;
        private bool _showAtLeastOneDeliveryMessage = false;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            await LoadCustomerById(CustomerId);
            _isLoading = false;
        }

        private async Task UpdateCustomer()
        {
            try
            {
                await Mediator.Send(new UpdateCustomerCommand(
                    id: Model.Id,
                    name: Model.Name,
                    surname: Model.Surname,
                    email: Model.Email,
                    isActive: Model.IsActive,
                    preference: new UpdateCustomerPreferenceDto
                    {
                        Id = Model.Preference.Id,
                        NewsLetter = Model.Preference.NewsLetter,
                        Specials = Model.Preference.Specials
                    },
                    loyalty: Model?.Loyalty is not null
                        ? new UpdateCustomerCommandLoyaltyDto
                        {
                            Id = Model.Loyalty.Id,
                            LoyaltyNo = Model.Loyalty.LoyaltyNo,
                            Points = Model.Loyalty.Points
                        }
                        : null,
                    addresses: Model.Addresses
                        .Select(a => new UpdateCustomerCommandAddressesDto
                        {
                            Id = a.Id,
                            Line1 = a.Line1,
                            Line2 = a.Line2,
                            City = a.City,
                            Postal = a.Postal,
                            AddressType = a.AddressType
                        })
                        .ToList()));
                Snackbar.Add("Customer updated.", Severity.Success);
                Dialog.Close(DialogResult.Ok(true));
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
            public bool IsActive { get; set; }
            public UpdateCustomerPreferenceModel Preference { get; set; }
            public UpdateCustomerCommandLoyaltyModel? Loyalty { get; set; }
            public List<UpdateCustomerCommandAddressesModel> Addresses { get; set; }
        }
        public class UpdateCustomerPreferenceModel
        {
            public Guid Id { get; set; }
            public bool NewsLetter { get; set; }
            public bool Specials { get; set; }
        }
        public class UpdateCustomerCommandLoyaltyModel
        {
            public Guid Id { get; set; }
            public string LoyaltyNo { get; set; }
            public int Points { get; set; }
        }
        public class UpdateCustomerCommandAddressesModel
        {
            public Guid Id { get; set; }
            public string Line1 { get; set; }
            public string? Line2 { get; set; }
            public string City { get; set; }
            public string Postal { get; set; }
            public AddressType AddressType { get; set; }
        }

        private async Task OnSubmit()
        {
            if (_form == null)
                return;

            _showAtLeastOneDeliveryMessage = false;

            if (Model is not null)
            {
                int deliveryCount = Model.Addresses.Count(a => a.AddressType == AddressType.Deliver);
                if (deliveryCount < 1)
                {
                    _showAtLeastOneDeliveryMessage = true;
                    await _form.Validate();
                    StateHasChanged();
                    return;
                }
            }
            await _form.Validate();
            if (_form.IsValid && !_showAtLeastOneDeliveryMessage)
            {
                await UpdateCustomer();
            }
        }

        private void OnCancel()
        {
            Dialog.Cancel();
        }

        private void AddLoyalty()
        {
            if (Model == null || Model.Loyalty != null)
                return;
            Model.Loyalty = new UpdateCustomerCommandLoyaltyModel
            {
                Id = Guid.NewGuid(),
                LoyaltyNo = string.Empty,
                Points = 0
            };
        }

        private void RemoveLoyalty()
        {
            if (Model == null)
                return;
            Model.Loyalty = null;
        }

        private void AddAddress()
        {
            if (Model == null)
                return;
            var deliveryAddresses = Model.Addresses?.Count(a => a.AddressType == AddressType.Deliver) ?? 0;
            var addressType = deliveryAddresses == 0 ? AddressType.Deliver : AddressType.Billing;

            Model.Addresses.Add(new UpdateCustomerCommandAddressesModel
            {
                Id = Guid.Empty,
                Line1 = string.Empty,
                Line2 = string.Empty,
                City = string.Empty,
                Postal = string.Empty,
                AddressType = addressType
            });
        }

        private void RemoveAddress(UpdateCustomerCommandAddressesModel address)
        {
            if (Model == null)
                return;
            // Only allow removal if not violating delivery address rule
            if (address.AddressType == AddressType.Deliver)
            {
                int deliveryAddresses = Model.Addresses.Count(a => a.AddressType == AddressType.Deliver);
                if (deliveryAddresses <= 1)
                {
                    _showAtLeastOneDeliveryMessage = true;
                    StateHasChanged();
                    return;
                }
            }
            Model.Addresses.Remove(address);
            // Check again after removal and clear message if now valid
            if (Model.Addresses.Count(a => a.AddressType == AddressType.Deliver) >= 1)
            {
                _showAtLeastOneDeliveryMessage = false;
            }
        }
    }
}