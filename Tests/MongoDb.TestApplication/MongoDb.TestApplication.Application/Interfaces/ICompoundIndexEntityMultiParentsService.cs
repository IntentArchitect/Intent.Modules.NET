using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.CompoundIndexEntityMultiParents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{
    public interface ICompoundIndexEntityMultiParentsService
    {
        Task<string> CreateCompoundIndexEntityMultiParent(CompoundIndexEntityMultiParentCreateDto dto, CancellationToken cancellationToken = default);
        Task<CompoundIndexEntityMultiParentDto> FindCompoundIndexEntityMultiParentById(string id, CancellationToken cancellationToken = default);
        Task<List<CompoundIndexEntityMultiParentDto>> FindCompoundIndexEntityMultiParents(CancellationToken cancellationToken = default);
        Task UpdateCompoundIndexEntityMultiParent(string id, CompoundIndexEntityMultiParentUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteCompoundIndexEntityMultiParent(string id, CancellationToken cancellationToken = default);
    }
}