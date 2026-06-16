using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Wolverine.CQRS.TestApplication.Application.Items;
using Wolverine.CQRS.TestApplication.Domain.Repositories.Items;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryHandler", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Application.Items.GetItemById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetItemByIdQueryHandler
    {
        private readonly IItemRepository _itemRepository;

        [IntentManaged(Mode.Merge)]
        public GetItemByIdQueryHandler(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<ItemDto> Handle(GetItemByIdQuery query, CancellationToken cancellationToken)
        {
            var item = await _itemRepository.FindByIdProjectToAsync<ItemDto>(query.Id, cancellationToken);
            if (item == null)
            {
                throw new InvalidOperationException($"Could not find Item '{query.Id}'");
            }
            return item;
        }
    }
}