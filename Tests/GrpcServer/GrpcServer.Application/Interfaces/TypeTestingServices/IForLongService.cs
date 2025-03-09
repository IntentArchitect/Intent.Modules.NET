using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GrpcServer.Application.Interfaces.TypeTestingServices
{
    public interface IForLongService
    {
        Task<long> Operation(long param, CancellationToken cancellationToken = default);
        Task<List<long>> OperationCollection(List<long> param, CancellationToken cancellationToken = default);
        Task<long?> OperationNullable(long? param, CancellationToken cancellationToken = default);
        Task<List<long>?> OperationNullableCollection(List<long>? param, CancellationToken cancellationToken = default);
    }
}