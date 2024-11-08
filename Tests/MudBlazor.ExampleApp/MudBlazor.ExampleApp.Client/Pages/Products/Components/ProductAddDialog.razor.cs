using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor.ExampleApp.Client.HttpClients;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.Products.Components
{
    public partial class ProductAddDialog
    {
        private MudForm _form;
        private bool _onSaveClickedProcessing = false;
        public ProductDto Model { get; set; } = new();
        [Inject]
        public IProductsService ProductsService { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        [CascadingParameter]
        public MudDialogInstance Dialog { get; set; }

        protected override async Task OnInitializedAsync()
        {
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
                await ProductsService.CreateProductAsync(new CreateProductCommand
                {
                    Name = Model.Name,
                    Description = Model.Description,
                    Price = Model.Price,
                    ImageUrl = Model.ImageUrl
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
    }
}