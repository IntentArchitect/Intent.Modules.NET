using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intent.Modules.Common.DriftEngine.Stacks.AspNetCore;

/// <summary>
/// One Minimal API endpoint: <c>group.MapPut("/cancel", CancelOrderAsync)</c> in a router type's mapping method.
/// <paramref name="Authorization"/> is what is chained on the endpoint itself (<c>RequireAuthorization</c> or
/// <c>AllowAnonymous</c>), <paramref name="GroupAuthorization"/> what is chained on its group.
/// </summary>
/// <param name="Handler">The handler method's name; for a lambda, the name <c>.WithName("...")</c> gives the endpoint, if any.</param>
/// <param name="Lambda">A lambda handler as a method: its parameters, its return type where written, and its body.</param>
/// <param name="Template">The pattern: a string literal's value, otherwise the argument as written (<paramref name="TemplateWritten"/>).</param>
/// <param name="Group">Its groups' prefixes joined; one given as anything but a literal is as written, and is <paramref name="GroupWritten"/>.</param>
internal sealed record MinimalEndpoint(string Router, string? Handler, string Verb, string Template, Span At, string? Group, Span? GroupAt,
    Authorization? Authorization, Authorization? GroupAuthorization, MemberNode? Lambda = null, bool TemplateWritten = false, string? GroupWritten = null)
{
    /// <summary>The API versions its groups declare (<c>HasApiVersion(1.0)</c>), in the order written.</summary>
    public IReadOnlyList<VersionValue> GroupVersions { get; init; } = [];

    /// <summary>The API versions the endpoint itself declares (<c>HasApiVersion</c>) or narrows its groups' to (<c>MapToApiVersion</c>).</summary>
    public IReadOnlyList<VersionValue> Versions { get; init; } = [];

    /// <summary>The name the last <c>.WithName(...)</c> chained on the endpoint gives it, which OpenAPI makes its operationId.</summary>
    public NamedValue? EndpointName { get; init; }

    /// <summary>
    /// The operationId the last OpenAPI operation callback chained on the endpoint sets (<c>.WithOpenApi(op => ...)</c>,
    /// <c>.AddOpenApiOperationTransformer((op, context, ct) => ...)</c>), which overrides its name.
    /// </summary>
    public NamedValue? OperationId { get; init; }

    /// <summary>Where the endpoint itself is left out of the API description (<c>.ExcludeFromDescription()</c>), if it is.</summary>
    public Span? Excluded { get; init; }

    /// <summary>Where a group it is mapped on is left out of the API description, if one is.</summary>
    public Span? GroupExcluded { get; init; }

    /// <summary>The pattern, a string constant's value where the code names one (<c>OrderRoutes.ById</c>); null when the code doesn't give it.</summary>
    public string? TemplateIn(CodeGraph code) => TemplateWritten ? Constant(code, Template) : Template;

    /// <summary>A value the endpoint's chain gives: a string literal's, else a string constant's the code declares; null when neither.</summary>
    public string? TextIn(CodeGraph code, WrittenValue value) => value.Literal ?? Constant(code, value.Written);

    /// <summary>The groups' prefixes, as <see cref="TemplateIn"/> reads a pattern; null when one can't be read.</summary>
    public string? GroupIn(CodeGraph code) =>
        GroupWritten is null ? Group : Constant(code, GroupWritten) is { } prefix ? Group!.Replace(GroupWritten, prefix.Trim('/')) : null;

    private string? Constant(CodeGraph code, string name) => code.Types.FirstOrDefault(x => x.Name == Router) is { } router ? code.ConstantOf(name, router) : null;
}

/// <summary>
/// An argument that names a role, a policy or a version: a literal's value, else the argument as written, for the claim
/// reader to read as a constant the code declares, or to report it can't.
/// </summary>
internal sealed record WrittenValue(string? Literal, string Written);

/// <summary>A value a call states and where: the name <c>.WithName("ListItems")</c> gives an endpoint.</summary>
internal sealed record NamedValue(WrittenValue Value, Span At);

/// <summary>
/// <c>RequireAuthorization(...)</c> or <c>AllowAnonymous()</c> (<paramref name="Kind"/>), where it is called, and the roles and
/// policies its arguments name: a policy's name, <c>new AuthorizeAttribute { Roles = ..., Policy = ... }</c>, or a policy
/// builder's <c>RequireRole(...)</c>. <paramref name="Opaque"/> holds an argument it can't tell is either, as written: a
/// constant may be a policy's name, a variable a policy.
/// </summary>
internal sealed record Authorization(string Kind, Span At, IReadOnlyList<WrittenValue> Roles, IReadOnlyList<WrittenValue> Policies, IReadOnlyList<string> Opaque)
{
    public bool Anonymous => Kind == "AllowAnonymous";

    /// <summary>An attribute of the code's own deriving from <c>AuthorizeAttribute</c>, which may set roles and a policy itself.</summary>
    public bool Custom { get; init; }

    /// <summary>A <see cref="Custom"/> attribute's class, when it is in scope: what its constructors set is what it requires.</summary>
    public TypeNode? Class { get; init; }
}

/// <summary>An API version as written: <c>HasApiVersion(1.0)</c>, <c>(1, 0)</c> and <c>(new ApiVersion(1, 0))</c> are <c>1.0</c>.</summary>
internal sealed record VersionValue(string? Version, string Written, Span At);

/// <summary>What <see cref="MinimalApiReader"/> found in the in-scope code.</summary>
internal sealed class MinimalApiModel
{
    public List<MinimalEndpoint> Endpoints { get; } = [];

