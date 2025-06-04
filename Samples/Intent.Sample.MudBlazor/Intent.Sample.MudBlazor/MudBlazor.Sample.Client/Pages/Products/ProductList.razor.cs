using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor.Sample.Client.HttpClients;
using MudBlazor.Sample.Client.HttpClients.Common;
using MudBlazor.Sample.Client.HttpClients.Contracts.Services.Products;
using MudBlazor.Sample.Client.Pages.Products.Components;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.Sample.Client.Pages.Products
{
    public partial class ProductList
    {
        private bool _addProductClickProcessing = false;
        public PagedResult<ProductDto>? Model { get; set; }
        public MudDataGrid<ProductDto> DataGrid { get; set; }
        public string SearchText { get; set; }
        [Inject]
        public IProductsService ProductsService { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        [Inject]
        public IDialogService DialogService { get; set; } = default!;

        private async Task FetchDataGridData(int pageNo, int pageSize, string sorting)
        {
            try
            {
                Model = await ProductsService.GetProductsAsync(
                    pageNo,
                    pageSize,
                    sorting,
                    SearchText);
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }

        private async Task AddProductClick()
        {
            try
            {
                _addProductClickProcessing = true;
                var dialog = await DialogService.ShowAsync<ProductAddDialog>("Add Product", new DialogOptions() { FullWidth = true });
                var result = await dialog.Result;

                if (result.Canceled)
                {
                    return;
                }
                await DataGrid.ReloadServerData();
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
            finally
            {
                _addProductClickProcessing = false;
            }
        }

        private async Task OnRowClick(string rowId)
        {
            try
            {
                var parameters = new DialogParameters<ProductEditDialog>
                {
                    { x => x.ProductId, Guid.Parse(rowId) },
                };
                var dialog = await DialogService.ShowAsync<ProductEditDialog>("Edit Product", parameters, new DialogOptions() { FullWidth = true });
                var result = await dialog.Result;

                if (result.Canceled)
                {
                    return;
                }
                await DataGrid.ReloadServerData();
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }

        private async Task OnSearch(string value)
        {
            SearchText = value;

            try
            {
                await DataGrid.ReloadServerData();
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }

        private async Task<GridData<ProductDto>> LoadDataGridData(GridState<ProductDto> state)
        {
            var pageNo = state.Page + 1;
            var pageSize = state.PageSize;
            var sorting = string.Join(", ", state.SortDefinitions.Select(x => $"{x.SortBy} {(x.Descending ? "desc" : "asc")}"));
            await FetchDataGridData(pageNo, pageSize, sorting);
            return new GridData<ProductDto>() { TotalItems = Model?.TotalCount ?? 0, Items = Model?.Data ?? [] };
        }
    }
}