using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FastEndpointsTest.Application.Interfaces.NamedQueryStrings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace FastEndpointsTest.Application.Implementation.NamedQueryStrings
{
    [IntentManaged(Mode.Merge)]
    public class ServiceWithNamedQueryStringService : IServiceWithNamedQueryStringService
    {
        [IntentManaged(Mode.Merge)]
        public ServiceWithNamedQueryStringService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task DoNamedQueryString(string customName, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DoNamedQueryString (ServiceWithNamedQueryStringService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        public void Dispose()
        {
        }
    }
}