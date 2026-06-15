using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryHandler", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Application.Items
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetItemsQueryHandler
    {
        [IntentManaged(Mode.Merge)]
        public GetItemsQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<ItemDto>> Handle(GetItemsQuery query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}