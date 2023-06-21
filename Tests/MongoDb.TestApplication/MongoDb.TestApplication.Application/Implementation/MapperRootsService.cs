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
        public MapperRootsService(IMapperRootRepository mapperRootRepository, IMapper mapper)
        {
            _mapperRootRepository = mapperRootRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> CreateMapperRoot(MapperRootCreateDto dto, CancellationToken cancellationToken = default)
        {
            var newMapperRoot = new MapperRoot
            {
                No = dto.No,
                MapAggChildrenIds = dto.MapAggChildrenIds.ToList(),
                MapAggPeerId = dto.MapAggPeerId,
            };
            _mapperRootRepository.Add(newMapperRoot);
            await _mapperRootRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newMapperRoot.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<MapperRootDto> FindMapperRootById(string id, CancellationToken cancellationToken = default)
        {
            var element = await _mapperRootRepository.FindByIdAsync(id, cancellationToken);

            if (element is null)
            {
                throw new NotFoundException($"Could not find MapperRoot {id}");
            }
            return element.MapToMapperRootDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<MapperRootDto>> FindMapperRoots(CancellationToken cancellationToken = default)
        {
            var elements = await _mapperRootRepository.FindAllAsync(cancellationToken);
            return elements.MapToMapperRootDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateMapperRoot(string id, MapperRootUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var existingMapperRoot = await _mapperRootRepository.FindByIdAsync(id, cancellationToken);

            if (existingMapperRoot is null)
            {
                throw new NotFoundException($"Could not find MapperRoot {id}");
            }
            existingMapperRoot.No = dto.No;
            existingMapperRoot.MapAggChildrenIds = dto.MapAggChildrenIds.ToList();
            existingMapperRoot.MapAggPeerId = dto.MapAggPeerId;
            _mapperRootRepository.Update(existingMapperRoot);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteMapperRoot(string id, CancellationToken cancellationToken = default)
        {
            var existingMapperRoot = await _mapperRootRepository.FindByIdAsync(id, cancellationToken);

            if (existingMapperRoot is null)
            {
                throw new NotFoundException($"Could not find MapperRoot {id}");
            }
            _mapperRootRepository.Remove(existingMapperRoot);
        }

        public void Dispose()
        {
        }
    }
}