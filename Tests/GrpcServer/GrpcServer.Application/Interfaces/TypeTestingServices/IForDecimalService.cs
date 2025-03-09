using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GrpcServer.Application.Interfaces.TypeTestingServices
{
    public interface IForDecimalService
    {
        Task<decimal> Operation(decimal param, CancellationToken cancellationToken = default);
        Task<List<decimal>> OperationCollection(List<decimal> param, CancellationToken cancellationToken = default);
        Task<decimal?> OperationNullable(decimal? param, CancellationToken cancellationToken = default);
        Task<List<decimal>?> OperationNullableCollection(List<decimal>? param, CancellationToken cancellationToken = default);
    }
}