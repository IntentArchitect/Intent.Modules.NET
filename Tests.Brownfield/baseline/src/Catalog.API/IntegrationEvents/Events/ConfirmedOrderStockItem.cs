using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]
[assembly: IntentTemplate("Intent.Services.Guardrails.Contracts.EventingDtoContract", Version = "1.0")]

namespace eShop.Catalog.API.IntegrationEvents.Events;

public record ConfirmedOrderStockItem(int ProductId, bool HasStock);
