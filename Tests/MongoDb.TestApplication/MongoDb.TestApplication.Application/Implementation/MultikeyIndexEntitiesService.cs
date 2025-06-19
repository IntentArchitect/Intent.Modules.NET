using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Application.MultikeyIndexEntities;
using MongoDb.TestApplication.Domain.Common.Exceptions;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class MultikeyIndexEntitiesService : IMultikeyIndexEntitiesService
    {

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MultikeyIndexEntitiesService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> CreateMultikeyIndexEntity(
            MultikeyIndexEntityCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateMultikeyIndexEntity (MultikeyIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<MultikeyIndexEntityDto> FindMultikeyIndexEntityById(
            string id,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindMultikeyIndexEntityById (MultikeyIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<MultikeyIndexEntityDto>> FindMultikeyIndexEntities(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindMultikeyIndexEntities (MultikeyIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateMultikeyIndexEntity(
            string id,
            MultikeyIndexEntityUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateMultikeyIndexEntity (MultikeyIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteMultikeyIndexEntity(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteMultikeyIndexEntity (MultikeyIndexEntitiesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}