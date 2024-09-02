using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Entities;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Domain.Services
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CategoriesService : ICategoriesService
    {
        private readonly ICategoryRepository _categoriesRepository;

        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public CategoriesService(ICategoryRepository categoriesRepository)
        {
            _categoriesRepository = categoriesRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<IEnumerable<Category>> GetOrCreateCategoriesAsync(
            IEnumerable<string> names,
            CancellationToken cancellationToken = default)
        {
            var categories = new List<Category>();
            foreach (var categoryName in names)
            {
                var category = _categoriesRepository.FindAsync(c => c.Name == categoryName).GetAwaiter().GetResult();
                if (category == null)
                {
                    category = new Category(name: categoryName);
                    _categoriesRepository.Add(category);
                }
                categories.Add(category);
            }
            return categories;
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