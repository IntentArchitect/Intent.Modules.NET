using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Integration.HttpClients.TestApplication.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Integration.HttpClients.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class VersionOneService : IVersionOneService
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public VersionOneService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task OperationForVersionOne(string param, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        public void Dispose()
        {
        }
    }
}