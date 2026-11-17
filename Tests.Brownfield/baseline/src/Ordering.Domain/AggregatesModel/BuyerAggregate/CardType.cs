using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]
[assembly: IntentTemplate("Intent.Domain.Guardrails.Contracts.EntityContract", Version = "1.0")]

namespace eShop.Ordering.Domain.AggregatesModel.BuyerAggregate;

public sealed class CardType
{
    public int Id { get; init; }
    public required string Name { get; init; }
}
