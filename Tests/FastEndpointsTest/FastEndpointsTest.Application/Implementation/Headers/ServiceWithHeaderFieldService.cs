using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FastEndpointsTest.Application.Interfaces.Headers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace FastEndpointsTest.Application.Implementation.Headers
{
    [IntentManaged(Mode.Merge)]
    public class ServiceWithHeaderFieldService : IServiceWithHeaderFieldService
    {
        [IntentManaged(Mode.Merge)]
        public ServiceWithHeaderFieldService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task DoHeaderField(string param, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DoHeaderField (ServiceWithHeaderFieldService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}