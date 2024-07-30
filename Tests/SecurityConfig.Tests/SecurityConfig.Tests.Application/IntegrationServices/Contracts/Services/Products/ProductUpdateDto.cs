using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace SecurityConfig.Tests.Application.IntegrationServices.Contracts.Services.Products
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