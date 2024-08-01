using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace BugSnagTest.AspNetCore.Application.Interfaces
{
    public interface ITestService : IDisposable
    {
        Task TestError(RequestDto dto, CancellationToken cancellationToken = default);
    }
}