using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.CompoundIndexEntitySingleParents;
using MongoDb.TestApplication.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class CompoundIndexEntitySingleParentsService : ICompoundIndexEntitySingleParentsService
    {
        [IntentManaged(Mode.Merge)]
        public CompoundIndexEntitySingleParentsService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> CreateCompoundIndexEntitySingleParent(
            CompoundIndexEntitySingleParentCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateCompoundIndexEntitySingleParent (CompoundIndexEntitySingleParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<CompoundIndexEntitySingleParentDto> FindCompoundIndexEntitySingleParentById(
            string id,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindCompoundIndexEntitySingleParentById (CompoundIndexEntitySingleParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<CompoundIndexEntitySingleParentDto>> FindCompoundIndexEntitySingleParents(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindCompoundIndexEntitySingleParents (CompoundIndexEntitySingleParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task UpdateCompoundIndexEntitySingleParent(
            string id,
            CompoundIndexEntitySingleParentUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateCompoundIndexEntitySingleParent (CompoundIndexEntitySingleParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task DeleteCompoundIndexEntitySingleParent(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteCompoundIndexEntitySingleParent (CompoundIndexEntitySingleParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}