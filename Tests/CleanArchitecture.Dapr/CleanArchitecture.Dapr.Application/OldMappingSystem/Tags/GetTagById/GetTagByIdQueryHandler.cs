using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Dapr.Domain.Common.Exceptions;
using CleanArchitecture.Dapr.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Tags.GetTagById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetTagByIdQueryHandler : IRequestHandler<GetTagByIdQuery, TagDto>
    {
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetTagByIdQueryHandler(ITagRepository tagRepository, IMapper mapper)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<TagDto> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
        {
            var tag = await _tagRepository.FindByIdAsync(request.Id, cancellationToken);
            if (tag is null)
            {
                throw new NotFoundException($"Could not find Tag '{request.Id}'");
            }

            return tag.MapToTagDto(_mapper);
        }
    }
}