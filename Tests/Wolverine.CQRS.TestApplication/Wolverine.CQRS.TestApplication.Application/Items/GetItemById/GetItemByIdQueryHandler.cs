using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Wolverine.CQRS.TestApplication.Application.Items;
using Wolverine.CQRS.TestApplication.Domain.Common.Exceptions;
using Wolverine.CQRS.TestApplication.Domain.Repositories.Items;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryHandler", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Application.Items.GetItemById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetItemByIdQueryHandler
    {
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetItemByIdQueryHandler(IItemRepository itemRepository, IMapper mapper)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
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