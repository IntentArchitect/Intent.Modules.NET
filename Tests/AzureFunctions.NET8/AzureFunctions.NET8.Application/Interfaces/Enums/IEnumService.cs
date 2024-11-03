using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.NET8.Application.Enums;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AzureFunctions.NET8.Application.Interfaces.Enums
{
    public interface IEnumService
    {
        Task TestRouteEnum(Company testEnum, CancellationToken cancellationToken = default);
        Task TestQueryEnum(Company testEnum, CancellationToken cancellationToken = default);
        Task TestHeaderEnum(Company testEnum, CancellationToken cancellationToken = default);
    }
}