using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Application.SingleIndexEntitySingleParents;
using MongoDb.TestApplication.Domain.Common.Exceptions;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class SingleIndexEntitySingleParentsService : ISingleIndexEntitySingleParentsService
    {
        private readonly ISingleIndexEntitySingleParentRepository _singleIndexEntitySingleParentRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SingleIndexEntitySingleParentsService(ISingleIndexEntitySingleParentRepository singleIndexEntitySingleParentRepository,
            IMapper mapper)
        {
            _singleIndexEntitySingleParentRepository = singleIndexEntitySingleParentRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> CreateSingleIndexEntitySingleParent(
            SingleIndexEntitySingleParentCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            var newSingleIndexEntitySingleParent = new SingleIndexEntitySingleParent
            {
                SomeField = dto.SomeField,
                SingleIndexEntitySingleChild = CreateSingleIndexEntitySingleChild(dto.SingleIndexEntitySingleChild),
            };
            _singleIndexEntitySingleParentRepository.Add(newSingleIndexEntitySingleParent);
            await _singleIndexEntitySingleParentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newSingleIndexEntitySingleParent.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<SingleIndexEntitySingleParentDto> FindSingleIndexEntitySingleParentById(
            string id,
            CancellationToken cancellationToken = default)
        {
            var element = await _singleIndexEntitySingleParentRepository.FindByIdAsync(id, cancellationToken);

            if (element is null)
            {
                throw new NotFoundException($"Could not find SingleIndexEntitySingleParent {id}");
            }
            return element.MapToSingleIndexEntitySingleParentDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<SingleIndexEntitySingleParentDto>> FindSingleIndexEntitySingleParents(CancellationToken cancellationToken = default)
        {
            var elements = await _singleIndexEntitySingleParentRepository.FindAllAsync(cancellationToken);
            return elements.MapToSingleIndexEntitySingleParentDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateSingleIndexEntitySingleParent(
            string id,
            SingleIndexEntitySingleParentUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            var existingSingleIndexEntitySingleParent = await _singleIndexEntitySingleParentRepository.FindByIdAsync(id, cancellationToken);

            if (existingSingleIndexEntitySingleParent is null)
            {
                throw new NotFoundException($"Could not find SingleIndexEntitySingleParent {id}");
            }
            existingSingleIndexEntitySingleParent.SomeField = dto.SomeField;
            _singleIndexEntitySingleParentRepository.Update(existingSingleIndexEntitySingleParent);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteSingleIndexEntitySingleParent(string id, CancellationToken cancellationToken = default)
        {
            var existingSingleIndexEntitySingleParent = await _singleIndexEntitySingleParentRepository.FindByIdAsync(id, cancellationToken);

            if (existingSingleIndexEntitySingleParent is null)
            {
                throw new NotFoundException($"Could not find SingleIndexEntitySingleParent {id}");
            }
            _singleIndexEntitySingleParentRepository.Remove(existingSingleIndexEntitySingleParent);
        }

        public void Dispose()
        {
        }

        [IntentManaged(Mode.Fully)]
        private SingleIndexEntitySingleChild CreateSingleIndexEntitySingleChild(SingleIndexEntitySingleChildDto dto)
        {
            return new SingleIndexEntitySingleChild
            {
                SingleIndex = dto.SingleIndex,
#warning No matching field found for Id
            };
        }
    }
}