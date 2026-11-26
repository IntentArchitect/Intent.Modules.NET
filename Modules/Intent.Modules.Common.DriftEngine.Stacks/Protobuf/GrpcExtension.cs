using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;

namespace Intent.Modules.Common.DriftEngine.Stacks.Protobuf;

/// <summary>
/// gRPC's conventions over the Protobuf language: a DTO with the Proto Message stereotype is the <c>.proto</c> message of
/// its name, even where a C# type has the same name, as a service's own model of a basket item often does; and a Service
/// whose class derives from the base protoc generates for a <c>.proto</c> <c>service</c> implements that service, so its
/// Operations are the service's rpcs (<c>grpc.service.rpcs</c>).
/// </summary>
public sealed class GrpcExtension : IDriftExtension
{
    public string Id => "intent.grpc";

    public IEnumerable<KindDefinition> Kinds => [new(GrpcService.Kind, "proto service rpcs")];

    public IEnumerable<IClaimSource> ClaimSources => [new GrpcServiceSource()];

    public IEnumerable<IClaimReader> ClaimReaders => [new ProtoMessageReader(), new GrpcServiceReader()];

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

/// <summary>A Service implementing a <c>.proto</c> service: its Operations are the service's rpcs, as signatures.</summary>
internal static class GrpcService
{
    public const string Kind = "grpc.service.rpcs";

    public const string Designer = "Services";

    /// <summary>A Service's Operations.</summary>
    public static IEnumerable<IElement> Operations(IElement service) => service.ChildElements.Where(x => x.SpecializationType == "Operation");

    /// <summary>The types an Operation takes, as the model names them.</summary>
    public static IReadOnlyList<string> Requests(IElement operation) =>
        operation.ChildElements.Where(x => x.SpecializationType == "Parameter").Select(x => Shown(x.TypeReference)).ToList();

    /// <summary>The type an Operation returns, as the model names it: <c>void</c> for none.</summary>
    public static string Response(IElement operation) => operation.TypeReference?.Element is null ? "void" : Shown(operation.TypeReference);

    /// <summary>A Service's Operations as sorted <c>Name(Request): Response</c> signatures, joined by <c>; </c>.</summary>
    public static string Signatures(IElement service) =>
        Join(Operations(service).Select(x => Signature(x.Name, Requests(x), Response(x))));

    /// <summary>A proto service's rpcs as sorted <c>Name(Request): Response</c> signatures, joined by <c>; </c>.</summary>
    public static string Signatures(IEnumerable<MemberNode> rpcs) =>
        Join(rpcs.Select(x => Signature(x.Name, x.Parameters.Select(p => Shown(p.Type)).ToList(), Shown(x.Type))));

    /// <summary>An rpc's request or response as the <c>.proto</c> writes it: a streamed side is <c>stream</c> of its message.</summary>
    public static string Shown(TypeRefNode? type) => type switch
    {
        null => "?",
        { Name: "stream", Arguments: [var message] } => $"stream {message.Name}",
        _ => type.Name
    };

    /// <summary>
    /// The model's type is the rpc's message: by name, as written or qualified (<c>google.protobuf.Empty</c>). A streamed side
    /// is its message, which the model may state as one or as a collection of them.
    /// </summary>
    public static bool Matches(ITypeReference? model, TypeRefNode? rpc)
    {
        if (model?.Element is not { } element || rpc is null)
        {
            return false;
        }

        var streamed = rpc is { Name: "stream", Arguments: [_] };
        var message = streamed ? rpc.Arguments[0] : rpc;
        return (streamed || !model.IsCollection)
               && (element.Name == message.Name || element.Name == message.QualifiedName.TrimStart('.'));
    }

    private static string Shown(ITypeReference? type) => type?.Element is { } element ? element.Name + (type.IsCollection ? "[]" : "") : "?";

    private static string Signature(string name, IReadOnlyList<string> requests, string response) => $"{name}({string.Join(", ", requests)}): {response}";

