using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Application.SingleIndexEntities;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class SingleIndexEntitiesService : ISingleIndexEntitiesService
    {
        private readonly ISingleIndexEntityRepository _singleIndexEntityRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SingleIndexEntitiesService(ISingleIndexEntityRepository singleIndexEntityRepository, IMapper mapper)
        {
            _singleIndexEntityRepository = singleIndexEntityRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> CreateSingleIndexEntity(
            SingleIndexEntityCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            var newSingleIndexEntity = new SingleIndexEntity
            {
                SomeField = dto.SomeField,
                SingleIndex = dto.SingleIndex,
            };
            _singleIndexEntityRepository.Add(newSingleIndexEntity);
            await _singleIndexEntityRepository.UnitOfWork.SaveChangesAsync();
            return newSingleIndexEntity.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<SingleIndexEntityDto> FindSingleIndexEntityById(
            string id,
            CancellationToken cancellationToken = default)
        {
            var element = await _singleIndexEntityRepository.FindByIdAsync(id);
            return element.MapToSingleIndexEntityDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<SingleIndexEntityDto>> FindSingleIndexEntities(CancellationToken cancellationToken = default)
        {
            var elements = await _singleIndexEntityRepository.FindAllAsync();
            return elements.MapToSingleIndexEntityDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateSingleIndexEntity(
            string id,
            SingleIndexEntityUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            var existingSingleIndexEntity = await _singleIndexEntityRepository.FindByIdAsync(id);
            existingSingleIndexEntity.SomeField = dto.SomeField;
            existingSingleIndexEntity.SingleIndex = dto.SingleIndex;
            _singleIndexEntityRepository.Update(existingSingleIndexEntity);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteSingleIndexEntity(string id, CancellationToken cancellationToken = default)
        {
            var existingSingleIndexEntity = await _singleIndexEntityRepository.FindByIdAsync(id);
            _singleIndexEntityRepository.Remove(existingSingleIndexEntity);
        }

        public void Dispose()
        {
        }
    }
}