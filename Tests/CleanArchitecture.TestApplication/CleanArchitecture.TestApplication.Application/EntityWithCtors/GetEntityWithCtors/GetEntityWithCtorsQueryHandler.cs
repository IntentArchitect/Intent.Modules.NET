using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.EntityWithCtors.GetEntityWithCtors
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEntityWithCtorsQueryHandler : IRequestHandler<GetEntityWithCtorsQuery, List<EntityWithCtorDto>>
    {
        private readonly IEntityWithCtorRepository _entityWithCtorRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetEntityWithCtorsQueryHandler(IEntityWithCtorRepository entityWithCtorRepository, IMapper mapper)
        {
            _entityWithCtorRepository = entityWithCtorRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<EntityWithCtorDto>> Handle(
            GetEntityWithCtorsQuery request,
            CancellationToken cancellationToken)
        {
            var entityWithCtors = await _entityWithCtorRepository.FindAllAsync(cancellationToken);
            return entityWithCtors.MapToEntityWithCtorDtoList(_mapper);
        }
    }
}