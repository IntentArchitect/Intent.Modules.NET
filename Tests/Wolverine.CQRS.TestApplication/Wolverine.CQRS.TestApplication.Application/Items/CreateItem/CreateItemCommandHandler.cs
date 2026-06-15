using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandHandler", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Application.Items.CreateItem
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateItemCommandHandler
    {
        [IntentManaged(Mode.Merge)]
        public CreateItemCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> Handle(CreateItemCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}