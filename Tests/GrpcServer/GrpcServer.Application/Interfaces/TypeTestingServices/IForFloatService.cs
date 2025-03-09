using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GrpcServer.Application.Interfaces.TypeTestingServices
{
    public interface IForFloatService
    {
        Task<float> Operation(float param, CancellationToken cancellationToken = default);
        Task<List<float>> OperationCollection(List<float> param, CancellationToken cancellationToken = default);
        Task<float?> OperationNullable(float? param, CancellationToken cancellationToken = default);
        Task<List<float>?> OperationNullableCollection(List<float>? param, CancellationToken cancellationToken = default);
    }
}