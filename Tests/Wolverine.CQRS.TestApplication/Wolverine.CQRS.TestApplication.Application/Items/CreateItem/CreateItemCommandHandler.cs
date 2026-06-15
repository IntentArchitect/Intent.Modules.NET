using Intent.RoslynWeaver.Attributes;
using Wolverine.CQRS.TestApplication.Domain.Entities.Items;
using Wolverine.CQRS.TestApplication.Domain.Repositories.Items;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Wolverine.CQRS.TestApplication.Application.Items.CreateItem
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateItemCommandHandler
    {
        private readonly IItemRepository _itemRepository;

        [IntentManaged(Mode.Merge)]
        public CreateItemCommandHandler(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<Guid> Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            var item = new Item
            {
                Name = request.Name,
            };

            _itemRepository.Add(item);
            await _itemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return item.Id;
        }
    }
}
