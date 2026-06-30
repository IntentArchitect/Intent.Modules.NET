using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandHandler", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application.New
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class NewCommandHandler
    {
        [IntentManaged(Mode.Merge)]
        public NewCommandHandler()
        {
        }

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public async Task Handle(NewCommand command, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (NewCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}