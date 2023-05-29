using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.TextIndexEntities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{
    public interface ITextIndexEntitiesService : IDisposable
    {
        Task<string> CreateTextIndexEntity(TextIndexEntityCreateDto dto, CancellationToken cancellationToken = default);
        Task<TextIndexEntityDto> FindTextIndexEntityById(string id, CancellationToken cancellationToken = default);
        Task<List<TextIndexEntityDto>> FindTextIndexEntities(CancellationToken cancellationToken = default);
        Task UpdateTextIndexEntity(string id, TextIndexEntityUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteTextIndexEntity(string id, CancellationToken cancellationToken = default);
    }
}