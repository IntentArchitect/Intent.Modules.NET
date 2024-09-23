using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Supers.GetSupers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetSupersQueryHandler : IRequestHandler<GetSupersQuery, List<SuperDto>>
    {
        private readonly ISuperRepository _superRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetSupersQueryHandler(ISuperRepository superRepository, IMapper mapper)
        {
            _superRepository = superRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<SuperDto>> Handle(GetSupersQuery request, CancellationToken cancellationToken)
        {
            var supers = await _superRepository.FindAllAsync(cancellationToken);
            return supers.MapToSuperDtoList(_mapper);
        }
    }
}