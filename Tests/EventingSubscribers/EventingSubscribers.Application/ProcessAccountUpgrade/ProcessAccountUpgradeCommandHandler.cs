using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace EventingSubscribers.Application.ProcessAccountUpgrade
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ProcessAccountUpgradeCommandHandler : IRequestHandler<ProcessAccountUpgradeCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ProcessAccountUpgradeCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(ProcessAccountUpgradeCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (ProcessAccountUpgradeCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}