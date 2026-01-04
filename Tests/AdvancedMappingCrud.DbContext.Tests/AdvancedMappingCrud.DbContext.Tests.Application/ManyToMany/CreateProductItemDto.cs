using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.ManyToMany
{
    public class CreateProductItemDto
    {
        public CreateProductItemDto()
        {
            Name = null!;
            TagIds = null!;
            CategoryIds = null!;
        }

        public string Name { get; set; }
        public List<Guid> TagIds { get; set; }
        public List<Guid> CategoryIds { get; set; }

        public static CreateProductItemDto Create(string name, List<Guid> tagIds, List<Guid> categoryIds)
        {
            return new CreateProductItemDto
            {
                Name = name,
                TagIds = tagIds,
                CategoryIds = categoryIds
            };
        }
    }
}