    /// <summary>
    /// A router's security chained on a call of its mapping method anywhere in scope (<c>orders.MapOrdersApiV1().RequireAuthorization()</c>
    /// in Program.cs): it applies to the group the method returns, so to the router's endpoints.
    /// </summary>
    public Dictionary<string, Authorization> RouterSecurity { get; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Where a call of a router's mapping method anywhere in scope is left out of the API description
    /// (<c>app.MapOrdersApi().ExcludeFromDescription()</c>): the group the method returns is, so the router's endpoints are.
    /// </summary>
    public Dictionary<string, Span> RouterExcluded { get; } = new(StringComparer.Ordinal);

    /// <summary>The types that map endpoints or groups, by name: a router whose method maps nothing still is one.</summary>
    public HashSet<string> Routers { get; } = new(StringComparer.Ordinal);
}

/// <summary>
/// Reads ASP.NET Core Minimal API endpoints from syntax: <c>MapGet</c>, <c>MapPost</c>, <c>MapPut</c>, <c>MapDelete</c> and
/// <c>MapPatch</c> of a method group or a lambda, the <c>MapGroup</c> prefixes of the builder they're called on, and the
/// <c>RequireAuthorization</c> or <c>AllowAnonymous</c> chained on the endpoint, its group or its router's mapping call. A
/// lambda is its operation's method: named by <c>.WithName("...")</c>, or else matched by its verb and route.
/// </summary>
internal sealed class MinimalApiReader : ICodeReader
{
    private static readonly Dictionary<string, string> Verbs = new(StringComparer.Ordinal)
    {
        ["MapGet"] = "GET", ["MapPost"] = "POST", ["MapPut"] = "PUT", ["MapDelete"] = "DELETE", ["MapPatch"] = "PATCH"
    };

    public void Read(CodeGraph graph)
    {
        if (CSharpSyntax.Of(graph) is not { } csharp)
        {
            return;
        }

        var model = new MinimalApiModel();
        var mappers = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var (file, tree) in csharp.Trees)
        {
            foreach (var call in tree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                if (call.Expression is not MemberAccessExpressionSyntax access || call.FirstAncestorOrSelf<BaseTypeDeclarationSyntax>() is not { } router)
                {
                    continue;
                }

                if (access.Name.Identifier.Text is "MapGroup" or "MapMethods" || Verbs.ContainsKey(access.Name.Identifier.Text))
                {
                    model.Routers.Add(router.Identifier.Text);
                }

                // MapMethods("/x", ["PUT"], handler) maps the verbs it lists; several are joined, and the endpoint serves each.
                string? verb;
                ExpressionSyntax? template;
                ExpressionSyntax? handler;
                if (access.Name.Identifier.Text == "MapMethods"
                    && call.ArgumentList.Arguments is [{ Expression: var mapped }, { Expression: var methods }, { Expression: var mappedHandler }, ..])
                {
                    var listed = methods.DescendantNodesAndSelf().OfType<LiteralExpressionSyntax>().Select(x => x.Token.ValueText.ToUpperInvariant()).Distinct().ToList();
                    (verb, template, handler) = (listed.Count == 0 ? null : string.Join(",", listed), mapped, mappedHandler);
                }
                else if (Verbs.TryGetValue(access.Name.Identifier.Text, out var named)
                         && call.ArgumentList.Arguments is [{ Expression: var plain }, { Expression: var plainHandler }, ..])
                {
                    (verb, template, handler) = (named, plain, plainHandler);
                }
                else
                {
                    continue;
                }

                if (verb is null)
                {
                    continue;
                }

                var lambda = handler as AnonymousFunctionExpressionSyntax;
                var name = lambda is null ? Handler(handler) : Named(call);
                if (lambda is null && name is null)
                {
                    continue;
                }

                var group = Group(access.Expression, file);
                var chained = Chained(call, file);
                var (pattern, written) = template is LiteralExpressionSyntax literal ? (literal.Token.ValueText, false) : (template.ToString(), true);
                model.Endpoints.Add(new MinimalEndpoint(router.Identifier.Text, name, verb, pattern, SpanOf(call, file),
                    group.Prefix, group.At, chained.Authorization, group.Authorization, lambda is null ? null : Method(lambda, name ?? $"{verb} {pattern}", file, graph), written, group.Written)
                {
                    GroupVersions = group.Versions,
                    Versions = chained.Versions,
                    EndpointName = chained.Name,
                    OperationId = chained.OperationId,
                    Excluded = chained.Excluded,
                    GroupExcluded = group.Excluded
                });

                if (call.FirstAncestorOrSelf<MethodDeclarationSyntax>() is { } mapper)
                {
                    mappers[mapper.Identifier.Text] = router.Identifier.Text;
                }
            }
        }

        foreach (var (file, tree) in csharp.Trees)
        {
            foreach (var call in tree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                if (call.Expression is not MemberAccessExpressionSyntax access || !mappers.TryGetValue(access.Name.Identifier.Text, out var router))
                {
                    continue;
                }

                var chained = Chained(call, file);
                if (chained.Authorization is { } authorization)
                {
                    model.RouterSecurity[router] = authorization;
                }

                if (chained.Excluded is { } excluded)
                {
                    model.RouterExcluded[router] = excluded;
                }
            }
        }

        graph.Set(model);
    }

    private static string? Handler(ExpressionSyntax handler) => handler switch
    {
        IdentifierNameSyntax identifier => identifier.Identifier.Text,
        MemberAccessExpressionSyntax access => access.Name.Identifier.Text,
        _ => null
    };

    /// <summary>The name <c>.WithName("...")</c> chained on the endpoint gives it.</summary>
    private static string? Named(InvocationExpressionSyntax call)
    {
        for (SyntaxNode node = call; node.Parent is MemberAccessExpressionSyntax access && access.Parent is InvocationExpressionSyntax next; node = next)
        {
            if (access.Name.Identifier.Text == "WithName" && next.ArgumentList.Arguments is [{ Expression: LiteralExpressionSyntax name }])
            {
                return name.Token.ValueText;
            }
        }

        return null;
    }

