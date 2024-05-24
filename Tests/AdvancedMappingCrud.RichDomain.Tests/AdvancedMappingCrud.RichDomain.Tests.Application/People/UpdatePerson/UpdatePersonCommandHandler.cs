using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.Tests.Domain;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.People.UpdatePerson
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonCommand>
    {
        private readonly IPersonRepository _personRepository;

        [IntentManaged(Mode.Merge)]
        public UpdatePersonCommandHandler(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
        {
            var person = await _personRepository.FindByIdAsync(request.Id, cancellationToken);
            if (person is null)
            {
                throw new NotFoundException($"Could not find Person '{request.Id}'");
            }

            person.Update(new PersonDetails(
                name: new Names(
                    first: request.Details.Name.First,
                    last: request.Details.Name.Last)));
        }
    }
}