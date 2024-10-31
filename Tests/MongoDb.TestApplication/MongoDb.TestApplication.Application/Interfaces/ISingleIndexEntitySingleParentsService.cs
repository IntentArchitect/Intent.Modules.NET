using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.SingleIndexEntitySingleParents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{
    public interface ISingleIndexEntitySingleParentsService
    {
        Task<string> CreateSingleIndexEntitySingleParent(SingleIndexEntitySingleParentCreateDto dto, CancellationToken cancellationToken = default);
        Task<SingleIndexEntitySingleParentDto> FindSingleIndexEntitySingleParentById(string id, CancellationToken cancellationToken = default);
        Task<List<SingleIndexEntitySingleParentDto>> FindSingleIndexEntitySingleParents(CancellationToken cancellationToken = default);
        Task UpdateSingleIndexEntitySingleParent(string id, SingleIndexEntitySingleParentUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteSingleIndexEntitySingleParent(string id, CancellationToken cancellationToken = default);
    }
}