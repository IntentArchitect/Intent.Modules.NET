using System.Text.RegularExpressions;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Tests.Harness;

/// <summary>
/// The C# a case's house modules extend the engine with, as a module does through <c>DriftEngineExtensionBase</c>. A case
/// names one in its packs, beside the house pack whose vocabulary states the claims it reads.
/// </summary>
internal static class HouseExtensions
{
    private static readonly IReadOnlyList<IDriftExtension> All = [new HouseReaders(), new FaultyReaders()];

    public static IDriftExtension? Find(string id) => All.FirstOrDefault(x => x.Id == id);
}

/// <summary>
/// What eShopCustom's and the Java spike's own modules check that no stock reader reads: registrations, an application
/// service's implementation, the files elements stand for, the folder a type is in, backing fields, a request's validator,
/// identified commands and wrapped invocations. Each reads only claims whose vocabulary binds what it needs, and leaves the
/// rest to the next rule. A Vue front end's links, uses, routes and requests are the stock Vue reader's (<c>intent.vue</c>).
/// </summary>
internal sealed class HouseReaders : IDriftExtension
{
    public string Id => "house.readers";

    public IEnumerable<IClaimReader> ClaimReaders =>
    [
        new Registered(), new Implemented(), new FileExists(), new Folder(), new BackingField(), new Validated(), new Identified(), new Wrapped(),
        new GrpcSecurity()
    ];

    private static string? Bound(ClaimContext context, string name) =>
        context.Claim.Bindings?.GetValueOrDefault(name) is { Length: > 0 } value ? value : null;

    /// <summary>A type as written without whitespace starts with <paramref name="prefix"/>; one ending in a name ends where the type's name does.</summary>
    private static bool Starts(TypeRefNode type, string prefix)
    {
        var text = Regex.Replace(type.Text, @"\s", "");
        return text.StartsWith(prefix, StringComparison.Ordinal)
               && (!char.IsLetterOrDigit(prefix[^1]) || text.Length == prefix.Length || !char.IsLetterOrDigit(text[prefix.Length]) && text[prefix.Length] != '_');
    }

    /// <summary>A FluentValidation validator of the request, <c>AbstractValidator&lt;CancelOrderCommand&gt;</c>, or of the request wrapped in <c>IdentifiedCommand</c>.</summary>
    private sealed class Validated : IClaimReader
    {
        public IReadOnlyCollection<string> Kinds => ["request.validated"];

        public ClaimReading? Read(ClaimContext context)
        {
            if (Bound(context, "Validates") is not { } validates)
            {
                return null;
            }

            var validator = context.Code.Types.FirstOrDefault(x => x.BaseTypes.Any(b => Starts(b, "AbstractValidator<" + validates)));
            return validator is not null ? ClaimReading.Found(validator, validator.At) : ClaimReading.Missing("the code", $"a class deriving `AbstractValidator<{validates}`");
        }
    }

    /// <summary>A Command is an Identified Command when a class derives eShopCustom's <c>IdentifiedCommandHandler&lt;Command, R&gt;</c> for it; none says it isn't.</summary>
    private sealed class Identified : IClaimReader
    {
        public IReadOnlyCollection<string> Kinds => ["command.identified"];

        public ClaimReading? Read(ClaimContext context)
        {
            if (Bound(context, "Command") is not { } command)
            {
                return null;
            }

            if (context.Code.Types.FirstOrDefault(x => x.BaseTypes.Any(b => Starts(b, $"IdentifiedCommandHandler<{command},"))) is { } handler)
            {
                return ClaimReading.Stated("true", handler.At);
            }

            var type = context.Type ?? context.TypeOf(context.Element) ?? context.Code.Types.FirstOrDefault(x => x.Name == command);
            return ClaimReading.Absent(type?.At, "false");
        }
    }

    /// <summary>
    /// How an operation's method sends the Command it invokes: wrapped in eShopCustom's <c>IdentifiedCommand&lt;Command, R&gt;</c>,
    /// or bare, a send of the Command itself.
    /// </summary>
    private sealed class Wrapped : IClaimReader
    {
        public IReadOnlyCollection<string> Kinds => ["invocation.wrapped"];

