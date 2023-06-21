using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Application.TextIndexEntities;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class TextIndexEntitiesService : ITextIndexEntitiesService
    {
        private readonly ITextIndexEntityRepository _textIndexEntityRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public TextIndexEntitiesService(ITextIndexEntityRepository textIndexEntityRepository, IMapper mapper)
        {
            _textIndexEntityRepository = textIndexEntityRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> CreateTextIndexEntity(
            TextIndexEntityCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            var newTextIndexEntity = new TextIndexEntity
            {
                FullText = dto.FullText,
                SomeField = dto.SomeField,
            };
            _textIndexEntityRepository.Add(newTextIndexEntity);
            await _textIndexEntityRepository.UnitOfWork.SaveChangesAsync();
            return newTextIndexEntity.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<TextIndexEntityDto> FindTextIndexEntityById(
            string id,
            CancellationToken cancellationToken = default)
        {
            var element = await _textIndexEntityRepository.FindByIdAsync(id);
            return element.MapToTextIndexEntityDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<TextIndexEntityDto>> FindTextIndexEntities(CancellationToken cancellationToken = default)
        {
            var elements = await _textIndexEntityRepository.FindAllAsync();
            return elements.MapToTextIndexEntityDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateTextIndexEntity(
            string id,
            TextIndexEntityUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            var existingTextIndexEntity = await _textIndexEntityRepository.FindByIdAsync(id);
            existingTextIndexEntity.FullText = dto.FullText;
            existingTextIndexEntity.SomeField = dto.SomeField;
            _textIndexEntityRepository.Update(existingTextIndexEntity);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteTextIndexEntity(string id, CancellationToken cancellationToken = default)
        {
            var existingTextIndexEntity = await _textIndexEntityRepository.FindByIdAsync(id);
            _textIndexEntityRepository.Remove(existingTextIndexEntity);
        }

        public void Dispose()
        {
        }
    }
}