using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Stacks.Markup;
using Intent.Modules.Common.DriftEngine.Stacks.TypeScriptUi;

namespace Intent.Modules.Common.DriftEngine.Stacks.Blazor;

/// <summary>
/// What a Razor component's markup (<see cref="CodeGraph.MarkupOf"/>) says beyond the components it renders: its section
/// outlets and a layout's body, the scripts it loads, the forms it posts, and how its JavaScript reaches it (a JS interop
/// module import, or a script acting on its elements through the selectors it passes to the DOM).
/// </summary>
internal static class BlazorMarkup
{
    /// <summary>The DOM calls a script finds elements with, each with how its argument is written as a selector.</summary>
    private static readonly Dictionary<string, Func<string, string>> SelectorCalls = new(StringComparer.Ordinal)
    {
        ["querySelector"] = x => x,
        ["querySelectorAll"] = x => x,
        ["closest"] = x => x,
        ["matches"] = x => x,
        ["getElementById"] = x => "#" + x,
        ["getElementsByClassName"] = x => string.Concat(x.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(c => "." + c))
    };

    /// <summary>A plain attribute's text, when it is written as text alone; null when it is absent, a directive or computed.</summary>
    public static string? Literal(MarkupNode node, string name) =>
        node.Attributes.FirstOrDefault(x => x.Directive is null && string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase))?.Literal is { Kind: "string", Value: { } value }
            ? value
            : null;

    /// <summary>
    /// The component's <c>&lt;SectionOutlet SectionName="..."/&gt;</c>s, each with its name, or null where the code computes it:
    /// the named placement points pages fill with <c>&lt;SectionContent&gt;</c>.
    /// </summary>
    public static IReadOnlyList<(string? Name, MarkupNode Node)> SectionOutlets(CodeGraph code, TypeNode type) =>
        code.MarkupOf(type)
            .Where(x => x.Component?.Name == "SectionOutlet" || x.Tag == "SectionOutlet")
            .Select(x => (Literal(x, "SectionName"), x))
            .ToList();

    /// <summary>
    /// The component's <c>RenderFragment</c> parameters (<c>[Parameter] RenderFragment Header</c>, <c>RenderFragment&lt;T&gt;</c>):
    /// named placement points a parent fills with content, which the designer's Regions are ("a named placement point ... where
    /// content is composed"), not Properties.
    /// </summary>
    public static IReadOnlyList<MemberNode> Fragments(CodeGraph code, TypeNode type) =>
        code.MembersOf(type).Where(IsFragment).ToList();

    /// <summary>A <c>[Parameter]</c> property of type <c>RenderFragment</c> or <c>RenderFragment&lt;T&gt;</c>.</summary>
    public static bool IsFragment(MemberNode member) => member is { Kind: "property", Type.Name: "RenderFragment" } && BlazorExtension.IsParameter(member);

    /// <summary>
    /// Where a layout renders the page it lays out (<c>@Body</c>), its one placement point the code doesn't name: the element
    /// holding it, else the layout itself. Null for a component that isn't a layout.
    /// </summary>
    public static Span? Body(CodeGraph code, TypeNode type)
    {
        if (!code.Supertypes(type).Any(x => x == "LayoutComponentBase" || x.EndsWith(".LayoutComponentBase", StringComparison.Ordinal)))
        {
            return null;
        }

        return code.MarkupOf(type).FirstOrDefault(x => x.Content?.Parts.Any(p => p.Literal is null && p.Text.TrimStart('@') == "Body") == true)?.At ?? type.At;
    }

    /// <summary>
    /// The plain HTML forms the component posts, <c>&lt;form method="post" action="..."&gt;</c>, each with its action. A form
    /// with an <c>@onsubmit</c> handler posts to the component's own handler (its action is only a fallback), so it isn't one.
    /// </summary>
    public static IReadOnlyList<(LiteralNode Action, MarkupNode Node)> PostedForms(CodeGraph code, TypeNode type) =>
        code.MarkupOf(type)
            .Where(x => x.Component is null && string.Equals(x.Tag, "form", StringComparison.OrdinalIgnoreCase)
                        && string.Equals(Literal(x, "method"), "post", StringComparison.OrdinalIgnoreCase)
                        && !x.Attributes.Any(a => a.Directive == "onsubmit"))
            .Select(x => (Action: x.Attributes.FirstOrDefault(a => a.Directive is null && string.Equals(a.Name, "action", StringComparison.OrdinalIgnoreCase))?.Literal, Node: x))
            .Where(x => x.Action is not null)
            .Select(x => (x.Action!, x.Node))
            .ToList();

    /// <summary>A file a script or module is: TypeScript or JavaScript.</summary>
    public static string? ScriptFile(TypeNode type) => type.Files.FirstOrDefault(UiCode.IsTypeScript);

    /// <summary>
    /// A JS interop import of the file the component's code makes: <c>JS.InvokeAsync&lt;IJSObjectReference&gt;("import",
    /// "./Components/Chatbot/Chatbot.razor.js")</c>, the path relative to the application's root, a library's under
    /// <c>_content/{Library}/</c>.
    /// </summary>
    public static CallNode? ModuleImport(CodeGraph code, TypeNode component, string file) =>
        code.MembersOf(component).SelectMany(x => x.Calls)
            .FirstOrDefault(x => x.Name is "InvokeAsync" or "InvokeVoidAsync" && x.Arguments.Count >= 2
                                 && code.StringOf(x.Arguments[0]) is { Kind: "string", Value: "import" }
                                 && code.StringOf(x.Arguments[1]) is { Kind: "string", Value: { } path }
                                 && Serves(code, path, file, component.At.File, webRoot: false));

    /// <summary>
    /// The <c>&lt;script src="..."&gt;</c> of an in-scope component (the application's <c>App.razor</c>) that loads the file for
    /// the whole application: the source relative to the host's project's <c>wwwroot</c>, or a library's under
    /// <c>_content/{Library}/</c>.
    /// </summary>
    public static (TypeNode Host, MarkupNode Tag, string Src)? ScriptLoading(CodeGraph code, string file)
    {
        foreach (var type in code.Types.Where(BlazorExtension.IsRazor))
        {
            foreach (var tag in code.MarkupOf(type).Where(x => string.Equals(x.Tag, "script", StringComparison.OrdinalIgnoreCase)))
            {
                if (Literal(tag, "src") is { } src && Serves(code, src, file, type.At.File, webRoot: true))
                {
                    return (type, tag, src);
                }
            }
        }

        return null;
    }

    /// <summary>
    /// The URL serves the file. A library's static asset, <c>_content/{Library}/{path}</c>, is the library project's
    /// <c>wwwroot/{path}</c> or its own <c>{path}</c> (a collocated module). Otherwise the path is relative to the project the
    /// referring code is in: its <c>wwwroot</c> for a script tag, its root or <c>wwwroot</c> for a module import.
    /// </summary>
    public static bool Serves(CodeGraph code, string url, string file, string from, bool webRoot)
    {
        var path = url.Split('?', '#')[0].Replace('\\', '/');
        while (path.StartsWith("./", StringComparison.Ordinal) || path.StartsWith("~/", StringComparison.Ordinal))
        {
            path = path[2..];
        }

        path = path.TrimStart('/');
        if (path.Length == 0 || path.Contains("://", StringComparison.Ordinal))
        {
            return false;
        }

        var target = "/" + file.Replace('\\', '/');
        if (path.StartsWith("_content/", StringComparison.OrdinalIgnoreCase))
        {
            return path.Split('/', 3) is [_, var library, var rest]
                   && (target.EndsWith($"/{library}/wwwroot/{rest}", StringComparison.OrdinalIgnoreCase) || target.EndsWith($"/{library}/{rest}", StringComparison.OrdinalIgnoreCase));
        }

        var referring = "/" + from.Replace('\\', '/');
        foreach (var suffix in webRoot ? ["wwwroot/" + path] : new[] { path, "wwwroot/" + path })
        {
            if (target.EndsWith("/" + suffix, StringComparison.OrdinalIgnoreCase)
                && target[..^suffix.Length] is var root && referring.StartsWith(root, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// The selectors a script's code passes to the DOM to find its elements (<c>querySelector</c>, <c>querySelectorAll</c>,
    /// <c>closest</c>, <c>matches</c>, and <c>getElementById</c> and <c>getElementsByClassName</c> as the selectors they stand
    /// for), as far as the code gives them as text, and of those the ones that mark an element (<see cref="CssSelector.Marks"/>).
    /// </summary>
    public static IReadOnlyList<CssSelector> MarkingSelectors(CodeGraph code, string file)
    {
        var calls = code.Files.Where(x => x.Path == file).Select(x => x.TopLevel).OfType<FunctionNode>().SelectMany(x => x.Calls)
            .Concat(code.Types.Where(x => x.Files.Contains(file)).SelectMany(code.MembersOf).SelectMany(x => x.Calls));
        return calls
            .Where(x => SelectorCalls.ContainsKey(x.Name) && x.Arguments.Count > 0)
            .Select(x => code.StringOf(x.Arguments[0]) is { Kind: "string", Value: { } text } && text.Trim().Length > 0 ? SelectorCalls[x.Name](text.Trim()) : null)
            .OfType<string>()
            .SelectMany(CssSelector.ParseList)
            .Where(x => x.Marks)
            .Distinct()
            .ToList();
    }
}
