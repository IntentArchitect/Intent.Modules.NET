using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GrpcServer.Application.Interfaces.TypeTestingServices
{
    public interface IForDateTimeService
    {
        Task<DateTime> Operation(DateTime param, CancellationToken cancellationToken = default);
        Task<List<DateTime>> OperationCollection(List<DateTime> param, CancellationToken cancellationToken = default);
        Task<DateTime?> OperationNullable(DateTime? param, CancellationToken cancellationToken = default);
        Task<List<DateTime>?> OperationNullableCollection(List<DateTime>? param, CancellationToken cancellationToken = default);
    }
}