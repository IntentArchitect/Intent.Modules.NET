using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityDefaults.GetEntityDefaults
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEntityDefaultsQueryHandler : IRequestHandler<GetEntityDefaultsQuery, List<EntityDefaultDto>>
    {
        private readonly IEntityDefaultRepository _entityDefaultRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetEntityDefaultsQueryHandler(IEntityDefaultRepository entityDefaultRepository, IMapper mapper)
        {
            _entityDefaultRepository = entityDefaultRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<EntityDefaultDto>> Handle(
            GetEntityDefaultsQuery request,
            CancellationToken cancellationToken)
        {
            var entityDefaults = await _entityDefaultRepository.FindAllAsync(cancellationToken);
            return entityDefaults.MapToEntityDefaultDtoList(_mapper);
        }
    }
}