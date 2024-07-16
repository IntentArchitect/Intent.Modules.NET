using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace SecurityConfig.Tests.Application.Products
{
    public class ProductUpdateDto
    {
        public ProductUpdateDto()
        {
            Name = null!;
            Surname = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public static ProductUpdateDto Create(Guid id, string name, string surname)
        {
            return new ProductUpdateDto
            {
                Id = id,
                Name = name,
                Surname = surname
            };
        }
    }
}