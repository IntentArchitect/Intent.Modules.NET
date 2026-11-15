using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Stacks.Protobuf;

/// <summary>
/// gRPC's conventions over the Protobuf language: a DTO with the Proto Message stereotype is the <c>.proto</c> message of
/// its name, even where a C# type has the same name, as a service's own model of a basket item often does.
/// </summary>
public sealed class GrpcExtension : IDriftExtension
{
    public string Id => "intent.grpc";

    public IEnumerable<IClaimReader> ClaimReaders => [new ProtoMessageReader()];

    // The Proto Message stereotype's presence says which code a DTO's existence claim reads.
    public IEnumerable<ModelFeature> Reads => [new("Services", Stereotype: "Proto Message", Property: "")];
}

internal sealed class ProtoMessageReader : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["type.exists"];

    public ClaimReading? Read(ClaimContext context) =>
        context.Model.HasStereotype(context.Element, "Proto Message")
        && context.Code.Types.Where(x => x.Kind == "message" && x.Name == context.Element.Name).ToList() is [var message]
            ? ClaimReading.Found(message)
            : null;
}
