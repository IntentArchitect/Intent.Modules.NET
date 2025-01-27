using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.CompoundIndexEntityMultiParents;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Domain.Common.Exceptions;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class CompoundIndexEntityMultiParentsService : ICompoundIndexEntityMultiParentsService
    {
        private readonly ICompoundIndexEntityMultiParentRepository _compoundIndexEntityMultiParentRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CompoundIndexEntityMultiParentsService(ICompoundIndexEntityMultiParentRepository compoundIndexEntityMultiParentRepository,
            IMapper mapper)
        {
            _compoundIndexEntityMultiParentRepository = compoundIndexEntityMultiParentRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> CreateCompoundIndexEntityMultiParent(
            CompoundIndexEntityMultiParentCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            var newCompoundIndexEntityMultiParent = new CompoundIndexEntityMultiParent
            {
                SomeField = dto.SomeField,
                CompoundIndexEntityMultiChild = dto.CompoundIndexEntityMultiChild.Select(CreateCompoundIndexEntityMultiChild).ToList(),
            };
            _compoundIndexEntityMultiParentRepository.Add(newCompoundIndexEntityMultiParent);
            await _compoundIndexEntityMultiParentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newCompoundIndexEntityMultiParent.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CompoundIndexEntityMultiParentDto> FindCompoundIndexEntityMultiParentById(
            string id,
            CancellationToken cancellationToken = default)
        {
            var element = await _compoundIndexEntityMultiParentRepository.FindByIdAsync(id, cancellationToken);

            if (element is null)
            {
                throw new NotFoundException($"Could not find CompoundIndexEntityMultiParent {id}");
            }
            return element.MapToCompoundIndexEntityMultiParentDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CompoundIndexEntityMultiParentDto>> FindCompoundIndexEntityMultiParents(CancellationToken cancellationToken = default)
        {
            var elements = await _compoundIndexEntityMultiParentRepository.FindAllAsync(cancellationToken);
            return elements.MapToCompoundIndexEntityMultiParentDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateCompoundIndexEntityMultiParent(
            string id,
            CompoundIndexEntityMultiParentUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            var existingCompoundIndexEntityMultiParent = await _compoundIndexEntityMultiParentRepository.FindByIdAsync(id, cancellationToken);

            if (existingCompoundIndexEntityMultiParent is null)
            {
                throw new NotFoundException($"Could not find CompoundIndexEntityMultiParent {id}");
            }
            existingCompoundIndexEntityMultiParent.SomeField = dto.SomeField;
            _compoundIndexEntityMultiParentRepository.Update(existingCompoundIndexEntityMultiParent);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteCompoundIndexEntityMultiParent(string id, CancellationToken cancellationToken = default)
        {
            var existingCompoundIndexEntityMultiParent = await _compoundIndexEntityMultiParentRepository.FindByIdAsync(id, cancellationToken);

            if (existingCompoundIndexEntityMultiParent is null)
            {
                throw new NotFoundException($"Could not find CompoundIndexEntityMultiParent {id}");
            }
            _compoundIndexEntityMultiParentRepository.Remove(existingCompoundIndexEntityMultiParent);
        }

        [IntentManaged(Mode.Fully)]
        private CompoundIndexEntityMultiChild CreateCompoundIndexEntityMultiChild(CompoundIndexEntityMultiChildDto dto)
        {
            return new CompoundIndexEntityMultiChild
            {
                CompoundOne = dto.CompoundOne,
                CompoundTwo = dto.CompoundTwo,
            };
        }
    }
}