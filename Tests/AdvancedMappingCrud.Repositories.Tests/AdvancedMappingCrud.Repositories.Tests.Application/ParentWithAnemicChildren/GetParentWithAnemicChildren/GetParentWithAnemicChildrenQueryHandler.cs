using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.AnemicChild;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ParentWithAnemicChildren.GetParentWithAnemicChildren
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetParentWithAnemicChildrenQueryHandler : IRequestHandler<GetParentWithAnemicChildrenQuery, List<ParentWithAnemicChildDto>>
    {
        private readonly IParentWithAnemicChildRepository _parentWithAnemicChildRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetParentWithAnemicChildrenQueryHandler(IParentWithAnemicChildRepository parentWithAnemicChildRepository,
            IMapper mapper)
        {
            _parentWithAnemicChildRepository = parentWithAnemicChildRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ParentWithAnemicChildDto>> Handle(
            GetParentWithAnemicChildrenQuery request,
            CancellationToken cancellationToken)
        {
            var parentWithAnemicChildren = await _parentWithAnemicChildRepository.FindAllAsync(cancellationToken);
            return parentWithAnemicChildren.MapToParentWithAnemicChildDtoList(_mapper);
        }
    }
}