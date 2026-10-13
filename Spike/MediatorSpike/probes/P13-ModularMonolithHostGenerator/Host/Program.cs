using Mediator;
using Microsoft.Extensions.DependencyInjection;

// One generator, in the host: the only project that references every module (as Intent's
// Modular Monolith host does). Each module keeps its own behaviour in the shared literal array.
var services = new ServiceCollection();
services.AddMediator((MediatorOptions options) =>
{
    options.ServiceLifetime = ServiceLifetime.Transient;
    options.Assemblies = [typeof(ModA.PingModA), typeof(ModB.PingModB)];
    options.PipelineBehaviors = [typeof(HostBehaviour<,>), typeof(ModA.ModABehaviour<,>), typeof(ModB.ModBBehaviour<,>)];
});
var sp = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
using var scope = sp.CreateScope();
var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
Console.WriteLine("ISender impl: " + mediator.GetType().FullName + " in " + mediator.GetType().Assembly.GetName().Name);
foreach (var message in new ICommand<string>[] { new ModA.PingModA(), new ModB.PingModB() })
{
    HostTrace.Lines.Clear(); ModA.Trace.Lines.Clear(); ModB.Trace.Lines.Clear();
    try
    {
        var result = await mediator.Send(message);
        Console.WriteLine($"{message.GetType().Name}: {result} | {string.Join(" > ", HostTrace.Lines.Concat(ModA.Trace.Lines).Concat(ModB.Trace.Lines))}");
    }
    catch (Exception e) { Console.WriteLine($"{message.GetType().Name} failed: {e.GetType().Name}: {e.Message}"); }
}
ModA.Trace.Lines.Clear(); ModB.Trace.Lines.Clear();
await mediator.Publish(new ModA.ModAHappened());
await mediator.Publish(new ModB.ModBHappened());
Console.WriteLine("notifications: " + string.Join(", ", ModA.Trace.Lines.Concat(ModB.Trace.Lines)));

public sealed class HostBehaviour<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse> where TMessage : IMessage
{
    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
    {
        HostTrace.Lines.Add("host behaviour");
        return await next(message, cancellationToken);
    }
}
public static class HostTrace { public static List<string> Lines { get; } = []; }
