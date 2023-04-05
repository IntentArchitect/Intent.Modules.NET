using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.IdTypeOjectIdStrs;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Domain.Entities.IdTypes;
using MongoDb.TestApplication.Domain.Repositories.IdTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class IdTypeOjectIdStrsService : IIdTypeOjectIdStrsService
    {
        private readonly IIdTypeOjectIdStrRepository _idTypeOjectIdStrRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IdTypeOjectIdStrsService(IIdTypeOjectIdStrRepository idTypeOjectIdStrRepository, IMapper mapper)
        {
            _idTypeOjectIdStrRepository = idTypeOjectIdStrRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Create(IdTypeOjectIdStrCreateDto dto)
        {
            var newIdTypeOjectIdStr = new IdTypeOjectIdStr
            {
                Attribute = dto.Attribute,
            };
            _idTypeOjectIdStrRepository.Add(newIdTypeOjectIdStr);
            await _idTypeOjectIdStrRepository.UnitOfWork.SaveChangesAsync();
            return newIdTypeOjectIdStr.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<IdTypeOjectIdStrDto> FindById(string id)
        {
            var element = await _idTypeOjectIdStrRepository.FindByIdAsync(id);
            return element.MapToIdTypeOjectIdStrDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<IdTypeOjectIdStrDto>> FindAll()
        {
            var elements = await _idTypeOjectIdStrRepository.FindAllAsync();
            return elements.MapToIdTypeOjectIdStrDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Put(string id, IdTypeOjectIdStrUpdateDto dto)
        {
            var existingIdTypeOjectIdStr = await _idTypeOjectIdStrRepository.FindByIdAsync(id);
            _idTypeOjectIdStrRepository.Update(existingIdTypeOjectIdStr);
            existingIdTypeOjectIdStr.Attribute = dto.Attribute;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<IdTypeOjectIdStrDto> Delete(string id)
        {
            var existingIdTypeOjectIdStr = await _idTypeOjectIdStrRepository.FindByIdAsync(id);
            _idTypeOjectIdStrRepository.Remove(existingIdTypeOjectIdStr);
            return existingIdTypeOjectIdStr.MapToIdTypeOjectIdStrDto(_mapper);
        }

        public void Dispose()
        {
        }
    }
}