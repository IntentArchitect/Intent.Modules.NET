using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders
{
    public class CreateOrderCommandOrderTagsDto
    {
        public CreateOrderCommandOrderTagsDto()
        {
            Name = null!;
            Value = null!;
        }

        public string Name { get; set; }
        public string Value { get; set; }

        public static CreateOrderCommandOrderTagsDto Create(string name, string value)
        {
            return new CreateOrderCommandOrderTagsDto
            {
                Name = name,
                Value = value
            };
        }
    }
}