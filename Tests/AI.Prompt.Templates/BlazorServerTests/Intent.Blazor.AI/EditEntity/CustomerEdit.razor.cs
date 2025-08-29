using System.ComponentModel.DataAnnotations;
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

namespace UI.AI.Samples.Api.Components.Pages.PageBased.Customers
{
    public partial class CustomerEdit
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
        private bool _hasLoyalty;

        protected override async Task OnInitializedAsync()
        {
            await LoadCustomerById(CustomerId);
            if (Model != null)
            {
                _hasLoyalty = Model.Loyalty != null;
                if (!_hasLoyalty)
                {
                    Model.Loyalty = null;
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
                if (!_hasLoyalty)
                {
                    Model.Loyalty = null;
                }
                else
                {
                    if (Model.Loyalty == null)
                    {
                        Model.Loyalty = new UpdateCustomerCommandLoyaltyModel
                        {
                            Id = Guid.Empty,
                            LoyaltyNo = string.Empty,
                            Points = 0
                        };
                    }
                }
                await UpdateCustomer();
                Snackbar.Add("Customer updated successfully.", Severity.Success);
                NavigationManager.NavigateTo("/page-based/customers/search");
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

        private void Cancel()
        {
            NavigationManager.NavigateTo("/page-based/customers/search");
        }

        private IEnumerable<string> ValidateAll(object? _)
        {
            if (Model == null) yield break;
            foreach (var address in Model.Addresses)
            {
                if (string.IsNullOrWhiteSpace(address.Line1))
                    yield return "Address line 1 is required.";
                if (string.IsNullOrWhiteSpace(address.City))
                    yield return "Address city is required.";
                if (string.IsNullOrWhiteSpace(address.Postal))
                    yield return "Address postal code is required.";
            }
            if (string.IsNullOrWhiteSpace(Model.Name))
                yield return "Name is required.";
            if (string.IsNullOrWhiteSpace(Model.Surname))
                yield return "Surname is required.";
            if (string.IsNullOrWhiteSpace(Model.Email))
                yield return "Email is required.";
            else if (!new EmailAddressAttribute().IsValid(Model.Email))
                yield return "Please enter a valid email.";
        }


        private async Task UpdateCustomer()
        {
            try
            {
                if (!_hasLoyalty && Model?.Loyalty != null)
                {
                    Model.Loyalty = null;
                }
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

        public void AddAddress()
        {
            if (Model == null) return;
            Model.Addresses.Add(new UpdateCustomerCommandAddressesModel
            {
                Id = Guid.Empty,
                Line1 = string.Empty,
                Line2 = string.Empty,
                City = string.Empty,
                Postal = string.Empty,
                AddressType = Model.Addresses.Count(a => a.AddressType == AddressType.Deliver) == 0 ? AddressType.Deliver : AddressType.Billing
            });
        }

        public void RemoveAddress(int index)
        {
            if (Model == null) return;
            if (index >= 0 && index < Model.Addresses.Count)
                Model.Addresses.RemoveAt(index);
        }

        public bool HasLoyalty
        {
            get => _hasLoyalty;
            set
            {
                if (_hasLoyalty == value) return;
                _hasLoyalty = value;
                if (_hasLoyalty)
                {
                    if (Model != null && Model.Loyalty == null)
                    {
                        Model.Loyalty = new UpdateCustomerCommandLoyaltyModel
                        {
                            Id = Guid.Empty,
                            LoyaltyNo = string.Empty,
                            Points = 0
                        };
                    }
                }
                else
                {
                    if (Model != null)
                    {
                        Model.Loyalty = null;
                    }
                }
            }
        }
    }
}
