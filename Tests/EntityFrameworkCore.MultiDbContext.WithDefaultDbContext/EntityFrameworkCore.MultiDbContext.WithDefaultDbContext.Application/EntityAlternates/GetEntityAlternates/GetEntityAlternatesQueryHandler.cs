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

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAlternates.GetEntityAlternates
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEntityAlternatesQueryHandler : IRequestHandler<GetEntityAlternatesQuery, List<EntityAlternateDto>>
    {
        private readonly IEntityAlternateRepository _entityAlternateRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetEntityAlternatesQueryHandler(IEntityAlternateRepository entityAlternateRepository, IMapper mapper)
        {
            _entityAlternateRepository = entityAlternateRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<EntityAlternateDto>> Handle(
            GetEntityAlternatesQuery request,
            CancellationToken cancellationToken)
        {
            var entityAlternates = await _entityAlternateRepository.FindAllAsync(cancellationToken);
            return entityAlternates.MapToEntityAlternateDtoList(_mapper);
        }
    }
}