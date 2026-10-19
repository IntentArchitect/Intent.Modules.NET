using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.QueryModels", Version = "1.0")]

namespace TestMediator.NoEventing.Application.Products.GetProductPriceHistory
{
    public class GetProductPriceHistoryQuery : IQuery<List<PriceHistoryEntryDto>>
    {
        public GetProductPriceHistoryQuery(Guid productId)
        {
            ProductId = productId;
        }

        public Guid ProductId { get; set; }
    }
}