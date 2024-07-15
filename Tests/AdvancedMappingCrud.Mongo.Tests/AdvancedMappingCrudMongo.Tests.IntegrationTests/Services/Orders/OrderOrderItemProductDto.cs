using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Orders
{
    public class OrderOrderItemProductDto
    {
        public OrderOrderItemProductDto()
        {
            Name = null!;
            Description = null!;
            Id = null!;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }

        public static OrderOrderItemProductDto Create(string name, string description, string id)
        {
            return new OrderOrderItemProductDto
            {
                Name = name,
                Description = description,
                Id = id
            };
        }
    }
}