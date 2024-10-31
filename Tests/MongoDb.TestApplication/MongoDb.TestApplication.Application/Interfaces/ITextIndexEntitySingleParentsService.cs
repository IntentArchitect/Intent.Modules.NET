using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.TextIndexEntitySingleParents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{
    public interface ITextIndexEntitySingleParentsService
    {
        Task<string> CreateTextIndexEntitySingleParent(TextIndexEntitySingleParentCreateDto dto, CancellationToken cancellationToken = default);
        Task<TextIndexEntitySingleParentDto> FindTextIndexEntitySingleParentById(string id, CancellationToken cancellationToken = default);
        Task<List<TextIndexEntitySingleParentDto>> FindTextIndexEntitySingleParents(CancellationToken cancellationToken = default);
        Task UpdateTextIndexEntitySingleParent(string id, TextIndexEntitySingleParentUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteTextIndexEntitySingleParent(string id, CancellationToken cancellationToken = default);
    }
}