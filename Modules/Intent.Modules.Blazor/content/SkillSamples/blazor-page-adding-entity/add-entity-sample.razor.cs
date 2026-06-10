using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using UI.AI.Samples.Application.Categories;
using UI.AI.Samples.Application.Categories.GetCategories;
using UI.AI.Samples.Application.Customers;
using UI.AI.Samples.Application.Customers.CreateCustomer;
using UI.AI.Samples.Application.SubCategories;
using UI.AI.Samples.Application.SubCategories.GetSubCategories;
using UI.AI.Samples.Domain;
using UI.AI.Samples.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace UI.AI.Samples.Api.Components.Pages.Templates.Pages
{
    public partial class CustomerAdd
    {
        public List<CategoryDto>? CategoriesLookupModels { get; set; }
        public List<SubCategoryDto>? SubCategoriesLookupModels { get; set; }
        public CreateCustomerModel Model { get; set; } = new();
        [Inject]
        public IScopedMediator Mediator { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        private bool _saving;
        private string? _errorMessage;
        private bool _hasLoyalty;
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

        protected override async Task OnInitializedAsync()
        {
            await LoadCategories();
            Model.Preference ??= new CreateCustomerPreferenceModel();
            Model.Addresses ??= new List<CreateCustomerCommandAddressesModel>();
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

        private async Task LoadCategories()
        {
            try
            {
                CategoriesLookupModels = await Mediator.Send(new GetCategoriesQuery());
            }
            catch (Exception e)
            {
                _errorMessage = e.Message;
            }
        }

        private async Task OnCategoryChanged()
        {
            Model.SubCategoryId = null;
            SubCategoriesLookupModels = null;
            if (Model.CategoryId != null)
            {
                await LoadSubCategories(Model.CategoryId);
            }
        }

        private async Task LoadSubCategories(Guid? categoryId)
        {
            try
            {
                SubCategoriesLookupModels = await Mediator.Send(new GetSubCategoriesQuery(
                    categoryId: categoryId));
            }
            catch (Exception e)
            {
                _errorMessage = e.Message;
            }
        }

        private async Task SaveAsync()
        {
            _saving = true;
            _errorMessage = null;
            try
            {
                if (!_hasLoyalty)
                {
                    Model.Loyalty = null;
                }
                await CreateCustomer();
                NavigationManager.NavigateTo("templates/pages/customers");
            }
            catch (Exception ex)
            {
                _errorMessage = $"Failed to save customer: {ex.Message}";
            }
            finally
            {
                _saving = false;
            }
        }

        private void Cancel()
        {
            NavigationManager.NavigateTo("templates/pages/customers");
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
                        Points = Model.Loyalty.Points.Value
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

        public class CreateCustomerModel
        {
            [Required(ErrorMessage = "Name is required")]
            public string Name { get; set; } = string.Empty;
            [Required(ErrorMessage = "Surname is required")]
            public string Surname { get; set; } = string.Empty;
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Please enter a valid email")]
            public string Email { get; set; } = string.Empty;
            [Required(ErrorMessage = "Category is required")]
            public Guid? CategoryId { get; set; }
            [Required(ErrorMessage = "Sub Category is required")]
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
            public int? Points { get; set; }
        }
        public class CreateCustomerCommandAddressesModel
        {
            [Required(ErrorMessage = "Address type required")]
            public AddressType AddressType { get; set; }
            [Required(ErrorMessage = "Line 1 is required")]
            public string Line1 { get; set; } = string.Empty;
            public string? Line2 { get; set; }
            [Required(ErrorMessage = "City is required")]
            public string City { get; set; } = string.Empty;
            [Required(ErrorMessage = "Postal code is required")]
            public string Postal { get; set; } = string.Empty;
        }
    }
}
