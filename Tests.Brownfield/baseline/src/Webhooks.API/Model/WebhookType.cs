using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]
[assembly: IntentTemplate("Intent.Domain.Guardrails.Contracts.EnumContract", Version = "1.0")]

namespace Webhooks.API.Model;

public enum WebhookType
{
    CatalogItemPriceChange = 1,
    OrderShipped = 2,
    OrderPaid = 3
}
