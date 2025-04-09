using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.People.GetPersonByName
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetPersonByNameQueryHandler : IRequestHandler<GetPersonByNameQuery, GetPersonByNamePersonDCDto>
    {
        private readonly IPersonRepository _personRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetPersonByNameQueryHandler(IPersonRepository personRepository, IMapper mapper)
        {
            _personRepository = personRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<GetPersonByNamePersonDCDto> Handle(
            GetPersonByNameQuery request,
            CancellationToken cancellationToken)
        {
            var result = _personRepository.GetPersonByName(request.Name);
            return result.MapToGetPersonByNamePersonDCDto(_mapper);
        }
    }
}