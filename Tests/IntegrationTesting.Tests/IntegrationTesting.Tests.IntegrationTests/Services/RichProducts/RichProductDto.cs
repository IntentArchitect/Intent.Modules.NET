using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.RichProducts
{
    public class RichProductDto
    {
        public RichProductDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public string Name { get; set; }

        public static RichProductDto Create(Guid id, Guid brandId, string name)
        {
            return new RichProductDto
            {
                Id = id,
                BrandId = brandId,
                Name = name
            };
        }
    }
}