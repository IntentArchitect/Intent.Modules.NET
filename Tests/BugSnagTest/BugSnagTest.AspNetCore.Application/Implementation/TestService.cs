using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BugSnagTest.AspNetCore.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace BugSnagTest.AspNetCore.Application.Implementation
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
            throw new Exception("Breaking the application on purpose for testing purposes");
        }

        public void Dispose()
        {
        }
    }
}