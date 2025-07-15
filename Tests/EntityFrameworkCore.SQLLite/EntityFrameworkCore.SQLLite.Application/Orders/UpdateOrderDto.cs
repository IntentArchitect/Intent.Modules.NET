using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EntityFrameworkCore.SQLLite.Application.Orders
{
    public class UpdateOrderDto
    {
        public UpdateOrderDto()
        {
            RefNo = null!;
        }

        public string RefNo { get; set; }

        public static UpdateOrderDto Create(string refNo)
        {
            return new UpdateOrderDto
            {
                RefNo = refNo
            };
        }
    }
}