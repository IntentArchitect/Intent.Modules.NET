using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Standard.AspNetCore.ServiceCallHandlers.Application.Implementation.PeopleServiceHandlers;
using Standard.AspNetCore.ServiceCallHandlers.Application.Interfaces;
using Standard.AspNetCore.ServiceCallHandlers.Application.People;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class PeopleService : IPeopleService
    {
        private readonly IServiceProvider _serviceProvider;

        [IntentManaged(Mode.Merge)]
        public PeopleService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreatePerson(PersonCreateDto dto, CancellationToken cancellationToken = default)
        {
            var sch = (CreatePersonSCH)_serviceProvider.GetRequiredService(typeof(CreatePersonSCH));
            var result = await sch.Handle(dto, cancellationToken);
            return result;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PersonDto> FindPersonById(Guid id, CancellationToken cancellationToken = default)
        {
            var sch = (FindPersonByIdSCH)_serviceProvider.GetRequiredService(typeof(FindPersonByIdSCH));
            var result = await sch.Handle(id, cancellationToken);
            return result;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<PersonDto>> FindPeople(CancellationToken cancellationToken = default)
        {
            var sch = (FindPeopleSCH)_serviceProvider.GetRequiredService(typeof(FindPeopleSCH));
            var result = await sch.Handle(cancellationToken);
            return result;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdatePerson(Guid id, PersonUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var sch = (UpdatePersonSCH)_serviceProvider.GetRequiredService(typeof(UpdatePersonSCH));
            await sch.Handle(id, dto, cancellationToken);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeletePerson(Guid id, CancellationToken cancellationToken = default)
        {
            var sch = (DeletePersonSCH)_serviceProvider.GetRequiredService(typeof(DeletePersonSCH));
            await sch.Handle(id, cancellationToken);
        }

        public void Dispose()
        {
        }
    }
}