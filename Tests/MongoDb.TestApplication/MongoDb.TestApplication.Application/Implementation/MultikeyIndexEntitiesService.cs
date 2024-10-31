using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Application.MultikeyIndexEntities;
using MongoDb.TestApplication.Domain.Common.Exceptions;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class MultikeyIndexEntitiesService : IMultikeyIndexEntitiesService
    {
        private readonly IMultikeyIndexEntityRepository _multikeyIndexEntityRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MultikeyIndexEntitiesService(IMultikeyIndexEntityRepository multikeyIndexEntityRepository, IMapper mapper)
        {
            _multikeyIndexEntityRepository = multikeyIndexEntityRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> CreateMultikeyIndexEntity(
            MultikeyIndexEntityCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            var newMultikeyIndexEntity = new MultikeyIndexEntity
            {
                MultiKey = dto.MultiKey.ToList(),
                SomeField = dto.SomeField,
            };
            _multikeyIndexEntityRepository.Add(newMultikeyIndexEntity);
            await _multikeyIndexEntityRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newMultikeyIndexEntity.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<MultikeyIndexEntityDto> FindMultikeyIndexEntityById(
            string id,
            CancellationToken cancellationToken = default)
        {
            var element = await _multikeyIndexEntityRepository.FindByIdAsync(id, cancellationToken);

            if (element is null)
            {
                throw new NotFoundException($"Could not find MultikeyIndexEntity {id}");
            }
            return element.MapToMultikeyIndexEntityDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<MultikeyIndexEntityDto>> FindMultikeyIndexEntities(CancellationToken cancellationToken = default)
        {
            var elements = await _multikeyIndexEntityRepository.FindAllAsync(cancellationToken);
            return elements.MapToMultikeyIndexEntityDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateMultikeyIndexEntity(
            string id,
            MultikeyIndexEntityUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            var existingMultikeyIndexEntity = await _multikeyIndexEntityRepository.FindByIdAsync(id, cancellationToken);

            if (existingMultikeyIndexEntity is null)
            {
                throw new NotFoundException($"Could not find MultikeyIndexEntity {id}");
            }
            existingMultikeyIndexEntity.MultiKey = dto.MultiKey.ToList();
            existingMultikeyIndexEntity.SomeField = dto.SomeField;
            _multikeyIndexEntityRepository.Update(existingMultikeyIndexEntity);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteMultikeyIndexEntity(string id, CancellationToken cancellationToken = default)
        {
            var existingMultikeyIndexEntity = await _multikeyIndexEntityRepository.FindByIdAsync(id, cancellationToken);

            if (existingMultikeyIndexEntity is null)
            {
                throw new NotFoundException($"Could not find MultikeyIndexEntity {id}");
            }
            _multikeyIndexEntityRepository.Remove(existingMultikeyIndexEntity);
        }
    }
}