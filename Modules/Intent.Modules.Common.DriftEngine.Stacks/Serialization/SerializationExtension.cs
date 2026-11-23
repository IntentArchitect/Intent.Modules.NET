using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.CSharp;
using Intent.Modules.Common.DriftEngine.Stacks.Java;
using Intent.Modules.Common.DriftEngine.Stacks.Python;

namespace Intent.Modules.Common.DriftEngine.Stacks.Serialization;

/// <summary>
/// The name a member is serialized as (<c>member.wireName</c>), as each language's serializers state it on the member: C#'s
/// System.Text.Json, Newtonsoft and data contract attributes, Java's Jackson and Gson annotations, Python's Pydantic,
/// marshmallow and msgspec fields. Each reader answers only for its own language's members. What a serializer's naming
/// policy would make of a member's name is never read: it isn't stated on the member (decision 4).
/// </summary>
public sealed class SerializationExtension : IDriftExtension
{
    public string Id => "intent.serialization";

    public IEnumerable<IClaimReader> ClaimReaders => [new CSharpWireName(), new JavaWireName(), new PythonWireName()];
}
