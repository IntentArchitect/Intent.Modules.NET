using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Application.MultikeyIndexEntityMultiParents;
using MongoDb.TestApplication.Domain.Common.Exceptions;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class MultikeyIndexEntityMultiParentsService : IMultikeyIndexEntityMultiParentsService
    {
        private readonly IMultikeyIndexEntityMultiParentRepository _multikeyIndexEntityMultiParentRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MultikeyIndexEntityMultiParentsService(IMultikeyIndexEntityMultiParentRepository multikeyIndexEntityMultiParentRepository,
            IMapper mapper)
        {
            _multikeyIndexEntityMultiParentRepository = multikeyIndexEntityMultiParentRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> CreateMultikeyIndexEntityMultiParent(
            MultikeyIndexEntityMultiParentCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            var newMultikeyIndexEntityMultiParent = new MultikeyIndexEntityMultiParent
            {
                SomeField = dto.SomeField,
                MultikeyIndexEntityMultiChild = dto.MultikeyIndexEntityMultiChild.Select(CreateMultikeyIndexEntityMultiChild).ToList(),
            };
            _multikeyIndexEntityMultiParentRepository.Add(newMultikeyIndexEntityMultiParent);
            await _multikeyIndexEntityMultiParentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newMultikeyIndexEntityMultiParent.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<MultikeyIndexEntityMultiParentDto> FindMultikeyIndexEntityMultiParentById(
            string id,
            CancellationToken cancellationToken = default)
        {
            var element = await _multikeyIndexEntityMultiParentRepository.FindByIdAsync(id, cancellationToken);

            if (element is null)
            {
                throw new NotFoundException($"Could not find MultikeyIndexEntityMultiParent {id}");
            }
            return element.MapToMultikeyIndexEntityMultiParentDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<MultikeyIndexEntityMultiParentDto>> FindMultikeyIndexEntityMultiParents(CancellationToken cancellationToken = default)
        {
            var elements = await _multikeyIndexEntityMultiParentRepository.FindAllAsync(cancellationToken);
            return elements.MapToMultikeyIndexEntityMultiParentDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateMultikeyIndexEntityMultiParent(
            string id,
            MultikeyIndexEntityMultiParentUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            var existingMultikeyIndexEntityMultiParent = await _multikeyIndexEntityMultiParentRepository.FindByIdAsync(id, cancellationToken);

            if (existingMultikeyIndexEntityMultiParent is null)
            {
                throw new NotFoundException($"Could not find MultikeyIndexEntityMultiParent {id}");
            }
            existingMultikeyIndexEntityMultiParent.SomeField = dto.SomeField;
            _multikeyIndexEntityMultiParentRepository.Update(existingMultikeyIndexEntityMultiParent);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteMultikeyIndexEntityMultiParent(string id, CancellationToken cancellationToken = default)
        {
            var existingMultikeyIndexEntityMultiParent = await _multikeyIndexEntityMultiParentRepository.FindByIdAsync(id, cancellationToken);

            if (existingMultikeyIndexEntityMultiParent is null)
            {
                throw new NotFoundException($"Could not find MultikeyIndexEntityMultiParent {id}");
            }
            _multikeyIndexEntityMultiParentRepository.Remove(existingMultikeyIndexEntityMultiParent);
        }

        public void Dispose()
        {
        }

        [IntentManaged(Mode.Fully)]
        private MultikeyIndexEntityMultiChild CreateMultikeyIndexEntityMultiChild(MultikeyIndexEntityMultiChildDto dto)
        {
            return new MultikeyIndexEntityMultiChild
            {
                MultiKey = dto.MultiKey.ToList(),
            };
        }
    }
}