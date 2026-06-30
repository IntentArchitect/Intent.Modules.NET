using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ObjectMappingTest.Application.Products
{
    public record DigitalProductDto
    {
        public DigitalProductDto()
        {
            Name = null!;
            DownloadUrl = null!;
        }

        public Guid Id { get; init; }
        public string Name { get; init; }
        public decimal Price { get; init; }
        public string DownloadUrl { get; init; }
    }
}