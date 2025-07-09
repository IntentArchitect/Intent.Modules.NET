using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.Invoices.UpdateInvoice
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateInvoiceCommandHandler : IRequestHandler<UpdateInvoiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateInvoiceCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (UpdateInvoiceCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}