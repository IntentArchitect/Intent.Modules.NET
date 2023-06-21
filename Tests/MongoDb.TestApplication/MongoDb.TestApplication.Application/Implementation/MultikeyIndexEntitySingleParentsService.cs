using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Application.MultikeyIndexEntitySingleParents;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class MultikeyIndexEntitySingleParentsService : IMultikeyIndexEntitySingleParentsService
    {
        private readonly IMultikeyIndexEntitySingleParentRepository _multikeyIndexEntitySingleParentRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MultikeyIndexEntitySingleParentsService(IMultikeyIndexEntitySingleParentRepository multikeyIndexEntitySingleParentRepository,
            IMapper mapper)
        {
            _multikeyIndexEntitySingleParentRepository = multikeyIndexEntitySingleParentRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> CreateMultikeyIndexEntitySingleParent(
            MultikeyIndexEntitySingleParentCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            var newMultikeyIndexEntitySingleParent = new MultikeyIndexEntitySingleParent
            {
                SomeField = dto.SomeField,
                MultikeyIndexEntitySingleChild = CreateMultikeyIndexEntitySingleChild(dto.MultikeyIndexEntitySingleChild),
            };
            _multikeyIndexEntitySingleParentRepository.Add(newMultikeyIndexEntitySingleParent);
            await _multikeyIndexEntitySingleParentRepository.UnitOfWork.SaveChangesAsync();
            return newMultikeyIndexEntitySingleParent.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<MultikeyIndexEntitySingleParentDto> FindMultikeyIndexEntitySingleParentById(
            string id,
            CancellationToken cancellationToken = default)
        {
            var element = await _multikeyIndexEntitySingleParentRepository.FindByIdAsync(id);
            return element.MapToMultikeyIndexEntitySingleParentDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<MultikeyIndexEntitySingleParentDto>> FindMultikeyIndexEntitySingleParents(CancellationToken cancellationToken = default)
        {
            var elements = await _multikeyIndexEntitySingleParentRepository.FindAllAsync();
            return elements.MapToMultikeyIndexEntitySingleParentDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateMultikeyIndexEntitySingleParent(
            string id,
            MultikeyIndexEntitySingleParentUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            var existingMultikeyIndexEntitySingleParent = await _multikeyIndexEntitySingleParentRepository.FindByIdAsync(id);
            existingMultikeyIndexEntitySingleParent.SomeField = dto.SomeField;
            _multikeyIndexEntitySingleParentRepository.Update(existingMultikeyIndexEntitySingleParent);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteMultikeyIndexEntitySingleParent(string id, CancellationToken cancellationToken = default)
        {
            var existingMultikeyIndexEntitySingleParent = await _multikeyIndexEntitySingleParentRepository.FindByIdAsync(id);
            _multikeyIndexEntitySingleParentRepository.Remove(existingMultikeyIndexEntitySingleParent);
        }

        public void Dispose()
        {
        }

        [IntentManaged(Mode.Fully)]
        private MultikeyIndexEntitySingleChild CreateMultikeyIndexEntitySingleChild(MultikeyIndexEntitySingleChildDto dto)
        {
            return new MultikeyIndexEntitySingleChild
            {
                MultiKey = dto.MultiKey.ToList(),
#warning No matching field found for Id
            };
        }
    }
}