    private static string Join(IEnumerable<string> signatures) => string.Join("; ", signatures.Order(StringComparer.Ordinal));
}

/// <summary>
/// One <c>grpc.service.rpcs</c> claim per Service in the Services designer: the <c>.proto</c> service its class implements
/// has the Service's Operations as its rpcs. Which Services implement one is the code's to say, so every Service is claimed
/// and one whose class derives from no generated gRPC base states nothing (<c>blocked</c>, with no finding). It waits on the
/// Service's <c>type.exists</c>, which locates the class.
/// </summary>
internal sealed class GrpcServiceSource : IClaimSource
{
    public IEnumerable<ClaimSpec> Claims(ModelContext model)
    {
        if (!model.Designers.Contains(GrpcService.Designer))
        {
            yield break;
        }

        foreach (var service in model.Elements(GrpcService.Designer, "Service"))
        {
            yield return new ClaimSpec
            {
                Designer = GrpcService.Designer,
                Element = service,
                Kind = GrpcService.Kind,
                Expected = GrpcService.Signatures(service),
                DependsOn = new ClaimRef(GrpcService.Designer, service.Id, "type.exists"),
                Source = "its Operations"
            };
        }
    }
}

/// <summary>
/// Reads the <c>.proto</c> service a Service's class implements: the class, or an in-scope class it derives from, derives from
/// the base protoc generates for <c>service X</c> (<c>X.XBase</c>, the nested <c>XBase</c> of the static class <c>X</c>), and an
/// in-scope <c>.proto</c> file declares that service. Its rpcs must be the Service's Operations, request and response
/// included. The reading is the proto service's declaration, so it also places the service, its rpcs and its file in the
/// model, which the opt-in rules otherwise report as unmodelled. Of several <c>.proto</c> services of the name (a client's
/// copy of the file beside the server's), the one in the C# namespace protoc generates the base into, as the code names it,
/// is it.
/// </summary>
internal sealed class GrpcServiceReader : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = [GrpcService.Kind];

    public ClaimReading? Read(ClaimContext context)
    {
        if (context.Type is not { } type)
        {
            return ClaimReading.CantVerify("blocked", $"Waits on `{context.Element.Name}`'s type.exists");
        }

        var bases = GeneratedBases(context.Code, type).ToList();
        if (bases.Count == 0)
        {
            return ClaimReading.CantVerify("blocked", $"`{type.Name}` derives from no base protoc generates for a .proto service, so it implements none.");
        }

        var syntax = ProtobufSyntax.Of(context.Code);
        var implemented = bases.SelectMany(x => Implemented(context.Code, syntax, x)).Distinct().ToList();
        if (implemented.Count == 0)
        {
            return ClaimReading.CantVerify("blocked", $"`{type.Name}` derives from `{bases[0].Written}`, but no in-scope .proto file declares `service {bases[0].Service}`.");
        }

        if (implemented.Count > 1)
        {
            return ClaimReading.CantVerify("ambiguous", $"`{type.Name}`'s gRPC base names {implemented.Count} .proto services, so which one it implements can't be told.",
                implemented[0].Service.At, implemented.Select(x => x.Service.At).ToList());
        }

        var (proto, via) = implemented[0];
        var rpcs = proto.Members.Where(x => x.Kind == "method").ToList();
        var differences = Differences(context.Element, proto, rpcs).ToList();
        if (differences.Count == 0)
        {
            return ClaimReading.Compared(context.Expected, proto.At, same: true).Through(rpcs.Select(x => x.At));
        }

        var message = $"`{context.Element.Name}` implements the proto service `{proto.Name}` ({proto.At.File}) through `{via.Written}`, but {string.Join("; ", differences)}. "
                      + $"Make the proto service's rpcs the Service's Operations, or model its rpcs as Operations on `{context.Element.Name}`.";
        return ClaimReading.Compared(GrpcService.Signatures(rpcs), proto.At, same: false, message).Through(rpcs.Select(x => x.At));
    }

    /// <summary>A base protoc generates for a proto service, as a class's code names it.</summary>
    /// <param name="Service">The proto service's name: <c>Basket</c> for <c>Basket.BasketBase</c>.</param>
    /// <param name="Qualifier">The namespace written before it, through an alias: <c>Shop.Grpc</c> for <c>Shop.Grpc.Basket.BasketBase</c>.</param>
    /// <param name="Written">The base as written.</param>
    /// <param name="From">The type whose declaration names it: the class, or an in-scope class it derives from.</param>
    private sealed record GeneratedBase(string Service, string Qualifier, string Written, TypeNode From);

    /// <summary>The generated gRPC bases (<c>X.XBase</c>) the class derives from, itself or through the code's own classes.</summary>
    private static IEnumerable<GeneratedBase> GeneratedBases(CodeGraph code, TypeNode type)
    {
        foreach (var current in new[] { type }.Concat(code.Bases(type)))
        {
            var files = code.Files.Where(x => current.Files.Contains(x.Path)).ToList();
            foreach (var reference in current.BaseTypes)
            {
                var segments = reference.QualifiedName.Split('.');
                if (segments.Length < 2 || segments[^2].Length == 0 || segments[^1] != segments[^2] + "Base")
                {
                    continue;
                }

                // An alias names what it stands for: `using Grpc = Shop.Grpc;` makes `Grpc.Basket.BasketBase` Shop.Grpc's.
                if (files.SelectMany(x => x.Imports).FirstOrDefault(x => x.Alias == segments[0]) is { } alias)
                {
                    segments = [.. alias.Target.Split('.'), .. segments[1..]];
                }

                yield return new GeneratedBase(segments[^2], string.Join(".", segments[..^2]), reference.Text, current);
            }
        }
    }

