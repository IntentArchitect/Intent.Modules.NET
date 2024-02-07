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

namespace IntegrationTesting.Tests.Application.Parents.GetParents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetParentsQueryHandler : IRequestHandler<GetParentsQuery, List<ParentDto>>
    {
        private readonly IParentRepository _parentRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetParentsQueryHandler(IParentRepository parentRepository, IMapper mapper)
        {
            _parentRepository = parentRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ParentDto>> Handle(GetParentsQuery request, CancellationToken cancellationToken)
        {
            var parents = await _parentRepository.FindAllAsync(cancellationToken);
            return parents.MapToParentDtoList(_mapper);
        }
    }
}