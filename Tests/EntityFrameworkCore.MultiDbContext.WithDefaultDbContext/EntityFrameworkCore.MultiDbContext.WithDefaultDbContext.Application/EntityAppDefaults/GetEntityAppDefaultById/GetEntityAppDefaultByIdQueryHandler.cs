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

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAppDefaults.GetEntityAppDefaultById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEntityAppDefaultByIdQueryHandler : IRequestHandler<GetEntityAppDefaultByIdQuery, EntityAppDefaultDto>
    {
        private readonly IEntityAppDefaultRepository _entityAppDefaultRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetEntityAppDefaultByIdQueryHandler(IEntityAppDefaultRepository entityAppDefaultRepository, IMapper mapper)
        {
            _entityAppDefaultRepository = entityAppDefaultRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<EntityAppDefaultDto> Handle(
            GetEntityAppDefaultByIdQuery request,
            CancellationToken cancellationToken)
        {
            var entityAppDefault = await _entityAppDefaultRepository.FindByIdAsync(request.Id, cancellationToken);
            if (entityAppDefault is null)
            {
                throw new NotFoundException($"Could not find EntityAppDefault '{request.Id}'");
            }
            return entityAppDefault.MapToEntityAppDefaultDto(_mapper);
        }
    }
}