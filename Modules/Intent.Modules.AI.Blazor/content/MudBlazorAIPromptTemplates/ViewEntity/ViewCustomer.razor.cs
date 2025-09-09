using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using UI.AI.Samples.Application.Customers;
using UI.AI.Samples.Application.Customers.GetCustomerById;
using UI.AI.Samples.Domain;
using UI.AI.Samples.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace UI.AI.Samples.Api.Components.Pages.PageBased.Customers
{
    public partial class ViewCustomer
    {
        [Parameter]
        public Guid CustomerId { get; set; }

        public CustomerDto? Model { get; set; }

        [Inject]
        public IScopedMediator Mediator { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await LoadCustomerById(CustomerId);
        }

        private void Cancel()
        {
            NavigationManager.NavigateTo("/page-based/customers/search");
        }

        private async Task LoadCustomerById(Guid id)
        {
            try
            {
                Model = await Mediator.Send(new GetCustomerByIdQuery(
                    id: id));
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }

        }

        private void Edit()
        {
            if (Model is null) return;
            NavigationManager.NavigateTo($"/page-based/customers/edit/{Model.Id}");
        }

        public class ViewCustomerModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Surname { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public bool IsActive { get; set; }
            public ViewCustomerPreferenceModel Preference { get; set; } = new();
            public ViewCustomerLoyaltyModel? Loyalty { get; set; }
            public List<ViewCustomerAddressModel> Addresses { get; set; } = new();
        }

        public class ViewCustomerPreferenceModel
        {
            public bool NewsLetter { get; set; }
            public bool Specials { get; set; }
        }

        public class ViewCustomerLoyaltyModel
        {
            public string LoyaltyNo { get; set; } = string.Empty;
            public int Points { get; set; }
        }

        public class ViewCustomerAddressModel
        {
            public string Line1 { get; set; } = string.Empty;
            public string? Line2 { get; set; }
            public string City { get; set; } = string.Empty;
            public string Postal { get; set; } = string.Empty;
            public AddressType AddressType { get; set; }
        }
    }
}
