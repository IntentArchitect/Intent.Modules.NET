using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace WebAndWorker.Application.OnTest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OnTestCommandHandler : IRequestHandler<OnTestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public OnTestCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(OnTestCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"{nameof(OnTestCommandHandler)}: {DateTime.Now:u}");
        }
    }
}