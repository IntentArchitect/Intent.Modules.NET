using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Orders
{
    public class OrderOrderTagsDto
    {
        public OrderOrderTagsDto()
        {
            Name = null!;
            Value = null!;
        }

        public string Name { get; set; }
        public string Value { get; set; }
        public Guid Id { get; set; }

        public static OrderOrderTagsDto Create(string name, string value, Guid id)
        {
            return new OrderOrderTagsDto
            {
                Name = name,
                Value = value,
                Id = id
            };
        }
    }
}