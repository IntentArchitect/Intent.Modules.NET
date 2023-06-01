using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.MapperRoots;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{
    public interface IMapperRootsService : IDisposable
    {
        Task<string> CreateMapperRoot(MapperRootCreateDto dto, CancellationToken cancellationToken = default);
        Task<MapperRootDto> FindMapperRootById(string id, CancellationToken cancellationToken = default);
        Task<List<MapperRootDto>> FindMapperRoots(CancellationToken cancellationToken = default);
        Task UpdateMapperRoot(string id, MapperRootUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteMapperRoot(string id, CancellationToken cancellationToken = default);
    }
}