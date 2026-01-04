using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ManyToMany
{
    public class UpdateProductItemDto
    {
        public UpdateProductItemDto()
        {
            Name = null!;
            TagIds = null!;
            CategoryIds = null!;
        }

        public string Name { get; set; }
        public List<Guid> TagIds { get; set; }
        public List<Guid> CategoryIds { get; set; }

        public static UpdateProductItemDto Create(string name, List<Guid> tagIds, List<Guid> categoryIds)
        {
            return new UpdateProductItemDto
            {
                Name = name,
                TagIds = tagIds,
                CategoryIds = categoryIds
            };
        }
    }
}