    /// <summary>
    /// A lambda handler as the operation's method: its parameters as written, its return type where it is written
    /// (<c>Task&lt;Ok&lt;T&gt;&gt; (...) =&gt;</c>), and its body, the unit the C# reader read for it.
    /// </summary>
    private static MemberNode Method(AnonymousFunctionExpressionSyntax lambda, string name, string file, CodeGraph graph)
    {
        var at = SpanOf(lambda, file);
        var parameters = lambda switch
        {
            ParenthesizedLambdaExpressionSyntax x => x.ParameterList.Parameters,
            SimpleLambdaExpressionSyntax x => [x.Parameter],
            _ => new SeparatedSyntaxList<ParameterSyntax>()
        };
        return new MemberNode
        {
            Name = name,
            Kind = "method",
            Type = lambda is ParenthesizedLambdaExpressionSyntax { ReturnType: { } returns } ? CSharpReader.TypeRef(returns) : null,
            Parameters = CSharpReader.ParametersOf(parameters, file),
            // A lambda's own attributes are its endpoint's metadata, as a method group's are: [ExcludeFromDescription], [EndpointName].
            Attributes = lambda is LambdaExpressionSyntax { AttributeLists: { Count: > 0 } lists } ? CSharpReader.AttributesOf(lists, file) : [],
            At = at,
            Body = graph.Types.SelectMany(x => x.Members).SelectMany(x => x.Calls).SelectMany(x => x.Arguments)
                .Select(x => x.Function).FirstOrDefault(x => x is not null && x.At.File == at.File && x.At.Line == at.Line && x.At.Column == at.Column)
        };
    }

