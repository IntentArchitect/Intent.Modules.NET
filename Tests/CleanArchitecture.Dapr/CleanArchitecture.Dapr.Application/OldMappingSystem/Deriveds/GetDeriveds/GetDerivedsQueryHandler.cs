using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Dapr.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Deriveds.GetDeriveds
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDerivedsQueryHandler : IRequestHandler<GetDerivedsQuery, List<DerivedDto>>
    {
        private readonly IDerivedRepository _derivedRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetDerivedsQueryHandler(IDerivedRepository derivedRepository, IMapper mapper)
        {
            _derivedRepository = derivedRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<DerivedDto>> Handle(GetDerivedsQuery request, CancellationToken cancellationToken)
        {
            var deriveds = await _derivedRepository.FindAllAsync(cancellationToken);
            return deriveds.MapToDerivedDtoList(_mapper);
        }
    }
}