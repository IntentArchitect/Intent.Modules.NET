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

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityDefaults.GetEntityDefaultById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEntityDefaultByIdQueryHandler : IRequestHandler<GetEntityDefaultByIdQuery, EntityDefaultDto>
    {
        private readonly IEntityDefaultRepository _entityDefaultRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetEntityDefaultByIdQueryHandler(IEntityDefaultRepository entityDefaultRepository, IMapper mapper)
        {
            _entityDefaultRepository = entityDefaultRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<EntityDefaultDto> Handle(GetEntityDefaultByIdQuery request, CancellationToken cancellationToken)
        {
            var entityDefault = await _entityDefaultRepository.FindByIdAsync(request.Id, cancellationToken);
            if (entityDefault is null)
            {
                throw new NotFoundException($"Could not find EntityDefault '{request.Id}'");
            }
            return entityDefault.MapToEntityDefaultDto(_mapper);
        }
    }
}