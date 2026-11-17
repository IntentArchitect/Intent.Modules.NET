using Intent.RoslynWeaver.Attributes;
using Intent.Sample.Blazor.Server.Application.Brands;
using Intent.Sample.Blazor.Server.Application.Brands.GetBrands;
using Mediator;
using Microsoft.AspNetCore.Components;
using MudBlazor;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace Intent.Sample.Blazor.Server.Api.Components.Pages.Brand
{
    public partial class BrandListPage
    {
        public List<BrandDto> Brands { get; set; } = [];

        [Inject]
        public IScopedMediator Mediator { get; set; } = default!;
        [Inject]
        public IDialogService DialogService { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await LoadBrandsAsync();
        }

        private async Task LoadBrandsAsync()
        {
            try
            {
                Brands = await Mediator.Send(new GetBrandsQuery());
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
                Brands = [];
            }
        }

        private async Task OpenBrandAddDialogAsync()
        {
            var dialog = await DialogService.ShowAsync<BrandAddDialog>("Add Brand");
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await LoadBrandsAsync();
                StateHasChanged();
            }
        }

        private async Task OpenBrandEditDialogAsync(Guid brandId)
        {
            var parameters = new DialogParameters<BrandEditDialog>
            {
            { x => x.BrandId, brandId }
            };

            var dialog = await DialogService.ShowAsync<BrandEditDialog>("Edit Brand", parameters);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await LoadBrandsAsync();
                StateHasChanged();
            }
        }
    }
}
