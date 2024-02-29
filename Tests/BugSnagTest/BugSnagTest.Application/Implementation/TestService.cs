using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BugSnagTest.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace BugSnagTest.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class TestService : ITestService
    {
        [IntentManaged(Mode.Merge)]
        public TestService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task TestError(RequestDto dto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        public void Dispose()
        {
        }
    }
}