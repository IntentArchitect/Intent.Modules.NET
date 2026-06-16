using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Wolverine.CQRS.TestApplication.Application.Items;
using Wolverine.CQRS.TestApplication.Domain.Repositories.Items;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryHandler", Version = "1.0")]

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

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<ItemDto>> Handle(GetItemsQuery query, CancellationToken cancellationToken)
        {
            var items = await _itemRepository.FindAllProjectToAsync<ItemDto>(cancellationToken);
            return items;
        }
    }
}