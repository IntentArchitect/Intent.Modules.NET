using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Dapr.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Tags.GetTags
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, List<TagDto>>
    {
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetTagsQueryHandler(ITagRepository tagRepository, IMapper mapper)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<TagDto>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
        {
            var tags = await _tagRepository.FindAllAsync(cancellationToken);
            return tags.MapToTagDtoList(_mapper);
        }
    }
}