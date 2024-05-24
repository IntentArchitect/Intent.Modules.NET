using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Interfaces;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.People;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class PeopleService : IPeopleService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public PeopleService(IPersonRepository personRepository, IMapper mapper)
        {
            _personRepository = personRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreatePerson(PersonCreateDto dto, CancellationToken cancellationToken = default)
        {
            var person = new Person(
                details: new PersonDetails(
                    name: new Names(
                        first: dto.Details.Name.First,
                        last: dto.Details.Name.Last)));

            _personRepository.Add(person);
            await _personRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return person.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PersonDto> FindPersonById(Guid id, CancellationToken cancellationToken = default)
        {
            var person = await _personRepository.FindByIdAsync(id, cancellationToken);
            if (person is null)
            {
                throw new NotFoundException($"Could not find Person '{id}'");
            }
            return person.MapToPersonDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<PersonDto>> FindPeople(CancellationToken cancellationToken = default)
        {
            var people = await _personRepository.FindAllAsync(cancellationToken);
            return people.MapToPersonDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeletePerson(Guid id, CancellationToken cancellationToken = default)
        {
            var person = await _personRepository.FindByIdAsync(id, cancellationToken);
            if (person is null)
            {
                throw new NotFoundException($"Could not find Person '{id}'");
            }

            _personRepository.Remove(person);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Update(Guid id, UpdateDto dto, CancellationToken cancellationToken = default)
        {
            var person = await _personRepository.FindByIdAsync(id, cancellationToken);
            if (person is null)
            {
                throw new NotFoundException($"Could not find Person '{id}'");
            }

            person.Update(new PersonDetails(
                name: new Names(
                    first: dto.Details.Name.First,
                    last: dto.Details.Name.Last)));
        }

        public void Dispose()
        {
        }
    }
}