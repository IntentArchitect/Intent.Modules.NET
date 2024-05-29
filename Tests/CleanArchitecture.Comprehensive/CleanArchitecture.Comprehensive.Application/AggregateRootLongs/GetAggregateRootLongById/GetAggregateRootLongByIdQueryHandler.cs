using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRootLongs.GetAggregateRootLongById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateRootLongByIdQueryHandler : IRequestHandler<GetAggregateRootLongByIdQuery, AggregateRootLongDto>
    {
        private readonly IAggregateRootLongRepository _aggregateRootLongRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetAggregateRootLongByIdQueryHandler(IAggregateRootLongRepository aggregateRootLongRepository,
            IMapper mapper)
        {
            _aggregateRootLongRepository = aggregateRootLongRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<AggregateRootLongDto> Handle(
            GetAggregateRootLongByIdQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateRootLong = await _aggregateRootLongRepository.FindByIdAsync(request.Id, cancellationToken);
            if (aggregateRootLong is null)
            {
                throw new NotFoundException($"Could not find AggregateRootLong '{request.Id}'");
            }

            return aggregateRootLong.MapToAggregateRootLongDto(_mapper);
        }
    }
}