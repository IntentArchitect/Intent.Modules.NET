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
    public class QueryStringNamesService : IQueryStringNamesService
    {
        [IntentManaged(Mode.Merge)]
        public QueryStringNamesService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Operation(string par1, CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation (QueryStringNamesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}