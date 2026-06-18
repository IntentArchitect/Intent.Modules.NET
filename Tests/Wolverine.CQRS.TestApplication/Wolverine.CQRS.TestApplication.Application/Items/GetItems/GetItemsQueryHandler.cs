using AutoMapper;
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
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetItemsQueryHandler(IItemRepository itemRepository, IMapper mapper)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ItemDto>> Handle(GetItemsQuery query, CancellationToken cancellationToken)
        {
            var items = await _itemRepository.FindAllAsync(cancellationToken);
            return items.MapToItemDtoList(_mapper);
        }
    }
}