using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace MudBlazor.Sample.Client.HttpClients.Contracts.Services.Products
{
    public class UpdateProductCommand
    {
        public UpdateProductCommand()
        {
            Name = null!;
            Description = null!;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public Guid Id { get; set; }

        public static UpdateProductCommand Create(string name, string description, decimal price, string? imageUrl, Guid id)
        {
            return new UpdateProductCommand
            {
                Name = name,
                Description = description,
                Price = price,
                ImageUrl = imageUrl,
                Id = id
            };
        }
    }
}