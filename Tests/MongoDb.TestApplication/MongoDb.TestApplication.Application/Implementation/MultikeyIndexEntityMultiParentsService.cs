using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Application.MultikeyIndexEntityMultiParents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class MultikeyIndexEntityMultiParentsService : IMultikeyIndexEntityMultiParentsService
    {
        [IntentManaged(Mode.Merge)]
        public MultikeyIndexEntityMultiParentsService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<string> CreateMultikeyIndexEntityMultiParent(
            MultikeyIndexEntityMultiParentCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateMultikeyIndexEntityMultiParent (MultikeyIndexEntityMultiParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<MultikeyIndexEntityMultiParentDto> FindMultikeyIndexEntityMultiParentById(
            string id,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindMultikeyIndexEntityMultiParentById (MultikeyIndexEntityMultiParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<MultikeyIndexEntityMultiParentDto>> FindMultikeyIndexEntityMultiParents(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindMultikeyIndexEntityMultiParents (MultikeyIndexEntityMultiParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task UpdateMultikeyIndexEntityMultiParent(
            string id,
            MultikeyIndexEntityMultiParentUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateMultikeyIndexEntityMultiParent (MultikeyIndexEntityMultiParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task DeleteMultikeyIndexEntityMultiParent(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteMultikeyIndexEntityMultiParent (MultikeyIndexEntityMultiParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}