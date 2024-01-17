using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Products
{
    public class ProductUpdateDto
    {
        public ProductUpdateDto()
        {
            Name = null!;
            Tags = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<UpdateProductTagDto> Tags { get; set; }

        public static ProductUpdateDto Create(Guid id, string name, List<UpdateProductTagDto> tags)
        {
            return new ProductUpdateDto
            {
                Id = id,
                Name = name,
                Tags = tags
            };
        }
    }
}