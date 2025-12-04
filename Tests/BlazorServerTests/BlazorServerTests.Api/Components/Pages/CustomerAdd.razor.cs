using System.ComponentModel.DataAnnotations;
using BlazorServerTests.Application.Customers.CreateCustomer;
using BlazorServerTests.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace BlazorServerTests.Api.Components.Pages
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

        protected override async Task OnInitializedAsync()
        {
        }

        private void Cancel()
        {
            NavigationManager.NavigateTo("customer-search");
        }

        private async Task CreateCustomer()
        {
            if (_form is not null)
            {
                await _form.Validate();
                if (!_form.IsValid)
                {
                    Snackbar.Add("Please fix validation errors before saving.", Severity.Warning);
                    return;
                }
            }
            try
            {
                await Mediator.Send(new CreateCustomerCommand(
                    name: Model.Name,
                    surname: Model.Surname,
                    email: Model.Email,
                    isActive: Model.IsActive));
                Snackbar.Add("Customer created successfully.", Severity.Success);
                NavigationManager.NavigateTo("/customer-search");
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }

        public class CreateCustomerModel
        {
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Email { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
