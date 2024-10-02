using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CosmosDB.EnumStrings.Domain.Common.Exceptions;
using CosmosDB.EnumStrings.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities.GetRootEntityById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetRootEntityByIdQueryHandler : IRequestHandler<GetRootEntityByIdQuery, RootEntityDto>
    {
        private readonly IRootEntityRepository _rootEntityRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetRootEntityByIdQueryHandler(IRootEntityRepository rootEntityRepository, IMapper mapper)
        {
            _rootEntityRepository = rootEntityRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<RootEntityDto> Handle(GetRootEntityByIdQuery request, CancellationToken cancellationToken)
        {
            var rootEntity = await _rootEntityRepository.FindByIdAsync(request.Id, cancellationToken);
            if (rootEntity is null)
            {
                throw new NotFoundException($"Could not find RootEntity '{request.Id}'");
            }
            return rootEntity.MapToRootEntityDto(_mapper);
        }
    }
}