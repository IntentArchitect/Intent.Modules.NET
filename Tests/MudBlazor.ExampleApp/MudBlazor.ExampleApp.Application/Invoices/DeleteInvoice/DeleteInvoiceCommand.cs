using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.ExampleApp.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Invoices.DeleteInvoice
{
    public class DeleteInvoiceCommand : IRequest, ICommand
    {
        public DeleteInvoiceCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}