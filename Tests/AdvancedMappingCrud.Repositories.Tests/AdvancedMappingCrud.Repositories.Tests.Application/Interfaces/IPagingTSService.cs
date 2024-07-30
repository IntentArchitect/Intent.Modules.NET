using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Pagination;
using AdvancedMappingCrud.Repositories.Tests.Application.PagingTS;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Interfaces
{
    public interface IPagingTSService : IDisposable
    {
        Task<Guid> CreatePagingTS(PagingTSCreateDto dto, CancellationToken cancellationToken = default);
        Task<PagingTSDto> FindPagingTSById(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<PagingTSDto>> FindPagingTS(int pageNo, int pageSize, string? orderBy, CancellationToken cancellationToken = default);
        Task UpdatePagingTS(Guid id, PagingTSUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeletePagingTS(Guid id, CancellationToken cancellationToken = default);
    }
}