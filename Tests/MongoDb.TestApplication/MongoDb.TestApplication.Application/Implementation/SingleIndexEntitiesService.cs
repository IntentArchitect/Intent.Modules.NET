using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Application.SingleIndexEntities;
using MongoDb.TestApplication.Domain.Common.Exceptions;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class SingleIndexEntitiesService : ISingleIndexEntitiesService
    {

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SingleIndexEntitiesService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> CreateSingleIndexEntity(
            SingleIndexEntityCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateSingleIndexEntity (SingleIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<SingleIndexEntityDto> FindSingleIndexEntityById(
            string id,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindSingleIndexEntityById (SingleIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<SingleIndexEntityDto>> FindSingleIndexEntities(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindSingleIndexEntities (SingleIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateSingleIndexEntity(
            string id,
            SingleIndexEntityUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateSingleIndexEntity (SingleIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteSingleIndexEntity(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteSingleIndexEntity (SingleIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}