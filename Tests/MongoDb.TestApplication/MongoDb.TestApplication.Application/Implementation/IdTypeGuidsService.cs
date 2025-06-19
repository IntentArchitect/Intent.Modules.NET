using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.IdTypeGuids;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Domain.Common.Exceptions;
using MongoDb.TestApplication.Domain.Entities.IdTypes;
using MongoDb.TestApplication.Domain.Repositories.IdTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class IdTypeGuidsService : IIdTypeGuidsService
    {

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IdTypeGuidsService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateIdTypeGuid(IdTypeGuidCreateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateIdTypeGuid (IdTypeGuidsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<IdTypeGuidDto> FindIdTypeGuidById(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindIdTypeGuidById (IdTypeGuidsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<IdTypeGuidDto>> FindIdTypeGuids(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindIdTypeGuids (IdTypeGuidsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateIdTypeGuid(Guid id, IdTypeGuidUpdateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateIdTypeGuid (IdTypeGuidsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteIdTypeGuid(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteIdTypeGuid (IdTypeGuidsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}