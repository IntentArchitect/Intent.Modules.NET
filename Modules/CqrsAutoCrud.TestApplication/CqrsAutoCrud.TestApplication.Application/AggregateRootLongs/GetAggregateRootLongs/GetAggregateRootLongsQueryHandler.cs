using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootLongs.GetAggregateRootLongs
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateRootLongsQueryHandler : IRequestHandler<GetAggregateRootLongsQuery, List<AggregateRootLongDTO>>
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
        public async Task<List<AggregateRootLongDTO>> Handle(GetAggregateRootLongsQuery request, CancellationToken cancellationToken)
        {
            var aggregateRootLongs = await _aggregateRootLongRepository.FindAllAsync(cancellationToken);
            return aggregateRootLongs.MapToAggregateRootLongDTOList(_mapper);
        }
    }
}