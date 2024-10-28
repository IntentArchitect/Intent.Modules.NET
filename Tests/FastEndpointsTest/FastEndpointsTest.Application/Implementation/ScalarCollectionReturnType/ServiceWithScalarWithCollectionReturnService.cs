using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FastEndpointsTest.Application.Interfaces.ScalarCollectionReturnType;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace FastEndpointsTest.Application.Implementation.ScalarCollectionReturnType
{
    [IntentManaged(Mode.Merge)]
    public class ServiceWithScalarWithCollectionReturnService : IServiceWithScalarWithCollectionReturnService
    {
        [IntentManaged(Mode.Merge)]
        public ServiceWithScalarWithCollectionReturnService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<string>> DoScalarWithCollectionReturn(CancellationToken cancellationToken = default)
        {
            // TODO: Implement DoScalarWithCollectionReturn (ServiceWithScalarWithCollectionReturnService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        public void Dispose()
        {
        }
    }
}