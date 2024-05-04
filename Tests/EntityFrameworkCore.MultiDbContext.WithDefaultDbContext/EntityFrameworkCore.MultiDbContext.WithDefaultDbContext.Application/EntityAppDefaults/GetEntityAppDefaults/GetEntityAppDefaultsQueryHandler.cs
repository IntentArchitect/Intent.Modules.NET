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

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAppDefaults.GetEntityAppDefaults
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEntityAppDefaultsQueryHandler : IRequestHandler<GetEntityAppDefaultsQuery, List<EntityAppDefaultDto>>
    {
        private readonly IEntityAppDefaultRepository _entityAppDefaultRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetEntityAppDefaultsQueryHandler(IEntityAppDefaultRepository entityAppDefaultRepository, IMapper mapper)
        {
            _entityAppDefaultRepository = entityAppDefaultRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<EntityAppDefaultDto>> Handle(
            GetEntityAppDefaultsQuery request,
            CancellationToken cancellationToken)
        {
            var entityAppDefaults = await _entityAppDefaultRepository.FindAllAsync(cancellationToken);
            return entityAppDefaults.MapToEntityAppDefaultDtoList(_mapper);
        }
    }
}