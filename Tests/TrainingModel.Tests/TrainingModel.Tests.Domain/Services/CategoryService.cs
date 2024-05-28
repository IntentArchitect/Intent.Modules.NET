using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using TrainingModel.Tests.Domain.Entities;
using TrainingModel.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceImplementation", Version = "1.0")]

namespace TrainingModel.Tests.Domain.Services
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoriesRepository;

        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public CategoryService(ICategoryRepository categoriesRepository)
        {
            _categoriesRepository = categoriesRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<IEnumerable<Category>> GetOrCreateCategoriesAsync(
            IEnumerable<string> categoryNames,
            CancellationToken cancellationToken = default)
        {
            var categories = new List<Category>();
            foreach (var categoryName in categoryNames)
            {
                var category = await _categoriesRepository.FindAsync(c => c.Name == categoryName, cancellationToken);
                if (category == null)
                {
                    category = new Category() { Name = categoryName };
                    _categoriesRepository.Add(category);
                }
                categories.Add(category);
            }
            return categories;
        }
    }
}