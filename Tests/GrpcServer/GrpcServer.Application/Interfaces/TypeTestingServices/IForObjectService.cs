using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GrpcServer.Application.Interfaces.TypeTestingServices
{
    public interface IForObjectService
    {
        Task<object> Operation(object param, CancellationToken cancellationToken = default);
        Task<List<object>> OperationCollection(List<object> param, CancellationToken cancellationToken = default);
        Task<object?> OperationNullable(object? param, CancellationToken cancellationToken = default);
        Task<List<object>?> OperationNullableCollection(List<object>? param, CancellationToken cancellationToken = default);
    }
}