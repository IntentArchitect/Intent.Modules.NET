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
/// identified commands and wrapped invocations, and a Vue front end's links, uses and routes. Each reads only claims whose
/// vocabulary binds what it needs, and leaves the rest to the next rule.
/// </summary>
internal sealed class HouseReaders : IDriftExtension
{
    public string Id => "house.readers";

    public IEnumerable<IClaimReader> ClaimReaders =>
    [
        new Registered(), new Implemented(), new FileExists(), new Folder(), new BackingField(), new Validated(), new Identified(), new Wrapped(),
        new GrpcSecurity(), new Navigates(), new Uses(), new Route(), new Request()
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

    /// <summary>A Vue page links to a route with a <c>to</c> attribute, or goes there with <c>router.push</c> or <c>router.replace</c>.</summary>
    private sealed class Navigates : IClaimReader
    {
        public IReadOnlyCollection<string> Kinds => ["intent.navigates"];

        public ClaimReading? Read(ClaimContext context)
        {
            if (context.Claim.Designer != "Front End" || Bound(context, "File") is not { } path || Bound(context, "Route") is not { } route)
            {
                return null;
            }

            if (context.Code.Files.FirstOrDefault(x => x.Path == path) is not { } file)
            {
                return ClaimReading.Missing("the code", $"`{path}`");
            }

            if (file.Markup.SelectMany(x => x.DescendantsAndSelf()).SelectMany(x => x.Attributes)
                    .FirstOrDefault(x => x is { Name: "to", Directive: null } && x.Value == route) is { } link)
            {
                return ClaimReading.Found(link, link.At);
            }

            if (CallsIn(context.Code, file).FirstOrDefault(x => x is { Name: "push" or "replace", Receiver: "router" } && x.Arguments.FirstOrDefault()?.Literal?.Value == route) is { } call)
            {
                return ClaimReading.Found(call, call.At);
            }

            return ClaimReading.Defaulted("absent", new Span(path, 1), $"no `to=\"{route}\"` or `router.push('{route}')` in `{path}`");
        }
    }

    /// <summary>A Vue component renders a child in its template (<c>Child</c>), or imports a module as a value (<c>Used</c>).</summary>
    private sealed class Uses : IClaimReader
    {
        public IReadOnlyCollection<string> Kinds => ["intent.uses"];

        public ClaimReading? Read(ClaimContext context)
        {
            if (context.Claim.Designer != "Front End" || Bound(context, "File") is not { } path)
            {
                return null;
            }

            if (context.Code.Files.FirstOrDefault(x => x.Path == path) is not { } file)
            {
                return ClaimReading.Missing("the code", $"`{path}`");
            }

            if (Bound(context, "Child") is { } child)
            {
                var kebab = Regex.Replace(child, "(?<=[a-z0-9])([A-Z])", "-$1").ToLowerInvariant();
                return file.Markup.SelectMany(x => x.DescendantsAndSelf()).FirstOrDefault(x => x.Tag == child || x.Tag == kebab) is { } tag
                    ? ClaimReading.Found(tag, tag.At)
                    : ClaimReading.Defaulted("absent", new Span(path, 1), $"no `<{child}>` or `<{kebab}>` in the template of `{path}`");
            }

            if (Bound(context, "Used") is { } used)
            {
                return file.Imports.FirstOrDefault(x => !x.TypeOnly && x.Module is { } module && Relative(module) == used) is { } import
                    ? ClaimReading.Found(import, import.At)
                    : ClaimReading.Defaulted("absent", new Span(path, 1), $"no value import of `{used}` in `{path}`");
            }

            return null;
        }

        /// <summary>A relative module without its leading <c>./</c> and <c>../</c> and its extension: <c>stores/cart</c> for <c>../stores/cart.ts</c>.</summary>
        private static string Relative(string module) =>
            Regex.Replace(Regex.Replace(module, @"^(\.\.?/)+", ""), @"\.(vue|ts)$", "");
    }

    /// <summary>A Vue page's route, from its <c>{ path, component }</c> record in <c>src/router.ts</c>.</summary>
    private sealed class Route : IClaimReader
    {
        private const string Router = "src/router.ts";

        public IReadOnlyCollection<string> Kinds => ["page.route"];

        public ClaimReading? Read(ClaimContext context)
        {
            if (context.Claim.Designer != "Front End" || Bound(context, "Page") is not { } page)
            {
                return null;
            }

            var record = context.Code.Files.FirstOrDefault(x => x.Path == Router) is { } file
                ? Values(CallsIn(context.Code, file).SelectMany(x => x.Arguments))
                    .FirstOrDefault(x => x.Members.Any(m => m.Member == "component" && m.Value.Text == page))
                : null;
            return record?.Members.FirstOrDefault(x => x.Member == "path")?.Value.Literal?.Value is { } route
                ? ClaimReading.Stated(route, record.At)
                : ClaimReading.Absent(new Span(Router, 1));
        }

        /// <summary>The values and everything they hold: object members, array items and what a name is given.</summary>
        private static IEnumerable<ValueNode> Values(IEnumerable<ValueNode> roots)
        {
            var seen = new HashSet<ValueNode>();
            var pending = new Stack<ValueNode>(roots);
            while (pending.TryPop(out var value))
            {
                if (!seen.Add(value))
                {
                    continue;
                }

                yield return value;
                foreach (var inner in value.Members.Select(x => x.Value).Concat(value.Items).Concat(value.Given))
                {
                    pending.Push(inner);
                }
            }
        }
    }

    /// <summary>
    /// A storefront proxy operation's call to its own <c>request(verb, path)</c> helper: the verb, and the path with each
    /// <c>${encodeURIComponent(id)}</c> as the route parameter <c>{id}</c>.
    /// </summary>
    private sealed class Request : IClaimReader
    {
        public IReadOnlyCollection<string> Kinds => ["proxy.verb", "proxy.route"];

        public ClaimReading? Read(ClaimContext context)
        {
            if (context.Claim.Designer != "Front End" || Bound(context, "File") is not { } path || Bound(context, "Operation") is not { } operation)
            {
                return null;
            }

            var method = context.Code.Types.Where(x => x.Files.Contains(path)).SelectMany(x => x.Members).FirstOrDefault(x => x.Name == operation);
            if (method?.Calls.FirstOrDefault(x => x.Name == "request" && x.Arguments.Count >= 2) is not { } call)
            {
                return ClaimReading.CantVerify("notFound", $"No `request(…)` in `{operation}` in `{path}`.");
            }

            var value = context.Kind == "proxy.verb"
                ? call.Arguments[0].Literal?.Value ?? call.Arguments[0].Text.Trim('\'', '"', '`')
                : Regex.Replace(Regex.Replace(call.Arguments[1].Text.Trim('\'', '"', '`'), @"\$\{encodeURIComponent\((\w+)\)\}", "{$1}"), @"\$\{(\w+)\}", "{$1}");
            return ClaimReading.Stated(value, method.At);
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
