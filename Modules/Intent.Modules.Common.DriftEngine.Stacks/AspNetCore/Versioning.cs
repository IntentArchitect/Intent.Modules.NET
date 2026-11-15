using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intent.Modules.Common.DriftEngine.Stacks.AspNetCore;

/// <summary>A scheme an <c>ApiVersionReader</c> reads the version with: <c>query: version</c>, <c>url segment</c>; null when it can't be read.</summary>
internal sealed record VersionScheme(string? Scheme, string Written, bool Defaulted = false);

/// <summary>How the code configures Asp.Versioning: each <c>ApiVersionReader</c> it sets, and where it calls <c>AddApiVersioning</c>.</summary>
internal sealed class ApiVersioning
{
    public List<(IReadOnlyList<VersionScheme> Schemes, Span At)> Readers { get; } = [];

    public List<Span> Configured { get; } = [];
}

/// <summary>
/// Reads <c>options.ApiVersionReader = new QueryStringApiVersionReader("version")</c>, and <c>ApiVersionReader.Combine(...)</c>
/// of several, wherever the code sets it (an <c>AddApiVersioning</c> options callback in Program.cs, typically).
/// </summary>
internal sealed class ApiVersioningReader : ICodeReader
{
    public void Read(CodeGraph graph)
    {
        if (CSharpSyntax.Of(graph) is not { } csharp)
        {
            return;
        }

        var versioning = new ApiVersioning();
        foreach (var (file, tree) in csharp.Trees)
        {
            foreach (var node in tree.GetRoot().DescendantNodes())
            {
                switch (node)
                {
                    case AssignmentExpressionSyntax assignment when (assignment.Left as MemberAccessExpressionSyntax)?.Name.Identifier.Text == "ApiVersionReader"
                                                                    || (assignment.Left as IdentifierNameSyntax)?.Identifier.Text == "ApiVersionReader":
                        versioning.Readers.Add((Schemes(assignment.Right).ToList(), SpanOf(assignment, file)));
                        break;
                    case InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax { Name.Identifier.Text: "AddApiVersioning" } } call:
                        versioning.Configured.Add(SpanOf(call, file));
                        break;
                }
            }
        }

        graph.Set(versioning);
    }

    private static IEnumerable<VersionScheme> Schemes(ExpressionSyntax reader)
    {
        if (reader is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax { Name.Identifier.Text: "Combine" } } combine)
        {
            return combine.ArgumentList.Arguments.SelectMany(x => Schemes(x.Expression));
        }

        if (reader is not ObjectCreationExpressionSyntax { Type: var type } creation)
        {
            return [new VersionScheme(null, reader.ToString())];
        }

        var names = creation.ArgumentList?.Arguments.Select(x => x.Expression).ToList() ?? [];
        var literals = names.OfType<LiteralExpressionSyntax>().Select(x => x.Token.ValueText).ToList();
        if (literals.Count != names.Count)
        {
            return [new VersionScheme(null, reader.ToString())];
        }

        return (type.ToString().Split('.')[^1]) switch
        {
            "QueryStringApiVersionReader" when literals.Count == 0 => [new VersionScheme("query: api-version", reader.ToString(), Defaulted: true)],
            "QueryStringApiVersionReader" => literals.Select(x => new VersionScheme($"query: {x}", reader.ToString())),
            "UrlSegmentApiVersionReader" => [new VersionScheme("url segment", reader.ToString())],
            "HeaderApiVersionReader" when literals.Count == 0 => [new VersionScheme("header", reader.ToString())],
            "HeaderApiVersionReader" => literals.Select(x => new VersionScheme($"header: {x}", reader.ToString())),
            "MediaTypeApiVersionReader" => [new VersionScheme("media type", reader.ToString())],
            _ => [new VersionScheme(null, reader.ToString())]
        };
    }

    private static Span SpanOf(SyntaxNode node, string file)
    {
        var lines = node.GetLocation().GetLineSpan();
        return new Span(file, lines.StartLinePosition.Line + 1, lines.StartLinePosition.Character + 1, lines.EndLinePosition.Line + 1, lines.EndLinePosition.Character + 1);
    }
}