    /// <summary>
    /// The <c>MapGroup</c> prefixes, security, API versions and API description exclusion of the builder an endpoint is mapped
    /// on, following a local variable to its initializer: <c>var api = app.MapGroup("api/orders").HasApiVersion(1.0);</c>. A
    /// group is also excluded by a statement on its variable (<c>api.ExcludeFromDescription();</c>).
    /// </summary>
    private static (string? Prefix, Span? At, Authorization? Authorization, IReadOnlyList<VersionValue> Versions, string? Written, Span? Excluded) Group(ExpressionSyntax builder, string file)
    {
        var prefixes = new List<string>();
        string? written = null;
        Span? at = null;
        Authorization? authorization = null;
        Span? excluded = null;
        var versions = new List<VersionValue>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var current = builder;
        while (true)
        {
            if (current is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax access } call)
            {
                if (Excludes(call))
                {
                    excluded ??= SpanOf(access.Name, file);
                }

                switch (access.Name.Identifier.Text)
                {
                    case "MapGroup" when call.ArgumentList.Arguments is [{ Expression: LiteralExpressionSyntax prefix }, ..]:
                        prefixes.Insert(0, prefix.Token.ValueText);
                        at ??= SpanOf(call, file);
                        break;
                    // A constant, or an interpolation: as written, for the claim reader to read or report it can't.
                    case "MapGroup" when call.ArgumentList.Arguments is [{ Expression: var named }, ..]:
                        written ??= named.ToString();
                        prefixes.Insert(0, named.ToString());
                        at ??= SpanOf(call, file);
                        break;
                    case "RequireAuthorization" or "AllowAnonymous" when authorization is null:
                        authorization = Authorize(call, access, file);
                        break;
                    case "HasApiVersion":
                        versions.InsertRange(0, Versions(call, file));
                        break;
                }

                current = access.Expression;
            }
            else if (current is IdentifierNameSyntax variable && seen.Add(variable.Identifier.Text)
                     && current.FirstAncestorOrSelf<MemberDeclarationSyntax>() is { } scope
                     && scope.DescendantNodes().OfType<VariableDeclaratorSyntax>()
                         .FirstOrDefault(x => x.Identifier.Text == variable.Identifier.Text)?.Initializer?.Value is { } initializer)
            {
                excluded ??= ExcludedByStatement(scope, variable.Identifier.Text, file);
                current = initializer;
            }
            else
            {
                return (prefixes.Count == 0 ? null : string.Join("/", prefixes.Select(x => x.Trim('/'))), at, authorization, versions, written, excluded);
            }
        }
    }

    /// <summary>
    /// A call that leaves what it's chained on out of the API description: <c>ExcludeFromDescription()</c>, or
    /// <c>WithMetadata(new ExcludeFromDescriptionAttribute())</c>, the metadata it adds.
    /// </summary>
    private static bool Excludes(InvocationExpressionSyntax call) =>
        call.Expression is MemberAccessExpressionSyntax access
        && (access.Name.Identifier.Text == "ExcludeFromDescription"
            || access.Name.Identifier.Text == "WithMetadata" && call.ArgumentList.Arguments.Any(x => x.Expression is ObjectCreationExpressionSyntax { Type: var type }
                && type.ToString().Split('.')[^1] is "ExcludeFromDescriptionAttribute" or "ExcludeFromDescription"));

    /// <summary>
    /// Where a statement in <paramref name="scope"/> excludes the group a variable holds: a chain on the variable reaching
    /// <c>ExcludeFromDescription()</c> through no <c>Map*</c> call (<c>api.MapGet(...).ExcludeFromDescription()</c> excludes that
    /// endpoint, and <c>api.MapGroup(...)</c> another group).
    /// </summary>
    private static Span? ExcludedByStatement(MemberDeclarationSyntax scope, string variable, string file)
    {
        foreach (var call in scope.DescendantNodes().OfType<InvocationExpressionSyntax>().Where(Excludes))
        {
            var receiver = ((MemberAccessExpressionSyntax)call.Expression).Expression;
            while (receiver is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax inner } && !inner.Name.Identifier.Text.StartsWith("Map", StringComparison.Ordinal))
            {
                receiver = inner.Expression;
            }

            if (receiver is IdentifierNameSyntax named && named.Identifier.Text == variable)
            {
                return SpanOf(((MemberAccessExpressionSyntax)call.Expression).Name, file);
            }
        }

        return null;
    }

    /// <summary>
    /// The <c>RequireAuthorization</c> or <c>AllowAnonymous</c> chained after a call, and the API versions chained on it:
    /// <c>HasApiVersion</c>, or <c>MapToApiVersion</c>, which narrows its group's. And what OpenAPI reads from the chain: the
    /// last <c>WithName</c>'s name, the operationId the last OpenAPI operation callback sets, and an exclusion from the API
    /// description.
    /// </summary>
    private static (Authorization? Authorization, IReadOnlyList<VersionValue> Versions, NamedValue? Name, NamedValue? OperationId, Span? Excluded) Chained(
        InvocationExpressionSyntax call, string file)
    {
        Authorization? authorization = null;
        NamedValue? name = null;
        NamedValue? operationId = null;
        Span? excluded = null;
        var versions = new List<VersionValue>();
        for (SyntaxNode node = call; node.Parent is MemberAccessExpressionSyntax access && access.Parent is InvocationExpressionSyntax next; node = next)
        {
            if (Excludes(next))
            {
                excluded ??= SpanOf(access.Name, file);
            }

            switch (access.Name.Identifier.Text)
            {
                case "RequireAuthorization" or "AllowAnonymous" when authorization is null:
                    authorization = Authorize(next, access, file);
                    break;
                case "HasApiVersion" or "MapToApiVersion":
                    versions.AddRange(Versions(next, file));
                    break;
                // The last name is the endpoint's: OpenAPI takes the last IEndpointNameMetadata.
                case "WithName" when next.ArgumentList.Arguments is [{ Expression: var named }]:
                    name = new NamedValue(Text(named, file), SpanOf(access.Name, next, file));
                    break;
                // Each callback runs after the ones before it, so the last one that sets the operationId decides it.
                case "WithOpenApi" or "AddOpenApiOperationTransformer" when OperationIdSet(next, file) is { } set:
                    operationId = set;
                    break;
            }
        }

        return (authorization, versions, name, operationId, excluded);
    }

    /// <summary>
    /// The operationId an OpenAPI operation callback sets on the operation it is given: <c>op =&gt; { op.OperationId = "x"; ... }</c>,
    /// or <c>op =&gt; new(op) { OperationId = "x" }</c>; the last it sets, when it sets one.
    /// </summary>
    private static NamedValue? OperationIdSet(InvocationExpressionSyntax call, string file)
    {
        if (call.ArgumentList.Arguments.FirstOrDefault()?.Expression is not LambdaExpressionSyntax lambda)
        {
            return null;
        }

        var operation = lambda switch
        {
            SimpleLambdaExpressionSyntax simple => simple.Parameter.Identifier.Text,
            ParenthesizedLambdaExpressionSyntax { ParameterList.Parameters: [var first, ..] } => first.Identifier.Text,
            _ => null
        };
        var set = lambda.Body.DescendantNodesAndSelf().OfType<AssignmentExpressionSyntax>()
            .Where(x => x.Left switch
            {
                MemberAccessExpressionSyntax { Name.Identifier.Text: "OperationId", Expression: IdentifierNameSyntax target } => target.Identifier.Text == operation,
                IdentifierNameSyntax { Identifier.Text: "OperationId" } => x.Parent is InitializerExpressionSyntax,
                _ => false
            })
            .LastOrDefault();
        return set is null ? null : new NamedValue(Text(set.Right, file), SpanOf(set, file));
    }

    /// <summary>A value as written, with its string literal's value (verbatim, raw and <c>nameof</c> included) where it is one.</summary>
    private static WrittenValue Text(ExpressionSyntax value, string file) =>
        new(CSharpReader.ValueOf(value, file).Literal is { Kind: "string" } literal ? literal.Value : null, value.ToString());

    /// <summary>
    /// What a <c>RequireAuthorization(...)</c> names: a policy's name (a literal, or a constant the claim reader reads), an
    /// <c>AuthorizeAttribute</c>'s <c>Roles</c> and <c>Policy</c>, or the roles a policy builder's <c>RequireRole</c> requires.
    /// </summary>
    private static Authorization Authorize(InvocationExpressionSyntax call, MemberAccessExpressionSyntax access, string file)
    {
        var roles = new List<WrittenValue>();
        var policies = new List<WrittenValue>();
        var opaque = new List<string>();
        foreach (var argument in call.ArgumentList.Arguments.Select(x => x.Expression))
        {
            switch (argument)
            {
                case LiteralExpressionSyntax:
                    policies.Add(Written(argument));
                    break;
                case BaseObjectCreationExpressionSyntax creation:
                    policies.AddRange(creation.ArgumentList?.Arguments.Select(x => Written(x.Expression)) ?? []);
                    foreach (var assignment in creation.Initializer?.Expressions.OfType<AssignmentExpressionSyntax>() ?? [])
                    {
                        switch ((assignment.Left as IdentifierNameSyntax)?.Identifier.Text)
                        {
                            case "Roles":
                                roles.Add(Written(assignment.Right));
                                break;
                            case "Policy":
                                policies.Add(Written(assignment.Right));
                                break;
                        }
                    }

                    break;
                case AnonymousFunctionExpressionSyntax lambda:
                    roles.AddRange(lambda.DescendantNodes().OfType<InvocationExpressionSyntax>()
                        .Where(x => x.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "RequireRole" })
                        .SelectMany(x => x.ArgumentList.Arguments.Select(a => Written(a.Expression))));
                    break;
                default:
                    opaque.Add(argument.ToString());
                    break;
            }
        }

        return new Authorization(access.Name.Identifier.Text, SpanOf(access.Name, file), roles, policies, opaque);
    }

    private static WrittenValue Written(ExpressionSyntax value) =>
        new(value is LiteralExpressionSyntax literal && literal.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.StringLiteralExpression) ? literal.Token.ValueText : null, value.ToString());

    /// <summary>
    /// The versions <c>HasApiVersion</c> or <c>MapToApiVersion</c> gives: <c>(1.0)</c>, <c>(1, 0)</c>, <c>("1.0")</c> or
    /// <c>(new ApiVersion(1, 0))</c> are <c>1.0</c>. Anything else is as written, with no version, for the claim reader to
    /// read as a constant or report it can't.
    /// </summary>
    private static IEnumerable<VersionValue> Versions(InvocationExpressionSyntax call, string file)
    {
        var at = SpanOf(call, file);
        var arguments = call.ArgumentList.Arguments.Select(x => x.Expression).ToList();
        if (arguments is [BaseObjectCreationExpressionSyntax { ArgumentList: { } created }])
        {
            arguments = created.Arguments.Select(x => x.Expression).ToList();
        }

        if (arguments.Count == 0)
        {
            yield break;
        }

        if (arguments.All(x => x is LiteralExpressionSyntax))
        {
            var parts = arguments.Cast<LiteralExpressionSyntax>().Select(x => x.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.StringLiteralExpression) ? x.Token.ValueText : x.Token.Text).ToList();
            // (major, minor) or (major, minor, status); a single argument is the version itself.
            yield return new VersionValue(parts.Count == 1 ? parts[0] : $"{parts[0]}.{parts[1]}{(parts.Count > 2 ? $"-{parts[2]}" : "")}", call.ArgumentList.ToString(), at);
            yield break;
        }

        yield return new VersionValue(null, string.Join(", ", arguments.Select(x => x.ToString())), at);
    }

    private static Span SpanOf(SyntaxNode node, string file)
    {
        var lines = node.GetLocation().GetLineSpan();
        return new Span(file, lines.StartLinePosition.Line + 1, lines.StartLinePosition.Character + 1, lines.EndLinePosition.Line + 1, lines.EndLinePosition.Character + 1);
    }

    /// <summary>From one node's start to another's end: a chained call's name and arguments, without what it's chained on.</summary>
    private static Span SpanOf(SyntaxNode from, SyntaxNode to, string file)
    {
        var start = from.GetLocation().GetLineSpan().StartLinePosition;
        var end = to.GetLocation().GetLineSpan().EndLinePosition;
        return new Span(file, start.Line + 1, start.Character + 1, end.Line + 1, end.Character + 1);
    }
}

