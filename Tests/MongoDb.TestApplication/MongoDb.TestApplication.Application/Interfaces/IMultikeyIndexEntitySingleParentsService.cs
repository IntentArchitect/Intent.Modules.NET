using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.MultikeyIndexEntitySingleParents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{
    public interface IMultikeyIndexEntitySingleParentsService : IDisposable
    {
        Task<string> CreateMultikeyIndexEntitySingleParent(MultikeyIndexEntitySingleParentCreateDto dto, CancellationToken cancellationToken = default);
        Task<MultikeyIndexEntitySingleParentDto> FindMultikeyIndexEntitySingleParentById(string id, CancellationToken cancellationToken = default);
        Task<List<MultikeyIndexEntitySingleParentDto>> FindMultikeyIndexEntitySingleParents(CancellationToken cancellationToken = default);
        Task UpdateMultikeyIndexEntitySingleParent(string id, MultikeyIndexEntitySingleParentUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteMultikeyIndexEntitySingleParent(string id, CancellationToken cancellationToken = default);
    }
}