using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.DerivedOfTS;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Domain.Common.Exceptions;
using MongoDb.TestApplication.Domain.Entities;
using MongoDb.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class DerivedOfTSService : IDerivedOfTSService
    {
        private readonly IDerivedOfTRepository _derivedOfTRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DerivedOfTSService(IDerivedOfTRepository derivedOfTRepository, IMapper mapper)
        {
            _derivedOfTRepository = derivedOfTRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> CreateDerivedOfT(DerivedOfTCreateDto dto, CancellationToken cancellationToken = default)
        {
            var newDerivedOfT = new DerivedOfT
            {
                DerivedAttribute = dto.DerivedAttribute,
                BaseAttribute = dto.BaseAttribute,
            };
            _derivedOfTRepository.Add(newDerivedOfT);
            await _derivedOfTRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newDerivedOfT.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<DerivedOfTDto> FindDerivedOfTById(string id, CancellationToken cancellationToken = default)
        {
            var element = await _derivedOfTRepository.FindByIdAsync(id, cancellationToken);

            if (element is null)
            {
                throw new NotFoundException($"Could not find DerivedOfT {id}");
            }
            return element.MapToDerivedOfTDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<DerivedOfTDto>> FindDerivedOfTS(CancellationToken cancellationToken = default)
        {
            var elements = await _derivedOfTRepository.FindAllAsync(cancellationToken);
            return elements.MapToDerivedOfTDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateDerivedOfT(string id, DerivedOfTUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var existingDerivedOfT = await _derivedOfTRepository.FindByIdAsync(id, cancellationToken);

            if (existingDerivedOfT is null)
            {
                throw new NotFoundException($"Could not find DerivedOfT {id}");
            }
            existingDerivedOfT.DerivedAttribute = dto.DerivedAttribute;
            existingDerivedOfT.BaseAttribute = dto.BaseAttribute;
            _derivedOfTRepository.Update(existingDerivedOfT);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteDerivedOfT(string id, CancellationToken cancellationToken = default)
        {
            var existingDerivedOfT = await _derivedOfTRepository.FindByIdAsync(id, cancellationToken);

            if (existingDerivedOfT is null)
            {
                throw new NotFoundException($"Could not find DerivedOfT {id}");
            }
            _derivedOfTRepository.Remove(existingDerivedOfT);
        }
    }
}