using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GrpcServer.Application.Interfaces.TypeTestingServices
{
    public interface IForTimeSpanService
    {
        Task<TimeSpan> Operation(TimeSpan param, CancellationToken cancellationToken = default);
        Task<List<TimeSpan>> OperationCollection(List<TimeSpan> param, CancellationToken cancellationToken = default);
        Task<TimeSpan?> OperationNullable(TimeSpan? param, CancellationToken cancellationToken = default);
        Task<List<TimeSpan>?> OperationNullableCollection(List<TimeSpan>? param, CancellationToken cancellationToken = default);
    }
}