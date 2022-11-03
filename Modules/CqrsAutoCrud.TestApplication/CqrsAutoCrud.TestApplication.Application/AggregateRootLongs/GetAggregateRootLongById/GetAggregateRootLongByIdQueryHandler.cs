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

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootLongs.GetAggregateRootLongById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateRootLongByIdQueryHandler : IRequestHandler<GetAggregateRootLongByIdQuery, AggregateRootLongDTO>
    {
        private IAggregateRootLongRepository _aggregateRootLongRepository;
        private IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetAggregateRootLongByIdQueryHandler(IAggregateRootLongRepository aggregateRootLongRepository, IMapper mapper)
        {
            _aggregateRootLongRepository = aggregateRootLongRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<AggregateRootLongDTO> Handle(GetAggregateRootLongByIdQuery request, CancellationToken cancellationToken)
        {
            var aggregateRootLong = await _aggregateRootLongRepository.FindByIdAsync(request.Id, cancellationToken);
            return aggregateRootLong.MapToAggregateRootLongDTO(_mapper);
        }
    }
}