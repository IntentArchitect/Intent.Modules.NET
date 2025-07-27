using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Products
{
    public class ProductPatchDto
    {
        public ProductPatchDto()
        {
        }

        public Guid Id { get; set; }
        public string? Name { get; set; }
        public List<PatchProductTagDto>? Tags { get; set; }

        public static ProductPatchDto Create(Guid id, string? name, List<PatchProductTagDto>? tags)
        {
            return new ProductPatchDto
            {
                Id = id,
                Name = name,
                Tags = tags
            };
        }
    }
}