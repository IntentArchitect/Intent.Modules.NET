using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor.ExampleApp.Client.HttpClients;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Invoices;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Products;
using MudBlazor.ExampleApp.Client.Pages.Invoices.Components;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.Invoices
{
    public partial class InvoiceAdd
    {
        public InvoiceModel Model { get; set; } = new();
        [Inject]
        public IInvoiceService InvoiceService { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
        }

        private async Task OnSaveClicked()
        {
            try
            {
                await InvoiceService.CreateInvoiceAsync(new CreateInvoiceCommand
                {
                    InvoiceNo = Model.InvoiceNo,
                    InvoiceDate = Model.InvoiceDate.Value,
                    DueDate = Model.DueDate.Value,
                    Reference = Model.Reference,
                    CustomerId = Model.CustomerId.Value,
                    OrderLines = Model.OrderLines
                        .Select(ol => new CreateInvoiceCommandOrderLinesDto
                        {
                            ProductId = ol.ProductId.Value,
                            Units = ol.Units,
                            UnitPrice = ol.UnitPrice,
                            Discount = ol.Discount
                        })
                        .ToList()
                });
                NavigationManager.NavigateTo("/invoices");
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }

        private void OnCancelClicked()
        {
            NavigationManager.NavigateTo("/invoices");
        }

        private async Task OnSaveClickedAlt()
        {
            try
            {
                await InvoiceService.CreateInvoiceConvolutedAsync(new CreateInvoiceConvolutedCommand()
                {
                    Invoice = new CreateInvoiceDTO
                    {
                        InvoiceNo = Model.InvoiceNo,
                        IssuedDate = Model.InvoiceDate.Value,
                        DueDate = Model.DueDate.Value,
                        Reference = Model.Reference,
                        CustomerId = Model.CustomerId.Value
                    }
                });
                NavigationManager.NavigateTo("/invoices");
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }
    }
}