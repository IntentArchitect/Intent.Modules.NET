using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using RichDomain.Domain.Common.Exceptions;
using RichDomain.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace RichDomain.Application.People.UpdatePersonPerson
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdatePersonPersonCommandHandler : IRequestHandler<UpdatePersonPersonCommand>
    {
        private readonly IPersonRepository _personRepository;

        [IntentManaged(Mode.Merge)]
        public UpdatePersonPersonCommandHandler(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdatePersonPersonCommand request, CancellationToken cancellationToken)
        {
            var existingPerson = await _personRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingPerson is null)
            {
                throw new NotFoundException($"Could not find Person '{request.Id}'");
            }

            existingPerson.UpdatePerson(request.FirstName);
        }
    }
}