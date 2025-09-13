using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using UI.AI.Samples.Application.Categories;
using UI.AI.Samples.Application.Categories.GetCategories;
using UI.AI.Samples.Application.Customers;
using UI.AI.Samples.Application.Customers.GetCustomerById;
using UI.AI.Samples.Application.Customers.UpdateCustomer;
using UI.AI.Samples.Application.SubCategories;
using UI.AI.Samples.Application.SubCategories.GetSubCategories;
using UI.AI.Samples.Domain;
using UI.AI.Samples.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace UI.AI.Samples.Api.Components.Pages.Templates.Pages
{
    public partial class CustomerEdit
    {
        [Parameter]
        public Guid CustomerId { get; set; }
        public List<CategoryDto>? CategoriesLookupModels { get; set; }
        public UpdateCustomerModel? Model { get; set; }
        public List<SubCategoryDto>? SubCategoriesLookupModels { get; set; }
        [Inject]
        public IScopedMediator Mediator { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        private MudForm? _form;
        private bool _saving;
        private bool _hasLoyalty;

        protected override async Task OnInitializedAsync()
        {
            await LoadCategories();
            await LoadCustomerById(CustomerId);
            if (Model?.CategoryId != null)
                await LoadSubCategories(Model.CategoryId);
            _hasLoyalty = Model?.Loyalty != null;
            StateHasChanged();
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

        private async Task UpdateCustomer()
        {
            try
            {
                await Mediator.Send(new UpdateCustomerCommand(
                    id: Model.Id.Value,
                    name: Model.Name,
                    surname: Model.Surname,
                    email: Model.Email,
                    isActive: Model.IsActive,
                    preference: new UpdateCustomerPreferenceDto
                    {
                        Id = Model.Preference.Id.Value,
                        NewsLetter = Model.Preference.NewsLetter,
                        Specials = Model.Preference.Specials
                    },
                    categoryId: Model.CategoryId.Value,
                    loyalty: Model?.Loyalty is not null
                        ? new UpdateCustomerCommandLoyaltyDto
                        {
                            Id = Model.Loyalty.Id.Value,
                            LoyaltyNo = Model.Loyalty.LoyaltyNo,
                            Points = Model.Loyalty.Points.Value
                        }
                        : null,
                    subCategoryId: Model.SubCategoryID.Value,
                    addresses: Model.Addresses
                        .Select(a => new UpdateCustomerCommandAddressesDto
                        {
                            Id = a.Id.Value,
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
                    CategoryId = customerDto.CategoryId,
                    Loyalty = customerDto.Loyalty is not null
                        ? new UpdateCustomerCommandLoyaltyModel
                        {
                            Id = customerDto.Loyalty?.Id,
                            LoyaltyNo = customerDto.Loyalty.LoyaltyNo,
                            Points = customerDto.Loyalty?.Points
                        }
                        : null,
                    SubCategoryID = customerDto.SubCategoryId,
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

        private async Task OnCategoryChanged(Guid? categoryId)
        {
            Model.CategoryId = categoryId;
            Model.SubCategoryID = null;
            await LoadSubCategories(categoryId);
            StateHasChanged();
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
                            Id = Guid.NewGuid(),
                            LoyaltyNo = string.Empty,
                            Points = 0
                        };
                    }
                }
                await UpdateCustomer();
                Snackbar.Add("Customer updated successfully.", Severity.Success);
                NavigationManager.NavigateTo("/templates/pages/customers");
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
            NavigationManager.NavigateTo("/templates/pages/customers");
        }

        public void RemoveAddress(int index)
        {
            if (Model == null) return;
            if (index >= 0 && index < Model.Addresses.Count)
                Model.Addresses.RemoveAt(index);
        }

        public void AddAddress()
        {
            if (Model == null) return;
            Model.Addresses.Add(new UpdateCustomerCommandAddressesModel
            {
                Id = Guid.NewGuid(),
                Line1 = string.Empty,
                Line2 = string.Empty,
                City = string.Empty,
                Postal = string.Empty,
                AddressType = Model.Addresses.Count(a => a.AddressType == AddressType.Deliver) == 0 ? AddressType.Deliver : AddressType.Billing
            });
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
                            Id = Guid.NewGuid(),
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

        public class UpdateCustomerModel
        {
            public Guid? Id { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Email { get; set; }
            public bool IsActive { get; set; }
            public UpdateCustomerPreferenceModel Preference { get; set; }
            public Guid? CategoryId { get; set; }
            public UpdateCustomerCommandLoyaltyModel? Loyalty { get; set; }
            public Guid? SubCategoryID { get; set; }
            public List<UpdateCustomerCommandAddressesModel> Addresses { get; set; }
        }
        public class UpdateCustomerPreferenceModel
        {
            public Guid? Id { get; set; }
            public bool NewsLetter { get; set; }
            public bool Specials { get; set; }
        }
        public class UpdateCustomerCommandLoyaltyModel
        {
            public Guid? Id { get; set; }
            public string LoyaltyNo { get; set; }
            public int? Points { get; set; }
        }
        public class UpdateCustomerCommandAddressesModel
        {
            public Guid? Id { get; set; }
            public string Line1 { get; set; }
            public string? Line2 { get; set; }
            public string City { get; set; }
            public string Postal { get; set; }
            public AddressType AddressType { get; set; }
        }
    }
}
