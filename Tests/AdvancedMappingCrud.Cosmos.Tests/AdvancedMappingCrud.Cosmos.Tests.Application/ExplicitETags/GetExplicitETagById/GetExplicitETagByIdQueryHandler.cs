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

namespace AdvancedMappingCrud.Cosmos.Tests.Application.ExplicitETags.GetExplicitETagById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetExplicitETagByIdQueryHandler : IRequestHandler<GetExplicitETagByIdQuery, ExplicitETagDto>
    {
        private readonly IExplicitETagRepository _explicitETagRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetExplicitETagByIdQueryHandler(IExplicitETagRepository explicitETagRepository, IMapper mapper)
        {
            _explicitETagRepository = explicitETagRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ExplicitETagDto> Handle(GetExplicitETagByIdQuery request, CancellationToken cancellationToken)
        {
            var explicitETag = await _explicitETagRepository.FindByIdAsync(request.Id, cancellationToken);
            if (explicitETag is null)
            {
                throw new NotFoundException($"Could not find ExplicitETag '{request.Id}'");
            }
            return explicitETag.MapToExplicitETagDto(_mapper);
        }
    }
}