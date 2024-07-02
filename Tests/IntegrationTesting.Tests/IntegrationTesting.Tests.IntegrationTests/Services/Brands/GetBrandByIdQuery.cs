using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Brands
{
    public class GetBrandByIdQuery
    {
        public Guid Id { get; set; }

        public static GetBrandByIdQuery Create(Guid id)
        {
            return new GetBrandByIdQuery
            {
                Id = id
            };
        }
    }
}