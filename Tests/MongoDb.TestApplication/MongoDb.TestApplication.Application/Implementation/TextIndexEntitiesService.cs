using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Application.TextIndexEntities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class TextIndexEntitiesService : ITextIndexEntitiesService
    {
        [IntentManaged(Mode.Merge)]
        public TextIndexEntitiesService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> CreateTextIndexEntity(
            TextIndexEntityCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateTextIndexEntity (TextIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<TextIndexEntityDto> FindTextIndexEntityById(
            string id,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindTextIndexEntityById (TextIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<TextIndexEntityDto>> FindTextIndexEntities(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindTextIndexEntities (TextIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task UpdateTextIndexEntity(
            string id,
            TextIndexEntityUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateTextIndexEntity (TextIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task DeleteTextIndexEntity(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteTextIndexEntity (TextIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}