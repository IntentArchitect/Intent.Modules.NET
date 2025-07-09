using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Application.MultikeyIndexEntities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class MultikeyIndexEntitiesService : IMultikeyIndexEntitiesService
    {
        [IntentManaged(Mode.Merge)]
        public MultikeyIndexEntitiesService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<string> CreateMultikeyIndexEntity(
            MultikeyIndexEntityCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateMultikeyIndexEntity (MultikeyIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<MultikeyIndexEntityDto> FindMultikeyIndexEntityById(
            string id,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindMultikeyIndexEntityById (MultikeyIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<MultikeyIndexEntityDto>> FindMultikeyIndexEntities(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindMultikeyIndexEntities (MultikeyIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task UpdateMultikeyIndexEntity(
            string id,
            MultikeyIndexEntityUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateMultikeyIndexEntity (MultikeyIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task DeleteMultikeyIndexEntity(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteMultikeyIndexEntity (MultikeyIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}