/// <summary>
/// The HTTP claims, and an operation's return type and parameters, of a Service that is a Minimal API router: its
/// endpoints' handler methods are its operations. A Service or operation that isn't one is passed on, to the controller
/// rules or the engine's own.
/// </summary>
internal sealed class MinimalApiClaims : IClaimReader
{
    /// <summary>The results a handler's success is: <c>Ok&lt;T&gt;</c> is T, <c>Ok</c> nothing.</summary>
    private static readonly HashSet<string> Success = ["Ok", "Created", "CreatedAtRoute", "Accepted", "AcceptedAtRoute", "NoContent"];

    /// <summary>The results that send a file, and what they return: its stream, or its content.</summary>
    private static readonly Dictionary<string, TypeRefNode> Files = new(StringComparer.Ordinal)
    {
        ["FileStreamHttpResult"] = new() { Text = "Stream", Name = "Stream", QualifiedName = "System.IO.Stream" },
        ["PushStreamHttpResult"] = new() { Text = "Stream", Name = "Stream", QualifiedName = "System.IO.Stream" },
        ["PhysicalFileHttpResult"] = new() { Text = "Stream", Name = "Stream", QualifiedName = "System.IO.Stream" },
        ["VirtualFileHttpResult"] = new() { Text = "Stream", Name = "Stream", QualifiedName = "System.IO.Stream" },
        // An array's element is its one type argument, as the comparer reads it.
        ["FileContentHttpResult"] = new()
        {
            Text = "byte[]", Name = "byte", QualifiedName = "byte", IsArray = true,
            Arguments = [new TypeRefNode { Text = "byte", Name = "byte", QualifiedName = "byte", IsPredefinedValueType = true }]
        }
    };

    /// <summary>Parameters the framework supplies; they're never part of the modelled signature.</summary>
    private static readonly HashSet<string> Injected = ["CancellationToken", "ILogger", "HttpContext", "HttpRequest", "HttpResponse", "ClaimsPrincipal", "IServiceProvider"];

    /// <summary>Interfaces that are request data, not services.</summary>
    private static readonly HashSet<string> RequestInterfaces = ["IFormFile", "IFormFileCollection", "IEnumerable", "IList", "ICollection", "IReadOnlyList", "IReadOnlyCollection"];

    public IReadOnlyCollection<string> Kinds { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        "member.exists", "service.route", "operation.verb", "operation.route", "operation.returns", "operation.parameters", "operation.status",
        "endpoint.exists", "security", "security.roles", "security.policy", "service.versions", "operation.versions", OpenApi.OperationIdKind, OpenApi.IgnoreKind
    };

    public ClaimReading? Read(ClaimContext context)
    {
        // A handler that returns ASP.NET Core's typed results returns what its success carries, wherever it's mapped.
        if (context.Kind == "operation.returns" && context.Member is { Type: { } returned } handler
            && (returned is { Name: "Task" or "ValueTask", Arguments: [var awaited] } ? awaited : returned) is { } result
            && (result.Name == "Results" || Success.Contains(result.Name) && result.Arguments.Count == 1 || Files.ContainsKey(result.Name)))
        {
            return Returns(context, handler);
        }

        // One whose return type doesn't say what it succeeds with (IActionResult, IResult) returns what its body's success carries.
        if (context.Kind == "operation.returns" && context.Member is { Type: { } untyped } action && ActionResults.IsUntyped(untyped))
        {
            return ActionResults.Read(context, action);
        }

        if (context.Code.Get<MinimalApiModel>() is not { } model || model.Endpoints.Count == 0 && model.Routers.Count == 0)
        {
            return null;
        }

        if (context.Kind == "member.exists")
        {
            return model.Endpoints.Count == 0 ? null : Lambda(context, model);
        }

        if (context.Kind == "endpoint.exists")
        {
            return Endpoint(context, model);
        }

        if (context.Kind.StartsWith("service.", StringComparison.Ordinal))
        {
            var type = context.Type!;
            var endpoints = model.Endpoints.Where(x => x.Router == type.Name).ToList();
            if (endpoints.Count == 0)
            {
                return null;
            }

            if (context.Kind == "service.versions")
            {
                // A versioned API maps a group per version (CatalogApi's v1 and v2): the router serves their union.
                return Versioning.Read(context, endpoints.SelectMany(x => x.GroupVersions.Concat(x.Versions)).DistinctBy(x => (x.Written, x.At)).ToList(), type,
                    endpoints[0].GroupAt ?? endpoints[0].At);
            }

            if (endpoints.FirstOrDefault(x => x.GroupWritten is not null && x.GroupIn(context.Code) is null) is { } unread)
            {
                return Unreadable($"The group's prefix is `{unread.GroupWritten}`", unread.GroupAt!);
            }

            var groups = endpoints.Select(x => x.GroupIn(context.Code)).Distinct().ToList();
            return groups is [{ } group]
                ? ClaimReading.Stated(group, endpoints[0].GroupAt!)
                : groups is [null] ? ClaimReading.Absent(endpoints[0].At) : ClaimReading.CantVerify("ambiguous", $"its endpoints are mapped on {groups.Count} different groups.");
        }

        if ((context.Member ?? context.MemberOf(context.Element)) is not { } member
            || context.Owner(context.Element) is not { } service || context.TypeOf(service) is not { } router
            || (context.Located<MinimalEndpoint>() ?? EndpointOf(model, router, member)) is not { } endpoint)
        {
            return null;
        }

        return context.Kind switch
        {
            // MapMethods with several verbs serves each, so the one Verb the designer takes is true when it is any of them.
            "operation.verb" when endpoint.Verb.Contains(',') => endpoint.Verb.Split(',').FirstOrDefault(x => string.Equals(x, context.Expected, StringComparison.OrdinalIgnoreCase)) is { } served
                ? ClaimReading.Compared(served, endpoint.At, true)
                : ClaimReading.Compared(endpoint.Verb.Replace(",", ", "), endpoint.At, false),
            "operation.verb" => ClaimReading.Stated(endpoint.Verb, endpoint.At),
            "operation.route" => endpoint.TemplateIn(context.Code) is { } template
                ? ClaimReading.Stated(template, endpoint.At)
                : Unreadable($"The pattern is `{endpoint.Template}`", endpoint.At),
            "security" or "security.roles" or "security.policy" => Security.Read(context, Levels(context.Code, model, endpoint, member, router), router, endpoint.At),
            // The endpoint's own versions, else its groups', which it serves.
            "operation.versions" => Versioning.Read(context, endpoint.Versions.Count > 0 ? endpoint.Versions : endpoint.GroupVersions, router, endpoint.At),
            "operation.returns" when endpoint.Lambda is not null && member.Type is null => LambdaReturns(context, endpoint, member),
            "operation.returns" => Returns(context, member),
            "operation.status" => Http.SuccessStatus.Read(context, member),
            OpenApi.OperationIdKind => OpenApi.OperationId(context, endpoint, member, router),
            OpenApi.IgnoreKind => OpenApi.Ignore(model, endpoint, member, router),
            _ => Parameters(context, member)
        };
    }

