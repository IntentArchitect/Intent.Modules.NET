using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.IdInts;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Domain.Entities;
using MongoDb.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class IdIntsService : IIdIntsService
    {
        private readonly IIdIntRepository _idIntRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IdIntsService(IIdIntRepository idIntRepository, IMapper mapper)
        {
            _idIntRepository = idIntRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<int> Create(IdIntCreateDto dto)
        {
            var newIdInt = new IdInt
            {
                Attribute = dto.Attribute,
            };
            _idIntRepository.Add(newIdInt);
            await _idIntRepository.UnitOfWork.SaveChangesAsync();
            return newIdInt.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<IdIntDto> FindById(int id)
        {
            var element = await _idIntRepository.FindByIdAsync(id);
            return element.MapToIdIntDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<IdIntDto>> FindAll()
        {
            var elements = await _idIntRepository.FindAllAsync();
            return elements.MapToIdIntDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Put(int id, IdIntUpdateDto dto)
        {
            var existingIdInt = await _idIntRepository.FindByIdAsync(id);
            _idIntRepository.Update(p => p.Id == id, existingIdInt);
            existingIdInt.Attribute = dto.Attribute;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<IdIntDto> Delete(int id)
        {
            var existingIdInt = await _idIntRepository.FindByIdAsync(id);
            _idIntRepository.Remove(existingIdInt);
            return existingIdInt.MapToIdIntDto(_mapper);
        }

        public void Dispose()
        {
        }
    }
}