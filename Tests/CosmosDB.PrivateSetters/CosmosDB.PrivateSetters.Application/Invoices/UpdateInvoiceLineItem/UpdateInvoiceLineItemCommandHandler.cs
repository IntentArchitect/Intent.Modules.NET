using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.Invoices.UpdateInvoiceLineItem
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateInvoiceLineItemCommandHandler : IRequestHandler<UpdateInvoiceLineItemCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateInvoiceLineItemCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(UpdateInvoiceLineItemCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (UpdateInvoiceLineItemCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}