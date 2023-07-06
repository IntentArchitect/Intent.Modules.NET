using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.Plurals;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Interfaces
{
    public interface IPluralsService : IDisposable
    {
        Task<Guid> CreatePlurals(PluralsCreateDto dto, CancellationToken cancellationToken = default);
        Task<PluralsDto> FindPluralsById(Guid id, CancellationToken cancellationToken = default);
        Task<List<PluralsDto>> FindPlurals(CancellationToken cancellationToken = default);
        Task UpdatePlurals(Guid id, PluralsUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeletePlurals(Guid id, CancellationToken cancellationToken = default);
    }
}