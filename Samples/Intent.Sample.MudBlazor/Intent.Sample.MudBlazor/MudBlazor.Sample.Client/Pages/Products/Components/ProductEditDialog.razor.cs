using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor.Sample.Client.HttpClients;
using MudBlazor.Sample.Client.HttpClients.Contracts.Services.Products;
using MudBlazor.Sample.Client.Pages.Customers.Components;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.Sample.Client.Pages.Products.Components
{
    public partial class ProductEditDialog
    {
        private MudForm _form;
        private bool _onSaveClickedProcessing = false;
        private bool _onDeleteClickProcessing = false;
        [Parameter]
        public Guid ProductId { get; set; }
        public ProductDto? Model { get; set; }
        [Inject]
        public IProductsService ProductsService { get; set; } = default!;
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
                Model = await ProductsService.GetProductByIdAsync(ProductId);
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
            StateHasChanged();
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
                await ProductsService.UpdateProductAsync(ProductId, new UpdateProductCommand
                {
                    Name = Model.Name,
                    Description = Model.Description,
                    Price = Model.Price,
                    ImageUrl = Model?.ImageUrl,
                    Id = Model.Id
                });
                Dialog.Close();
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

        private async Task OnDeleteClick()
        {
            try
            {
                _onDeleteClickProcessing = true;
                var parameters = new DialogParameters<ConfirmationDialog>
                {
                    { x => x.ContentText, "Are you sure you want to delete this Product?" },
                    { x => x.ButtonText, "Delete" },
                    { x => x.Color, Color.Error },
                };
                var dialog = await DialogService.ShowAsync<ConfirmationDialog>("Delete Product", parameters, new DialogOptions() { FullWidth = true });
                var result = await dialog.Result;

                if (result.Canceled)
                {
                    return;
                }
                await ProductsService.DeleteProductAsync(ProductId);
                Dialog.Close();
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
            finally
            {
                _onDeleteClickProcessing = false;
            }
        }
    }
}