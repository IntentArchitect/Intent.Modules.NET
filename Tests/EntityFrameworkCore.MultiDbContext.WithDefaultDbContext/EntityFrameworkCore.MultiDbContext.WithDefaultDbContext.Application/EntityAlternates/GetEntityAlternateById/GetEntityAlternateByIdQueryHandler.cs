using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Common.Exceptions;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAlternates.GetEntityAlternateById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEntityAlternateByIdQueryHandler : IRequestHandler<GetEntityAlternateByIdQuery, EntityAlternateDto>
    {
        private readonly IEntityAlternateRepository _entityAlternateRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetEntityAlternateByIdQueryHandler(IEntityAlternateRepository entityAlternateRepository, IMapper mapper)
        {
            _entityAlternateRepository = entityAlternateRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<EntityAlternateDto> Handle(
            GetEntityAlternateByIdQuery request,
            CancellationToken cancellationToken)
        {
            var entityAlternate = await _entityAlternateRepository.FindByIdAsync(request.Id, cancellationToken);
            if (entityAlternate is null)
            {
                throw new NotFoundException($"Could not find EntityAlternate '{request.Id}'");
            }
            return entityAlternate.MapToEntityAlternateDto(_mapper);
        }
    }
}