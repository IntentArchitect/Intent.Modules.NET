using System.Runtime.CompilerServices;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Stacks.Markup;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intent.Modules.Common.DriftEngine.Stacks.CSharp;

/// <summary>
/// A Razor component (<c>.razor</c>) as the C# compiler sees it: the C# the Razor compiler generates for it, with its
/// <c>_Imports.razor</c> files, parsed with Roslyn. The generated code places its <c>@code</c>, <c>@using</c> and
/// <c>@attribute</c> in the <c>.razor</c> file they are written in with <c>#line</c>, which <see cref="SpanOf"/> follows.
/// What a directive generates without a place (<c>@page</c>'s <c>[Route]</c>, <c>@layout</c>'s <c>[Layout]</c>,
/// <c>@inject</c>'s property) is placed at the directive, and the component's class at the whole file. Its markup is read as
/// Razor parses it (<see cref="Markup"/>), not from the code generated for it: without the referenced assemblies'
/// components the compiler can't tell a component's parameters from HTML.
/// <para>
/// An MVC view or Razor Page (<c>.cshtml</c>) is read the same way, as a component of its own named after its file: it is
/// Razor too, and only the component file kind builds an element tree (the MVC file kind writes markup out as text). Its
/// <c>@model</c> and <c>@section</c> are known as the directives they are in an MVC document (<see cref="Directives"/>), and
/// no <c>_Imports.razor</c> applies to it. Tag helpers aren't bound, so <c>asp-action</c>, <c>&lt;partial name&gt;</c> and
/// <c>~/</c> URLs stay as written.
/// </para>
/// </summary>
internal sealed class RazorFile
{
    private static readonly ConditionalWeakTable<SyntaxTree, RazorFile> Generated = new();

    private readonly string _text;
    private readonly IReadOnlyList<RazorDirective> _directives;

    private RazorFile(string path, string text, SyntaxTree tree, IReadOnlyList<RazorDirective> directives, IReadOnlyList<string> imports, IReadOnlyList<MarkupNode> markup)
    {
        Path = path;
        _text = text;
        Tree = tree;
        _directives = directives;
        ImportsFrom = imports;
        Markup = markup;
        IsView = IsCshtml(path);
        IsImports = !IsView && FileKinds.IsComponentImport(FileKinds.GetComponentFileKindFromFilePath(path));
        Component = IsImports
            ? null
            : tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault(x => x.Parent is BaseNamespaceDeclarationSyntax or CompilationUnitSyntax);
    }

    public string Path { get; }

    /// <summary>An MVC view or Razor Page (<c>.cshtml</c>), not a Razor component.</summary>
    public bool IsView { get; }

    /// <summary>
    /// The directives written in it (its imports' too, for a component), each with its tokens and where it is written:
    /// <c>@page "/orders"</c>, an MVC view's <c>@model LoginViewModel</c> and <c>@section scripts</c>.
    /// </summary>
    public IReadOnlyList<RazorDirective> Directives => _directives;

    /// <summary>A file the C# language reads as an MVC view or Razor Page.</summary>
    public static bool IsCshtml(string path) => path.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// An MVC file that configures the views of its folder and holds no view: <c>_ViewImports.cshtml</c> (their imports and
    /// tag helpers) or <c>_ViewStart.cshtml</c> (code run before each, which sets their layout).
    /// </summary>
    public static bool IsViewConfiguration(string path) =>
        System.IO.Path.GetFileName(path) is var name && (name.Equals("_ViewImports.cshtml", StringComparison.OrdinalIgnoreCase) || name.Equals("_ViewStart.cshtml", StringComparison.OrdinalIgnoreCase));

    /// <summary>The C# generated for it, its file path the <c>.razor</c> file's.</summary>
    public SyntaxTree Tree { get; }

    /// <summary>An <c>_Imports.razor</c>: what it has applies to the components in its folder and below, and it declares no type.</summary>
    public bool IsImports { get; }

    /// <summary>The component's class; null for an <c>_Imports.razor</c>.</summary>
    public ClassDeclarationSyntax? Component { get; }

    /// <summary>The <c>_Imports.razor</c> files that apply to it, by path: its folder's and those of the folders above it, in its project.</summary>
    public IReadOnlyList<string> ImportsFrom { get; }

    /// <summary>
    /// The elements written at its top level, as Razor parses them: an element inside an <c>@foreach</c> or <c>@if</c> is
    /// inside the element that holds the code. A tag that starts with a capital letter, or is qualified, names a component.
    /// </summary>
    public IReadOnlyList<MarkupNode> Markup { get; }

