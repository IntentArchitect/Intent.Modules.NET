using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.IdTypeOjectIdStrs;
using MongoDb.TestApplication.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class IdTypeOjectIdStrsService : IIdTypeOjectIdStrsService
    {
        [IntentManaged(Mode.Merge)]
        public IdTypeOjectIdStrsService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<string> CreateIdTypeOjectIdStr(
            IdTypeOjectIdStrCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateIdTypeOjectIdStr (IdTypeOjectIdStrsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<IdTypeOjectIdStrDto> FindIdTypeOjectIdStrById(
            string id,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindIdTypeOjectIdStrById (IdTypeOjectIdStrsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<IdTypeOjectIdStrDto>> FindIdTypeOjectIdStrs(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindIdTypeOjectIdStrs (IdTypeOjectIdStrsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task UpdateIdTypeOjectIdStr(
            string id,
            IdTypeOjectIdStrUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateIdTypeOjectIdStr (IdTypeOjectIdStrsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task DeleteIdTypeOjectIdStr(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteIdTypeOjectIdStr (IdTypeOjectIdStrsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}