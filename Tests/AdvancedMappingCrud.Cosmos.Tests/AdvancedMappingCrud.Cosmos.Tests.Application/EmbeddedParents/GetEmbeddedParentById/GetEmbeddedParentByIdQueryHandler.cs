using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.EmbeddedParents.GetEmbeddedParentById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEmbeddedParentByIdQueryHandler : IRequestHandler<GetEmbeddedParentByIdQuery, EmbeddedParentDto>
    {
        private readonly IEmbeddedParentRepository _embeddedParentRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetEmbeddedParentByIdQueryHandler(IEmbeddedParentRepository embeddedParentRepository, IMapper mapper)
        {
            _embeddedParentRepository = embeddedParentRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<EmbeddedParentDto> Handle(
            GetEmbeddedParentByIdQuery request,
            CancellationToken cancellationToken)
        {
            var embeddedParent = await _embeddedParentRepository.FindByIdAsync(request.Id, cancellationToken);
            if (embeddedParent is null)
            {
                throw new NotFoundException($"Could not find EmbeddedParent '{request.Id}'");
            }
            return embeddedParent.MapToEmbeddedParentDto(_mapper);
        }
    }
}