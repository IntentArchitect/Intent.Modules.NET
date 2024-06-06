using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateTestNoIdReturns.GetAggregateTestNoIdReturns
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateTestNoIdReturnsQueryHandler : IRequestHandler<GetAggregateTestNoIdReturnsQuery, List<AggregateTestNoIdReturnDto>>
    {
        private readonly IAggregateTestNoIdReturnRepository _aggregateTestNoIdReturnRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetAggregateTestNoIdReturnsQueryHandler(IAggregateTestNoIdReturnRepository aggregateTestNoIdReturnRepository,
            IMapper mapper)
        {
            _aggregateTestNoIdReturnRepository = aggregateTestNoIdReturnRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<AggregateTestNoIdReturnDto>> Handle(
            GetAggregateTestNoIdReturnsQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateTestNoIdReturns = await _aggregateTestNoIdReturnRepository.FindAllAsync(cancellationToken);
            return aggregateTestNoIdReturns.MapToAggregateTestNoIdReturnDtoList(_mapper);
        }
    }
}