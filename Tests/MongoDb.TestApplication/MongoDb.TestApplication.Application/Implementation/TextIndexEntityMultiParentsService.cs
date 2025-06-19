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
        public TextIndexEntityMultiParentsService()
        {
            _textIndexEntityMultiParentRepository = textIndexEntityMultiParentRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> CreateTextIndexEntityMultiParent(
            TextIndexEntityMultiParentCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateTextIndexEntityMultiParent (TextIndexEntityMultiParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<TextIndexEntityMultiParentDto> FindTextIndexEntityMultiParentById(
            string id,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindTextIndexEntityMultiParentById (TextIndexEntityMultiParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<TextIndexEntityMultiParentDto>> FindTextIndexEntityMultiParents(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindTextIndexEntityMultiParents (TextIndexEntityMultiParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateTextIndexEntityMultiParent(
            string id,
            TextIndexEntityMultiParentUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateTextIndexEntityMultiParent (TextIndexEntityMultiParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteTextIndexEntityMultiParent(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteTextIndexEntityMultiParent (TextIndexEntityMultiParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}