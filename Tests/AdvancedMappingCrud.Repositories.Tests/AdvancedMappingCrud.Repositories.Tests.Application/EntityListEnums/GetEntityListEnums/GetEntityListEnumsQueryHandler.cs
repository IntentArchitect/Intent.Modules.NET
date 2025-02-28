using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.EntityListEnums.GetEntityListEnums
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEntityListEnumsQueryHandler : IRequestHandler<GetEntityListEnumsQuery, List<EntityListEnumDto>>
    {
        private readonly IEntityListEnumRepository _entityListEnumRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetEntityListEnumsQueryHandler(IEntityListEnumRepository entityListEnumRepository, IMapper mapper)
        {
            _entityListEnumRepository = entityListEnumRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<EntityListEnumDto>> Handle(
            GetEntityListEnumsQuery request,
            CancellationToken cancellationToken)
        {
            var entityListEnums = await _entityListEnumRepository.FindAllAsync(cancellationToken);
            return entityListEnums.MapToEntityListEnumDtoList(_mapper);
        }
    }
}