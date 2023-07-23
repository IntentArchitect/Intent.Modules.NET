using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Deriveds;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Domain.Common.Exceptions;
using MongoDb.TestApplication.Domain.Entities;
using MongoDb.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class DerivedsService : IDerivedsService
    {
        private readonly IDerivedRepository _derivedRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DerivedsService(IDerivedRepository derivedRepository, IMapper mapper)
        {
            _derivedRepository = derivedRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> CreateDerived(DerivedCreateDto dto, CancellationToken cancellationToken = default)
        {
            var newDerived = new Derived
            {
                DerivedAttribute = dto.DerivedAttribute,
                BaseAttribute = dto.BaseAttribute,
            };
            _derivedRepository.Add(newDerived);
            await _derivedRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newDerived.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<DerivedDto> FindDerivedById(string id, CancellationToken cancellationToken = default)
        {
            var element = await _derivedRepository.FindByIdAsync(id, cancellationToken);

            if (element is null)
            {
                throw new NotFoundException($"Could not find Derived {id}");
            }
            return element.MapToDerivedDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<DerivedDto>> FindDeriveds(CancellationToken cancellationToken = default)
        {
            var elements = await _derivedRepository.FindAllAsync(cancellationToken);
            return elements.MapToDerivedDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateDerived(string id, DerivedUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var existingDerived = await _derivedRepository.FindByIdAsync(id, cancellationToken);

            if (existingDerived is null)
            {
                throw new NotFoundException($"Could not find Derived {id}");
            }
            existingDerived.DerivedAttribute = dto.DerivedAttribute;
            existingDerived.BaseAttribute = dto.BaseAttribute;
            _derivedRepository.Update(existingDerived);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteDerived(string id, CancellationToken cancellationToken = default)
        {
            var existingDerived = await _derivedRepository.FindByIdAsync(id, cancellationToken);

            if (existingDerived is null)
            {
                throw new NotFoundException($"Could not find Derived {id}");
            }
            _derivedRepository.Remove(existingDerived);
        }

        public void Dispose()
        {
        }
    }
}