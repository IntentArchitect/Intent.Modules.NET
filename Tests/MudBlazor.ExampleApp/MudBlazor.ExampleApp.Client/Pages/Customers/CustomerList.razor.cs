using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor.ExampleApp.Client.HttpClients;
using MudBlazor.ExampleApp.Client.HttpClients.Common;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Customers;
using MudBlazor.ExampleApp.Client.Pages.Customers.Components;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.Customers
{
    public partial class CustomerList
    {
        private bool _addCustomerClickProcessing = false;
        private bool _onDeleteClickProcessing = false;
        public PagedResult<CustomerDto>? Model { get; set; }
        public MudDataGrid<CustomerDto> DataGrid { get; set; }
        public string SearchText { get; set; }
        [Inject]
        public ICustomersService Customers { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        [Inject]
        public IDialogService DialogService { get; set; } = default!;

        private async Task FetchDataGridData(int pageNo, int pageSize, string sorting)
        {
            try
            {
                Model = await Customers.GetCustomersAsync(new GetCustomersQuery
                {
                    PageNo = pageNo,
                    PageSize = pageSize,
                    OrderBy = sorting,
                    SearchText = SearchText
                });
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }

        private async Task AddCustomerClick()
        {
            try
            {
                _addCustomerClickProcessing = true;
                var dialog = await DialogService.ShowAsync<CustomerAddDialog>("Add Customer", new DialogOptions() { FullWidth = true });
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
                _addCustomerClickProcessing = false;
            }
        }

        private async Task DataGridRowClick(string rowId)
        {
            try
            {
                var parameters = new DialogParameters<CustomerEditDialog>
                {
                    { x => x.CustomerId, Guid.Parse(rowId) },
                };
                var dialog = await DialogService.ShowAsync<CustomerEditDialog>("Edit Customer", parameters, new DialogOptions() { FullWidth = true });
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

        private async Task OnDeleteClick(Guid rowId)
        {
            try
            {
                _onDeleteClickProcessing = true;
                var parameters = new DialogParameters<ConfirmationDialog>
                {
                    { x => x.ContentText, "Are you sure you want to delete this customer?" },
                    { x => x.ButtonText, "Delete" },
                    { x => x.Color, Color.Error },
                };
                var dialog = await DialogService.ShowAsync<ConfirmationDialog>("Delete Customer", parameters, new DialogOptions() { FullWidth = true });
                var result = await dialog.Result;

                if (result.Canceled)
                {
                    return;
                }
                await Customers.DeleteCustomerAsync(new DeleteCustomerCommand
                {
                    Id = rowId
                });
                await DataGrid.ReloadServerData();
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

        private async Task<GridData<CustomerDto>> LoadDataGridData(GridState<CustomerDto> state)
        {
            var pageNo = state.Page + 1;
            var pageSize = state.PageSize;
            var sorting = string.Join(", ", state.SortDefinitions.Select(x => $"{x.SortBy} {(x.Descending ? "desc" : "asc")}"));
            await FetchDataGridData(pageNo, pageSize, sorting);
            return new GridData<CustomerDto>() { TotalItems = Model?.TotalCount ?? 0, Items = Model?.Data ?? [] };
        }
    }
}