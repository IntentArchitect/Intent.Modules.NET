using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Application.TextIndexEntitySingleParents;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class TextIndexEntitySingleParentsService : ITextIndexEntitySingleParentsService
    {
        private readonly ITextIndexEntitySingleParentRepository _textIndexEntitySingleParentRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public TextIndexEntitySingleParentsService(ITextIndexEntitySingleParentRepository textIndexEntitySingleParentRepository,
            IMapper mapper)
        {
            _textIndexEntitySingleParentRepository = textIndexEntitySingleParentRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> CreateTextIndexEntitySingleParent(
            TextIndexEntitySingleParentCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            var newTextIndexEntitySingleParent = new TextIndexEntitySingleParent
            {
                SomeField = dto.SomeField,
                TextIndexEntitySingleChild = CreateTextIndexEntitySingleChild(dto.TextIndexEntitySingleChild),
            };
            _textIndexEntitySingleParentRepository.Add(newTextIndexEntitySingleParent);
            await _textIndexEntitySingleParentRepository.UnitOfWork.SaveChangesAsync();
            return newTextIndexEntitySingleParent.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<TextIndexEntitySingleParentDto> FindTextIndexEntitySingleParentById(
            string id,
            CancellationToken cancellationToken = default)
        {
            var element = await _textIndexEntitySingleParentRepository.FindByIdAsync(id);
            return element.MapToTextIndexEntitySingleParentDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<TextIndexEntitySingleParentDto>> FindTextIndexEntitySingleParents(CancellationToken cancellationToken = default)
        {
            var elements = await _textIndexEntitySingleParentRepository.FindAllAsync();
            return elements.MapToTextIndexEntitySingleParentDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateTextIndexEntitySingleParent(
            string id,
            TextIndexEntitySingleParentUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            var existingTextIndexEntitySingleParent = await _textIndexEntitySingleParentRepository.FindByIdAsync(id);
            existingTextIndexEntitySingleParent.SomeField = dto.SomeField;
            _textIndexEntitySingleParentRepository.Update(existingTextIndexEntitySingleParent);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteTextIndexEntitySingleParent(string id, CancellationToken cancellationToken = default)
        {
            var existingTextIndexEntitySingleParent = await _textIndexEntitySingleParentRepository.FindByIdAsync(id);
            _textIndexEntitySingleParentRepository.Remove(existingTextIndexEntitySingleParent);
        }

        public void Dispose()
        {
        }

        [IntentManaged(Mode.Fully)]
        private TextIndexEntitySingleChild CreateTextIndexEntitySingleChild(TextIndexEntitySingleChildDto dto)
        {
            return new TextIndexEntitySingleChild
            {
                FullText = dto.FullText,
#warning No matching field found for Id
            };
        }
    }
}