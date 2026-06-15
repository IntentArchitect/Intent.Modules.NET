using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryHandler", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Application.Items
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetItemByIdQueryHandler
    {
        [IntentManaged(Mode.Merge)]
        public GetItemByIdQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<ItemDto> Handle(GetItemByIdQuery query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}