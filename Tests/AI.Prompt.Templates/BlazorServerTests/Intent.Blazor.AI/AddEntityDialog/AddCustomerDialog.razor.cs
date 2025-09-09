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

namespace UI.AI.Samples.Api.Components.Pages.Dialogs.Customers
{
    public partial class AddCustomerDialog
    {
        public CreateCustomerModel2 Model { get; set; } = new();
        [Inject]
        public IScopedMediator Mediator { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        [CascadingParameter]
        public IMudDialogInstance Dialog { get; set; }

        private MudForm? _form;
        private MudForm? _addressForm;
        private MudForm? _loyaltyForm;
        private bool _saving;
        private CreateCustomerCommandAddressesModel1 _newAddress = new();
        private bool _showLoyalty;
        private CreateCustomerCommandLoyaltyModel1 _loyaltyModel = new();

        protected override async Task OnInitializedAsync()
        {
            Model.Preference ??= new CreateCustomerPreferenceModel2();
            Model.Addresses ??= new List<CreateCustomerCommandAddressesModel1>();
            _loyaltyModel = Model.Loyalty ?? new CreateCustomerCommandLoyaltyModel1();
            _showLoyalty = Model.Loyalty is not null;
            await Task.CompletedTask;
        }

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
                    loyalty: Model.Loyalty is not null
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

        private void Cancel()
        {
            Dialog.Close(DialogResult.Cancel());
        }

        private async Task SaveAsync()
        {
            if (_form is null)
                return;
            await _form.Validate();
            if (!_form.IsValid)
            {
                Snackbar.Add("Please fix validation errors before saving.", Severity.Warning);
                return;
            }
            if (Model.Addresses == null || Model.Addresses.Count == 0)
            {
                Snackbar.Add("At least one address is required.", Severity.Warning);
                return;
            }

            if (_showLoyalty && _loyaltyForm is not null)
            {
                await _loyaltyForm.Validate();
                if (!_loyaltyForm.IsValid)
                {
                    Snackbar.Add("Ensure loyalty info is valid or clear Loyalty panel.", Severity.Warning);
                    return;
                }
            }

            _saving = true;
            try
            {
                Model.Loyalty = _showLoyalty ? _loyaltyModel : null;
                await CreateCustomer();
                Snackbar.Add("Customer created successfully.", Severity.Success);
                Dialog.Close(DialogResult.Ok(true));
            }
            catch (Exception e)
            {
                Snackbar.Add($"Failed to save customer: {e.Message}", Severity.Error);
            }
            finally { _saving = false; }
        }

        private string? EmailRule(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            return new EmailAddressAttribute().IsValid(value) ? null : "Please enter a valid email address";
        }

        private async Task AddAddress()
        {
            if (_addressForm is null)
                return;
            await _addressForm.Validate();
            if (!_addressForm.IsValid)
            {
                Snackbar.Add("Please fill required address fields.", Severity.Warning);
                return;
            }
            Model.Addresses.Add(new CreateCustomerCommandAddressesModel1
            {
                Line1 = _newAddress.Line1,
                Line2 = _newAddress.Line2,
                City = _newAddress.City,
                Postal = _newAddress.Postal,
                AddressType = _newAddress.AddressType
            });
            _newAddress = new CreateCustomerCommandAddressesModel1();
            await _addressForm.ResetAsync();
            _addressForm.ResetValidation();
        }

        private void RemoveAddress(CreateCustomerCommandAddressesModel1 addr)
        {
            Model.Addresses.Remove(addr);
        }

        public class CreateCustomerModel
        {
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Email { get; set; }
            public bool IsActive { get; set; }
            public CreateCustomerPreferenceModel Preference { get; set; }
        }
        public class CreateCustomerPreferenceModel
        {
            public bool NewsLetter { get; set; }
            public bool Specials { get; set; }
        }
        public class CreateCustomerModel1
        {
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Email { get; set; }
            public bool IsActive { get; set; }
            public CreateCustomerPreferenceModel1 Preference { get; set; }
            public CreateCustomerCommandLoyaltyModel? Loyalty { get; set; }
            public List<CreateCustomerCommandAddressesModel> Addresses { get; set; }
        }
        public class CreateCustomerPreferenceModel1
        {
            public bool NewsLetter { get; set; }
            public bool Specials { get; set; }
        }
        public class CreateCustomerCommandLoyaltyModel
        {
            public string LoyaltyNo { get; set; }
            public int Points { get; set; }
        }
        public class CreateCustomerCommandAddressesModel
        {
            public string Line1 { get; set; }
            public string? Line2 { get; set; }
            public string City { get; set; }
            public string Postal { get; set; }
            public string AddressType { get; set; }
        }
        public class CreateCustomerModel2
        {
            public string Name { get; set; } = string.Empty;
            public string Surname { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public bool IsActive { get; set; }
            public CreateCustomerPreferenceModel2 Preference { get; set; } = new();
            public CreateCustomerCommandLoyaltyModel1? Loyalty { get; set; }
            public List<CreateCustomerCommandAddressesModel1> Addresses { get; set; } = new();
        }
        public class CreateCustomerPreferenceModel2
        {
            public bool NewsLetter { get; set; }
            public bool Specials { get; set; }
        }
        public class CreateCustomerCommandLoyaltyModel1
        {
            public string LoyaltyNo { get; set; } = string.Empty;
            public int Points { get; set; }
        }
        public class CreateCustomerCommandAddressesModel1
        {
            public string Line1 { get; set; } = string.Empty;
            public string? Line2 { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string Postal { get; set; } = string.Empty;
            public AddressType AddressType { get; set; }
        }
    }
}
