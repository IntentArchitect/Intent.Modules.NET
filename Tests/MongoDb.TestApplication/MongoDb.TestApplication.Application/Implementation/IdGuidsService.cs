using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.IdGuids;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Domain.Entities;
using MongoDb.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class IdGuidsService : IIdGuidsService
    {
        private readonly IIdGuidRepository _idGuidRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IdGuidsService(IIdGuidRepository idGuidRepository, IMapper mapper)
        {
            _idGuidRepository = idGuidRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Create(IdGuidCreateDto dto)
        {
            var newIdGuid = new IdGuid
            {
                Attribute = dto.Attribute,
            };
            _idGuidRepository.Add(newIdGuid);
            await _idGuidRepository.UnitOfWork.SaveChangesAsync();
            return newIdGuid.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<IdGuidDto> FindById(Guid id)
        {
            var element = await _idGuidRepository.FindByIdAsync(id);
            return element.MapToIdGuidDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<IdGuidDto>> FindAll()
        {
            var elements = await _idGuidRepository.FindAllAsync();
            return elements.MapToIdGuidDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Put(Guid id, IdGuidUpdateDto dto)
        {
            var existingIdGuid = await _idGuidRepository.FindByIdAsync(id);
            _idGuidRepository.Update(p => p.Id == id, existingIdGuid);
            existingIdGuid.Attribute = dto.Attribute;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<IdGuidDto> Delete(Guid id)
        {
            var existingIdGuid = await _idGuidRepository.FindByIdAsync(id);
            _idGuidRepository.Remove(existingIdGuid);
            return existingIdGuid.MapToIdGuidDto(_mapper);
        }

        public void Dispose()
        {
        }
    }
}