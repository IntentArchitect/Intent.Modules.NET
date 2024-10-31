using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace FastEndpointsTest.Application.Interfaces.NamedQueryStrings
{
    public interface IServiceWithNamedQueryStringService
    {
        Task DoNamedQueryString(string customName, CancellationToken cancellationToken = default);
    }
}