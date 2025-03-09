using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GrpcServer.Application.Interfaces.TypeTestingServices
{
    public interface IForShortService
    {
        Task<short> Operation(short param, CancellationToken cancellationToken = default);
        Task<List<short>> OperationCollection(List<short> param, CancellationToken cancellationToken = default);
        Task<short?> OperationNullable(short? param, CancellationToken cancellationToken = default);
        Task<List<short>?> OperationNullableCollection(List<short>? param, CancellationToken cancellationToken = default);
    }
}