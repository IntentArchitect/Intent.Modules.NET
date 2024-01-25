using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.ServiceCallHandlers.Application.People;
using Standard.AspNetCore.ServiceCallHandlers.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceCallHandlers.ServiceCallHandlerImplementation", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.Implementation.PeopleServiceHandlers
{
    [IntentManaged(Mode.Merge)]
    public class FindPeopleSCH
    {
        private readonly IPersonRepository _personRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public FindPeopleSCH(IPersonRepository personRepository, IMapper mapper)
        {
            _personRepository = personRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<PersonDto>> Handle(CancellationToken cancellationToken = default)
        {
            var elements = await _personRepository.FindAllAsync(cancellationToken);
            return elements.MapToPersonDtoList(_mapper);
        }
    }
}