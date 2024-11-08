using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor.ExampleApp.Client.HttpClients;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Invoices;
using MudBlazor.ExampleApp.Client.Pages.Invoices.Components;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.Invoices
{
    public partial class InvoiceEdit
    {
        [Parameter]
        public Guid InvoiceId { get; set; }
        public InvoiceModel? Model { get; set; } = new();
        [Inject]
        public IInvoiceService InvoiceService { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var invoiceDto = await InvoiceService.GetInvoiceByIdAsync(InvoiceId);
                Model = new InvoiceModel
                {
                    Id = invoiceDto.Id,
                    InvoiceNo = invoiceDto.InvoiceNo,
                    InvoiceDate = invoiceDto.InvoiceDate,
                    OrderLines = invoiceDto.OrderLines
                        .Select(ol => new InvoiceLineModel
                        {
                            InvoiceId = ol.InvoiceId,
                            ProductId = ol.ProductId,
                            Units = ol.Units,
                            UnitPrice = ol.UnitPrice,
                            Discount = ol.Discount,
                            Id = ol.Id,
                            ProductName = ol.ProductName,
                            ProductDescription = ol.ProductDescription,
                            ProductImageUrl = ol.ProductImageUrl
                        })
                        .ToList(),
                    CustomerId = invoiceDto.CustomerId,
                    CustomerName = invoiceDto.CustomerName,
                    CustomerAccountNo = invoiceDto.CustomerAccountNo,
                    AddressLine1 = invoiceDto.AddressLine1,
                    AddressLine2 = invoiceDto.AddressLine2,
                    AddressCity = invoiceDto.AddressCity,
                    AddressCountry = invoiceDto.AddressCountry,
                    AddressPostal = invoiceDto.AddressPostal,
                    DueDate = invoiceDto.DueDate,
                    Reference = invoiceDto.Reference
                };
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
                await InvoiceService.UpdateInvoiceAsync(InvoiceId, new UpdateInvoiceCommand
                {
                    Id = Model.Id,
                    InvoiceNo = Model.InvoiceNo,
                    InvoiceDate = Model.InvoiceDate.Value,
                    DueDate = Model.DueDate.Value,
                    Reference = Model?.Reference,
                    CustomerId = Model.CustomerId.Value,
                    OrderLines = Model.OrderLines
                        .Select(ol => new UpdateInvoiceCommandOrderLinesDto
                        {
                            Id = ol.Id,
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
    }
}