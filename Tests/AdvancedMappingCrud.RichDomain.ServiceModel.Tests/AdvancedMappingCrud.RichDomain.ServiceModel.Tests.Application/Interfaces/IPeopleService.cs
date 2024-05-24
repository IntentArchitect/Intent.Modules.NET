using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.People;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Interfaces
{
    public interface IPeopleService : IDisposable
    {
        Task<Guid> CreatePerson(PersonCreateDto dto, CancellationToken cancellationToken = default);
        Task<PersonDto> FindPersonById(Guid id, CancellationToken cancellationToken = default);
        Task<List<PersonDto>> FindPeople(CancellationToken cancellationToken = default);
        Task DeletePerson(Guid id, CancellationToken cancellationToken = default);
        Task Update(Guid id, UpdateDto dto, CancellationToken cancellationToken = default);
    }
}