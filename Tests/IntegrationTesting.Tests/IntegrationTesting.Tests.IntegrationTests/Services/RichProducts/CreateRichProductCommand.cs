using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.RichProducts
{
    public class CreateRichProductCommand
    {
        public CreateRichProductCommand()
        {
            Name = null!;
        }

        public Guid BrandId { get; set; }
        public string Name { get; set; }

        public static CreateRichProductCommand Create(Guid brandId, string name)
        {
            return new CreateRichProductCommand
            {
                BrandId = brandId,
                Name = name
            };
        }
    }
}