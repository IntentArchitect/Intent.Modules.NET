using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace FastEndpointsTest.Application.Interfaces.ScalarCollectionReturnType
{
    public interface IServiceWithScalarWithCollectionReturnService
    {
        Task<List<string>> DoScalarWithCollectionReturn(CancellationToken cancellationToken = default);
    }
}