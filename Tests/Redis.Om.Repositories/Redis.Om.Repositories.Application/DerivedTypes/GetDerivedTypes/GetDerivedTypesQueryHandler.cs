using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Redis.Om.Repositories.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Redis.Om.Repositories.Application.DerivedTypes.GetDerivedTypes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDerivedTypesQueryHandler : IRequestHandler<GetDerivedTypesQuery, List<DerivedTypeDto>>
    {
        private readonly IDerivedTypeRepository _derivedTypeRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetDerivedTypesQueryHandler(IDerivedTypeRepository derivedTypeRepository, IMapper mapper)
        {
            _derivedTypeRepository = derivedTypeRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<DerivedTypeDto>> Handle(GetDerivedTypesQuery request, CancellationToken cancellationToken)
        {
            var derivedTypes = await _derivedTypeRepository.FindAllAsync(cancellationToken);
            return derivedTypes.MapToDerivedTypeDtoList(_mapper);
        }
    }
}