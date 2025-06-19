using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Application.MapperRoots;
using MongoDb.TestApplication.Domain.Common.Exceptions;
using MongoDb.TestApplication.Domain.Entities;
using MongoDb.TestApplication.Domain.Entities.Mappings;
using MongoDb.TestApplication.Domain.Repositories;
using MongoDb.TestApplication.Domain.Repositories.Mappings;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class MapperRootsService : IMapperRootsService
    {
        private readonly IMapperRootRepository _mapperRootRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MapperRootsService()
        {
            _mapperRootRepository = mapperRootRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> CreateMapperRoot(MapperRootCreateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateMapperRoot (MapperRootsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<MapperRootDto> FindMapperRootById(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindMapperRootById (MapperRootsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<MapperRootDto>> FindMapperRoots(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindMapperRoots (MapperRootsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateMapperRoot(string id, MapperRootUpdateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateMapperRoot (MapperRootsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteMapperRoot(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteMapperRoot (MapperRootsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}