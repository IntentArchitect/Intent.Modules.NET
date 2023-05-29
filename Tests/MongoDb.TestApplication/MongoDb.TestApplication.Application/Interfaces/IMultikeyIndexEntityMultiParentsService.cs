using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.MultikeyIndexEntityMultiParents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{
    public interface IMultikeyIndexEntityMultiParentsService : IDisposable
    {
        Task<string> CreateMultikeyIndexEntityMultiParent(MultikeyIndexEntityMultiParentCreateDto dto, CancellationToken cancellationToken = default);
        Task<MultikeyIndexEntityMultiParentDto> FindMultikeyIndexEntityMultiParentById(string id, CancellationToken cancellationToken = default);
        Task<List<MultikeyIndexEntityMultiParentDto>> FindMultikeyIndexEntityMultiParents(CancellationToken cancellationToken = default);
        Task UpdateMultikeyIndexEntityMultiParent(string id, MultikeyIndexEntityMultiParentUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteMultikeyIndexEntityMultiParent(string id, CancellationToken cancellationToken = default);
    }
}