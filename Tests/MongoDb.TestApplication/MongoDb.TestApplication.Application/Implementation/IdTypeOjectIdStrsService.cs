using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.IdTypeOjectIdStrs;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Domain.Common.Exceptions;
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
        public async Task<string> CreateIdTypeOjectIdStr(
            IdTypeOjectIdStrCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            var newIdTypeOjectIdStr = new IdTypeOjectIdStr
            {
                Attribute = dto.Attribute,
            };
            _idTypeOjectIdStrRepository.Add(newIdTypeOjectIdStr);
            await _idTypeOjectIdStrRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newIdTypeOjectIdStr.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<IdTypeOjectIdStrDto> FindIdTypeOjectIdStrById(
            string id,
            CancellationToken cancellationToken = default)
        {
            var element = await _idTypeOjectIdStrRepository.FindByIdAsync(id, cancellationToken);

            if (element is null)
            {
                throw new NotFoundException($"Could not find IdTypeOjectIdStr {id}");
            }
            return element.MapToIdTypeOjectIdStrDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<IdTypeOjectIdStrDto>> FindIdTypeOjectIdStrs(CancellationToken cancellationToken = default)
        {
            var elements = await _idTypeOjectIdStrRepository.FindAllAsync(cancellationToken);
            return elements.MapToIdTypeOjectIdStrDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateIdTypeOjectIdStr(
            string id,
            IdTypeOjectIdStrUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            var existingIdTypeOjectIdStr = await _idTypeOjectIdStrRepository.FindByIdAsync(id, cancellationToken);

            if (existingIdTypeOjectIdStr is null)
            {
                throw new NotFoundException($"Could not find IdTypeOjectIdStr {id}");
            }
            existingIdTypeOjectIdStr.Attribute = dto.Attribute;
            _idTypeOjectIdStrRepository.Update(existingIdTypeOjectIdStr);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteIdTypeOjectIdStr(string id, CancellationToken cancellationToken = default)
        {
            var existingIdTypeOjectIdStr = await _idTypeOjectIdStrRepository.FindByIdAsync(id, cancellationToken);

            if (existingIdTypeOjectIdStr is null)
            {
                throw new NotFoundException($"Could not find IdTypeOjectIdStr {id}");
            }
            _idTypeOjectIdStrRepository.Remove(existingIdTypeOjectIdStr);
        }
    }
}