using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.NullableRouteParameters;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Interfaces.NullableRouteParameters
{
    public interface INullableRouteParameterService
    {
        Task RoutedOperation(string? nullableString, int? nullableInt, NullableRouteParameterEnum? nullableEnum, CancellationToken cancellationToken = default);
    }
}