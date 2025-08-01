using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.Interfaces;
using Standard.AspNetCore.TestApplication.Application.Plurals;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class PluralsService : IPluralsService
    {
        [IntentManaged(Mode.Merge)]
        public PluralsService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<Guid> CreatePlurals(PluralsCreateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreatePlurals (PluralsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<PluralsDto> FindPluralsById(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindPluralsById (PluralsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<PluralsDto>> FindPlurals(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindPlurals (PluralsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task UpdatePlurals(Guid id, PluralsUpdateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdatePlurals (PluralsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task DeletePlurals(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeletePlurals (PluralsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}