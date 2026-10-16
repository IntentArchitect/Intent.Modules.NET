using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandHandler", Version = "1.0")]

namespace TestMediator.WolverineSubscribeOnly.Application.Orders.RecordOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class RecordOrderCommandHandler : ICommandHandler<RecordOrderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public RecordOrderCommandHandler()
        {
        }

        // Test hook: the app has no persistence, so the handler records what it was asked to do for the tests to observe.
        public static readonly System.Collections.Concurrent.ConcurrentQueue<(Guid OrderId, string CustomerName)> Recorded = new();

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async ValueTask<Unit> Handle(RecordOrderCommand request, CancellationToken cancellationToken)
        {
            Recorded.Enqueue((request.OrderId, request.CustomerName));
            await Task.CompletedTask;
            return Unit.Value;
        }
    }
}