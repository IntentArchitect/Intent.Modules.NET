using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GrpcServer.Application.Interfaces.TypeTestingServices
{
    public interface IForDictionaryService
    {
        Task<Dictionary<string, string>> Operation(Dictionary<string, string> param, CancellationToken cancellationToken = default);
        Task<List<Dictionary<string, string>>> OperationCollection(List<Dictionary<string, string>> param, CancellationToken cancellationToken = default);
        Task<Dictionary<string, string>?> OperationNullable(Dictionary<string, string>? param, CancellationToken cancellationToken = default);
        Task<List<Dictionary<string, string>>?> OperationNullableCollection(List<Dictionary<string, string>>? param, CancellationToken cancellationToken = default);
    }
}