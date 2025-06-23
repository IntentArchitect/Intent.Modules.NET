using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Application.SingleIndexEntityMultiParents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class SingleIndexEntityMultiParentsService : ISingleIndexEntityMultiParentsService
    {
        [IntentManaged(Mode.Merge)]
        public SingleIndexEntityMultiParentsService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> CreateSingleIndexEntityMultiParent(
            SingleIndexEntityMultiParentCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateSingleIndexEntityMultiParent (SingleIndexEntityMultiParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<SingleIndexEntityMultiParentDto> FindSingleIndexEntityMultiParentById(
            string id,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindSingleIndexEntityMultiParentById (SingleIndexEntityMultiParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<SingleIndexEntityMultiParentDto>> FindSingleIndexEntityMultiParents(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindSingleIndexEntityMultiParents (SingleIndexEntityMultiParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task UpdateSingleIndexEntityMultiParent(
            string id,
            SingleIndexEntityMultiParentUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateSingleIndexEntityMultiParent (SingleIndexEntityMultiParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task DeleteSingleIndexEntityMultiParent(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteSingleIndexEntityMultiParent (SingleIndexEntityMultiParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}