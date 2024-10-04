using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Services
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CategoriesService : ICategoriesService
    {
        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public CategoriesService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<IEnumerable<Category>> GetOrCreateCategoriesAsync(
            IEnumerable<string> names,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement GetOrCreateCategoriesAsync (CategoriesService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoIt<T>(T it)
        {
            // TODO: Implement DoIt (CategoriesService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task ManualAsync(CancellationToken cancellationToken = default)
        {
            // TODO: Implement ManualAsync (CategoriesService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }
    }
}