using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.SingleIndexEntityMultiParents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{
    public interface ISingleIndexEntityMultiParentsService : IDisposable
    {
        Task<string> CreateSingleIndexEntityMultiParent(SingleIndexEntityMultiParentCreateDto dto, CancellationToken cancellationToken = default);
        Task<SingleIndexEntityMultiParentDto> FindSingleIndexEntityMultiParentById(string id, CancellationToken cancellationToken = default);
        Task<List<SingleIndexEntityMultiParentDto>> FindSingleIndexEntityMultiParents(CancellationToken cancellationToken = default);
        Task UpdateSingleIndexEntityMultiParent(string id, SingleIndexEntityMultiParentUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteSingleIndexEntityMultiParent(string id, CancellationToken cancellationToken = default);
    }
}