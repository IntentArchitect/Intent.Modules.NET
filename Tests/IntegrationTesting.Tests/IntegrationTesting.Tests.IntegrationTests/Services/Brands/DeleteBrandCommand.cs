using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Brands
{
    public class DeleteBrandCommand
    {
        public Guid Id { get; set; }

        public static DeleteBrandCommand Create(Guid id)
        {
            return new DeleteBrandCommand
            {
                Id = id
            };
        }
    }
}