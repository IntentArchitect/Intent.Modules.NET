using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.CompoundIndexEntitySingleParents;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class CompoundIndexEntitySingleParentsService : ICompoundIndexEntitySingleParentsService
    {
        private readonly ICompoundIndexEntitySingleParentRepository _compoundIndexEntitySingleParentRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CompoundIndexEntitySingleParentsService(ICompoundIndexEntitySingleParentRepository compoundIndexEntitySingleParentRepository,
            IMapper mapper)
        {
            _compoundIndexEntitySingleParentRepository = compoundIndexEntitySingleParentRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> CreateCompoundIndexEntitySingleParent(
            CompoundIndexEntitySingleParentCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            var newCompoundIndexEntitySingleParent = new CompoundIndexEntitySingleParent
            {
                SomeField = dto.SomeField,
                CompoundIndexEntitySingleChild = CreateCompoundIndexEntitySingleChild(dto.CompoundIndexEntitySingleChild),
            };
            _compoundIndexEntitySingleParentRepository.Add(newCompoundIndexEntitySingleParent);
            await _compoundIndexEntitySingleParentRepository.UnitOfWork.SaveChangesAsync();
            return newCompoundIndexEntitySingleParent.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CompoundIndexEntitySingleParentDto> FindCompoundIndexEntitySingleParentById(
            string id,
            CancellationToken cancellationToken = default)
        {
            var element = await _compoundIndexEntitySingleParentRepository.FindByIdAsync(id);
            return element.MapToCompoundIndexEntitySingleParentDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CompoundIndexEntitySingleParentDto>> FindCompoundIndexEntitySingleParents(CancellationToken cancellationToken = default)
        {
            var elements = await _compoundIndexEntitySingleParentRepository.FindAllAsync();
            return elements.MapToCompoundIndexEntitySingleParentDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateCompoundIndexEntitySingleParent(
            string id,
            CompoundIndexEntitySingleParentUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            var existingCompoundIndexEntitySingleParent = await _compoundIndexEntitySingleParentRepository.FindByIdAsync(id);
            existingCompoundIndexEntitySingleParent.SomeField = dto.SomeField;
            _compoundIndexEntitySingleParentRepository.Update(existingCompoundIndexEntitySingleParent);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteCompoundIndexEntitySingleParent(string id, CancellationToken cancellationToken = default)
        {
            var existingCompoundIndexEntitySingleParent = await _compoundIndexEntitySingleParentRepository.FindByIdAsync(id);
            _compoundIndexEntitySingleParentRepository.Remove(existingCompoundIndexEntitySingleParent);
        }

        public void Dispose()
        {
        }

        [IntentManaged(Mode.Fully)]
        private CompoundIndexEntitySingleChild CreateCompoundIndexEntitySingleChild(CompoundIndexEntitySingleChildDto dto)
        {
            return new CompoundIndexEntitySingleChild
            {
                CompoundOne = dto.CompoundOne,
                CompoundTwo = dto.CompoundTwo,
#warning No matching field found for Id
            };
        }
    }
}