using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.RichProducts
{
    public class UpdateRichProductCommand
    {
        public UpdateRichProductCommand()
        {
            Name = null!;
        }

        public Guid BrandId { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }

        public static UpdateRichProductCommand Create(Guid brandId, string name, Guid id)
        {
            return new UpdateRichProductCommand
            {
                BrandId = brandId,
                Name = name,
                Id = id
            };
        }
    }
}