using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.CompoundIndexEntities;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Domain.Common.Exceptions;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class CompoundIndexEntitiesService : ICompoundIndexEntitiesService
    {
        private readonly ICompoundIndexEntityRepository _compoundIndexEntityRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CompoundIndexEntitiesService(ICompoundIndexEntityRepository compoundIndexEntityRepository, IMapper mapper)
        {
            _compoundIndexEntityRepository = compoundIndexEntityRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> CreateCompoundIndexEntity(
            CompoundIndexEntityCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            var newCompoundIndexEntity = new CompoundIndexEntity
            {
                SomeField = dto.SomeField,
                CompoundOne = dto.CompoundOne,
                CompoundTwo = dto.CompoundTwo,
            };
            _compoundIndexEntityRepository.Add(newCompoundIndexEntity);
            await _compoundIndexEntityRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newCompoundIndexEntity.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CompoundIndexEntityDto> FindCompoundIndexEntityById(
            string id,
            CancellationToken cancellationToken = default)
        {
            var element = await _compoundIndexEntityRepository.FindByIdAsync(id, cancellationToken);

            if (element is null)
            {
                throw new NotFoundException($"Could not find CompoundIndexEntity {id}");
            }
            return element.MapToCompoundIndexEntityDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CompoundIndexEntityDto>> FindCompoundIndexEntities(CancellationToken cancellationToken = default)
        {
            var elements = await _compoundIndexEntityRepository.FindAllAsync(cancellationToken);
            return elements.MapToCompoundIndexEntityDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateCompoundIndexEntity(
            string id,
            CompoundIndexEntityUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            var existingCompoundIndexEntity = await _compoundIndexEntityRepository.FindByIdAsync(id, cancellationToken);

            if (existingCompoundIndexEntity is null)
            {
                throw new NotFoundException($"Could not find CompoundIndexEntity {id}");
            }
            existingCompoundIndexEntity.SomeField = dto.SomeField;
            existingCompoundIndexEntity.CompoundOne = dto.CompoundOne;
            existingCompoundIndexEntity.CompoundTwo = dto.CompoundTwo;
            _compoundIndexEntityRepository.Update(existingCompoundIndexEntity);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteCompoundIndexEntity(string id, CancellationToken cancellationToken = default)
        {
            var existingCompoundIndexEntity = await _compoundIndexEntityRepository.FindByIdAsync(id, cancellationToken);

            if (existingCompoundIndexEntity is null)
            {
                throw new NotFoundException($"Could not find CompoundIndexEntity {id}");
            }
            _compoundIndexEntityRepository.Remove(existingCompoundIndexEntity);
        }

        public void Dispose()
        {
        }
    }
}