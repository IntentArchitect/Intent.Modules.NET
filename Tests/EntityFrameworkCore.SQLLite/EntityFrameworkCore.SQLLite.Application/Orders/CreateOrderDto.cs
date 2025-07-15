using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EntityFrameworkCore.SQLLite.Application.Orders
{
    public class CreateOrderDto
    {
        public CreateOrderDto()
        {
            RefNo = null!;
        }

        public string RefNo { get; set; }

        public static CreateOrderDto Create(string refNo)
        {
            return new CreateOrderDto
            {
                RefNo = refNo
            };
        }
    }
}