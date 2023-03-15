using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.IdObjectIds;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Domain.Entities;
using MongoDb.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class IdObjectIdsService : IIdObjectIdsService
    {
        private readonly IIdObjectIdRepository _idObjectIdRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IdObjectIdsService(IIdObjectIdRepository idObjectIdRepository, IMapper mapper)
        {
            _idObjectIdRepository = idObjectIdRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Create(IdObjectIdCreateDto dto)
        {
            var newIdObjectId = new IdObjectId
            {
                Attribute = dto.Attribute,
            };
            _idObjectIdRepository.Add(newIdObjectId);
            await _idObjectIdRepository.UnitOfWork.SaveChangesAsync();
            return newIdObjectId.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<IdObjectIdDto> FindById(string id)
        {
            var element = await _idObjectIdRepository.FindByIdAsync(id);
            return element.MapToIdObjectIdDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<IdObjectIdDto>> FindAll()
        {
            var elements = await _idObjectIdRepository.FindAllAsync();
            return elements.MapToIdObjectIdDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Put(string id, IdObjectIdUpdateDto dto)
        {
            var existingIdObjectId = await _idObjectIdRepository.FindByIdAsync(id);
            _idObjectIdRepository.Update(p => p.Id == id, existingIdObjectId);
            existingIdObjectId.Attribute = dto.Attribute;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<IdObjectIdDto> Delete(string id)
        {
            var existingIdObjectId = await _idObjectIdRepository.FindByIdAsync(id);
            _idObjectIdRepository.Remove(existingIdObjectId);
            return existingIdObjectId.MapToIdObjectIdDto(_mapper);
        }

        public void Dispose()
        {
        }
    }
}