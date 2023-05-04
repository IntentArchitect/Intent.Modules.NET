using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Application.Products
{
    public class ProductUpdateDto
    {
        public ProductUpdateDto()
        {
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public static ProductUpdateDto Create(Guid id, string name, string description, bool isActive)
        {
            return new ProductUpdateDto
            {
                Id = id,
                Name = name,
                Description = description,
                IsActive = isActive
            };
        }
    }
}