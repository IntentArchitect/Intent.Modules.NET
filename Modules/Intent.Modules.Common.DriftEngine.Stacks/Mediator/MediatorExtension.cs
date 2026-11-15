using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.Requests;

namespace Intent.Modules.Common.DriftEngine.Stacks.Mediator;

/// <summary>
/// Mediator's code (martinothamar/Mediator, namespace <c>Mediator</c>, source-generated) for the requests <c>intent.cqrs</c>
/// claims: a request's result is the <c>R</c> of the <c>IRequest&lt;R&gt;</c>, <c>ICommand&lt;R&gt;</c> or <c>IQuery&lt;R&gt;</c> it
/// implements (<c>IRequest</c>, <c>ICommand</c> and a <c>Unit</c> result return nothing), and its handler is the class that
/// implements that request's own handler interface for it, with that result: <c>IRequestHandler&lt;X, R&gt;</c>,
/// <c>ICommandHandler&lt;X, R&gt;</c> or <c>IQueryHandler&lt;X, R&gt;</c>, or <c>IRequestHandler&lt;X&gt;</c> and
/// <c>ICommandHandler&lt;X&gt;</c> for none. Its interfaces share MediatR's names, so it answers only for an interface the code
/// writes where it imports <c>Mediator</c> (or qualifies it so); the rest are MediatR's or another framework's. Streamed requests
/// (<c>IStreamRequest&lt;R&gt;</c> and the like) aren't read.
/// </summary>
public sealed class MediatorExtension : IDriftExtension
{
    public string Id => "intent.mediator";

    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new MarkerResultReader(MarkerFramework.Mediator), new MarkerHandlerReader(MarkerFramework.Mediator)];
}
