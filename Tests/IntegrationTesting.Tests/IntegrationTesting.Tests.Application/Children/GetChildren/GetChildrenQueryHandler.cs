using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Children.GetChildren
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetChildrenQueryHandler : IRequestHandler<GetChildrenQuery, List<ChildDto>>
    {
        private readonly IChildRepository _childRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetChildrenQueryHandler(IChildRepository childRepository, IMapper mapper)
        {
            _childRepository = childRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ChildDto>> Handle(GetChildrenQuery request, CancellationToken cancellationToken)
        {
            var children = await _childRepository.FindAllAsync(cancellationToken);
            return children.MapToChildDtoList(_mapper);
        }
    }
}