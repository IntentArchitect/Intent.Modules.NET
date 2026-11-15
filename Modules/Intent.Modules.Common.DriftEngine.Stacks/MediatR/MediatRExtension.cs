using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.Requests;

namespace Intent.Modules.Common.DriftEngine.Stacks.MediatR;

/// <summary>
/// MediatR's code for the requests <c>intent.cqrs</c> claims: a request's result is the <c>R</c> of the <c>IRequest&lt;R&gt;</c> it
/// implements (<c>IRequest</c> and <c>IRequest&lt;Unit&gt;</c> return nothing), and its handler is the class that implements
/// <c>IRequestHandler&lt;X, R&gt;</c> for it, with that result. Both are followed through the code's own bases, so a house's
/// <c>ICommand&lt;T&gt; : IRequest&lt;T&gt;</c> or <c>CommandHandler&lt;T&gt;</c> is read as what it derives. It answers only for a
/// request type that implements MediatR's <c>IRequest</c>: one written where the code imports Mediator's namespace, not MediatR's,
/// is Mediator's (<c>intent.mediator</c>), and any other is left to another framework's extension, or can't be verified.
/// </summary>
public sealed class MediatRExtension : IDriftExtension
{
    public string Id => "intent.mediatr";

    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new MarkerResultReader(MarkerFramework.MediatR), new MarkerHandlerReader(MarkerFramework.MediatR)];
}
