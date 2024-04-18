using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.EnumRoute;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Interfaces.EnumRoute
{
    public interface IRouteEnumService : IDisposable
    {
        Task TestRouteEnum(Company testEnum, CancellationToken cancellationToken = default);
    }
}