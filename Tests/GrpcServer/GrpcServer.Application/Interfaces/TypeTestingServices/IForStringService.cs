using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GrpcServer.Application.Interfaces.TypeTestingServices
{
    public interface IForStringService
    {
        Task<string> Operation(string param, CancellationToken cancellationToken = default);
        Task<List<string>> OperationCollection(List<string> param, CancellationToken cancellationToken = default);
        Task<string?> OperationNullable(string? param, CancellationToken cancellationToken = default);
        Task<List<string>?> OperationNullableCollection(List<string>? param, CancellationToken cancellationToken = default);
    }
}