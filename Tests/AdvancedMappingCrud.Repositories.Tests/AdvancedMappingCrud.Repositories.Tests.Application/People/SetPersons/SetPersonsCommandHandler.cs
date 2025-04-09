using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Contracts;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.People.SetPersons
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SetPersonsCommandHandler : IRequestHandler<SetPersonsCommand, List<Guid>>
    {
        private readonly IPersonRepository _personRepository;

        [IntentManaged(Mode.Merge)]
        public SetPersonsCommandHandler(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<Guid>> Handle(SetPersonsCommand request, CancellationToken cancellationToken)
        {
            var result = _personRepository.SetPersons(request.People
                .Select(p => new PersonDC(
                    firstName: p.FirstName,
                    surname: p.Surname))
                .ToList());
            return result;
        }
    }
}