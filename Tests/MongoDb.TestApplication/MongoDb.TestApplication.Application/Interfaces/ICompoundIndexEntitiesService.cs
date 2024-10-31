using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.CompoundIndexEntities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{
    public interface ICompoundIndexEntitiesService
    {
        Task<string> CreateCompoundIndexEntity(CompoundIndexEntityCreateDto dto, CancellationToken cancellationToken = default);
        Task<CompoundIndexEntityDto> FindCompoundIndexEntityById(string id, CancellationToken cancellationToken = default);
        Task<List<CompoundIndexEntityDto>> FindCompoundIndexEntities(CancellationToken cancellationToken = default);
        Task UpdateCompoundIndexEntity(string id, CompoundIndexEntityUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteCompoundIndexEntity(string id, CancellationToken cancellationToken = default);
    }
}