using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Application.MultikeyIndexEntitySingleParents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class MultikeyIndexEntitySingleParentsService : IMultikeyIndexEntitySingleParentsService
    {
        [IntentManaged(Mode.Merge)]
        public MultikeyIndexEntitySingleParentsService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> CreateMultikeyIndexEntitySingleParent(
            MultikeyIndexEntitySingleParentCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateMultikeyIndexEntitySingleParent (MultikeyIndexEntitySingleParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<MultikeyIndexEntitySingleParentDto> FindMultikeyIndexEntitySingleParentById(
            string id,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindMultikeyIndexEntitySingleParentById (MultikeyIndexEntitySingleParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<MultikeyIndexEntitySingleParentDto>> FindMultikeyIndexEntitySingleParents(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindMultikeyIndexEntitySingleParents (MultikeyIndexEntitySingleParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task UpdateMultikeyIndexEntitySingleParent(
            string id,
            MultikeyIndexEntitySingleParentUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateMultikeyIndexEntitySingleParent (MultikeyIndexEntitySingleParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task DeleteMultikeyIndexEntitySingleParent(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteMultikeyIndexEntitySingleParent (MultikeyIndexEntitySingleParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}