/// <summary>
/// The API versions an ASP.NET Core service and its endpoints serve (<c>service.versions</c>, <c>operation.versions</c>), and
/// the scheme a request states its version in (<c>api.versionScheme</c>), as Asp.Versioning reads them.
/// </summary>
internal static class Versioning
{
    /// <summary>
    /// The versions a set of declarations gives, joined; a value that isn't a version written as a literal, or a constant the
    /// code declares, is can't verify. With none, the code doesn't state any.
    /// </summary>
    public static ClaimReading Read(ClaimContext context, IReadOnlyList<VersionValue> versions, TypeNode from, Span at)
    {
        var read = new List<string>();
        foreach (var version in versions)
        {
            if ((version.Version ?? context.Code.LiteralOf(version.Written, from)?.Value) is not { } text)
            {
                return ClaimReading.CantVerify("unreadable", $"The code can't read `{version.Written}` as an API version: it isn't a literal or a constant of the in-scope code.", version.At);
            }

            read.Add(text);
        }

        return read.Count == 0 ? ClaimReading.Absent(at) : ClaimReading.Stated(string.Join(", ", read.Distinct(StringComparer.Ordinal)), versions[0].At);
    }

    /// <summary>A controller's <c>[ApiVersion]</c> attributes, its bases' included; or an action's <c>[MapToApiVersion]</c> and <c>[ApiVersion]</c>.</summary>
    public static IReadOnlyList<VersionValue> Attributes(IEnumerable<AttributeNode> attributes, params string[] names) =>
        attributes.Where(x => names.Contains(x.Name) && x.Values.Count > 0)
            .Select(x => x.Values.All(v => v.Literal is not null)
                ? new VersionValue(x.Values.Count == 1 ? x.Values[0].Literal : $"{x.Values[0].Literal}.{x.Values[1].Literal}", string.Join(", ", x.Values.Select(v => v.Text)), x.At)
                : new VersionValue(null, x.Values[0].Text, x.At))
            .ToList();

    /// <summary>
    /// The scheme the code reads a version with: the <c>ApiVersionReader</c> it sets; with none, a route that carries the
    /// version (<c>{version:apiVersion}</c>), which Asp.Versioning's default reader reads as a URL segment. Its default
    /// query string parameter isn't stated, so a model's scheme is contradicted as the code not stating it (decision 4).
    /// </summary>
    public static ClaimReading? Scheme(ClaimContext context)
    {
        var versioning = context.Code.Get<ApiVersioning>();
        if (versioning is { Readers.Count: > 1 })
        {
            return ClaimReading.CantVerify("ambiguous", $"The code sets the ApiVersionReader {versioning.Readers.Count} times.", versioning.Readers[0].At, versioning.Readers.Select(x => x.At).ToList());
        }

        if (versioning is { Readers: [var (schemes, at)] })
        {
            if (schemes.FirstOrDefault(x => x.Scheme is null) is { } unread)
            {
                return ClaimReading.CantVerify("unreadable", $"The code can't read `{unread.Written}` as an ApiVersionReader this extension knows.", at);
            }

            return schemes is [{ Defaulted: true } only]
                ? ClaimReading.Defaulted(only.Scheme!, at)
                : schemes.Count == 1
                    ? ClaimReading.Stated(schemes[0].Scheme!, at)
                    : ClaimReading.Compared(string.Join(" + ", schemes.Select(x => x.Scheme)), at, schemes.Any(x => x.Scheme == context.Expected));
        }

        if (RouteVersion(context.Code, context.Type) is { } segment)
        {
            return ClaimReading.Stated("url segment", segment);
        }

        return (versioning?.Configured.FirstOrDefault() ?? context.Type?.At) is { } configured
            ? ClaimReading.Absent(configured)
            : ClaimReading.CantVerify("notFound", "The code configures no API versioning.");
    }

    /// <summary>Where a route carries the API version (<c>{version:apiVersion}</c>): the service's, or any when the claim locates none.</summary>
    private static Span? RouteVersion(CodeGraph code, TypeNode? type)
    {
        const string constraint = ":apiVersion";
        foreach (var endpoint in code.Get<MinimalApiModel>()?.Endpoints.Where(x => type is null || x.Router == type.Name) ?? [])
        {
            if ((endpoint.GroupIn(code) ?? endpoint.Group ?? "").Contains(constraint, StringComparison.OrdinalIgnoreCase))
            {
                return endpoint.GroupAt ?? endpoint.At;
            }

            if ((endpoint.TemplateIn(code) ?? endpoint.Template).Contains(constraint, StringComparison.OrdinalIgnoreCase))
            {
                return endpoint.At;
            }
        }

        foreach (var candidate in type is null ? code.Types : [type])
        {
            if (candidate.Attributes.Concat(candidate.Members.SelectMany(x => x.Attributes))
                    .FirstOrDefault(x => x.Arguments.Any(a => a.Contains(constraint, StringComparison.OrdinalIgnoreCase))) is { } route)
            {
                return route.At;
            }
        }

        return null;
    }
}
