using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.EntityListEnums.GetEntityListEnumById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEntityListEnumByIdQueryHandler : IRequestHandler<GetEntityListEnumByIdQuery, EntityListEnumDto>
    {
        private readonly IEntityListEnumRepository _entityListEnumRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetEntityListEnumByIdQueryHandler(IEntityListEnumRepository entityListEnumRepository, IMapper mapper)
        {
            _entityListEnumRepository = entityListEnumRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<EntityListEnumDto> Handle(
            GetEntityListEnumByIdQuery request,
            CancellationToken cancellationToken)
        {
            var entityListEnum = await _entityListEnumRepository.FindByIdAsync(request.Id, cancellationToken);
            if (entityListEnum is null)
            {
                throw new NotFoundException($"Could not find EntityListEnum '{request.Id}'");
            }
            return entityListEnum.MapToEntityListEnumDto(_mapper);
        }
    }
}