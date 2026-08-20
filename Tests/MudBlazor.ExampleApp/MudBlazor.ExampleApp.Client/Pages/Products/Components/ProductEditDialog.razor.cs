using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor.ExampleApp.Client.HttpClients;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Products;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.Products.Components
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
        public IDummyService DummyService { get; set; } = default!;
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

        private async Task CallDummyOperation(Guid oddlyNamedParameter, string dummyName)
        {
            try
            {
                await DummyService.DummyOperationAsync(oddlyNamedParameter, dummyName);
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }
    }
}