using Mediator;
namespace ModA;
public interface IModAMessage : IMessage { }
public record PingModA : ICommand<string>, IModAMessage;
public class PingModAHandler : ICommandHandler<PingModA, string>
{
    public ValueTask<string> Handle(PingModA command, CancellationToken cancellationToken) => ValueTask.FromResult("pong from ModA");
}
public sealed record ModAHappened : INotification;
public class ModAHappenedHandler : INotificationHandler<ModAHappened>
{
    public ValueTask Handle(ModAHappened notification, CancellationToken cancellationToken) { Trace.Lines.Add("ModA notification handled"); return default; }
}
// A module's own behaviour (e.g. its ValidationBehaviour), constrained to its own messages.
public sealed class ModABehaviour<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse> where TMessage : IModAMessage
{
    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
    {
        Trace.Lines.Add("ModA behaviour: " + typeof(TMessage).Name);
        return await next(message, cancellationToken);
    }
}
public static class Trace { public static List<string> Lines { get; } = []; }
