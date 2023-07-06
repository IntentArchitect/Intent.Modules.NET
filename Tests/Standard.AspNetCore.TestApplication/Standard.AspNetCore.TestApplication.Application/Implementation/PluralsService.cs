using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.Interfaces;
using Standard.AspNetCore.TestApplication.Application.Plurals;
using Standard.AspNetCore.TestApplication.Domain.Common.Exceptions;
using Standard.AspNetCore.TestApplication.Domain.Entities;
using Standard.AspNetCore.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class PluralsService : IPluralsService
    {
        private readonly IPluralsRepository _pluralsRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PluralsService(IPluralsRepository pluralsRepository, IMapper mapper)
        {
            _pluralsRepository = pluralsRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreatePlurals(PluralsCreateDto dto, CancellationToken cancellationToken = default)
        {
            var newPlurals = new Domain.Entities.Plurals
            {
            };
            _pluralsRepository.Add(newPlurals);
            await _pluralsRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newPlurals.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PluralsDto> FindPluralsById(Guid id, CancellationToken cancellationToken = default)
        {
            var element = await _pluralsRepository.FindByIdAsync(id, cancellationToken);

            if (element is null)
            {
                throw new NotFoundException($"Could not find Plurals {id}");
            }
            return element.MapToPluralsDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<PluralsDto>> FindPlurals(CancellationToken cancellationToken = default)
        {
            var elements = await _pluralsRepository.FindAllAsync(cancellationToken);
            return elements.MapToPluralsDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdatePlurals(Guid id, PluralsUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var existingPlurals = await _pluralsRepository.FindByIdAsync(id, cancellationToken);

            if (existingPlurals is null)
            {
                throw new NotFoundException($"Could not find Plurals {id}");
            }
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeletePlurals(Guid id, CancellationToken cancellationToken = default)
        {
            var existingPlurals = await _pluralsRepository.FindByIdAsync(id, cancellationToken);

            if (existingPlurals is null)
            {
                throw new NotFoundException($"Could not find Plurals {id}");
            }
            _pluralsRepository.Remove(existingPlurals);
        }

        public void Dispose()
        {
        }
    }
}