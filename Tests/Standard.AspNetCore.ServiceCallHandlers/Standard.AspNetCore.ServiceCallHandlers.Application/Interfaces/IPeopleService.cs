using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.ServiceCallHandlers.Application.Common.Pagination;
using Standard.AspNetCore.ServiceCallHandlers.Application.People;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.Interfaces
{
    public interface IPeopleService : IDisposable
    {
        Task<Guid> CreatePerson(PersonCreateDto dto, CancellationToken cancellationToken = default);
        Task<PersonDto> FindPersonById(Guid id, CancellationToken cancellationToken = default);
        Task<List<PersonDto>> FindPeople(CancellationToken cancellationToken = default);
        Task UpdatePerson(Guid id, PersonUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeletePerson(Guid id, CancellationToken cancellationToken = default);
    }
}