using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GrpcServer.Application.Interfaces.TypeTestingServices
{
    public interface IForDateOnlyService
    {
        Task<DateOnly> Operation(DateOnly param, CancellationToken cancellationToken = default);
        Task<List<DateOnly>> OperationCollection(List<DateOnly> param, CancellationToken cancellationToken = default);
        Task<DateOnly?> OperationNullable(DateOnly? param, CancellationToken cancellationToken = default);
        Task<List<DateOnly>?> OperationNullableCollection(List<DateOnly>? param, CancellationToken cancellationToken = default);
    }
}