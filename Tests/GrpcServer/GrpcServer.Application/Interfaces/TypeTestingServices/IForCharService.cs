using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GrpcServer.Application.Interfaces.TypeTestingServices
{
    public interface IForCharService
    {
        Task<char> Operation(char param, CancellationToken cancellationToken = default);
        Task<List<char>> OperationCollection(List<char> param, CancellationToken cancellationToken = default);
        Task<char?> OperationNullable(char? param, CancellationToken cancellationToken = default);
        Task<List<char>?> OperationNullableCollection(List<char>? param, CancellationToken cancellationToken = default);
    }
}