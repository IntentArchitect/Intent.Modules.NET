using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.Invoices.CreateInvoiceLineItem
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateInvoiceLineItemCommandHandler : IRequestHandler<CreateInvoiceLineItemCommand, string>
    {
        [IntentManaged(Mode.Merge)]
        public CreateInvoiceLineItemCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<string> Handle(CreateInvoiceLineItemCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CreateInvoiceLineItemCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}