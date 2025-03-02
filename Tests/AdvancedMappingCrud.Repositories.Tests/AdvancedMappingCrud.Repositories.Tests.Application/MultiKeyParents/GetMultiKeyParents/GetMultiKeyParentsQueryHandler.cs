using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.MultiKeyParents.GetMultiKeyParents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetMultiKeyParentsQueryHandler : IRequestHandler<GetMultiKeyParentsQuery, List<MultiKeyParentDto>>
    {
        private readonly IMultiKeyParentRepository _multiKeyParentRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetMultiKeyParentsQueryHandler(IMultiKeyParentRepository multiKeyParentRepository, IMapper mapper)
        {
            _multiKeyParentRepository = multiKeyParentRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<MultiKeyParentDto>> Handle(
            GetMultiKeyParentsQuery request,
            CancellationToken cancellationToken)
        {
            var multiKeyParents = await _multiKeyParentRepository.FindAllAsync(cancellationToken);
            return multiKeyParents.MapToMultiKeyParentDtoList(_mapper);
        }
    }
}