using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
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
        public string? ErrorMessage { get; set; }
        [Inject]
        public IScopedMediator Mediator { get; set; } = default!;
        [Parameter]
        public EventCallback<bool> OnClosed { get; set; }

        private bool _isSaving;
        private bool _showAtLeastOneDeliveryMessage;

        protected override async Task OnInitializedAsync()
        {
            await LoadCustomerById(CustomerId);
        }

        private async Task UpdateCustomer()
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
            await OnClosed.InvokeAsync(true);
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
                    IsActive = customerDto.IsActive,
                    Preference = new UpdateCustomerPreferenceModel
                    {
                        Id = customerDto.Preference.Id,
                        NewsLetter = customerDto.Preference.NewsLetter,
                        Specials = customerDto.Preference.Specials
                    },
                    Loyalty = customerDto.Loyalty is not null
                    ? new UpdateCustomerCommandLoyaltyModel
                    {
                        Id = customerDto.Loyalty.Id,
                        LoyaltyNo = customerDto.Loyalty.LoyaltyNo,
                        Points = customerDto.Loyalty.Points
                    }
                    : null,
                    Addresses = customerDto.Addresses
                        .Select(a => new UpdateCustomerCommandAddressesModel
                        {
                            Id = a.Id,
                            Line1 = a.Line1,
                            Line2 = a.Line2,
                            City = a.City,
                            Postal = a.Postal,
                            AddressType = a.AddressType
                        })
                        .ToList()
                };
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
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
            _showAtLeastOneDeliveryMessage = false;

            if (Model is not null)
            {
                int deliveryCount = Model.Addresses.Count(a => a.AddressType == AddressType.Deliver);
                if (deliveryCount < 1)
                {
                    _showAtLeastOneDeliveryMessage = true;
                    return;
                }
            }

            _isSaving = true;
            try
            {
                await UpdateCustomer();
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            finally
            {
                _isSaving = false;
            }
        }

        private async Task OnCancel()
        {
            await OnClosed.InvokeAsync(false);
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
            if (address.AddressType == AddressType.Deliver)
            {
                int deliveryAddresses = Model.Addresses.Count(a => a.AddressType == AddressType.Deliver);
                if (deliveryAddresses <= 1)
                {
                    _showAtLeastOneDeliveryMessage = true;
                    return;
                }
            }
            Model.Addresses.Remove(address);
            if (Model.Addresses.Count(a => a.AddressType == AddressType.Deliver) >= 1)
            {
                _showAtLeastOneDeliveryMessage = false;
            }
        }
    }
}
