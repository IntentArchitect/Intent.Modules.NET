using Intent.RoslynWeaver.Attributes;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Invoices
{
    public class CreateInvoiceConvolutedCommand
    {
        public CreateInvoiceConvolutedCommand()
        {
            Invoice = new();
        }

        public CreateInvoiceDTO Invoice { get; set; }

        public static CreateInvoiceConvolutedCommand Create(CreateInvoiceDTO invoice)
        {
            return new CreateInvoiceConvolutedCommand
            {
                Invoice = invoice
            };
        }
    }
}