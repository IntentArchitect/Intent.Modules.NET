using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.UniqueIndexConstraint;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.GetAggregateWithUniqueConstraintIndexStereotypeById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateWithUniqueConstraintIndexStereotypeByIdQueryHandler : IRequestHandler<GetAggregateWithUniqueConstraintIndexStereotypeByIdQuery, AggregateWithUniqueConstraintIndexStereotypeDto>
    {
        private readonly IAggregateWithUniqueConstraintIndexStereotypeRepository _aggregateWithUniqueConstraintIndexStereotypeRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetAggregateWithUniqueConstraintIndexStereotypeByIdQueryHandler(IAggregateWithUniqueConstraintIndexStereotypeRepository aggregateWithUniqueConstraintIndexStereotypeRepository,
            IMapper mapper)
        {
            _aggregateWithUniqueConstraintIndexStereotypeRepository = aggregateWithUniqueConstraintIndexStereotypeRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<AggregateWithUniqueConstraintIndexStereotypeDto> Handle(
            GetAggregateWithUniqueConstraintIndexStereotypeByIdQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateWithUniqueConstraintIndexStereotype = await _aggregateWithUniqueConstraintIndexStereotypeRepository.FindByIdAsync(request.Id, cancellationToken);
            if (aggregateWithUniqueConstraintIndexStereotype is null)
            {
                throw new NotFoundException($"Could not find AggregateWithUniqueConstraintIndexStereotype '{request.Id}'");
            }

            return aggregateWithUniqueConstraintIndexStereotype.MapToAggregateWithUniqueConstraintIndexStereotypeDto(_mapper);
        }
    }
}