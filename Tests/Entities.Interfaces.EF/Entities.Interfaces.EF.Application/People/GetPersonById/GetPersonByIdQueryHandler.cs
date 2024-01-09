using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Entities.Interfaces.EF.Domain.Common.Exceptions;
using Entities.Interfaces.EF.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Entities.Interfaces.EF.Application.People.GetPersonById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetPersonByIdQueryHandler : IRequestHandler<GetPersonByIdQuery, PersonDto>
    {
        private readonly IPersonRepository _personRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetPersonByIdQueryHandler(IPersonRepository personRepository, IMapper mapper)
        {
            _personRepository = personRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PersonDto> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
        {
            var person = await _personRepository.FindByIdAsync(request.Id, cancellationToken);
            if (person is null)
            {
                throw new NotFoundException($"Could not find Person '{request.Id}'");
            }

            return person.MapToPersonDto(_mapper);
        }
    }
}