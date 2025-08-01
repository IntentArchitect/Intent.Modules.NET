using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Dapr.Domain.Common.Exceptions;
using CleanArchitecture.Dapr.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.DerivedOfTS.GetDerivedOfTById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDerivedOfTByIdQueryHandler : IRequestHandler<GetDerivedOfTByIdQuery, DerivedOfTDto>
    {
        private readonly IDerivedOfTRepository _derivedOfTRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetDerivedOfTByIdQueryHandler(IDerivedOfTRepository derivedOfTRepository, IMapper mapper)
        {
            _derivedOfTRepository = derivedOfTRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<DerivedOfTDto> Handle(GetDerivedOfTByIdQuery request, CancellationToken cancellationToken)
        {
            var derivedOfT = await _derivedOfTRepository.FindByIdAsync(request.Id, cancellationToken);
            if (derivedOfT is null)
            {
                throw new NotFoundException($"Could not find DerivedOfT '{request.Id}'");
            }

            return derivedOfT.MapToDerivedOfTDto(_mapper);
        }
    }
}