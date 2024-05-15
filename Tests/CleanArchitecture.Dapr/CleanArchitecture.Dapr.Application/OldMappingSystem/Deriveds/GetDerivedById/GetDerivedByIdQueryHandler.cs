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

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Deriveds.GetDerivedById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDerivedByIdQueryHandler : IRequestHandler<GetDerivedByIdQuery, DerivedDto>
    {
        private readonly IDerivedRepository _derivedRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetDerivedByIdQueryHandler(IDerivedRepository derivedRepository, IMapper mapper)
        {
            _derivedRepository = derivedRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<DerivedDto> Handle(GetDerivedByIdQuery request, CancellationToken cancellationToken)
        {
            var derived = await _derivedRepository.FindByIdAsync(request.Id, cancellationToken);
            if (derived is null)
            {
                throw new NotFoundException($"Could not find Derived '{request.Id}'");
            }

            return derived.MapToDerivedDto(_mapper);
        }
    }
}