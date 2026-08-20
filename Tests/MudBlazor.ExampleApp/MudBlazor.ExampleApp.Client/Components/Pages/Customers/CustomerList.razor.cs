using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor.ExampleApp.Client.HttpClients;
using MudBlazor.ExampleApp.Client.HttpClients.Common;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Customers;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.PageCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Components.Pages.Customers
{
    public partial class CustomerList
    {
        private bool _addCustomerClickProcessing = false;
        private bool _onDeleteClickProcessing = false;
        public PagedResult<CustomerDto>? Model { get; set; }
        public MudDataGrid<CustomerDto> DataGrid { get; set; }
        public string SearchText { get; set; }
        [Inject]
        public ICustomersService CustomersService { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        private async Task FetchDataGridData(int pageNo, int pageSize, string sorting)
        {
            try
            {
                Model = await CustomersService.GetCustomersAsync(new GetCustomersQuery
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
                await CustomersService.DeleteCustomerAsync(rowId);
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