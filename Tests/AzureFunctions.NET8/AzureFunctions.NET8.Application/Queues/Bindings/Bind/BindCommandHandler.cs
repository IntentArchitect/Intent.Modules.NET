using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.NET8.Application.Customers;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AzureFunctions.NET8.Application.Queues.Bindings.Bind
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class BindCommandHandler : IRequestHandler<BindCommand, CustomerDto>
    {
        [IntentManaged(Mode.Merge)]
        public BindCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<CustomerDto> Handle(BindCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (BindCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}