using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using RichDomain.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace RichDomain.Application.People.GetPeople
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetPeopleQueryHandler : IRequestHandler<GetPeopleQuery, List<PersonDto>>
    {
        private readonly IPersonRepository _personRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetPeopleQueryHandler(IPersonRepository personRepository, IMapper mapper)
        {
            _personRepository = personRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<PersonDto>> Handle(GetPeopleQuery request, CancellationToken cancellationToken)
        {
            var people = await _personRepository.FindAllAsync(cancellationToken);
            return people.MapToPersonDtoList(_mapper);
        }
    }
}