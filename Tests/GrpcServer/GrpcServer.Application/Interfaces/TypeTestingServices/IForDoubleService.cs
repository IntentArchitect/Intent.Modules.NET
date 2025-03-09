using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GrpcServer.Application.Interfaces.TypeTestingServices
{
    public interface IForDoubleService
    {
        Task<double> Operation(double param, CancellationToken cancellationToken = default);
        Task<List<double>> OperationCollection(List<double> param, CancellationToken cancellationToken = default);
        Task<double?> OperationNullable(double? param, CancellationToken cancellationToken = default);
        Task<List<double>?> OperationNullableCollection(List<double>? param, CancellationToken cancellationToken = default);
    }
}