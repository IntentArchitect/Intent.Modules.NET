using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.IdTypeInts;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Domain.Entities.IdTypes;
using MongoDb.TestApplication.Domain.Repositories.IdTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class IdTypeIntsService : IIdTypeIntsService
    {
        private readonly IIdTypeIntRepository _idTypeIntRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IdTypeIntsService(IIdTypeIntRepository idTypeIntRepository, IMapper mapper)
        {
            _idTypeIntRepository = idTypeIntRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<int> Create(IdTypeIntCreateDto dto)
        {
            var newIdTypeInt = new IdTypeInt
            {
                Attribute = dto.Attribute,
            };
            _idTypeIntRepository.Add(newIdTypeInt);
            await _idTypeIntRepository.UnitOfWork.SaveChangesAsync();
            return newIdTypeInt.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<IdTypeIntDto> FindById(int id)
        {
            var element = await _idTypeIntRepository.FindByIdAsync(id);
            return element.MapToIdTypeIntDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<IdTypeIntDto>> FindAll()
        {
            var elements = await _idTypeIntRepository.FindAllAsync();
            return elements.MapToIdTypeIntDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Put(int id, IdTypeIntUpdateDto dto)
        {
            var existingIdTypeInt = await _idTypeIntRepository.FindByIdAsync(id);
            _idTypeIntRepository.Update(existingIdTypeInt);
            existingIdTypeInt.Attribute = dto.Attribute;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<IdTypeIntDto> Delete(int id)
        {
            var existingIdTypeInt = await _idTypeIntRepository.FindByIdAsync(id);
            _idTypeIntRepository.Remove(existingIdTypeInt);
            return existingIdTypeInt.MapToIdTypeIntDto(_mapper);
        }

        public void Dispose()
        {
        }
    }
}