using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using UI.AI.Samples.Application.Customers;
using UI.AI.Samples.Application.Customers.CreateCustomer;
using UI.AI.Samples.Domain;
using UI.AI.Samples.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace UI.AI.Samples.Api.Components.Pages.PageBased.Customers
{
    public partial class CustomerAdd
    {
        public CreateCustomerModel Model { get; set; } = new();
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        public IScopedMediator Mediator { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        private MudForm? _form;
        private bool _saving;
        private bool _hasLoyalty; 

        protected override async Task OnInitializedAsync()
        {
            Model.Preference ??= new CreateCustomerPreferenceModel();
            Model.Addresses ??= new List<CreateCustomerCommandAddressesModel>();

            // Start with no loyalty by default; toggle will create it on demand
            _hasLoyalty = false;
            Model.Loyalty = null;

            if (Model.Addresses.Count == 0)
            {
                Model.Addresses.Add(new CreateCustomerCommandAddressesModel
                {
                    AddressType = AddressType.Deliver
                });
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
                // Ensure loyalty is null when toggle is off (parity with Edit page)
                if (!_hasLoyalty)
                {
                    Model.Loyalty = null;
                }

                await CreateCustomer();
                Snackbar.Add("Customer created successfully.", Severity.Success);
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

        [IntentIgnore]
        private async Task CreateCustomer()
        {
            try
            {
                await Mediator.Send(new CreateCustomerCommand(
                    name: Model.Name,
                    surname: Model.Surname,
                    email: Model.Email,
                    isActive: Model.IsActive,
                    preference: new CreateCustomerPreferenceDto
                    {
                        NewsLetter = Model.Preference.NewsLetter,
                        Specials = Model.Preference.Specials
                    },
                    loyalty: _hasLoyalty && Model.Loyalty is not null
                        ? new CreateCustomerCommandLoyaltyDto
                        {
                            LoyaltyNo = Model.Loyalty.LoyaltyNo,
                            Points = Model.Loyalty.Points
                        }
                        : null,
                    addresses: Model.Addresses
                        .Select(a => new CreateCustomerCommandAddressesDto
                        {
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

        public void AddAddress()
        {
            var nextType = Model.Addresses.Count(a => a.AddressType == AddressType.Deliver) == 0
                ? AddressType.Deliver
                : AddressType.Billing;

            Model.Addresses.Add(new CreateCustomerCommandAddressesModel
            {
                AddressType = nextType
            });
        }

        public void RemoveAddress(int index)
        {
            if (index >= 0 && index < Model.Addresses.Count)
            {
                Model.Addresses.RemoveAt(index);
            }
        }

        private IEnumerable<string> ValidateAll(object? _)
        {
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

        // Toggle used by the Razor to show/hide Loyalty and keep Model in sync
        public bool HasLoyalty
        {
            get => _hasLoyalty;
            set
            {
                if (_hasLoyalty == value) return;
                _hasLoyalty = value;
                if (_hasLoyalty)
                {
                    Model.Loyalty ??= new CreateCustomerCommandLoyaltyModel
                    {
                        LoyaltyNo = string.Empty,
                        Points = 0
                    };
                }
                else
                {
                    Model.Loyalty = null;
                }
            }
        }

        public class CreateCustomerModel
        {
            public string Name { get; set; } = string.Empty;
            public string Surname { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public bool IsActive { get; set; }
            public CreateCustomerPreferenceModel Preference { get; set; } = new();
            public CreateCustomerCommandLoyaltyModel? Loyalty { get; set; }
            public List<CreateCustomerCommandAddressesModel> Addresses { get; set; } = new();
        }
        public class CreateCustomerPreferenceModel
        {
            public bool NewsLetter { get; set; }
            public bool Specials { get; set; }
        }
        public class CreateCustomerCommandLoyaltyModel
        {
            public string LoyaltyNo { get; set; } = string.Empty;
            public int Points { get; set; }
        }
        public class CreateCustomerCommandAddressesModel
        {
            public string Line1 { get; set; } = string.Empty;
            public string? Line2 { get; set; }
            public string City { get; set; } = string.Empty;
            public string Postal { get; set; } = string.Empty;
            public AddressType AddressType { get; set; }
        }
    }
}
