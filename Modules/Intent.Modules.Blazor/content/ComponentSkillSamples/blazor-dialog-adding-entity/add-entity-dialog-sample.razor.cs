using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
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
        public CreateCustomerModel Model { get; set; } = new();
        public List<CategoryDto>? CategoryLookup { get; set; }
        public List<SubCategoryDto>? SubCategoryLookup { get; set; }
        public string? ErrorMessage { get; set; }

        [Inject]
        public IScopedMediator Mediator { get; set; } = default!;
        [Parameter]
        public EventCallback<bool> OnClosed { get; set; }

        private bool _saving;
        private CreateCustomerCommandAddressesModel _newAddress = new();
        private bool _showLoyalty;
        private CreateCustomerCommandLoyaltyModel _loyaltyModel = new();

        protected override async Task OnInitializedAsync()
        {
            await GetCategories();

            Model.Preference ??= new CreateCustomerPreferenceModel();
            Model.Addresses ??= new List<CreateCustomerCommandAddressesModel>();
            _loyaltyModel = Model.Loyalty ?? new CreateCustomerCommandLoyaltyModel();
            _showLoyalty = Model.Loyalty is not null;
        }

        private async Task GetCategories()
        {
            try
            {
                CategoryLookup = await Mediator.Send(new GetCategoryLookupQuery());
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }

        private async Task GetSubCategories(Guid categoryId)
        {
            try
            {
                SubCategoryLookup = await Mediator.Send(new GetSubCategoryLookupQuery(categoryId));
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }

        private async Task OnCategoryChanged(Guid? id)
        {
            Model.CategoryId = id;
            Model.SubCategoryId = null;
            SubCategoryLookup = null;
            if (id != null && id.Value != Guid.Empty)
            {
                await GetSubCategories(id.Value);
            }
            StateHasChanged();
        }

        private async Task CreateCustomer()
        {
            await Mediator.Send(new CreateCustomerCommand(
                name: Model.Name,
                surname: Model.Surname,
                email: Model.Email,
                categoryId: Model.CategoryId.Value,
                subCategoryId: Model.SubCategoryId.Value,
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

        private async Task Cancel()
        {
            await OnClosed.InvokeAsync(false);
        }

        private async Task SaveAsync()
        {
            ErrorMessage = null;
            if (Model.Addresses == null || Model.Addresses.Count == 0)
            {
                ErrorMessage = "At least one address is required.";
                return;
            }

            _saving = true;
            try
            {
                Model.Loyalty = _showLoyalty ? _loyaltyModel : null;
                await CreateCustomer();
                await OnClosed.InvokeAsync(true);
            }
            catch (Exception e)
            {
                ErrorMessage = $"Failed to save customer: {e.Message}";
            }
            finally { _saving = false; }
        }

        private void AddAddress()
        {
            Model.Addresses.Add(new CreateCustomerCommandAddressesModel
            {
                Line1 = _newAddress.Line1,
                Line2 = _newAddress.Line2,
                City = _newAddress.City,
                Postal = _newAddress.Postal,
                AddressType = _newAddress.AddressType
            });
            _newAddress = new CreateCustomerCommandAddressesModel();
        }

        private void RemoveAddress(CreateCustomerCommandAddressesModel addr)
        {
            Model.Addresses.Remove(addr);
        }

        public class CreateCustomerModel
        {
            [Required]
            public string Name { get; set; } = string.Empty;
            [Required]
            public string Surname { get; set; } = string.Empty;
            [Required, EmailAddress]
            public string Email { get; set; } = string.Empty;
            [Required]
            public Guid? CategoryId { get; set; }
            [Required]
            public Guid? SubCategoryId { get; set; }
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
            [Required]
            public string Line1 { get; set; } = string.Empty;
            public string? Line2 { get; set; } = string.Empty;
            [Required]
            public string City { get; set; } = string.Empty;
            [Required]
            public string Postal { get; set; } = string.Empty;
            public AddressType AddressType { get; set; }
        }
    }
}
