using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GrpcServer.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GrpcServer.Application.Interfaces.TypeTestingServices
{
    public interface IForPagedResultService
    {
        Task<PagedResult<ComplexTypeDto>> Operation(PagedResult<ComplexTypeDto> param, CancellationToken cancellationToken = default);
        Task<List<PagedResult<ComplexTypeDto>>> OperationCollection(List<PagedResult<ComplexTypeDto>> param, CancellationToken cancellationToken = default);
        Task<PagedResult<ComplexTypeDto>> OperationNullable(PagedResult<ComplexTypeDto> param, CancellationToken cancellationToken = default);
        Task<List<PagedResult<ComplexTypeDto>>> OperationNullableCollection(List<PagedResult<ComplexTypeDto>> param, CancellationToken cancellationToken = default);
    }
}