using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace TestMediator.NoEventing.Application.Products
{
    public record PriceHistoryEntryDto
    {
        public PriceHistoryEntryDto()
        {
        }

        public decimal OldPrice { get; init; }
        public decimal NewPrice { get; init; }
        public DateTimeOffset ChangedAt { get; init; }
    }
}