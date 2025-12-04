using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using UI.AI.Samples.Application.Customers;
using UI.AI.Samples.Application.Customers.GetCustomerById;
using UI.AI.Samples.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace UI.AI.Samples.Api.Components.Pages.Templates.Pages
{
    public partial class CustomerView
    {
        [Parameter]
        public Guid CustomerId { get; set; }
        public CustomerDto? CustomerByIdModels { get; set; }
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

        private async Task LoadCustomerById(Guid id)
        {
            try
            {
                CustomerByIdModels = await Mediator.Send(new GetCustomerByIdQuery(
                    id: id));
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }

        private void Cancel()
        {
            NavigationManager.NavigateTo("templates/pages/customers");
        }
    }
}
