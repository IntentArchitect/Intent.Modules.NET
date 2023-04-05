using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.IdTypeLongs;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Domain.Entities.IdTypes;
using MongoDb.TestApplication.Domain.Repositories.IdTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class IdTypeLongsService : IIdTypeLongsService
    {
        private readonly IIdTypeLongRepository _idTypeLongRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IdTypeLongsService(IIdTypeLongRepository idTypeLongRepository, IMapper mapper)
        {
            _idTypeLongRepository = idTypeLongRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<long> Create(IdTypeLongCreateDto dto)
        {
            var newIdTypeLong = new IdTypeLong
            {
                Attribute = dto.Attribute,
            };
            _idTypeLongRepository.Add(newIdTypeLong);
            await _idTypeLongRepository.UnitOfWork.SaveChangesAsync();
            return newIdTypeLong.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<IdTypeLongDto> FindById(long id)
        {
            var element = await _idTypeLongRepository.FindByIdAsync(id);
            return element.MapToIdTypeLongDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<IdTypeLongDto>> FindAll()
        {
            var elements = await _idTypeLongRepository.FindAllAsync();
            return elements.MapToIdTypeLongDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Put(long id, IdTypeLongUpdateDto dto)
        {
            var existingIdTypeLong = await _idTypeLongRepository.FindByIdAsync(id);
            _idTypeLongRepository.Update(existingIdTypeLong);
            existingIdTypeLong.Attribute = dto.Attribute;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<IdTypeLongDto> Delete(long id)
        {
            var existingIdTypeLong = await _idTypeLongRepository.FindByIdAsync(id);
            _idTypeLongRepository.Remove(existingIdTypeLong);
            return existingIdTypeLong.MapToIdTypeLongDto(_mapper);
        }

        public void Dispose()
        {
        }
    }
}