        public ClaimReading? Read(ClaimContext context)
        {
            if (Bound(context, "Service") is not { } service || Bound(context, "Operation") is not { } operation || Bound(context, "Command") is not { } command)
            {
                return null;
            }

            var method = context.Code.Types.Where(x => x.Name == service && x.Kind == "class").SelectMany(x => x.Members)
                .FirstOrDefault(x => x.Name == operation && x.Kind == "method");
            if (method?.Creations.FirstOrDefault(x => Starts(x.Type, $"IdentifiedCommand<{command},")) is { } wrapped)
            {
                return ClaimReading.Stated("IdentifiedCommand", wrapped.At);
            }

            return method?.Calls.FirstOrDefault(x => x.Name == "Send" && x.Arguments.FirstOrDefault() is { } sent
                                                     && (sent.Creations.Any(c => c.Type.Name == command) || sent.Types.Any(t => t.Name == command))) is { } bare
                ? ClaimReading.Stated("bare", bare.At)
                : null;
        }
    }

    /// <summary>The calls the code makes in a file: its types' members' and its top level's.</summary>
    private static IEnumerable<CallNode> CallsIn(CodeGraph code, FileNode file) =>
        code.Types.Where(x => x.Files.Contains(file.Path)).SelectMany(x => x.Members).SelectMany(x => x.Calls)
            .Concat(file.TopLevel?.Calls ?? []);

    /// <summary>A registration call, as C#'s canonical text writes it (<c>AddHostedService&lt;GracePeriodManagerService&gt;</c>), anywhere in the code.</summary>
    private sealed class Registered : IClaimReader
    {
        public IReadOnlyCollection<string> Kinds => ["type.registered"];

        public ClaimReading? Read(ClaimContext context)
        {
            if (Bound(context, "Registration") is not { } registration)
            {
                return null;
            }

            // In a chain such as AddRabbitMqEventBus(...).AddSubscription<...>(), the outer call's text holds the inner's too.
            var call = context.Code.Files.SelectMany(x => CallsIn(context.Code, x)).Distinct()
                .Where(x => x.Canonical.Contains(registration + "(", StringComparison.Ordinal))
                .MinBy(x => x.Canonical.Length);
            return call is not null ? ClaimReading.Found(call, call.At) : ClaimReading.Missing("the code", $"a call to `{registration}`");
        }
    }

    /// <summary>The class an Application Service's Implementation names implements the interface the Service describes, <c>I&lt;Service&gt;</c>.</summary>
    private sealed class Implemented : IClaimReader
    {
        public IReadOnlyCollection<string> Kinds => ["service.implementation"];

        public ClaimReading? Read(ClaimContext context)
        {
            if (Bound(context, "Implementation") is not { } name || Bound(context, "Service") is not { } service)
            {
                return null;
            }

            if (context.Code.Types.FirstOrDefault(x => x.Name == name && x.Kind == "class") is not { } type)
            {
                return ClaimReading.Missing("the code", $"a class `{name}`");
            }

            var contract = "I" + service;
            return context.Code.Supertypes(type).Any(x => x == contract || x.EndsWith("." + contract, StringComparison.Ordinal))
                ? ClaimReading.Found(type)
                : ClaimReading.Absent(type.At, "absent");
        }
    }

    /// <summary>
    /// The file a <c>File</c> binding names is in scope; a gRPC service's <c>.proto</c> must also declare the service its
    /// <c>ProtoService</c> binding names.
    /// </summary>
    private sealed class FileExists : IClaimReader
    {
        public IReadOnlyCollection<string> Kinds => ["file.exists"];

