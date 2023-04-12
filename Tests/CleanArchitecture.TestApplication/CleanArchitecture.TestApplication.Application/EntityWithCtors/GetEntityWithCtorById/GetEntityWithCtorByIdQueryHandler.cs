using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.EntityWithCtors.GetEntityWithCtorById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEntityWithCtorByIdQueryHandler : IRequestHandler<GetEntityWithCtorByIdQuery, EntityWithCtorDto>
    {
        private readonly IEntityWithCtorRepository _entityWithCtorRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetEntityWithCtorByIdQueryHandler(IEntityWithCtorRepository entityWithCtorRepository, IMapper mapper)
        {
            _entityWithCtorRepository = entityWithCtorRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<EntityWithCtorDto> Handle(
            GetEntityWithCtorByIdQuery request,
            CancellationToken cancellationToken)
        {
            var entityWithCtor = await _entityWithCtorRepository.FindByIdAsync(request.Id, cancellationToken);
            return entityWithCtor.MapToEntityWithCtorDto(_mapper);
        }
    }
}