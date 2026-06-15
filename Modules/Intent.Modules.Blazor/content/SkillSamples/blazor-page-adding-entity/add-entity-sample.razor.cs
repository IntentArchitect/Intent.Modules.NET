using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor;
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
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        private MudForm? _form;
        private bool _saving;
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
                Snackbar.Add(e.Message, Severity.Error);
            }
        }

        private async Task OnCategoryChanged(Guid? id)
        {
            Model.CategoryId = id;
            Model.SubCategoryId = null;
            SubCategoriesLookupModels = null;
            if (id != null)
            {
                await LoadSubCategories(id);
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
                Snackbar.Add(e.Message, Severity.Error);
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
                await CreateCustomer();
                Snackbar.Add("Customer created successfully.", Severity.Success);
                NavigationManager.NavigateTo("templates/pages/customers");
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
            NavigationManager.NavigateTo("templates/pages/customers");
        }

        private async Task CreateCustomer()
        {
            try
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

        public class CreateCustomerModel
        {
            public string Name { get; set; } = string.Empty;
            public string Surname { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public Guid? CategoryId { get; set; }
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
            public string Line1 { get; set; } = string.Empty;
            public string? Line2 { get; set; }
            public string City { get; set; } = string.Empty;
            public string Postal { get; set; } = string.Empty;
            public AddressType AddressType { get; set; }
        }
    }
}
