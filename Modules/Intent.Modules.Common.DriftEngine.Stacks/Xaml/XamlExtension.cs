using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Stacks.Xaml;

/// <summary>
/// XAML as a language the engine reads: <c>.xaml</c> views, the markup half of a C# class (.NET MAUI, WPF, WinUI, Avalonia
/// alike). A view adds its file, its markup (<see cref="FileNode.Markup"/>) and its document (<see cref="XamlSyntax"/>), and no
/// type: its <c>x:Class</c>'s code-behind is the C# language's. Its code reader joins each view to that class. What a XAML
/// element means to a framework (a .NET MAUI Shell's items, a page's binding context) is that framework's stack's. XAML has no
/// profile: it declares no types of its own, so the code's are compared under their own language's.
/// </summary>
public sealed class XamlExtension : IDriftExtension
{
    public string Id => "intent.xaml";

    public IEnumerable<ILanguage> Languages => [new XamlLanguage()];

    public IEnumerable<ICodeReader> Readers { get; } = [new CodeBehind()];
}

internal sealed class XamlLanguage : ILanguage
{
    internal const string LanguageName = "xaml";

    /// <summary>
    /// A root that holds the application or resources for the whole of it rather than a view, which no designer has a construct
    /// for: an <c>Application</c>, or one deriving from it named so (a MAUI app's Windows head, <c>MauiWinUIApplication</c>), or a
    /// <c>ResourceDictionary</c>.
    /// </summary>
    private static bool IsResourceRoot(string name) => name is "ResourceDictionary" || name.EndsWith("Application", StringComparison.Ordinal);

    public string Name => LanguageName;

    public string Title => "XAML";

    public bool Reads(string path) => path.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Each file is parsed as the XML it is, never by patterns. One with an <c>x:Class</c> is a view: its file, markup and
    /// document. One with none, or whose root is an application (<c>App.xaml</c>, a platform head's <c>MauiWinUIApplication</c>)
    /// or a <c>ResourceDictionary</c> (a colour or style dictionary), holds the application or its resources, not a view, and
    /// adds nothing, as MVC's
    /// <c>_ViewImports.cshtml</c> doesn't: <c>unmodelled.file</c> mustn't ask for it. A file that isn't well-formed XML fails.
    /// </summary>
    public void Read(string root, IReadOnlyList<string> files, CodeGraph graph, CancellationToken cancellationToken)
    {
        var syntax = XamlSyntax.Of(graph) ?? new XamlSyntax();
        graph.Set(syntax);
        foreach (var path in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var text = File.ReadAllText(Path.Combine(root, path));
                var (document, elements, lines) = XamlDocument.Parse(text);
                if (document.Root is not { } element || IsResourceRoot(element.Name.LocalName)
                    || element.Attributes().FirstOrDefault(x => x.Name.LocalName == "Class" && XamlDocument.LanguageNamespaces.Contains(x.Name.NamespaceName))?.Value.Trim() is not { Length: > 0 } xamlClass)
                {
                    continue;
                }

                var view = new XamlDocument(path, xamlClass, element, text, elements, lines);
                var dot = xamlClass.LastIndexOf('.');
                graph.Files.Add(new FileNode
                {
                    Path = path,
                    Language = LanguageName,
                    Project = ProjectOf(root, path),
                    Namespaces = dot < 0 ? [] : [xamlClass[..dot]],
                    Markup = [Markup(view, element)]
                });
                syntax.Documents[path] = view;
            }
            catch (Exception e) when (e is XmlException or IOException or UnauthorizedAccessException)
            {
                graph.Failures.Add((path, e.Message));
            }
        }
    }

    /// <summary>
    /// An element as markup, one shape for every language (E4): its tag as written, the type an object element creates (none
    /// for a property element, <c>&lt;ContentPage.BindingContext&gt;</c>), its attributes (a plain value as a literal; a markup
    /// extension's is code, with no literal), the elements inside it and its text.
    /// </summary>
    private static MarkupNode Markup(XamlDocument view, XElement element)
    {
        var text = string.Concat(element.Nodes().OfType<XText>().Select(x => x.Value));
        var content = Regex.Replace(text, @"\s+", " ").Trim();
        return new MarkupNode
        {
            Tag = XamlDocument.Written(element.Name, element),
            Component = XamlDocument.TypeOf(element)?.Reference(),
            Attributes = element.Attributes().Where(x => !x.IsNamespaceDeclaration).Select(x => new MarkupAttributeNode
            {
                Name = XamlDocument.Written(x.Name, element),
                Value = x.Value,
                Literal = XamlMarkupExtension.Parse(x.Value) is null ? new LiteralNode { Kind = "string", Value = XamlMarkupExtension.Unescaped(x.Value) } : null,
                At = view.At(x)
            }).ToList(),
            Children = element.Elements().Select(x => Markup(view, x)).ToList(),
            Content = content.Length == 0 ? null : new LiteralNode { Kind = "string", Value = content },
            At = view.At(element),
            TextOf = () => CanonicalText.OfMarkup(view.TextOf(element))
        };
    }

    /// <summary>The project file nearest above the file, relative to the output folder with <c>/</c>; null when there is none.</summary>
    private static string? ProjectOf(string root, string path)
    {
        var folder = Path.GetDirectoryName(path)?.Replace('\\', '/') ?? "";
        while (true)
        {
            var full = Path.Combine(root, folder);
            if (Directory.Exists(full) && Directory.EnumerateFiles(full, "*.csproj").Order(StringComparer.Ordinal).FirstOrDefault() is { } project)
            {
                return (folder.Length == 0 ? "" : folder + "/") + Path.GetFileName(project);
            }

            if (folder.Length == 0)
            {
                return null;
            }

            folder = Path.GetDirectoryName(folder)?.Replace('\\', '/') ?? "";
        }
    }
}

