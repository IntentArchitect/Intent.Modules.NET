using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.TextIndexEntityMultiParents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{
    public interface ITextIndexEntityMultiParentsService
    {
        Task<string> CreateTextIndexEntityMultiParent(TextIndexEntityMultiParentCreateDto dto, CancellationToken cancellationToken = default);
        Task<TextIndexEntityMultiParentDto> FindTextIndexEntityMultiParentById(string id, CancellationToken cancellationToken = default);
        Task<List<TextIndexEntityMultiParentDto>> FindTextIndexEntityMultiParents(CancellationToken cancellationToken = default);
        Task UpdateTextIndexEntityMultiParent(string id, TextIndexEntityMultiParentUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteTextIndexEntityMultiParent(string id, CancellationToken cancellationToken = default);
    }
}