    private static MinimalEndpoint? EndpointOf(MinimalApiModel model, TypeNode router, MemberNode member) =>
        model.Endpoints.FirstOrDefault(x => x.Router == router.Name && (x.Lambda is { } lambda ? lambda.At == member.At : x.Handler == member.Name));

    /// <summary>
    /// The places that may secure an endpoint, most specific first: its own <c>RequireAuthorization</c> or <c>AllowAnonymous</c>,
    /// its handler's attributes, its group's call, and its router's mapping call (in Program.cs).
    /// </summary>
    private static List<Authorization> Levels(CodeGraph code, MinimalApiModel model, MinimalEndpoint endpoint, MemberNode member, TypeNode router) =>
        new[] { endpoint.Authorization }
            .Concat(Security.Attributes(code, member.Attributes, router))
            .Append(endpoint.GroupAuthorization)
            .Append(model.RouterSecurity.GetValueOrDefault(router.Name))
            .OfType<Authorization>()
            .ToList();

    /// <summary>
    /// The endpoint an Operation is (<c>endpoint.exists</c>): the router's mapping of its handler method, or of the lambda
    /// <c>member.exists</c> found for it. An Operation without Http Settings states it isn't one.
    /// </summary>
    private static ClaimReading? Endpoint(ClaimContext context, MinimalApiModel model)
    {
        if (context.Type is not { } router || !model.Routers.Contains(router.Name) && model.Endpoints.All(x => x.Router != router.Name))
        {
            return null;
        }

        var member = context.MemberOf(context.Element);
        var endpoint = member is null
            ? model.Endpoints.FirstOrDefault(x => x.Router == router.Name && x.Handler == context.Element.Name)
            : EndpointOf(model, router, member);
        var modelled = context.Expected != "absent";
        if (endpoint is null)
        {
            return ClaimReading.Compared("absent", member?.At ?? router.At, !modelled,
                $"`{router.Name}.{context.Element.Name}` isn't in the code as an endpoint: no `MapGet`, `MapPost`, `MapPut`, `MapDelete`, `MapPatch` or `MapMethods` in `{router.Name}` maps {(member is null ? "it" : $"`{member.Name}`")}. The model states it in the operation's Http Settings. Map it, or remove its Http Settings.");
        }

        return modelled
            ? ClaimReading.Found(endpoint, endpoint.At)
            : ClaimReading.Compared("present", endpoint.At, false,
                $"`{router.Name}.{context.Element.Name}` is mapped as an endpoint ({endpoint.Verb} {endpoint.Template}, {endpoint.At}), and the Operation has no Http Settings, so the model says it isn't one. Give it Http Settings, or stop mapping it.");
    }

    /// <summary>
    /// An operation whose endpoint's handler isn't a method of the router of its name: the lambda <c>.WithName</c> names
    /// after it or after the OpenAPI operationId its OpenAPI Settings give it, or else the only endpoint mapped with the
    /// operation's verb and route, a lambda or a method group whose
    /// method has another name (<c>CancelOrderAsync</c> for the Operation <c>CancelOrder</c>).
    /// </summary>
    private static ClaimReading? Lambda(ClaimContext context, MinimalApiModel model)
    {
        if (context.Element.SpecializationType != "Operation" || context.Type is not { } router || router.Members.Any(x => x.Name == context.Element.Name))
        {
            return null;
        }

        var endpoints = model.Endpoints.Where(x => x.Router == router.Name).ToList();
        var verb = context.Model.Stereotype(context.Element, "Http Settings", "Verb");
        var route = context.Model.Stereotype(context.Element, "Http Settings", "Route");
        // A lambda's name is its Operation's, or the OpenAPI operationId the Operation's OpenAPI Settings give it.
        var operationId = context.ClaimOn(context.Element, OpenApi.OperationIdKind)?.Expected;
        var match = endpoints.FirstOrDefault(x => x.Lambda is not null && x.Handler == context.Element.Name)
                    ?? endpoints.FirstOrDefault(x => x.Lambda is not null && operationId is { Length: > 0 } && x.Handler == operationId)
                    ?? (endpoints.Where(x => (x.Lambda is null || x.Handler is null) && string.Equals(x.Verb, verb, StringComparison.OrdinalIgnoreCase)
                                             && Route(x.TemplateIn(context.Code) ?? x.Template) == Route(route ?? "")).ToList() is [var only] ? only : null);
        return match switch
        {
            { Lambda: { } lambda } => ClaimReading.Found(lambda with { Name = context.Element.Name }),
            { Handler: { } handler } when router.Members.Where(x => x is { Kind: "method" } && x.Name == handler).ToList() is [var method] => ClaimReading.Found(method),
            _ => null
        };
    }

