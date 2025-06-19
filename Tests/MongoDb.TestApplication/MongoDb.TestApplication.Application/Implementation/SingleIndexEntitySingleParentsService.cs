using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Application.SingleIndexEntitySingleParents;
using MongoDb.TestApplication.Domain.Common.Exceptions;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class SingleIndexEntitySingleParentsService : ISingleIndexEntitySingleParentsService
    {

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SingleIndexEntitySingleParentsService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> CreateSingleIndexEntitySingleParent(
            SingleIndexEntitySingleParentCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateSingleIndexEntitySingleParent (SingleIndexEntitySingleParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<SingleIndexEntitySingleParentDto> FindSingleIndexEntitySingleParentById(
            string id,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindSingleIndexEntitySingleParentById (SingleIndexEntitySingleParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<SingleIndexEntitySingleParentDto>> FindSingleIndexEntitySingleParents(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindSingleIndexEntitySingleParents (SingleIndexEntitySingleParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateSingleIndexEntitySingleParent(
            string id,
            SingleIndexEntitySingleParentUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateSingleIndexEntitySingleParent (SingleIndexEntitySingleParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteSingleIndexEntitySingleParent(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteSingleIndexEntitySingleParent (SingleIndexEntitySingleParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}