    /// <summary>
    /// The in-scope proto services a generated base can be: those of its name, narrowed, when there are several, to the ones
    /// generated into the namespace the code names it in (written before it, or one the declaring file reaches).
    /// </summary>
    private static IEnumerable<(TypeNode Service, GeneratedBase Base)> Implemented(CodeGraph code, ProtobufSyntax? syntax, GeneratedBase generated)
    {
        if (syntax is null)
        {
            return [];
        }

        var services = code.TypesNamed(generated.Service).Where(x => x.Kind == "service" && syntax.Files.ContainsKey(x.At.File)).ToList();
        if (services.Count > 1)
        {
            var files = code.Files.Where(x => generated.From.Files.Contains(x.Path)).ToList();
            bool Named(TypeNode service) => CSharpNamespace(syntax.Files[service.At.File]) is var ns
                                            && (generated.Qualifier.Length > 0
                                                ? ns == generated.Qualifier || ns.EndsWith("." + generated.Qualifier, StringComparison.Ordinal)
                                                : files.Any(x => code.Reaches(x, ns)));
            if (services.Where(Named).ToList() is { Count: > 0 } named)
            {
                services = named;
            }
        }

        return services.Select(x => (x, generated));
    }

    /// <summary>
    /// The C# namespace protoc generates a file's types into: its <c>csharp_namespace</c> option, else its package with each
    /// part in Pascal case and underscores dropped (<c>shop.basket_api</c> is <c>Shop.BasketApi</c>), as protoc's C# generator does.
    /// </summary>
    internal static string CSharpNamespace(ProtobufFile file)
    {
        if (file.Options.FirstOrDefault(x => x.Name == "csharp_namespace") is { Arguments: [var written] })
        {
            return written;
        }

        var result = new System.Text.StringBuilder();
        var capital = true;
        foreach (var c in file.Package)
        {
            if (char.IsAsciiLetterLower(c))
            {
                result.Append(capital ? char.ToUpperInvariant(c) : c);
                capital = false;
            }
            else if (char.IsAsciiLetterUpper(c))
            {
                result.Append(c);
                capital = false;
            }
            else if (char.IsAsciiDigit(c))
            {
                result.Append(c);
                capital = true;
            }
            else
            {
                capital = true;
                if (c == '.')
                {
                    result.Append('.');
                }
            }
        }

        return result.ToString();
    }

    /// <summary>What differs between the Service's Operations and the proto service's rpcs, each as a clause.</summary>
    private static IEnumerable<string> Differences(IElement service, TypeNode proto, IReadOnlyList<MemberNode> rpcs)
    {
        var operations = GrpcService.Operations(service).GroupBy(x => x.Name, StringComparer.Ordinal).ToDictionary(x => x.Key, x => x.First(), StringComparer.Ordinal);
        var byName = rpcs.GroupBy(x => x.Name, StringComparer.Ordinal).ToDictionary(x => x.Key, x => x.First(), StringComparer.Ordinal);

        foreach (var rpc in byName.Values.Where(x => !operations.ContainsKey(x.Name)))
        {
            yield return $"its rpc `{rpc.Name}` is no Operation of `{service.Name}`";
        }

        foreach (var operation in operations.Values.Where(x => !byName.ContainsKey(x.Name)))
        {
            yield return $"its Operation `{operation.Name}` is no rpc of `{proto.Name}`";
        }

        foreach (var (name, rpc) in byName.Where(x => operations.ContainsKey(x.Key)))
        {
            var operation = operations[name];
            var parameters = operation.ChildElements.Where(x => x.SpecializationType == "Parameter").ToList();
            var same = parameters.Count == rpc.Parameters.Count
                       && parameters.Zip(rpc.Parameters).All(x => GrpcService.Matches(x.First.TypeReference, x.Second.Type))
                       && GrpcService.Matches(operation.TypeReference, rpc.Type);
            if (!same)
            {
                var request = string.Join(", ", rpc.Parameters.Select(x => GrpcService.Shown(x.Type)));
                yield return $"`{name}` is `({request}): {GrpcService.Shown(rpc.Type)}` in the .proto and `({string.Join(", ", GrpcService.Requests(operation))}): {GrpcService.Response(operation)}` in the model";
            }
        }
    }
}
