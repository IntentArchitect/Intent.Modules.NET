using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.Controllers.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application
{
    public record OrderDto
    {
        public OrderDto()
        {
            OrderNumber = null!;
            CustomerName = null!;
        }

        public Guid Id { get; init; }
        public string OrderNumber { get; init; }
        public string CustomerName { get; init; }
        public OrderStatus Status { get; init; }
        public DateTime PlacedDate { get; init; }
        public string? Notes { get; init; }
    }
}