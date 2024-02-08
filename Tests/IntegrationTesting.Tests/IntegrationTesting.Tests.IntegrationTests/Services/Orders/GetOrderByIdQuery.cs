using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Orders
{
    public class GetOrderByIdQuery
    {
        public Guid Id { get; set; }

        public static GetOrderByIdQuery Create(Guid id)
        {
            return new GetOrderByIdQuery
            {
                Id = id
            };
        }
    }
}