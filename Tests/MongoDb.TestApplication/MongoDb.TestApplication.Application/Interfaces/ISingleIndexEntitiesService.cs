using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.SingleIndexEntities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{
    public interface ISingleIndexEntitiesService : IDisposable
    {
        Task<string> CreateSingleIndexEntity(SingleIndexEntityCreateDto dto, CancellationToken cancellationToken = default);
        Task<SingleIndexEntityDto> FindSingleIndexEntityById(string id, CancellationToken cancellationToken = default);
        Task<List<SingleIndexEntityDto>> FindSingleIndexEntities(CancellationToken cancellationToken = default);
        Task UpdateSingleIndexEntity(string id, SingleIndexEntityUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteSingleIndexEntity(string id, CancellationToken cancellationToken = default);
    }
}