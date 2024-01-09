using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities.Interfaces.EF.Domain.Entities;
using Entities.Interfaces.EF.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.Interfaces.EF.Application.People.CreatePerson
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
            var newPerson = new Person
            {
                Name = request.Name,
            };

            _personRepository.Add(newPerson);
            await _personRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newPerson.Id;
        }
    }
}