using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities.Interfaces.EF.Domain.Common.Exceptions;
using Entities.Interfaces.EF.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.Interfaces.EF.Application.People.UpdatePerson
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
            var existingPerson = await _personRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingPerson is null)
            {
                throw new NotFoundException($"Could not find Person '{request.Id}'");
            }

            existingPerson.Name = request.Name;
        }
    }
}