/// <summary>
/// Joins each XAML view to its code-behind once every language has read: the class its <c>x:Class</c> names, by its full name
/// as the XAML compiler requires it. The view's file becomes one of the class's files, so whatever locates the class locates
/// its markup too, and a code-behind written with no base class (<c>partial class CatalogView</c>, its base in the markup's
/// root, <c>&lt;views:ContentPageBase x:Class="..."&gt;</c>) gets the root's type as its base, as the XAML compiler's generated
/// half gives it. A base whose type isn't in the graph is taken for an interface by C#'s naming (<c>IQueryAttributable</c>).
/// </summary>
internal sealed class CodeBehind : ICodeReader
{
    public void Read(CodeGraph graph)
    {
        if (XamlSyntax.Of(graph) is not { } syntax)
        {
            return;
        }

        var changed = false;
        foreach (var view in syntax.Documents.Values)
        {
            var name = view.Class[(view.Class.LastIndexOf('.') + 1)..];
            if (graph.TypesNamed(name).FirstOrDefault(x => x.Kind != "interface" && Qualified(x) == view.Class) is not { } type)
            {
                continue;
            }

            view.Type = type;
            if (!type.Files.Contains(view.Path))
            {
                type.Files.Add(view.Path);
                changed = true;
            }

            if (XamlDocument.TypeOf(view.Root) is { } root && !type.BaseTypes.Any(x => IsClass(graph, type, x)))
            {
                type.BaseTypes.Insert(0, root.Reference());
                changed = true;
            }
        }

        if (changed)
        {
            graph.Reindex();
        }
    }

    internal static string Qualified(TypeNode type) => type.Namespace.Length > 0 ? $"{type.Namespace}.{type.Name}" : type.Name;

    private static bool IsClass(CodeGraph graph, TypeNode type, TypeRefNode reference) =>
        graph.TypeFor(reference, type) is { } resolved
            ? resolved.Kind != "interface"
            : !(reference.Name.Length > 1 && reference.Name[0] == 'I' && char.IsUpper(reference.Name[1]));
}
