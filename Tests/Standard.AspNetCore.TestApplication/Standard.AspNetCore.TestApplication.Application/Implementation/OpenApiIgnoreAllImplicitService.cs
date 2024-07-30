using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class OpenApiIgnoreAllImplicitService : IOpenApiIgnoreAllImplicitService
    {
        [IntentManaged(Mode.Merge)]
        public OpenApiIgnoreAllImplicitService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task OperationA(CancellationToken cancellationToken = default)
        {
            // TODO: Implement OperationA (IgnoreAllImplicitService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task OperationB(CancellationToken cancellationToken = default)
        {
            // TODO: Implement OperationB (IgnoreAllImplicitService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        public void Dispose()
        {
        }
    }
}