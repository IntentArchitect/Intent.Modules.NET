using Intent.RoslynWeaver.Attributes;
using Wolverine.CQRS.TestApplication.Domain.Common.Exceptions;
using Wolverine.CQRS.TestApplication.Domain.Repositories.Items;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

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

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<ItemDto> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
        {
            var item = await _itemRepository.FindByIdAsync(request.Id, cancellationToken);

            if (item is null)
            {
                throw new NotFoundException($@"Could not find Item '{request.Id}'");
            }

            return new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
            };
        }
    }
}
