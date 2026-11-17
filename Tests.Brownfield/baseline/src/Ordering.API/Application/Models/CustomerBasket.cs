using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]
[assembly: IntentTemplate("Intent.Services.Guardrails.Contracts.DtoContract", Version = "1.0")]

namespace eShop.Ordering.API.Application.Models;

public class CustomerBasket
{
    public string BuyerId { get; set; }
    public List<BasketItem> Items { get; set; }

    public CustomerBasket(string buyerId, List<BasketItem> items)
    {
        BuyerId = buyerId;
        Items = items;
    }
}
