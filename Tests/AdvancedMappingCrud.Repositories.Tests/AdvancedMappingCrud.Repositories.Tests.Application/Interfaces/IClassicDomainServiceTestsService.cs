using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.ClassicDomainServiceTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Interfaces
{
    public interface IClassicDomainServiceTestsService : IDisposable
    {
        Task<Guid> CreateClassicDomainServiceTest(ClassicDomainServiceTestCreateDto dto, CancellationToken cancellationToken = default);
        Task<ClassicDomainServiceTestDto> FindClassicDomainServiceTestById(Guid id, CancellationToken cancellationToken = default);
        Task<List<ClassicDomainServiceTestDto>> FindClassicDomainServiceTests(CancellationToken cancellationToken = default);
        Task UpdateClassicDomainServiceTest(Guid id, ClassicDomainServiceTestUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteClassicDomainServiceTest(Guid id, CancellationToken cancellationToken = default);
    }
}