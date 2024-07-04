using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Products
{
    public class GetProductByIdQuery
    {
        public GetProductByIdQuery()
        {
            Id = null!;
        }

        public string Id { get; set; }

        public static GetProductByIdQuery Create(string id)
        {
            return new GetProductByIdQuery
            {
                Id = id
            };
        }
    }
}