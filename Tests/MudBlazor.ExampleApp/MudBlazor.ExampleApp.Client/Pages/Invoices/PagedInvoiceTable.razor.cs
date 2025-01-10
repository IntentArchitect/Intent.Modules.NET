using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor.ExampleApp.Client.HttpClients;
using MudBlazor.ExampleApp.Client.HttpClients.Common;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Invoices;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.Invoices
{
    public partial class PagedInvoiceTable
    {
        private bool _onPageChangedProcessing = false;
        public PagedResult<InvoiceDto>? Model { get; set; }
        public int PageSize { get; set; } = 20;
        public string OrderBy { get; set; } = "DueDate";
        [Inject]
        public IInvoiceService InvoiceService { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await InvoiceService.GetInvoicesAsync(
                    1,
                    PageSize,
                    OrderBy);
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
            StateHasChanged();
        }

        private async Task OnPageChanged(int pageNo)
        {
            try
            {
                _onPageChangedProcessing = true;
                await InvoiceService.GetInvoicesAsync(
                    pageNo,
                    PageSize,
                    OrderBy);
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }
    }
}