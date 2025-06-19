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
        public CompoundIndexEntityMultiParentsService()
        {
            _compoundIndexEntityMultiParentRepository = compoundIndexEntityMultiParentRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> CreateCompoundIndexEntityMultiParent(
            CompoundIndexEntityMultiParentCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateCompoundIndexEntityMultiParent (CompoundIndexEntityMultiParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CompoundIndexEntityMultiParentDto> FindCompoundIndexEntityMultiParentById(
            string id,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindCompoundIndexEntityMultiParentById (CompoundIndexEntityMultiParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CompoundIndexEntityMultiParentDto>> FindCompoundIndexEntityMultiParents(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindCompoundIndexEntityMultiParents (CompoundIndexEntityMultiParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateCompoundIndexEntityMultiParent(
            string id,
            CompoundIndexEntityMultiParentUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateCompoundIndexEntityMultiParent (CompoundIndexEntityMultiParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteCompoundIndexEntityMultiParent(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteCompoundIndexEntityMultiParent (CompoundIndexEntityMultiParentsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}