using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Application.TextIndexEntitySingleParents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class TextIndexEntitySingleParentsService : ITextIndexEntitySingleParentsService
    {
        [IntentManaged(Mode.Merge)]
        public TextIndexEntitySingleParentsService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<string> CreateTextIndexEntitySingleParent(
            TextIndexEntitySingleParentCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateTextIndexEntitySingleParent (TextIndexEntitySingleParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<TextIndexEntitySingleParentDto> FindTextIndexEntitySingleParentById(
            string id,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindTextIndexEntitySingleParentById (TextIndexEntitySingleParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<TextIndexEntitySingleParentDto>> FindTextIndexEntitySingleParents(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindTextIndexEntitySingleParents (TextIndexEntitySingleParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task UpdateTextIndexEntitySingleParent(
            string id,
            TextIndexEntitySingleParentUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateTextIndexEntitySingleParent (TextIndexEntitySingleParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task DeleteTextIndexEntitySingleParent(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteTextIndexEntitySingleParent (TextIndexEntitySingleParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}