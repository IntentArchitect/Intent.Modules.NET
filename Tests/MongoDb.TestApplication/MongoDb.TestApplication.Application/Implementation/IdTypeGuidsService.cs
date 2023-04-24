using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.IdTypeGuids;
using MongoDb.TestApplication.Application.Interfaces;
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
        public async Task<Guid> CreateIdTypeGuid(IdTypeGuidCreateDto dto)
        {
            var newIdTypeGuid = new IdTypeGuid
            {
                Attribute = dto.Attribute,
            };
            _idTypeGuidRepository.Add(newIdTypeGuid);
            await _idTypeGuidRepository.UnitOfWork.SaveChangesAsync();
            return newIdTypeGuid.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<IdTypeGuidDto> FindIdTypeGuidById(Guid id)
        {
            var element = await _idTypeGuidRepository.FindByIdAsync(id);
            return element.MapToIdTypeGuidDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<IdTypeGuidDto>> FindIdTypeGuids()
        {
            var elements = await _idTypeGuidRepository.FindAllAsync();
            return elements.MapToIdTypeGuidDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateIdTypeGuid(Guid id, IdTypeGuidUpdateDto dto)
        {
            var existingIdTypeGuid = await _idTypeGuidRepository.FindByIdAsync(id);
            existingIdTypeGuid.Attribute = dto.Attribute;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteIdTypeGuid(Guid id)
        {
            var existingIdTypeGuid = await _idTypeGuidRepository.FindByIdAsync(id);
            _idTypeGuidRepository.Remove(existingIdTypeGuid);
        }

        public void Dispose()
        {
        }
    }
}