    /// <summary>
    /// The Minimal API router a Service is, when no type has its name: the type whose endpoints are mapped on a group whose
    /// prefix is the Service's Route (eShop's <c>OrdersApi</c> maps <c>api/orders</c>, the <c>Orders</c> Service's), as the
    /// route comparer reads both.
    /// </summary>
    internal static ClaimReading? Router(ClaimContext context)
    {
        if (context.Code.Get<MinimalApiModel>() is not { } model || context.Model.Stereotype(context.Element, "Http Service Settings", "Route") is not { Length: > 0 } route)
        {
            return null;
        }

        var names = model.Endpoints.Where(x => x.GroupIn(context.Code) is { } group && Route(group) == Route(route)).Select(x => x.Router).ToHashSet(StringComparer.Ordinal);
        return context.Code.Types.Where(x => names.Contains(x.Name)).ToList() switch
        {
            [var router] => ClaimReading.Found(router),
            [] => null,
            var several => ClaimReading.CantVerify("ambiguous", $"{several.Count} Minimal API routers map the group `{route}`.", several[0].At, several.Select(x => x.At).ToList())
        };
    }

    private static ClaimReading Unreadable(string what, Span at) =>
        ClaimReading.CantVerify("unreadable", $"{what}, which the rule can't read as a route: it isn't a literal or a string constant the code declares.", at);

    /// <summary>A template as the route comparer reads it: no slashes at its ends, no constraints, case aside.</summary>
    private static string Route(string template) =>
        System.Text.RegularExpressions.Regex.Replace(template.Trim().Trim('/'), @"(?::[^}]*?)?(\??)\}", "$1}").ToLowerInvariant();

    /// <summary>
    /// The handler's success result: <c>Task</c> unwrapped, then of <c>Results&lt;...&gt;</c> the success result, whose
    /// type argument is what it returns (<c>Ok&lt;Order&gt;</c> is Order, <c>Ok</c> nothing), or the file a file result
    /// sends. The error results aren't modelled.
    /// </summary>
    private static ClaimReading Returns(ClaimContext context, MemberNode member)
    {
        var type = member.Type;
        if (type is { Name: "Task" or "ValueTask" })
        {
            type = type.Arguments.FirstOrDefault();
        }

        // A model may state the result as the code writes it (Ok<PaginatedItems<CatalogItem>>), as one drawn from the method
        // does, rather than what it carries.
        var written = type;
        if (type is { Name: "Results" })
        {
            type = type.Arguments.FirstOrDefault(x => Success.Contains(x.Name) || Files.ContainsKey(x.Name));
        }

        if (type is not null && Files.TryGetValue(type.Name, out var file))
        {
            type = file;
        }
        else if (type is not null && Success.Contains(type.Name))
        {
            type = type.Arguments.FirstOrDefault();
        }

        return ClaimReading.Compared(member.Type?.Text ?? "void", member.At, context.ExpectedValue is ITypeReference model
            ? type is not null && type.Text != "void" && context.TypeMatches(model, type) || written is not null && context.TypeMatches(model, written)
            : type is null || type.Text == "void");
    }

    /// <summary>
    /// What a lambda that doesn't write its return type returns, read from its body: nothing, when it returns no value; for
    /// each value it does, a success result's payload (<c>TypedResults.Ok(data)</c> is data's type; the error results aren't
    /// modelled), or the value's own type, as the code gives it (a creation, a local given one, an in-scope method's result)
    /// or as a query gives it (<see cref="Sequences"/>). A value that can't be read, or values that disagree, are can't
    /// verify, never a guess.
    /// </summary>
    private static ClaimReading LambdaReturns(ClaimContext context, MinimalEndpoint endpoint, MemberNode member)
    {
        if (member.Body is not { } body)
        {
            return ClaimReading.CantVerify("unresolved", $"The lambda that handles {endpoint.Verb} {endpoint.Template} doesn't write its return type, and its body isn't read.", endpoint.At);
        }

        var returned = new List<TypeRefNode?>();
        foreach (var value in body.Returns)
        {
            var outcome = Returned(body, value);
            if (!outcome.Read)
            {
                return ClaimReading.CantVerify("unresolved",
                    $"The lambda that handles {endpoint.Verb} {endpoint.Template} doesn't write its return type, and what it returns at {value.At.File}:{value.At.Line} (`{value.Text}`) isn't read.",
                    value.At);
            }

            if (!outcome.Error)
            {
                returned.Add(outcome.Type);
            }
        }

        if (returned.Select(x => x?.Text ?? "void").Distinct(StringComparer.Ordinal).Count() > 1)
        {
            return ClaimReading.CantVerify("ambiguous",
                $"The lambda that handles {endpoint.Verb} {endpoint.Template} returns {string.Join(" or ", returned.Select(x => $"`{x?.Text ?? "nothing"}`").Distinct())}.", endpoint.At);
        }

        var type = returned.FirstOrDefault();
        return ClaimReading.Compared(type?.Text ?? "void", body.Returns.FirstOrDefault()?.At ?? member.At, context.ExpectedValue is ITypeReference model
            ? type is not null && context.TypeMatches(model, type)
            : type is null);
    }

    /// <param name="Read">Whether what it returns is known.</param>
    /// <param name="Error">An error result, which the model doesn't state.</param>
    /// <param name="Type">The success body's type; null for none.</param>
    private readonly record struct Outcome(bool Read, bool Error, TypeRefNode? Type);