        public ClaimReading? Read(ClaimContext context)
        {
            if (Bound(context, "File") is not { } path)
            {
                return null;
            }

            if (!context.Scope.Files.Contains(path))
            {
                return ClaimReading.Missing("the code", $"`{path}`");
            }

            if (!context.Scope.Includes(path))
            {
                return ClaimReading.OutOfScope(path);
            }

            var start = new Span(path, 1);
            if (Bound(context, "ProtoService") is { } service)
            {
                return context.Code.Types.FirstOrDefault(x => x.Name == service && x.Files.Contains(path)) is { } declared
                    ? ClaimReading.Found(declared, declared.At)
                    : ClaimReading.Absent(start, "absent");
            }

            return ClaimReading.Found(path, start);
        }
    }

    /// <summary>The folder of the file that declares the type, for the Services and User Interface designers: a file may declare several types.</summary>
    private sealed class Folder : IClaimReader
    {
        public IReadOnlyCollection<string> Kinds => ["type.location"];

        public ClaimReading? Read(ClaimContext context)
        {
            if (context.Claim.Designer is not ("Services" or "User Interface") || context.Type is not { } type)
            {
                return null;
            }

            var folders = type.Files.Select(x => x.Contains('/') ? x[..x.LastIndexOf('/')] : "").ToList();
            var folder = folders.FirstOrDefault(x => x == context.Expected) ?? folders.FirstOrDefault() ?? "";
            return ClaimReading.Compared(folder, type.At, folder == context.Expected);
        }
    }

    /// <summary>The private field an attribute is persisted from, by the name the Backing Field stereotype gives.</summary>
    private sealed class BackingField : IClaimReader
    {
        public IReadOnlyCollection<string> Kinds => ["member.backingField"];

        public ClaimReading? Read(ClaimContext context)
        {
            if (Bound(context, "Entity") is not { } entity || Bound(context, "BackingField") is not { } name)
            {
                return null;
            }

            var owner = (context.Owner(context.Element) is { } element ? context.TypeOf(element) : null)
                        ?? context.Code.Types.FirstOrDefault(x => x.Name == entity);
            if (owner is null)
            {
                return null;
            }

            return context.Code.MembersOf(owner).FirstOrDefault(x => x.Kind == "field" && x.Name == name) is { } field
                ? ClaimReading.Stated(field.Name, field.At)
                : ClaimReading.Absent(owner.At);
        }
    }

    /// <summary>
    /// A gRPC Service's operation is secured by <c>[Authorize]</c> or left open by <c>[AllowAnonymous]</c>: the method's own
    /// attribute wins, else the class's, else nothing secures it.
    /// </summary>
    private sealed class GrpcSecurity : IClaimReader
    {
        public IReadOnlyCollection<string> Kinds => ["security"];

        public ClaimReading? Read(ClaimContext context)
        {
            if (context.Owner(context.Element) is not { } service || service.Stereotypes?.Any(x => x.Name == "gRPC Service") != true
                || context.Code.Types.FirstOrDefault(x => x.Name == service.Name && x.Kind == "class") is not { } type)
            {
                return null;
            }

            var method = type.Members.FirstOrDefault(x => x.Name == context.Element.Name && x.Kind == "method");
            var attribute = Secures(method?.Attributes ?? []) ?? Secures(type.Attributes);
            return attribute is null
                ? ClaimReading.Absent(method?.At ?? type.At, "None")
                : ClaimReading.Stated(attribute.Name == "AllowAnonymous" ? "Unsecured" : "Secured", attribute.At);

            static AttributeNode? Secures(IEnumerable<AttributeNode> attributes) => attributes.FirstOrDefault(x => x.Name is "AllowAnonymous" or "Authorize");
        }
    }

}

/// <summary>A module whose claim reader throws, as one with a bug does: its claims are can't verify, <c>ruleFailed</c>.</summary>
internal sealed class FaultyReaders : IDriftExtension
{
    public string Id => "house.faulty";

    public IEnumerable<IClaimReader> ClaimReaders => [new Throws()];

    private sealed class Throws : IClaimReader
    {
        public IReadOnlyCollection<string> Kinds => ["type.marker"];

        public ClaimReading? Read(ClaimContext context) => throw new InvalidOperationException("The marker's attribute has no name.");
    }
}
