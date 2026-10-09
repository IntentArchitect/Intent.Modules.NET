using Mediator;
using Microsoft.Extensions.DependencyInjection;
namespace ModB;
public record PingModB : ICommand<string>;
public class PingModBHandler : ICommandHandler<PingModB, string> { public ValueTask<string> Handle(PingModB c, CancellationToken ct) => ValueTask.FromResult("pong from ModB"); }
public static class DI { public static IServiceCollection AddModB(this IServiceCollection s) { s.AddMediator((MediatorOptions o) => { o.ServiceLifetime = ServiceLifetime.Scoped; }); return s; } }
