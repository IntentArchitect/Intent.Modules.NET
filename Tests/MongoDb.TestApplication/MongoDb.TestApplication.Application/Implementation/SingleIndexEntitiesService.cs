using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Application.SingleIndexEntities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class SingleIndexEntitiesService : ISingleIndexEntitiesService
    {
        [IntentManaged(Mode.Merge)]
        public SingleIndexEntitiesService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> CreateSingleIndexEntity(
            SingleIndexEntityCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateSingleIndexEntity (SingleIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<SingleIndexEntityDto> FindSingleIndexEntityById(
            string id,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindSingleIndexEntityById (SingleIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<SingleIndexEntityDto>> FindSingleIndexEntities(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindSingleIndexEntities (SingleIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task UpdateSingleIndexEntity(
            string id,
            SingleIndexEntityUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateSingleIndexEntity (SingleIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task DeleteSingleIndexEntity(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteSingleIndexEntity (SingleIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}