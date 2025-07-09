using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Deriveds;
using MongoDb.TestApplication.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class DerivedsService : IDerivedsService
    {
        [IntentManaged(Mode.Merge)]
        public DerivedsService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<string> CreateDerived(DerivedCreateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateDerived (DerivedsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<DerivedDto> FindDerivedById(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindDerivedById (DerivedsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<DerivedDto>> FindDeriveds(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindDeriveds (DerivedsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task UpdateDerived(string id, DerivedUpdateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateDerived (DerivedsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task DeleteDerived(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteDerived (DerivedsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}