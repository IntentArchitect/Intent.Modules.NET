using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.EmbeddedParents.GetEmbeddedParents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEmbeddedParentsQueryHandler : IRequestHandler<GetEmbeddedParentsQuery, List<EmbeddedParentDto>>
    {
        private readonly IEmbeddedParentRepository _embeddedParentRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetEmbeddedParentsQueryHandler(IEmbeddedParentRepository embeddedParentRepository, IMapper mapper)
        {
            _embeddedParentRepository = embeddedParentRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<EmbeddedParentDto>> Handle(
            GetEmbeddedParentsQuery request,
            CancellationToken cancellationToken)
        {
            var embeddedParents = await _embeddedParentRepository.FindAllAsync(cancellationToken);
            return embeddedParents.MapToEmbeddedParentDtoList(_mapper);
        }
    }
}