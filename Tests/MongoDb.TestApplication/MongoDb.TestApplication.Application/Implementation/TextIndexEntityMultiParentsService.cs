using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Application.TextIndexEntityMultiParents;
using MongoDb.TestApplication.Domain.Common.Exceptions;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class TextIndexEntityMultiParentsService : ITextIndexEntityMultiParentsService
    {
        private readonly ITextIndexEntityMultiParentRepository _textIndexEntityMultiParentRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public TextIndexEntityMultiParentsService(ITextIndexEntityMultiParentRepository textIndexEntityMultiParentRepository,
            IMapper mapper)
        {
            _textIndexEntityMultiParentRepository = textIndexEntityMultiParentRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> CreateTextIndexEntityMultiParent(
            TextIndexEntityMultiParentCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            var newTextIndexEntityMultiParent = new TextIndexEntityMultiParent
            {
                SomeField = dto.SomeField,
                TextIndexEntityMultiChild = dto.TextIndexEntityMultiChild.Select(CreateTextIndexEntityMultiChild).ToList(),
            };
            _textIndexEntityMultiParentRepository.Add(newTextIndexEntityMultiParent);
            await _textIndexEntityMultiParentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newTextIndexEntityMultiParent.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<TextIndexEntityMultiParentDto> FindTextIndexEntityMultiParentById(
            string id,
            CancellationToken cancellationToken = default)
        {
            var element = await _textIndexEntityMultiParentRepository.FindByIdAsync(id, cancellationToken);

            if (element is null)
            {
                throw new NotFoundException($"Could not find TextIndexEntityMultiParent {id}");
            }
            return element.MapToTextIndexEntityMultiParentDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<TextIndexEntityMultiParentDto>> FindTextIndexEntityMultiParents(CancellationToken cancellationToken = default)
        {
            var elements = await _textIndexEntityMultiParentRepository.FindAllAsync(cancellationToken);
            return elements.MapToTextIndexEntityMultiParentDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateTextIndexEntityMultiParent(
            string id,
            TextIndexEntityMultiParentUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            var existingTextIndexEntityMultiParent = await _textIndexEntityMultiParentRepository.FindByIdAsync(id, cancellationToken);

            if (existingTextIndexEntityMultiParent is null)
            {
                throw new NotFoundException($"Could not find TextIndexEntityMultiParent {id}");
            }
            existingTextIndexEntityMultiParent.SomeField = dto.SomeField;
            _textIndexEntityMultiParentRepository.Update(existingTextIndexEntityMultiParent);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteTextIndexEntityMultiParent(string id, CancellationToken cancellationToken = default)
        {
            var existingTextIndexEntityMultiParent = await _textIndexEntityMultiParentRepository.FindByIdAsync(id, cancellationToken);

            if (existingTextIndexEntityMultiParent is null)
            {
                throw new NotFoundException($"Could not find TextIndexEntityMultiParent {id}");
            }
            _textIndexEntityMultiParentRepository.Remove(existingTextIndexEntityMultiParent);
        }

        public void Dispose()
        {
        }

        [IntentManaged(Mode.Fully)]
        private TextIndexEntityMultiChild CreateTextIndexEntityMultiChild(TextIndexEntityMultiChildDto dto)
        {
            return new TextIndexEntityMultiChild
            {
                FullText = dto.FullText,
#warning No matching field found for Id
            };
        }
    }
}