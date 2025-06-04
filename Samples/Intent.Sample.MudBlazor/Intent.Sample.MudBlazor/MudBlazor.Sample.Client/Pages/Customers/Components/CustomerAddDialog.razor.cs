using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor.Sample.Client.HttpClients;
using MudBlazor.Sample.Client.HttpClients.Contracts.Services.Customers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.Sample.Client.Pages.Customers.Components
{
    public partial class CustomerAddDialog
    {
        private MudForm _form;
        private bool _onSaveClickedProcessing = false;
        public CustomerDto Model { get; set; } = new();
        [Inject]
        public ICustomersService CustomersService { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        [CascadingParameter]
        public IMudDialogInstance Dialog { get; set; }

        private async Task OnSaveClicked()
        {
            try
            {
                _onSaveClickedProcessing = true;
                await _form!.Validate();
                if (!_form.IsValid)
                {
                    return;
                }
                await CustomersService.CreateCustomerAsync(new CreateCustomerCommand
                {
                    Name = Model.Name,
                    AccountNo = Model.AccountNo,
                    Address = new CreateCustomerAddressDto
                    {
                        Line1 = Model.AddressLine1,
                        Line2 = Model.AddressLine2,
                        City = Model.AddressCity,
                        Country = Model.AddressCountry,
                        Postal = Model.AddressPostal
                    }
                });
                Dialog.Close(true);
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
            finally
            {
                _onSaveClickedProcessing = false;
            }
        }

        private void OnCancelClicked()
        {
            Dialog.Cancel();
        }
    }
}