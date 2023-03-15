using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.IdLongs;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Domain.Entities;
using MongoDb.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class IdLongsService : IIdLongsService
    {
        private readonly IIdLongRepository _idLongRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IdLongsService(IIdLongRepository idLongRepository, IMapper mapper)
        {
            _idLongRepository = idLongRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<long> Create(IdLongCreateDto dto)
        {
            var newIdLong = new IdLong
            {
                Attribute = dto.Attribute,
            };
            _idLongRepository.Add(newIdLong);
            await _idLongRepository.UnitOfWork.SaveChangesAsync();
            return newIdLong.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<IdLongDto> FindById(long id)
        {
            var element = await _idLongRepository.FindByIdAsync(id);
            return element.MapToIdLongDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<IdLongDto>> FindAll()
        {
            var elements = await _idLongRepository.FindAllAsync();
            return elements.MapToIdLongDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Put(long id, IdLongUpdateDto dto)
        {
            var existingIdLong = await _idLongRepository.FindByIdAsync(id);
            _idLongRepository.Update(p => p.Id == id, existingIdLong);
            existingIdLong.Attribute = dto.Attribute;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<IdLongDto> Delete(long id)
        {
            var existingIdLong = await _idLongRepository.FindByIdAsync(id);
            _idLongRepository.Remove(existingIdLong);
            return existingIdLong.MapToIdLongDto(_mapper);
        }

        public void Dispose()
        {
        }
    }
}