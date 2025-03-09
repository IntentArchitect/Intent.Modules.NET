using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GrpcServer.Application.Interfaces.TypeTestingServices
{
    public interface IForGuidService
    {
        Task<Guid> Operation(Guid param, CancellationToken cancellationToken = default);
        Task<List<Guid>> OperationCollection(List<Guid> param, CancellationToken cancellationToken = default);
        Task<Guid?> OperationNullable(Guid? param, CancellationToken cancellationToken = default);
        Task<List<Guid>?> OperationNullableCollection(List<Guid>? param, CancellationToken cancellationToken = default);
    }
}