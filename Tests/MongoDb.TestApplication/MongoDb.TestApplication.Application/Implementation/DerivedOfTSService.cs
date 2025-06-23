using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.DerivedOfTS;
using MongoDb.TestApplication.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class DerivedOfTSService : IDerivedOfTSService
    {
        [IntentManaged(Mode.Merge)]
        public DerivedOfTSService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> CreateDerivedOfT(DerivedOfTCreateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateDerivedOfT (DerivedOfTSService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<DerivedOfTDto> FindDerivedOfTById(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindDerivedOfTById (DerivedOfTSService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<DerivedOfTDto>> FindDerivedOfTS(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindDerivedOfTS (DerivedOfTSService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task UpdateDerivedOfT(string id, DerivedOfTUpdateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateDerivedOfT (DerivedOfTSService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task DeleteDerivedOfT(string id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteDerivedOfT (DerivedOfTSService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}