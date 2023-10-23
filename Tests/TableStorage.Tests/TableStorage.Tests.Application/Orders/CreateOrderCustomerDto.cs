using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace TableStorage.Tests.Application.Orders
{
    public class CreateOrderCustomerDto
    {
        public CreateOrderCustomerDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateOrderCustomerDto Create(string name)
        {
            return new CreateOrderCustomerDto
            {
                Name = name
            };
        }
    }
}