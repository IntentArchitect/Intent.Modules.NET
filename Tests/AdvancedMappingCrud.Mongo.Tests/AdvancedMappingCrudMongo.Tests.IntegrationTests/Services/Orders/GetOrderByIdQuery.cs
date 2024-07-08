using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Orders
{
    public class GetOrderByIdQuery
    {
        public GetOrderByIdQuery()
        {
            Id = null!;
        }

        public string Id { get; set; }

        public static GetOrderByIdQuery Create(string id)
        {
            return new GetOrderByIdQuery
            {
                Id = id
            };
        }
    }
}