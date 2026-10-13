using Mediator;
using Microsoft.Extensions.DependencyInjection;
namespace ModA;
public record PingModA : ICommand<string>;
public class PingModAHandler : ICommandHandler<PingModA, string> { public ValueTask<string> Handle(PingModA c, CancellationToken ct) => ValueTask.FromResult("pong from ModA"); }
public static class DI { public static IServiceCollection AddModA(this IServiceCollection s) { s.AddMediator((MediatorOptions o) => { o.ServiceLifetime = ServiceLifetime.Scoped; }); return s; } }
