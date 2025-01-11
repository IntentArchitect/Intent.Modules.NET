using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace AspNetCore.Controllers.Secured.Application.IntegrationServices.Contracts.Services.Products
{
    public class CreateProductCommand
    {
        public CreateProductCommand()
        {
            Name = null!;
            Description = null!;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public static CreateProductCommand Create(string name, string description, decimal price)
        {
            return new CreateProductCommand
            {
                Name = name,
                Description = description,
                Price = price
            };
        }
    }
}