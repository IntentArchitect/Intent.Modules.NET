using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Products
{
    public class ProductCreateDto
    {
        public ProductCreateDto()
        {
            Name = null!;
            CategoryNames = null!;
        }

        public string Name { get; set; }
        public List<string> CategoryNames { get; set; }

        public static ProductCreateDto Create(string name, List<string> categoryNames)
        {
            return new ProductCreateDto
            {
                Name = name,
                CategoryNames = categoryNames
            };
        }
    }
}