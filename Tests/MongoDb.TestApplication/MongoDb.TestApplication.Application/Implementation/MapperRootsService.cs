using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Application.MapperRoots;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class MapperRootsService : IMapperRootsService
    {
        [IntentManaged(Mode.Merge)]
        public MapperRootsService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<string> CreateMapperRoot(MapperRootCreateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateMapperRoot (MapperRootsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<MapperRootDto> FindMapperRootById(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindMapperRootById (MapperRootsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<MapperRootDto>> FindMapperRoots(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindMapperRoots (MapperRootsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task UpdateMapperRoot(string id, MapperRootUpdateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateMapperRoot (MapperRootsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task DeleteMapperRoot(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteMapperRoot (MapperRootsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}