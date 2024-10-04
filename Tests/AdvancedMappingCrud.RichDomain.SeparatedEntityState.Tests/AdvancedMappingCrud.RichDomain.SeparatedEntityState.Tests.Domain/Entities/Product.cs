using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Services;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public partial class Product
    {
        public Product(string name, IEnumerable<string> categoryNames, ICategoriesService categoryService)
        {
            Name = name;
        }

        public async Task ChangeCategoriesAsync(
            IEnumerable<string> categoryNames,
            ICategoriesService categoryService,
            CancellationToken cancellationToken = default)
        {
            // [IntentFully]
            // TODO: Implement ChangeCategoriesAsync (Product) functionality
            throw new NotImplementedException("Replace with your implementation...");
        }

        public void ChangeCategoriesSync(IEnumerable<string> categoryNames, ICategoriesService categoryService)
        {
            // [IntentFully]
            // TODO: Implement ChangeCategoriesSync (Product) functionality
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