    /// <summary>The whole file, where the component's class is.</summary>
    public Span At => SpanAt(Path, _text, 0, _text.TrimEnd().Length);

    /// <summary>The file as written, read as any file's text is (code-reader.md, Canonical text).</summary>
    public string Canonical => CanonicalText.OfText(_text);

    /// <summary>The Razor file a syntax tree was generated for; null for a <c>.cs</c> file's.</summary>
    public static RazorFile? Of(SyntaxTree tree) => Generated.TryGetValue(tree, out var file) ? file : null;

    /// <summary>
    /// Generates <paramref name="path"/>'s C#, with each <c>_Imports.razor</c> from its folder up to its project's.
    /// </summary>
    /// <param name="root">The output folder.</param>
    /// <param name="path">The file, relative to <paramref name="root"/> with <c>/</c> separators.</param>
    /// <param name="project">Its project's folder, relative to <paramref name="root"/>; null when no project governs it.</param>
    /// <param name="engine">Its project's Razor engine, with the project's root namespace.</param>
    public static RazorFile Generate(string root, string path, string? project, RazorProjectEngine engine)
    {
        var text = File.ReadAllText(System.IO.Path.Combine(root, path));
        var view = IsCshtml(path);
        var imports = view ? [] : Imports(root, path, project).ToList();
        var texts = imports.ToDictionary(x => x.Path, x => x.Text, StringComparer.Ordinal);
        texts[path] = text;

        var document = engine.Process(Source(path, text, project), view ? FileKinds.Component : FileKinds.GetComponentFileKindFromFilePath(path),
            imports.Select(x => Source(x.Path, x.Text, project)).ToList(), []);
        var parsed = document.Items[typeof(Capture)] as Captured;
        var directives = (parsed?.Directives ?? [])
            .Where(x => x.Source is { } source && texts.ContainsKey(source.FilePath))
            .Select(x =>
            {
                var source = x.Source!.Value;
                var written = texts[source.FilePath];
                return new RazorDirective(x.Name, x.Tokens,
                    SpanAt(source.FilePath, written, source.AbsoluteIndex, source.AbsoluteIndex + written.Substring(source.AbsoluteIndex, source.Length).TrimEnd().Length),
                    written.Substring(source.AbsoluteIndex, source.Length).Trim());
            })
            .ToList();

        // An _Imports.razor's markup isn't its components'.
        var markup = (parsed?.Elements ?? []).Where(x => x.File == path).Select(x => MarkupOf(x, path, text)).ToList();
        var tree = CSharpSyntaxTree.ParseText(document.GetCSharpDocument().GeneratedCode, path: path);
        var file = new RazorFile(path, text, tree, directives, imports.Select(x => x.Path).ToList(), markup);
        Generated.AddOrUpdate(tree, file);
        return file;
    }

    private static MarkupNode MarkupOf(RazorElement element, string path, string text) => new()
    {
        Tag = element.Tag,
        Component = char.IsUpper(element.Tag[0]) || element.Tag.Contains('.')
            ? new TypeRefNode { Text = element.Tag, Name = element.Tag[(element.Tag.LastIndexOf('.') + 1)..], QualifiedName = element.Tag, Language = CSharpLanguage.LanguageName }
            : null,
        Attributes = element.Attributes.Select(x => AttributeOf(x, path, text)).ToList(),
        Children = element.Children.Select(x => MarkupOf(x, path, text)).ToList(),
        Content = element.Content is { } content ? MarkupParts.Content(content.Select(x => PartOf(x, path, text)).ToList()) : null,
        At = SpanAt(path, text, element.Start, element.End),
        TextOf = () => CanonicalText.OfMarkup(text[element.Start..element.End])
    };

