using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OpenApiImporterTest.Application.Players
{
    public class BetItemViewModel
    {
        public BetItemViewModel()
        {
        }

        public int EventId { get; set; }
        public int MarketId { get; set; }
        public int SelectionId { get; set; }
        public string? RacingSelection { get; set; }
        public decimal? Price { get; set; }

        public static BetItemViewModel Create(
            int eventId,
            int marketId,
            int selectionId,
            string? racingSelection,
            decimal? price)
        {
            return new BetItemViewModel
            {
                EventId = eventId,
                MarketId = marketId,
                SelectionId = selectionId,
                RacingSelection = racingSelection,
                Price = price
            };
        }
    }
}