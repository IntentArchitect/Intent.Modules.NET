using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateTestNoIdReturns.GetAggregateTestNoIdReturnById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateTestNoIdReturnByIdQueryHandler : IRequestHandler<GetAggregateTestNoIdReturnByIdQuery, AggregateTestNoIdReturnDto>
    {
        private readonly IAggregateTestNoIdReturnRepository _aggregateTestNoIdReturnRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetAggregateTestNoIdReturnByIdQueryHandler(IAggregateTestNoIdReturnRepository aggregateTestNoIdReturnRepository, IMapper mapper)
        {
            _aggregateTestNoIdReturnRepository = aggregateTestNoIdReturnRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<AggregateTestNoIdReturnDto> Handle(GetAggregateTestNoIdReturnByIdQuery request, CancellationToken cancellationToken)
        {
            var aggregateTestNoIdReturn = await _aggregateTestNoIdReturnRepository.FindByIdAsync(request.Id, cancellationToken);
            return aggregateTestNoIdReturn.MapToAggregateTestNoIdReturnDto(_mapper);
        }
    }
}