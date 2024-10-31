using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Application.SingleIndexEntityMultiParents;
using MongoDb.TestApplication.Domain.Common.Exceptions;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class SingleIndexEntityMultiParentsService : ISingleIndexEntityMultiParentsService
    {
        private readonly ISingleIndexEntityMultiParentRepository _singleIndexEntityMultiParentRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SingleIndexEntityMultiParentsService(ISingleIndexEntityMultiParentRepository singleIndexEntityMultiParentRepository,
            IMapper mapper)
        {
            _singleIndexEntityMultiParentRepository = singleIndexEntityMultiParentRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> CreateSingleIndexEntityMultiParent(
            SingleIndexEntityMultiParentCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            var newSingleIndexEntityMultiParent = new SingleIndexEntityMultiParent
            {
                SomeField = dto.SomeField,
                SingleIndexEntityMultiChild = dto.SingleIndexEntityMultiChild.Select(CreateSingleIndexEntityMultiChild).ToList(),
            };
            _singleIndexEntityMultiParentRepository.Add(newSingleIndexEntityMultiParent);
            await _singleIndexEntityMultiParentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newSingleIndexEntityMultiParent.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<SingleIndexEntityMultiParentDto> FindSingleIndexEntityMultiParentById(
            string id,
            CancellationToken cancellationToken = default)
        {
            var element = await _singleIndexEntityMultiParentRepository.FindByIdAsync(id, cancellationToken);

            if (element is null)
            {
                throw new NotFoundException($"Could not find SingleIndexEntityMultiParent {id}");
            }
            return element.MapToSingleIndexEntityMultiParentDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<SingleIndexEntityMultiParentDto>> FindSingleIndexEntityMultiParents(CancellationToken cancellationToken = default)
        {
            var elements = await _singleIndexEntityMultiParentRepository.FindAllAsync(cancellationToken);
            return elements.MapToSingleIndexEntityMultiParentDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateSingleIndexEntityMultiParent(
            string id,
            SingleIndexEntityMultiParentUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            var existingSingleIndexEntityMultiParent = await _singleIndexEntityMultiParentRepository.FindByIdAsync(id, cancellationToken);

            if (existingSingleIndexEntityMultiParent is null)
            {
                throw new NotFoundException($"Could not find SingleIndexEntityMultiParent {id}");
            }
            existingSingleIndexEntityMultiParent.SomeField = dto.SomeField;
            _singleIndexEntityMultiParentRepository.Update(existingSingleIndexEntityMultiParent);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteSingleIndexEntityMultiParent(string id, CancellationToken cancellationToken = default)
        {
            var existingSingleIndexEntityMultiParent = await _singleIndexEntityMultiParentRepository.FindByIdAsync(id, cancellationToken);

            if (existingSingleIndexEntityMultiParent is null)
            {
                throw new NotFoundException($"Could not find SingleIndexEntityMultiParent {id}");
            }
            _singleIndexEntityMultiParentRepository.Remove(existingSingleIndexEntityMultiParent);
        }

        [IntentManaged(Mode.Fully)]
        private SingleIndexEntityMultiChild CreateSingleIndexEntityMultiChild(SingleIndexEntityMultiChildDto dto)
        {
            return new SingleIndexEntityMultiChild
            {
                SingleIndex = dto.SingleIndex,
            };
        }
    }
}