    /// <summary>
    /// An attribute: a directive attribute (<c>@bind-Value:event</c>) is its directive (<c>bind</c>), argument
    /// (<c>Value</c>) and modifiers (<c>event</c>), and its value is code; a plain one's value is its literal.
    /// </summary>
    private static MarkupAttributeNode AttributeOf(RazorAttribute attribute, string path, string text)
    {
        var written = text[attribute.Start..attribute.End];
        var value = attribute.Prefix.Contains('=') && written.Length >= attribute.Prefix.Length + attribute.Suffix.Length
            ? written[attribute.Prefix.Length..^attribute.Suffix.Length]
            : null;
        string? directive = null, argument = null;
        IReadOnlyList<string> modifiers = [];
        if (attribute.Name.StartsWith('@'))
        {
            var segments = attribute.Name[1..].Split(':');
            var dash = segments[0].IndexOf('-');
            (directive, argument) = dash < 0 ? (segments[0], null) : (segments[0][..dash], segments[0][(dash + 1)..]);
            modifiers = segments[1..];
        }

        return new MarkupAttributeNode
        {
            Name = attribute.Name,
            Directive = directive,
            Argument = argument,
            Modifiers = modifiers,
            Value = value,
            Literal = directive is null && value is not null && attribute.Value is { } parts ? MarkupParts.Value(parts.Select(x => PartOf(x, path, text)).ToList()) : null,
            At = SpanAt(path, text, attribute.Start + (written.Length - written.TrimStart().Length), attribute.End)
        };
    }

    private static MarkupParts.Part PartOf(RazorPart part, string path, string text) => new(part.Code, part.Text, SpanAt(path, text, part.Start, part.End));

    /// <summary>
    /// A Razor engine for a project: components, with the project's root namespace when it has one. For views, it also knows
    /// the directives an MVC document has that a component doesn't, <c>@model</c> and <c>@section</c>, so they are read as
    /// directives, not as code and text.
    /// </summary>
    public static RazorProjectEngine Engine(string root, string? rootNamespace, bool views = false) =>
        RazorProjectEngine.Create(RazorConfiguration.Default, RazorProjectFileSystem.Create(root), builder =>
        {
            if (rootNamespace is not null)
            {
                builder.SetRootNamespace(rootNamespace);
            }

            if (views)
            {
                builder.AddDirective(DirectiveDescriptor.CreateDirective("model", DirectiveKind.SingleLine, x =>
                {
                    x.AddTypeToken();
                    x.Usage = DirectiveUsage.FileScopedSinglyOccurring;
                }), FileKinds.Component);
                builder.AddDirective(DirectiveDescriptor.CreateRazorBlockDirective("section", x => x.AddMemberToken()), FileKinds.Component);
            }

            builder.Features.Add(new Capture());
        });

    /// <summary>
    /// The node is written in a Razor file: placed there by <c>#line</c>, or an <c>@inject</c>'s property. The class's
    /// <c>BuildRenderTree</c>, which the compiler writes from the markup, isn't.
    /// </summary>
    public bool IsWritten(SyntaxNode node) => node.GetLocation().GetMappedLineSpan().HasMappedPath || Injected(node) is not null;

    /// <summary>A <c>using</c> its own <c>@using</c> wrote, not an <c>_Imports.razor</c>'s or one the compiler adds.</summary>
    public bool IsOwn(UsingDirectiveSyntax directive) => directive.GetLocation().GetMappedLineSpan() is { HasMappedPath: true } span && span.Path == Path;

    /// <summary>Where a node of the generated code is written (code-reader.md, Locations).</summary>
    public Span SpanOf(SyntaxNode node)
    {
        // The class starts at its first attribute, which may be an @attribute's, but it is the whole file.
        if (node == Component)
        {
            return At;
        }

        var mapped = node.GetLocation().GetMappedLineSpan();
        if (mapped.HasMappedPath)
        {
            return new Span(mapped.Path, mapped.StartLinePosition.Line + 1, mapped.StartLinePosition.Character + 1, mapped.EndLinePosition.Line + 1, mapped.EndLinePosition.Character + 1);
        }

        return (Injected(node) ?? Attributed(node))?.At ?? At;
    }

    /// <summary>The canonical text of a member: an <c>@inject</c>'s as its directive is written.</summary>
    public string? TextOf(SyntaxNode node) => Injected(node) is { } directive ? CanonicalText.OfText(directive.Text) : null;

    /// <summary>The <c>@inject</c> an <c>[Inject]</c> property, or a node in it, was generated for.</summary>
    private RazorDirective? Injected(SyntaxNode node) =>
        node.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>().FirstOrDefault() is { } property && !property.GetLocation().GetMappedLineSpan().HasMappedPath
            ? _directives.FirstOrDefault(x => x.Name == "inject" && x.Tokens is [_, var name] && name == property.Identifier.Text)
            : null;

