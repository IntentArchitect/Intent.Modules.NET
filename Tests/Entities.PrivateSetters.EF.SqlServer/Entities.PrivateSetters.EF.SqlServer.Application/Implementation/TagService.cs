using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Entities.PrivateSetters.EF.SqlServer.Application.Interfaces;
using Entities.PrivateSetters.EF.SqlServer.Domain.Entities;
using Entities.PrivateSetters.EF.SqlServer.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Entities.PrivateSetters.EF.SqlServer.Application.Implementation
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
        public async Task Create(CreateTagDto dto)
        {
            var tag = new Tag(dto.Name);

            _tagRepository.Add(tag);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<TagDto>> GetAll()
        {
            var results = await _tagRepository.FindAllAsync();

            return results.MapToTagDtoList(_mapper);
        }

        public void Dispose()
        {
        }
    }
}