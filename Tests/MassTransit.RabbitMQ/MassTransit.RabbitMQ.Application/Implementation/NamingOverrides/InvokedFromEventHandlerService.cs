using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Application.Interfaces.NamingOverrides;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MassTransit.RabbitMQ.Application.Implementation.NamingOverrides
{
    [IntentManaged(Mode.Merge)]
    public class InvokedFromEventHandlerService : IInvokedFromEventHandlerService
    {
        [IntentManaged(Mode.Merge)]
        public InvokedFromEventHandlerService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Operation(string message, CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation (InvokedFromEventHandlerService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}