using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GrpcServer.Application.TypeTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GrpcServer.Application.Interfaces.TypeTestingServices
{
    public interface IForEnumService
    {
        Task<EnumType> Operation(EnumType param, CancellationToken cancellationToken = default);
        Task<List<EnumType>> OperationCollection(List<EnumType> param, CancellationToken cancellationToken = default);
        Task<EnumType?> OperationNullable(EnumType? param, CancellationToken cancellationToken = default);
        Task<List<EnumType>?> OperationNullableCollection(List<EnumType>? param, CancellationToken cancellationToken = default);
    }
}