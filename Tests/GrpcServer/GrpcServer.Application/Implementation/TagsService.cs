using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GrpcServer.Application.Interfaces;
using GrpcServer.Application.Tags;
using GrpcServer.Domain.Common.Exceptions;
using GrpcServer.Domain.Entities;
using GrpcServer.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace GrpcServer.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class TagsService : ITagsService
    {
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public TagsService(ITagRepository tagRepository, IMapper mapper)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateTag(TagCreateDto dto, CancellationToken cancellationToken = default)
        {
            var tag = new Tag
            {
                Name = dto.Name
            };

            _tagRepository.Add(tag);
            await _tagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return tag.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateTag(Guid id, TagUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var tag = await _tagRepository.FindByIdAsync(id, cancellationToken);
            if (tag is null)
            {
                throw new NotFoundException($"Could not find Tag '{id}'");
            }

            tag.Name = dto.Name;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<TagDto> FindTagById(Guid id, CancellationToken cancellationToken = default)
        {
            var tag = await _tagRepository.FindByIdAsync(id, cancellationToken);
            if (tag is null)
            {
                throw new NotFoundException($"Could not find Tag '{id}'");
            }
            return tag.MapToTagDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<TagDto>> FindTags(CancellationToken cancellationToken = default)
        {
            var tags = await _tagRepository.FindAllAsync(cancellationToken);
            return tags.MapToTagDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteTag(Guid id, CancellationToken cancellationToken = default)
        {
            var tag = await _tagRepository.FindByIdAsync(id, cancellationToken);
            if (tag is null)
            {
                throw new NotFoundException($"Could not find Tag '{id}'");
            }

            _tagRepository.Remove(tag);
        }
    }
}