    /// <summary>The directive a generated attribute stands for: <c>@page</c> for a <c>[Route]</c>, <c>@layout</c> for a <c>[Layout]</c>.</summary>
    private RazorDirective? Attributed(SyntaxNode node)
    {
        if (node.AncestorsAndSelf().OfType<AttributeSyntax>().FirstOrDefault() is not { } attribute)
        {
            return null;
        }

        var name = attribute.Name.ToString();
        if (name.EndsWith("RouteAttribute", StringComparison.Ordinal))
        {
            var route = attribute.ArgumentList?.Arguments.FirstOrDefault()?.Expression is LiteralExpressionSyntax literal ? literal.Token.ValueText : null;
            var pages = _directives.Where(x => x.Name == "page").ToList();
            return pages.FirstOrDefault(x => x.Tokens is [var written] && written.Trim('"') == route) ?? pages.FirstOrDefault();
        }

        return name.EndsWith("LayoutAttribute", StringComparison.Ordinal) ? _directives.LastOrDefault(x => x.Name == "layout") : null;
    }

    /// <summary>The <c>_Imports.razor</c> files that apply to a file, its project's outermost first, as Razor orders them.</summary>
    private static IEnumerable<(string Path, string Text)> Imports(string root, string path, string? project)
    {
        var folders = new List<string>();
        var folder = System.IO.Path.GetDirectoryName(path)?.Replace('\\', '/') ?? "";
        while (true)
        {
            folders.Add(folder);
            if (folder == (project ?? "") || folder.Length == 0)
            {
                break;
            }

            folder = System.IO.Path.GetDirectoryName(folder)?.Replace('\\', '/') ?? "";
        }

        foreach (var import in Enumerable.Reverse(folders).Select(x => x.Length == 0 ? "_Imports.razor" : $"{x}/_Imports.razor"))
        {
            if (import != path && File.Exists(System.IO.Path.Combine(root, import)))
            {
                yield return (import, File.ReadAllText(System.IO.Path.Combine(root, import)));
            }
        }
    }

    /// <summary>
    /// A file to compile: its path, for the generated code's <c>#line</c>, and its path in its project, which gives its
    /// namespace (the project's root namespace and its folders).
    /// </summary>
    private static RazorSourceDocument Source(string path, string text, string? project) =>
        RazorSourceDocument.Create(text, new RazorSourceDocumentProperties(path, project is null or "" ? path : path[(project.Length + 1)..]));

    private static Span SpanAt(string path, string text, int start, int end)
    {
        var (line, column) = Position(text, start);
        var (endLine, endColumn) = Position(text, Math.Max(start, end));
        return new Span(path, line, column, endLine, endColumn);
    }

    private static (int Line, int Column) Position(string text, int index)
    {
        var line = 1;
        var lineStart = 0;
        for (var i = 0; i < index && i < text.Length; i++)
        {
            if (text[i] == '\n')
            {
                line++;
                lineStart = i + 1;
            }
        }

        return (line, index - lineStart + 1);
    }

    /// <summary>A directive as written: its name (<c>page</c>), its tokens (<c>"/orders"</c>, quotes kept), where it is and its text.</summary>
    internal sealed record RazorDirective(string Name, IReadOnlyList<string> Tokens, Span At, string Text);

    /// <summary>What Razor parsed: its directives, and its elements as they were before the compiler's passes.</summary>
    private sealed record Captured(IReadOnlyList<CapturedDirective> Directives, IReadOnlyList<RazorElement> Elements);

    /// <summary>A directive as Razor parsed it: its name, its tokens' content and where it is.</summary>
    private sealed record CapturedDirective(string Name, IReadOnlyList<string> Tokens, SourceSpan? Source);

    /// <summary>An element as Razor parsed it, by absolute positions in <see cref="File"/>; <see cref="Content"/> is null when it holds an element or code.</summary>
    private sealed record RazorElement(string Tag, string File, int Start, int End, IReadOnlyList<RazorAttribute> Attributes, IReadOnlyList<RazorElement> Children, IReadOnlyList<RazorPart>? Content);

    /// <summary>An attribute as written: its text before and after its value (<c> Item="</c> and <c>"</c>), and its value's parts; null parts for code that isn't a value.</summary>
    private sealed record RazorAttribute(string Name, string Prefix, string Suffix, int Start, int End, IReadOnlyList<RazorPart>? Value);

    private sealed record RazorPart(bool Code, string Text, int Start, int End);

    /// <summary>
    /// Keeps what Razor parsed, with its places, before the compiler's own passes turn directives into code and drop where
    /// they were written, and fold and encode the markup: every directive, and every element as a tree.
    /// </summary>
    private sealed class Capture : IntermediateNodePassBase, IRazorDocumentClassifierPass
    {
        public override int Order => int.MinValue;

