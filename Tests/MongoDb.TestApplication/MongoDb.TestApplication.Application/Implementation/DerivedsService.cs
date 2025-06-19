using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Deriveds;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Domain.Common.Exceptions;
using MongoDb.TestApplication.Domain.Entities;
using MongoDb.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class DerivedsService : IDerivedsService
    {
        private readonly IDerivedRepository _derivedRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DerivedsService()
        {
            _derivedRepository = derivedRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> CreateDerived(DerivedCreateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateDerived (DerivedsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<DerivedDto> FindDerivedById(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindDerivedById (DerivedsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<DerivedDto>> FindDeriveds(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindDeriveds (DerivedsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateDerived(string id, DerivedUpdateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateDerived (DerivedsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteDerived(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteDerived (DerivedsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}