using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GrpcServer.Application.Interfaces.TypeTestingServices
{
    public interface IForByteService
    {
        Task<byte> Operation(byte param, CancellationToken cancellationToken = default);
        Task<List<byte>> OperationCollection(List<byte> param, CancellationToken cancellationToken = default);
        Task<byte?> OperationNullable(byte? param, CancellationToken cancellationToken = default);
        Task<List<byte>?> OperationNullableCollection(List<byte>? param, CancellationToken cancellationToken = default);
    }
}