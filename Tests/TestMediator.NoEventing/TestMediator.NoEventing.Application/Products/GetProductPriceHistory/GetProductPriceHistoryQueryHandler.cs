using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.NoEventing.Domain.Repositories.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.QueryHandler", Version = "1.0")]

namespace TestMediator.NoEventing.Application.Products.GetProductPriceHistory
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetProductPriceHistoryQueryHandler : IQueryHandler<GetProductPriceHistoryQuery, List<PriceHistoryEntryDto>>
    {
        private readonly IPriceHistoryEntryRepository _priceHistoryEntryRepository;

        [IntentManaged(Mode.Merge)]
        public GetProductPriceHistoryQueryHandler(IPriceHistoryEntryRepository priceHistoryEntryRepository)
        {
            _priceHistoryEntryRepository = priceHistoryEntryRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async ValueTask<List<PriceHistoryEntryDto>> Handle(
            GetProductPriceHistoryQuery request,
            CancellationToken cancellationToken)
        {
            return await _priceHistoryEntryRepository.FindAllProjectToAsync<PriceHistoryEntryDto>(
                x => x.ProductId == request.ProductId,
                query => query.OrderBy(x => x.ChangedAt),
                cancellationToken);
        }
    }
}
