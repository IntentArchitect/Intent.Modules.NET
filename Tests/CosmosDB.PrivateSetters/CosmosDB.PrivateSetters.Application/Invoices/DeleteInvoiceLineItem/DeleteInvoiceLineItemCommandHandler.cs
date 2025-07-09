using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.Invoices.DeleteInvoiceLineItem
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteInvoiceLineItemCommandHandler : IRequestHandler<DeleteInvoiceLineItemCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteInvoiceLineItemCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(DeleteInvoiceLineItemCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (DeleteInvoiceLineItemCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}