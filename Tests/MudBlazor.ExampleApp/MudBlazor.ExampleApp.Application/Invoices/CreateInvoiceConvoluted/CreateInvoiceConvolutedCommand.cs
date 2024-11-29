using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.ExampleApp.Application.Common.Interfaces;
using MudBlazor.ExampleApp.Application.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Invoices.CreateInvoiceConvoluted
{
    public class CreateInvoiceConvolutedCommand : IRequest, ICommand
    {
        public CreateInvoiceConvolutedCommand(CreateInvoiceDTO invoice)
        {
            Invoice = invoice;
        }

        public CreateInvoiceDTO Invoice { get; set; }
    }
}