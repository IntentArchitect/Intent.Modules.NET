using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.MappingTests;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.NestingParents.GetNestingParents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetNestingParentsQueryHandler : IRequestHandler<GetNestingParentsQuery, List<NestingParentDto>>
    {
        private readonly INestingParentRepository _nestingParentRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetNestingParentsQueryHandler(INestingParentRepository nestingParentRepository, IMapper mapper)
        {
            _nestingParentRepository = nestingParentRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<NestingParentDto>> Handle(
            GetNestingParentsQuery request,
            CancellationToken cancellationToken)
        {
            var nestingParents = await _nestingParentRepository.FindAllAsync(cancellationToken);
            return nestingParents.MapToNestingParentDtoList(_mapper);
        }
    }
}