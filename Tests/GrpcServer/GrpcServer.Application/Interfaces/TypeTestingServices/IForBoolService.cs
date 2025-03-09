using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GrpcServer.Application.Interfaces.TypeTestingServices
{
    public interface IForBoolService
    {
        Task<bool> Operation(bool param, CancellationToken cancellationToken = default);
        Task<List<bool>> OperationCollection(List<bool> param, CancellationToken cancellationToken = default);
        Task<bool?> OperationNullable(bool? param, CancellationToken cancellationToken = default);
        Task<List<bool>?> OperationNullableCollection(List<bool>? param, CancellationToken cancellationToken = default);
    }
}