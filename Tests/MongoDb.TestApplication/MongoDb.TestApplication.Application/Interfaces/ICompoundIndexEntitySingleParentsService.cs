using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.CompoundIndexEntitySingleParents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{
    public interface ICompoundIndexEntitySingleParentsService
    {
        Task<string> CreateCompoundIndexEntitySingleParent(CompoundIndexEntitySingleParentCreateDto dto, CancellationToken cancellationToken = default);
        Task<CompoundIndexEntitySingleParentDto> FindCompoundIndexEntitySingleParentById(string id, CancellationToken cancellationToken = default);
        Task<List<CompoundIndexEntitySingleParentDto>> FindCompoundIndexEntitySingleParents(CancellationToken cancellationToken = default);
        Task UpdateCompoundIndexEntitySingleParent(string id, CompoundIndexEntitySingleParentUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteCompoundIndexEntitySingleParent(string id, CancellationToken cancellationToken = default);
    }
}