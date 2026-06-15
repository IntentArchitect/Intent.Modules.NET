using Intent.RoslynWeaver.Attributes;
using Wolverine.CQRS.TestApplication.Domain.Repositories.Items;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Application.Items.GetItems
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetItemsQueryHandler
    {
        private readonly IItemRepository _itemRepository;

        [IntentManaged(Mode.Merge)]
        public GetItemsQueryHandler(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<ItemDto>> Handle(GetItemsQuery request, CancellationToken cancellationToken)
        {
            return await _itemRepository.FindAllProjectToAsync<ItemDto>(cancellationToken);
        }
    }
}
