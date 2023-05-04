using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Application.Products
{
    public class ProductCreateDto
    {
        public ProductCreateDto()
        {
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public static ProductCreateDto Create(string name, string description, bool isActive)
        {
            return new ProductCreateDto
            {
                Name = name,
                Description = description,
                IsActive = isActive
            };
        }
    }
}