    private static Outcome Returned(FunctionNode body, ValueNode value)
    {
        if ((value.Call ?? Outermost(body, value.At)) is { Receiver: "Results" or "TypedResults" } result)
        {
            if (!Success.Contains(result.Name))
            {
                return new Outcome(true, true, null);
            }

            // Created(uri, value) and CreatedAtRoute(name, values, value) put the body last.
            var payload = result.Name switch
            {
                "Ok" => result.Arguments.FirstOrDefault(),
                "Created" or "Accepted" => result.Arguments.Count > 1 ? result.Arguments[1] : null,
                "CreatedAtRoute" or "AcceptedAtRoute" => result.Arguments.Count > 2 ? result.Arguments[2] : null,
                _ => null
            };
            return payload is null ? new Outcome(true, false, null) : TypeOf(body, payload, 0) is { } type ? new Outcome(true, false, type) : default;
        }

        return TypeOf(body, value, 0) is { } plain ? new Outcome(true, false, plain) : default;
    }

    /// <summary>A value's type: as the code gives it, through what a local is given, or as the query it is gives it.</summary>
    internal static TypeRefNode? TypeOf(FunctionNode body, ValueNode value, int depth)
    {
        if (value.IsKnown && value.Types is [var known])
        {
            return known;
        }

        if (depth < 3 && value.Given.Count > 0)
        {
            var given = value.Given.Select(x => TypeOf(body, x, depth + 1)).ToList();
            return given.All(x => x is not null) && given.Select(x => x!.Text).Distinct(StringComparer.Ordinal).Count() == 1 ? given[0] : null;
        }

        return (value.Call ?? Outermost(body, value.At)) is { } call ? Sequences.Result(call) : null;
    }

    /// <summary>The call a value is, under an <c>await</c>: the outermost call within it, the last of a chain.</summary>
    internal static CallNode? Outermost(FunctionNode body, Span span) =>
        body.Calls.Where(x => !x.IsReference && span.Contains(x.At))
            .OrderBy(x => (x.At.Line, x.At.Column ?? 0))
            .ThenByDescending(x => (x.At.EndLine ?? x.At.Line, x.At.EndColumn ?? 0))
            .FirstOrDefault();

    /// <summary>The handler's parameters in order, each its name and type, without the ones the framework injects.</summary>
    private static ClaimReading Parameters(ClaimContext context, MemberNode member)
    {
        var code = member.Parameters.Where(x => !IsInjected(x, context.Code)).ToList();
        var model = context.Element.ChildElements.Where(x => x.SpecializationType == "Parameter").ToList();
        // What the request binds; a model that also lists what is injected, as one drawn from the method does, states the
        // method's whole signature, which is as true. A parameter.type claim compares its parameter's type, once.
        bool Same(IReadOnlyList<ParameterNode> parameters) => parameters.Count == model.Count && parameters.Zip(model).All(x =>
            x.First.Name.TrimStart('@') == x.Second.Name.TrimStart('@') && x.First.Type is { } type && x.Second.TypeReference is { Element: not null } modelType
            && (context.ClaimOn(x.Second, "parameter.type") is not null || context.TypeMatches(modelType, type)));
        return ClaimReading.Compared(string.Join(", ", code.Select(x => $"{x.Name}: {x.Type?.Text ?? "?"}")), member.At, Same(code) || Same(member.Parameters));
    }

    /// <summary>
    /// <c>[FromServices]</c>, the framework's own types, an interface (a service), a DbContext, and an <c>[AsParameters]</c>
    /// holder of services. An <c>[AsParameters]</c> type of request data (only simple properties) is a parameter.
    /// </summary>
    internal static bool IsInjected(ParameterNode parameter, CodeGraph code)
    {
        if (parameter.Attributes.Any(x => x.Name == "FromServices") || parameter.Type is not { } type)
        {
            return parameter.Type is not null;
        }

        var declared = code.Types.FirstOrDefault(x => x.Name == type.Name);
        if (parameter.Attributes.Any(x => x.Name == "AsParameters"))
        {
            return declared is null || code.MembersOf(declared).Any(x => x.Kind == "property" && x.Type is { } property && IsService(property, code));
        }

        return IsService(type, code)
               || declared is not null && code.Bases(declared).Any(x => x.Name.EndsWith("DbContext", StringComparison.Ordinal))
               || declared?.BaseTypes.Any(x => x.Name.EndsWith("DbContext", StringComparison.Ordinal)) == true;
    }

    private static bool IsService(TypeRefNode type, CodeGraph code) =>
        Injected.Contains(type.Name)
        || Registered(code).Contains(type.Name)
        || !RequestInterfaces.Contains(type.Name) && (code.Types.FirstOrDefault(x => x.Name == type.Name) is { } declared
            ? declared.Kind == "interface"
            : type.Name.Length > 1 && type.Name[0] == 'I' && char.IsUpper(type.Name[1]));

    /// <summary>
    /// The types the code registers with the container, which ASP.NET resolves a parameter of from it: the type arguments of
    /// <c>AddSingleton&lt;HooksRepository&gt;()</c>, <c>AddScoped&lt;IOrders, Orders&gt;()</c> and the like, anywhere in scope.
    /// </summary>
    private static IReadOnlySet<string> Registered(CodeGraph code)
    {
        if (code.Get<RegisteredServices>() is not { } registered)
        {
            code.Set(registered = new RegisteredServices(AllCalls(code)
                .Where(x => Registrations.Contains(x.Name))
                .SelectMany(x => x.TypeArguments)
                .Select(x => x.Name)
                .ToHashSet(StringComparer.Ordinal)));
        }

        return registered.Names;
    }

    /// <summary>Every call the code makes: its types' members' and its files' top-level statements' (Program.cs).</summary>
    internal static IEnumerable<CallNode> AllCalls(CodeGraph code) =>
        code.Types.SelectMany(x => x.Members).SelectMany(x => x.Calls).Concat(code.Files.Select(x => x.TopLevel).OfType<FunctionNode>().SelectMany(x => x.Calls));

    private static readonly HashSet<string> Registrations = new(StringComparer.Ordinal)
    {
        "AddSingleton", "AddScoped", "AddTransient", "TryAddSingleton", "TryAddScoped", "TryAddTransient", "AddKeyedSingleton", "AddKeyedScoped",
        "AddKeyedTransient", "AddHttpClient", "AddDbContext", "AddDbContextPool", "AddHostedService"
    };
}

/// <summary>The type names the code registers with the container (<see cref="MinimalApiClaims.IsInjected"/>), found once per run.</summary>
internal sealed record RegisteredServices(IReadOnlySet<string> Names);
