using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.MultikeyIndexEntities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{
    public interface IMultikeyIndexEntitiesService
    {
        Task<string> CreateMultikeyIndexEntity(MultikeyIndexEntityCreateDto dto, CancellationToken cancellationToken = default);
        Task<MultikeyIndexEntityDto> FindMultikeyIndexEntityById(string id, CancellationToken cancellationToken = default);
        Task<List<MultikeyIndexEntityDto>> FindMultikeyIndexEntities(CancellationToken cancellationToken = default);
        Task UpdateMultikeyIndexEntity(string id, MultikeyIndexEntityUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteMultikeyIndexEntity(string id, CancellationToken cancellationToken = default);
    }
}