using Mediator;
namespace ModB;
public interface IModBMessage : IMessage { }
public record PingModB : ICommand<string>, IModBMessage;
public class PingModBHandler : ICommandHandler<PingModB, string>
{
    public ValueTask<string> Handle(PingModB command, CancellationToken cancellationToken) => ValueTask.FromResult("pong from ModB");
}
public sealed record ModBHappened : INotification;
public class ModBHappenedHandler : INotificationHandler<ModBHappened>
{
    public ValueTask Handle(ModBHappened notification, CancellationToken cancellationToken) { Trace.Lines.Add("ModB notification handled"); return default; }
}
// A module's own behaviour (e.g. its ValidationBehaviour), constrained to its own messages.
public sealed class ModBBehaviour<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse> where TMessage : IModBMessage
{
    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
    {
        Trace.Lines.Add("ModB behaviour: " + typeof(TMessage).Name);
        return await next(message, cancellationToken);
    }
}
public static class Trace { public static List<string> Lines { get; } = []; }
