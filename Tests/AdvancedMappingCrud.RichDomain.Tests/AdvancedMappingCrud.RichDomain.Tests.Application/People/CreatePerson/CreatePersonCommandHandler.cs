using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.Tests.Domain;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Entities;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.People.CreatePerson
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, Guid>
    {
        private readonly IPersonRepository _personRepository;

        [IntentManaged(Mode.Merge)]
        public CreatePersonCommandHandler(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            var person = new Person(
                details: new PersonDetails(
                    name: new Names(
                        first: request.Details.Name.First,
                        last: request.Details.Name.Last)));

            _personRepository.Add(person);
            await _personRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return person.Id;
        }
    }
}