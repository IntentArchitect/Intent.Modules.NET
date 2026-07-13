using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application
{
    public record OrderStatisticsDto
    {
        public OrderStatisticsDto()
        {
        }

        public int TotalOrders { get; init; }
        public int PendingOrders { get; init; }
        public decimal TotalRevenue { get; init; }
    }
}