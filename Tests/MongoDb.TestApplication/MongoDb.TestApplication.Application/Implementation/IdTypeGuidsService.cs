using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.IdTypeGuids;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Domain.Common.Exceptions;
using MongoDb.TestApplication.Domain.Entities.IdTypes;
using MongoDb.TestApplication.Domain.Repositories.IdTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class IdTypeGuidsService : IIdTypeGuidsService
    {
        private readonly IIdTypeGuidRepository _idTypeGuidRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IdTypeGuidsService(IIdTypeGuidRepository idTypeGuidRepository, IMapper mapper)
        {
            _idTypeGuidRepository = idTypeGuidRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateIdTypeGuid(IdTypeGuidCreateDto dto, CancellationToken cancellationToken = default)
        {
            var newIdTypeGuid = new IdTypeGuid
            {
                Attribute = dto.Attribute,
            };
            _idTypeGuidRepository.Add(newIdTypeGuid);
            await _idTypeGuidRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newIdTypeGuid.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<IdTypeGuidDto> FindIdTypeGuidById(Guid id, CancellationToken cancellationToken = default)
        {
            var element = await _idTypeGuidRepository.FindByIdAsync(id, cancellationToken);

            if (element is null)
            {
                throw new NotFoundException($"Could not find IdTypeGuid {id}");
            }
            return element.MapToIdTypeGuidDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<IdTypeGuidDto>> FindIdTypeGuids(CancellationToken cancellationToken = default)
        {
            var elements = await _idTypeGuidRepository.FindAllAsync(cancellationToken);
            return elements.MapToIdTypeGuidDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateIdTypeGuid(Guid id, IdTypeGuidUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var existingIdTypeGuid = await _idTypeGuidRepository.FindByIdAsync(id, cancellationToken);

            if (existingIdTypeGuid is null)
            {
                throw new NotFoundException($"Could not find IdTypeGuid {id}");
            }
            existingIdTypeGuid.Attribute = dto.Attribute;
            _idTypeGuidRepository.Update(existingIdTypeGuid);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteIdTypeGuid(Guid id, CancellationToken cancellationToken = default)
        {
            var existingIdTypeGuid = await _idTypeGuidRepository.FindByIdAsync(id, cancellationToken);

            if (existingIdTypeGuid is null)
            {
                throw new NotFoundException($"Could not find IdTypeGuid {id}");
            }
            _idTypeGuidRepository.Remove(existingIdTypeGuid);
        }
    }
}