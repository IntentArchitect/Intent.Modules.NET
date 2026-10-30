using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.MessageBusFlushOwner", Version = "1.0")]

namespace TestMediator.Outbox.Application.Common.Eventing
{
    /// <summary>
    /// Decides who flushes the scoped <see cref="IMessageBus"/> buffer. Dispatches nest: a
    /// domain-event handler or an integration consumer sends further commands into the same DI
    /// scope, and they share one buffer. Only the outermost owner may flush, once its unit of work
    /// has committed. A nested dispatch that flushed would send the outer work's messages before
    /// the outer commit.
    /// </summary>
    public sealed class MessageBusFlushOwner
    {
        private int _depth;

        /// <summary>Returns true when the caller is the outermost owner and must flush.</summary>
        public bool Enter() => Interlocked.Increment(ref _depth) == 1;

        public void Exit() => Interlocked.Decrement(ref _depth);
    }
}