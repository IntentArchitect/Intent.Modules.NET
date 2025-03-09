using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GrpcServer.Application.Interfaces.TypeTestingServices
{
    public interface IForDateTimeOffsetService
    {
        Task<DateTimeOffset> Operation(DateTimeOffset param, CancellationToken cancellationToken = default);
        Task<List<DateTimeOffset>> OperationCollection(List<DateTimeOffset> param, CancellationToken cancellationToken = default);
        Task<DateTimeOffset?> OperationNullable(DateTimeOffset? param, CancellationToken cancellationToken = default);
        Task<List<DateTimeOffset>?> OperationNullableCollection(List<DateTimeOffset>? param, CancellationToken cancellationToken = default);
    }
}