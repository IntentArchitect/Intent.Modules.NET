using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace AspNetCore.Controllers.Secured.Application.IntegrationServices.Contracts.Services.Products
{
    public class UpdateProductCommand
    {
        public UpdateProductCommand()
        {
            Name = null!;
            Description = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public static UpdateProductCommand Create(Guid id, string name, string description, decimal price)
        {
            return new UpdateProductCommand
            {
                Id = id,
                Name = name,
                Description = description,
                Price = price
            };
        }
    }
}