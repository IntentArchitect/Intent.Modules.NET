using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Entities.PrivateSetters.MongoDb.Application.Interfaces;
using Entities.PrivateSetters.MongoDb.Domain.Entities;
using Entities.PrivateSetters.MongoDb.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public TagService(
            ITagRepository tagRepository,
            IMapper mapper)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Create(CreateTagDto dto, CancellationToken cancellationToken = default)
        {
            var tag = new Tag(dto.Name);

            _tagRepository.Add(tag);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<TagDto>> GetAll(CancellationToken cancellationToken = default)
        {
            var results = await _tagRepository.FindAllAsync();

            return results.MapToTagDtoList(_mapper);
        }
    }
}