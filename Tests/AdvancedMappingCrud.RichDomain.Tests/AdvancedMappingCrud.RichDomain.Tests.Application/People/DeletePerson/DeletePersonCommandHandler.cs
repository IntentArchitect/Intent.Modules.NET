using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.People.DeletePerson
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand>
    {
        private readonly IPersonRepository _personRepository;

        [IntentManaged(Mode.Merge)]
        public DeletePersonCommandHandler(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeletePersonCommand request, CancellationToken cancellationToken)
        {
            var person = await _personRepository.FindByIdAsync(request.Id, cancellationToken);
            if (person is null)
            {
                throw new NotFoundException($"Could not find Person '{request.Id}'");
            }

            _personRepository.Remove(person);
        }
    }
}