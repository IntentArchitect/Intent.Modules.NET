using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace HttpQuery.Client.Application.IntegrationServices.Contracts.Controllers.Swashbuckle.Services.Products
{
    public class CreateProductCommand
    {
        public CreateProductCommand()
        {
            Name = null!;
            Category = null!;
        }

        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }

        public static CreateProductCommand Create(string name, string category, decimal price)
        {
            return new CreateProductCommand
            {
                Name = name,
                Category = category,
                Price = price
            };
        }
    }
}