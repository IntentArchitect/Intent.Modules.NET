using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders
{
    public class UpdateOrderCommandOrderTagsDto
    {
        public UpdateOrderCommandOrderTagsDto()
        {
            Name = null!;
            Value = null!;
        }

        public string Name { get; set; }
        public string Value { get; set; }

        public static UpdateOrderCommandOrderTagsDto Create(string name, string value)
        {
            return new UpdateOrderCommandOrderTagsDto
            {
                Name = name,
                Value = value
            };
        }
    }
}