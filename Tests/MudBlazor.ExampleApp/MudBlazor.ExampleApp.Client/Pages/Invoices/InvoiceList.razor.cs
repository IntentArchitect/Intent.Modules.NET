using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor.ExampleApp.Client.HttpClients;
using MudBlazor.ExampleApp.Client.HttpClients.Common;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Invoices;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.Invoices
{
    public partial class InvoiceList
    {
        public PagedResult<InvoiceDto>? Model { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        public IInvoiceService InvoiceService { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        private void AddInvoiceClick()
        {
            NavigationManager.NavigateTo("/invoices/add");
        }

        private void OnRowClicked(string invoiceId)
        {
            NavigationManager.NavigateTo($"/invoices/{Guid.Parse(invoiceId)}");
        }

        private async Task FetchDataGridData(int pageNo, int pageSize, string sorting)
        {
            try
            {
                Model = await InvoiceService.GetInvoicesAsync(
                    pageNo,
                    pageSize,
                    sorting);
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }

        private async Task<GridData<InvoiceDto>> LoadDataGridData(GridState<InvoiceDto> state)
        {
            var pageNo = state.Page + 1;
            var pageSize = state.PageSize;
            var sorting = string.Join(", ", state.SortDefinitions.Select(x => $"{x.SortBy} {(x.Descending ? "desc" : "asc")}"));
            await FetchDataGridData(pageNo, pageSize, sorting);
            return new GridData<InvoiceDto>() { TotalItems = Model?.TotalCount ?? 0, Items = Model?.Data ?? [] };
        }
    }
}