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
    public class OpenApiOptOutSingleService : IOpenApiOptOutSingleService
    {
        [IntentManaged(Mode.Merge)]
        public OpenApiOptOutSingleService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task OperationA(CancellationToken cancellationToken = default)
        {
            // TODO: Implement OperationA (OpenApiOptOutSingleService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task OperationB(CancellationToken cancellationToken = default)
        {
            // TODO: Implement OperationB (OpenApiOptOutSingleService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        public void Dispose()
        {
        }
    }
}