using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRootLongs.GetAggregateRootLongs
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateRootLongsQueryHandler : IRequestHandler<GetAggregateRootLongsQuery, List<AggregateRootLongDto>>
    {
        private readonly IAggregateRootLongRepository _aggregateRootLongRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetAggregateRootLongsQueryHandler(IAggregateRootLongRepository aggregateRootLongRepository, IMapper mapper)
        {
            _aggregateRootLongRepository = aggregateRootLongRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<AggregateRootLongDto>> Handle(GetAggregateRootLongsQuery request, CancellationToken cancellationToken)
        {
            var aggregateRootLongs = await _aggregateRootLongRepository.FindAllAsync(cancellationToken);
            return aggregateRootLongs.MapToAggregateRootLongDtoList(_mapper);
        }
    }
}