using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GrpcServer.Application.Interfaces.TypeTestingServices
{
    public interface IForIntService
    {
        Task<int> Operation(int param, CancellationToken cancellationToken = default);
        Task<List<int>> OperationCollection(List<int> param, CancellationToken cancellationToken = default);
        Task<int?> OperationNullable(int? param, CancellationToken cancellationToken = default);
        Task<List<int>?> OperationNullableCollection(List<int>? param, CancellationToken cancellationToken = default);
    }
}