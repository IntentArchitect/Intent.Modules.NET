using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.Interfaces.EF.Application.Orders
{
    public class CreateOrderOrderItemDto
    {
        public CreateOrderOrderItemDto()
        {
            Description = null!;
        }

        public string Description { get; set; }

        public static CreateOrderOrderItemDto Create(string description)
        {
            return new CreateOrderOrderItemDto
            {
                Description = description
            };
        }
    }
}