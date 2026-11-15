using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;

namespace Intent.Modules.Common.DriftEngine.Stacks.Cqrs;

/// <summary>
/// What the Services designer's Commands and Queries state, whatever framework dispatches them: each is a request with one
/// handler (<c>request.handler</c>) and the result its type gives, or none (<c>request.returns</c>). It reads no code; a request
/// framework's extension (<c>intent.mediatr</c>, <c>intent.mediator</c>, <c>intent.wolverine</c>) reads these claims from its own
/// code, as <c>intent.http</c>'s are read by ASP.NET
/// Core's or Spring MVC's. It also defines the request kinds a house states with its own stereotypes, which no stock designer
/// has: a request's validator (<c>request.validated</c>), an idempotent command (<c>command.identified</c>) and how an
/// invocation wraps what it sends (<c>invocation.wrapped</c>).
/// </summary>
public sealed class CqrsExtension : IDriftExtension
{
    public string Id => "intent.cqrs";

    public IEnumerable<KindDefinition> Kinds { get; } =
    [
        new("request.returns", "request result", ValueType: "type", Comparer: "type"),
        new("request.handler", "handler", ValueType: "presence", Comparer: "presence", Existence: true),
        new("request.validated", "validator", ValueType: "presence", Comparer: "presence", Existence: true),
        new("command.identified", "identified command", ValueType: "bool"),
        new("invocation.wrapped", "wrapped invocation", Stated: "whenSet")
    ];

    public IEnumerable<IClaimSource> ClaimSources { get; } = [new Requests()];
}

/// <summary>
/// Each Command and Query is a request: it has a handler, and returns its type, or nothing when it has none. Both wait on the
/// request's own type, which the Services vocabulary locates, and the handler on the result.
/// </summary>
internal sealed class Requests : IClaimSource
{
    public const string Designer = "Services";

    public IEnumerable<ClaimSpec> Claims(ModelContext model)
    {
        if (!model.Designers.Contains(Designer))
        {
            yield break;
        }

        foreach (var request in model.Elements(Designer).Where(x => x.SpecializationType is "Command" or "Query"))
        {
            var what = request.SpecializationType.ToLowerInvariant();
            var exists = new ClaimRef(Designer, request.Id, "type.exists");
            var result = request.TypeReference is { Element: not null } type ? type : null;
            yield return new ClaimSpec
            {
                Designer = Designer,
                Element = request,
                Kind = "request.returns",
                // A request with no type returns nothing, which the code states as MediatR's IRequest or IRequest<Unit>, Mediator's
                // ICommand, or a Wolverine handler that returns no value.
                Expected = result is null ? "none" : Text(result),
                Value = result,
                DependsOn = exists,
                Source = $"the {what}'s type"
            };
            yield return new ClaimSpec
            {
                Designer = Designer,
                Element = request,
                Kind = "request.handler",
                Expected = "present",
                DependsOn = exists,
                // A handler is looked for by the result the request's code gives, so a request whose result is wrong is that
                // one finding, not a second for the handler that still handles the result it had.
                WaitsOn = [new ClaimRef(Designer, request.Id, "request.returns")],
                Source = $"the {what}"
            };
        }
    }

    /// <summary>A type reference as the vocabulary writes one: <c>bool?</c>, <c>OrderItem[]</c>.</summary>
    private static string Text(ITypeReference type) => $"{type.Element.Name}{(type.IsCollection ? "[]" : "")}{(type.IsNullable ? "?" : "")}";
}
