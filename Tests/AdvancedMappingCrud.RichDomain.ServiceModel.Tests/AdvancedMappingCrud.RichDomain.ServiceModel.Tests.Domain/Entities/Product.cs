using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Common;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Services;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities
{
    public class Product : IHasDomainEvent
    {
        private List<Category> _categories = new List<Category>();

        public Product(string name, IEnumerable<string> categoryNames, ICategoriesService categoryService)
        {
            Name = name;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Product()
        {
            Name = null!;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public virtual IReadOnlyCollection<Category> Categories
        {
            get => _categories.AsReadOnly();
            private set => _categories = new List<Category>(value);
        }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public async Task ChangeCategoriesAsync(
            IEnumerable<string> categoryNames,
            ICategoriesService categoryService,
            CancellationToken cancellationToken = default)
        {
            // [IntentFully]
            throw new NotImplementedException("Replace with your implementation...");
        }

        public void ChangeCategoriesSync(IEnumerable<string> categoryNames, ICategoriesService categoryService)
        {
            // [IntentFully]
            throw new NotImplementedException("Replace with your implementation...");
        }

        public async Task DoManualAsync(ICategoriesService catService, CancellationToken cancellationToken = default)
        {
            // [IntentFully]
            // TODO: Implement DoManualAsync (Product) functionality
            throw new NotImplementedException("Replace with your implementation...");
        }

        public void Another(ISecondService secondSevice)
        {
            // [IntentFully]
            // TODO: Implement Another (Product) functionality
            throw new NotImplementedException("Replace with your implementation...");
        }
    }
}