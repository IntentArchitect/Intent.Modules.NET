using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor.ExampleApp.Client.HttpClients;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Customers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.Customers.Components
{
    public partial class CustomerEditDialog
    {
        private MudForm _form;
        private bool _onSaveClickedProcessing = false;
        private bool _onDeleteClickedProcessing = false;
        [Parameter]
        public Guid CustomerId { get; set; }
        public CustomerDto? Model { get; set; }
        [Inject]
        public ICustomersService CustomersService { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        [Inject]
        public IDialogService DialogService { get; set; } = default!;
        [CascadingParameter]
        public IMudDialogInstance Dialog { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Model = await CustomersService.GetCustomerByIdAsync(CustomerId);
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }

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
                await CustomersService.UpdateCustomerAsync(new UpdateCustomerCommand
                {
                    Id = CustomerId,
                    Name = Model.Name,
                    AccountNo = Model?.AccountNo,
                    Address = new UpdateCustomerAddressDto
                    {
                        Line1 = Model.AddressLine1,
                        Line2 = Model?.AddressLine2,
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

        private async Task OnDeleteClicked()
        {
            try
            {
                _onDeleteClickedProcessing = true;
                var parameters = new DialogParameters<ConfirmationDialog>
                {
                    { x => x.ContentText, "Are you sure you want to delete this customer" },
                    { x => x.ButtonText, "Delete" },
                    { x => x.Color, Color.Error },
                };
                var dialog = await DialogService.ShowAsync<ConfirmationDialog>("Delete Customer", parameters, new DialogOptions() { FullWidth = true });
                var result = await dialog.Result;

                if (result.Canceled)
                {
                    return;
                }
                await CustomersService.DeleteCustomerAsync(CustomerId);
                Dialog.Close(true);
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
            finally
            {
                _onDeleteClickedProcessing = false;
            }
        }
    }
}