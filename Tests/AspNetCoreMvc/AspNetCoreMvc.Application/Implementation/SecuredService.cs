using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreMvc.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AspNetCoreMvc.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class SecuredService : ISecuredService
    {
        [IntentManaged(Mode.Merge)]
        public SecuredService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Operation(CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation (SecuredService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}