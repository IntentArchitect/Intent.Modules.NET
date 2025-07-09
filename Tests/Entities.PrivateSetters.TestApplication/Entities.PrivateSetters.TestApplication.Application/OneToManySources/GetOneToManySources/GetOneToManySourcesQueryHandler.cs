using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Compositional;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources.GetOneToManySources
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOneToManySourcesQueryHandler : IRequestHandler<GetOneToManySourcesQuery, List<OneToManySourceDto>>
    {
        private readonly IOneToManySourceRepository _oneToManySourceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetOneToManySourcesQueryHandler(IOneToManySourceRepository oneToManySourceRepository, IMapper mapper)
        {
            _oneToManySourceRepository = oneToManySourceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<OneToManySourceDto>> Handle(
            GetOneToManySourcesQuery request,
            CancellationToken cancellationToken)
        {
            var oneToManySources = await _oneToManySourceRepository.FindAllAsync(cancellationToken);
            return oneToManySources.MapToOneToManySourceDtoList(_mapper);
        }
    }
}