        // A view's directive the component file kind doesn't take as written is kept too: a Razor Page's @page has no route
        // of its own to give, which a component's must.
        protected override void ExecuteCore(RazorCodeDocument codeDocument, DocumentIntermediateNode documentNode) =>
            codeDocument.Items[typeof(Capture)] = new Captured(
                documentNode.FindDescendantNodes<DirectiveIntermediateNode>()
                    .Select(x => new CapturedDirective(x.DirectiveName, x.Tokens.Select(t => t.Content).ToList(), x.Source))
                    .Concat(IsCshtml(codeDocument.Source.FilePath ?? "")
                        ? documentNode.FindDescendantNodes<MalformedDirectiveIntermediateNode>()
                            .Select(x => new CapturedDirective(x.DirectiveName, x.Tokens.Select(t => t.Content).ToList(), x.Source))
                        : [])
                    .ToList(),
                Elements(documentNode).ToList());

        /// <summary>The elements inside a node, those inside its code included, not those inside them.</summary>
        private static IEnumerable<RazorElement> Elements(IntermediateNode node)
        {
            foreach (var child in node.Children)
            {
                if (child is MarkupElementIntermediateNode { Source: { } source } element)
                {
                    yield return new RazorElement(element.TagName, source.FilePath, source.AbsoluteIndex, source.AbsoluteIndex + source.Length,
                        element.Children.OfType<HtmlAttributeIntermediateNode>().Where(x => x.Source is not null).Select(Attribute).ToList(),
                        Elements(element).ToList(), Content(element));
                }
                else if (child is not HtmlAttributeIntermediateNode)
                {
                    foreach (var inner in Elements(child))
                    {
                        yield return inner;
                    }
                }
            }
        }

        private static IReadOnlyList<RazorPart>? Content(MarkupElementIntermediateNode element)
        {
            var parts = new List<RazorPart>();
            foreach (var child in element.Children.Where(x => x is not HtmlAttributeIntermediateNode))
            {
                switch (child)
                {
                    case HtmlContentIntermediateNode { Source: { } source }:
                        parts.Add(new RazorPart(false, Tokens(child), source.AbsoluteIndex, source.AbsoluteIndex + source.Length));
                        break;
                    case CSharpExpressionIntermediateNode { Source: { } source }:
                        parts.Add(new RazorPart(true, Tokens(child).Trim(), source.AbsoluteIndex, source.AbsoluteIndex + source.Length));
                        break;
                    default:
                        return null;
                }
            }

            return parts;
        }

        private static RazorAttribute Attribute(HtmlAttributeIntermediateNode attribute)
        {
            var source = attribute.Source!.Value;
            var parts = new List<RazorPart>();
            foreach (var value in attribute.Children)
            {
                // A value's source starts at the whitespace before it, which is its prefix.
                var (prefix, start, end) = value.Source is { } at ? (Prefix(value), at.AbsoluteIndex, at.AbsoluteIndex + at.Length) : ("", source.AbsoluteIndex, source.AbsoluteIndex);
                switch (value)
                {
                    case HtmlAttributeValueIntermediateNode:
                        parts.Add(new RazorPart(false, prefix + Tokens(value), start, end));
                        break;
                    case CSharpExpressionAttributeValueIntermediateNode:
                        if (prefix.Length > 0)
                        {
                            parts.Add(new RazorPart(false, prefix, start, start + prefix.Length));
                        }

                        parts.Add(new RazorPart(true, Tokens(value).Trim(), start + prefix.Length, end));
                        break;
                    default:
                        return new RazorAttribute(attribute.AttributeName, attribute.Prefix, attribute.Suffix, source.AbsoluteIndex, source.AbsoluteIndex + source.Length, null);
                }
            }

            return new RazorAttribute(attribute.AttributeName, attribute.Prefix, attribute.Suffix, source.AbsoluteIndex, source.AbsoluteIndex + source.Length, parts);
        }

        private static string Prefix(IntermediateNode value) => value switch
        {
            HtmlAttributeValueIntermediateNode x => x.Prefix ?? "",
            CSharpExpressionAttributeValueIntermediateNode x => x.Prefix ?? "",
            _ => ""
        };

        private static string Tokens(IntermediateNode node) => string.Concat(node.Children.OfType<IntermediateToken>().Select(x => x.Content));
    }
}
