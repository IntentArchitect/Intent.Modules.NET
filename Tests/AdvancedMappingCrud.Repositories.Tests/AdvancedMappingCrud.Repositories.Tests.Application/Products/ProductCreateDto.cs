using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Products
{
    public class ProductCreateDto
    {
        public ProductCreateDto()
        {
            Name = null!;
            Tags = null!;
        }

        public string Name { get; set; }
        public List<CreateProductTagDto> Tags { get; set; }

        public static ProductCreateDto Create(string name, List<CreateProductTagDto> tags)
        {
            return new ProductCreateDto
            {
                Name = name,
                Tags = tags
            };
        }
    }
}