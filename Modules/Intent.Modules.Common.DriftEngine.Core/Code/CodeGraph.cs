namespace Intent.Modules.Common.DriftEngine.Code;

/// <summary>A type reference as written in code (code-reader.md, Type references).</summary>
public sealed class TypeRefNode
{
    /// <summary>The type as written, whitespace normalised: <c>List&lt;string&gt;</c>, <c>Guid?</c>.</summary>
    public required string Text { get; init; }

    /// <summary>The type's name without namespace, generic arguments or nullable marker.</summary>
    public required string Name { get; init; }

    /// <summary>The name as written, qualified if it was: <c>System.Guid</c>.</summary>
    public required string QualifiedName { get; init; }

    public IReadOnlyList<TypeRefNode> Arguments { get; init; } = [];

    public bool IsArray { get; init; }

    /// <summary>Written with <c>?</c>.</summary>
    public bool NullableMarked { get; init; }

    /// <summary><c>true</c>, <c>false</c>, or <c>unknown</c> for a reference type outside a nullable context.</summary>
    public string Nullable { get; set; } = "unknown";

    /// <summary>A predefined value type keyword, such as <c>int</c>; other value types are known from the graph.</summary>
    public bool IsPredefinedValueType { get; init; }
}

public sealed class MemberNode
{
    public required string Name { get; init; }

    /// <summary><c>property</c>, <c>field</c>, <c>method</c>, <c>constructor</c> or <c>enumMember</c>.</summary>
    public required string Kind { get; init; }

    public TypeRefNode? Type { get; init; }

    public bool IsStatic { get; init; }

    /// <summary>A method's or constructor's parameters, in order.</summary>
    public IReadOnlyList<ParameterNode> Parameters { get; init; } = [];

    public IReadOnlyList<AttributeNode> Attributes { get; init; } = [];

    /// <summary>
    /// <c>public</c>, <c>protected</c>, <c>internal</c>, <c>private</c>, or a combination as written: <c>protected internal</c>.
    /// With none written, what the language makes it: <c>private</c> for a C# class's member, <c>package</c> for Java's.
    /// </summary>
    public string Access { get; init; } = "public";

    /// <summary>
    /// The modifiers written besides access and <c>static</c>, as the language writes them: <c>override</c>, <c>virtual</c>,
    /// <c>readonly</c>, <c>abstract</c>, <c>const</c>; Java's <c>final</c>, <c>transient</c>, <c>default</c>.
    /// </summary>
    public IReadOnlyList<string> Modifiers { get; init; } = [];

    /// <summary>A constructor declared with its type: C#'s primary constructor, whose parameters a record also has as properties.</summary>
    public bool IsPrimary { get; init; }

    /// <summary>The value written with it: an enum member's (<c>Paid = 4</c>), a field's or property's initializer; null when none is.</summary>
    public ValueNode? Value { get; init; }

    /// <summary>
    /// The calls in a method's or constructor's body, so a rule can follow what it does without the syntax. A language reader
    /// sets them once it has read every type, since what a call's receiver and arguments are can depend on other types.
    /// </summary>
    public IReadOnlyList<CallNode> Calls { get; set; } = [];

    /// <summary>The types a method's or constructor's body creates (<c>new X(...)</c>).</summary>
    public IReadOnlyList<TypeRefNode> Creates { get; init; } = [];

    public required Span At { get; init; }
}

/// <summary>A call in a body: <c>AddDomainEvent(...)</c>, <c>_repository.Add(...)</c>.</summary>
public sealed class CallNode
{
    public required string Name { get; init; }

    /// <summary>What it is called on, as written; null for the type's own member, called unqualified or on <c>this</c>.</summary>
    public string? Receiver { get; init; }

    /// <summary>
    /// The type of what it is called on, when the code says: a parameter, a local, a field or property of the type (also on
    /// <c>this</c>), or a property of one of those whose type is in scope (<c>services.Mediator</c>). Null when it doesn't.
    /// </summary>
    public TypeRefNode? ReceiverType { get; init; }

    /// <summary>The arguments, in order, each with what the code says it is.</summary>
    public IReadOnlyList<ValueNode> Arguments { get; init; } = [];

    /// <summary>The call as written, whitespace normalised, for a finding to quote.</summary>
    public string Text { get; init; } = "";

    public required Span At { get; init; }
}

/// <summary>
/// A value in a body, such as a call's argument, and the types the code says it can be without running it: a creation is
/// its type; a local is what it is given before it is used (its initializer and each assignment, one type each), or its
/// declared type when one of those isn't known; a parameter, field or property is its declared type; a cast is what it
/// casts, or else the cast's type; a conditional is both its branches.
/// </summary>
public sealed class ValueNode
{
    /// <summary>The value as written, whitespace normalised.</summary>
    public required string Text { get; init; }

    /// <summary>The types it can be, one per way the code gives it; the known ones when <see cref="IsKnown"/> is false.</summary>
    public IReadOnlyList<TypeRefNode> Types { get; init; } = [];

    /// <summary>Every way the code gives it has its type in <see cref="Types"/>.</summary>
    public bool IsKnown { get; init; }

    public required Span At { get; init; }
}

public sealed class AttributeNode
{
    /// <summary>The name as used, without a namespace or the <c>Attribute</c> suffix: <c>HttpGet</c>.</summary>
    public required string Name { get; init; }

    /// <summary>The positional arguments: a string literal's value, otherwise the argument as written.</summary>
    public IReadOnlyList<string> Arguments { get; init; } = [];

    /// <summary>The named arguments, valued as positional ones: <c>Roles = "Admin"</c>.</summary>
    public IReadOnlyDictionary<string, string> Named { get; init; } = new Dictionary<string, string>();

    /// <summary>
    /// Every argument, positional then named, with its structure where <see cref="Arguments"/> and <see cref="Named"/> have
    /// only text: a list's items (<c>cascade = {ALL, MERGE}</c>), an annotation given as a value (Java's
    /// <c>@Table(indexes = @Index(columnList = "name"))</c>).
    /// </summary>
    public IReadOnlyList<AttributeValueNode> Values { get; init; } = [];

    public required Span At { get; init; }
}

/// <summary>An annotation's argument, or an item of a list given as one.</summary>
public sealed class AttributeValueNode
{
    /// <summary>The argument's name, for a named one; null for a positional argument or a list's item.</summary>
    public string? Name { get; init; }

    /// <summary>The value as written, whitespace normalised.</summary>
    public required string Text { get; init; }

    /// <summary>A literal's value: a string's content, a number or boolean as written; null for anything else.</summary>
    public string? Literal { get; init; }

    /// <summary>A list's items: C#'s <c>new[] { ... }</c> or <c>[ ... ]</c>, Java's <c>{ ... }</c>.</summary>
    public IReadOnlyList<AttributeValueNode> Items { get; init; } = [];

    /// <summary>An annotation given as the value.</summary>
    public AttributeNode? Annotation { get; init; }
}

public sealed class ParameterNode
{
    public required string Name { get; init; }

    public TypeRefNode? Type { get; init; }

    public IReadOnlyList<AttributeNode> Attributes { get; init; } = [];

    /// <summary>The default value as written (<c>pageSize = 10</c> is <c>10</c>); null when none is.</summary>
    public string? Default { get; init; }

    public required Span At { get; init; }
}

public sealed class TypeNode
{
    public required string Name { get; init; }

    public required string Namespace { get; init; }

    /// <summary><c>class</c>, <c>record</c>, <c>struct</c>, <c>interface</c> or <c>enum</c>.</summary>
    public required string Kind { get; init; }

    /// <summary>Where the type is declared; a partial type's first part.</summary>
    public required Span At { get; init; }

    /// <summary>Every file a part of the type is in.</summary>
    public List<string> Files { get; } = [];

    public bool IsAbstract { get; set; }

    public bool IsStatic { get; set; }

    /// <summary>
    /// As <see cref="MemberNode.Access"/>; with none written, what the language makes it: <c>internal</c> for a C# top-level
    /// type, <c>package</c> for Java's. A TypeScript type is <c>public</c> when it is exported.
    /// </summary>
    public string Access { get; set; } = "public";

    /// <summary>Its own generic type parameters, in order: <c>T</c> for <c>PagedResult&lt;T&gt;</c>.</summary>
    public IReadOnlyList<string> TypeParameters { get; set; } = [];

    /// <summary>The attributes on every part of the type.</summary>
    public List<AttributeNode> Attributes { get; } = [];

    public List<MemberNode> Members { get; } = [];

    /// <summary>The types it derives from or implements, as written, from every part of the type.</summary>
    public List<TypeRefNode> BaseTypes { get; } = [];
}

/// <summary>An import: a C# <c>using</c>, a Java or TypeScript <c>import</c> (code-reader.md, import nodes).</summary>
public sealed class ImportNode
{
    /// <summary>What it imports, as written without whitespace: <c>Ordering.Domain.AggregatesModel</c>.</summary>
    public required string Target { get; init; }

    /// <summary>The import as written, whitespace normalised: <c>using static System.Math;</c>.</summary>
    public required string Text { get; init; }

    public string? Alias { get; init; }

    /// <summary>It imports a type's members (<c>using static</c>, Java's <c>import static</c>).</summary>
    public bool Static { get; init; }

    /// <summary>It applies to every file of the project (<c>global using</c>).</summary>
    public bool Global { get; init; }

    public required Span At { get; init; }
}

/// <summary>An in-scope source file: its language, the namespaces it declares and what it imports.</summary>
public sealed class FileNode
{
    public required string Path { get; init; }

    public required string Language { get; init; }

    /// <summary>The project file of the build unit it belongs to (a <c>.csproj</c>), or null when none governs it.</summary>
    public string? Project { get; init; }

    /// <summary>The namespaces (packages, modules) it declares, qualified.</summary>
    public IReadOnlyList<string> Namespaces { get; init; } = [];

    public IReadOnlyList<ImportNode> Imports { get; init; } = [];
}

/// <summary>
/// What the language readers produced from the in-scope files, in no language's terms. A language keeps its own syntax
/// here too (<see cref="Set{T}"/>), for the extensions that read it.
/// </summary>
public sealed class CodeGraph
{
    private readonly Dictionary<Type, object> _extensions = [];

    /// <summary>Every in-scope source file a reader read, in path order.</summary>
    public List<FileNode> Files { get; } = [];

    public List<TypeNode> Types { get; } = [];

    /// <summary>Files a reader failed on, with its diagnostic (code-reader.md, Failures).</summary>
    public List<(string File, string Error)> Failures { get; } = [];

    /// <summary>Source files whose governing project file failed to read, so its settings are unknown.</summary>
    public Dictionary<string, string> UngovernedFiles { get; } = new(StringComparer.Ordinal);

    /// <summary>What a file's code can name without qualifying it: its own imports, and the global ones of its project.</summary>
    public IEnumerable<ImportNode> ImportsOf(FileNode file) =>
        file.Imports.Concat(file.Project is null
            ? []
            : Files.Where(x => x != file && x.Project == file.Project).SelectMany(x => x.Imports.Where(i => i.Global)));

    /// <summary>
    /// A file's code can name a namespace's types unqualified: the file imports it, or declares it or a namespace inside it.
    /// </summary>
    public bool Reaches(FileNode file, string ns) =>
        file.Namespaces.Any(x => x == ns || x.StartsWith(ns + ".", StringComparison.Ordinal)) || ImportsOf(file).Any(x => !x.Static && x.Alias is null && x.Target == ns);

    /// <summary>
    /// Everything a type derives from or implements, nearest first: an in-scope base by its qualified name
    /// (<c>Shop.Domain.SeedWork.Entity</c>), and its own bases in turn; a library base, which isn't in the graph, by its name
    /// as written and its bare name, without generic arguments (<c>IRequest</c> for <c>IRequest&lt;bool&gt;</c>). So a model
    /// that names a codebase's own base type by its qualified name isn't matched by a same-named type that isn't declared.
    /// </summary>
    public IReadOnlyList<string> Supertypes(TypeNode type)
    {
        var names = new List<string>();
        var seen = new HashSet<TypeNode> { type };
        var queue = new Queue<TypeNode>([type]);
        while (queue.TryDequeue(out var current))
        {
            foreach (var reference in current.BaseTypes)
            {
                if (Resolve(reference, current) is { } found)
                {
                    if (seen.Add(found))
                    {
                        names.Add(found.Namespace.Length > 0 ? $"{found.Namespace}.{found.Name}" : found.Name);
                        queue.Enqueue(found);
                    }
                }
                else
                {
                    names.Add(reference.QualifiedName);
                    names.Add(reference.Name);
                }
            }
        }

        return names.Distinct(StringComparer.Ordinal).ToList();
    }

    /// <summary>
    /// The in-scope types a type derives from or implements, nearest first. A base type is found by its name; of several
    /// with the name, the one in the type's namespace or an enclosing one. A library's base type isn't in the graph.
    /// </summary>
    public IReadOnlyList<TypeNode> Bases(TypeNode type)
    {
        var bases = new List<TypeNode>();
        var seen = new HashSet<TypeNode> { type };
        var queue = new Queue<TypeNode>([type]);
        while (queue.TryDequeue(out var current))
        {
            foreach (var reference in current.BaseTypes)
            {
                if (Resolve(reference, current) is { } found && seen.Add(found))
                {
                    bases.Add(found);
                    queue.Enqueue(found);
                }
            }
        }

        return bases;
    }

    /// <summary>
    /// A type's members, then the ones it inherits, nearest first: <c>Id</c> declared on an <c>Entity</c> base is a member
    /// of every entity. Constructors aren't inherited; an interface's members are inherited only by an interface.
    /// </summary>
    public IEnumerable<MemberNode> MembersOf(TypeNode type) =>
        type.Members.Concat(Bases(type)
            .Where(x => x.Kind != "interface" || type.Kind == "interface")
            .SelectMany(x => x.Members.Where(m => m.Kind != "constructor")));

    /// <summary>
    /// The calls a method or constructor makes, then those of the methods of its own type it calls, transitively, each with
    /// the method it is in: a method does what the methods it delegates to do. A call to its own type's method is
    /// unqualified, on <c>this</c>, or on the type's name, and reaches every method of the type with that name.
    /// </summary>
    /// <param name="follow">Which of those methods to follow, such as only private ones; all of them by default.</param>
    public IEnumerable<(CallNode Call, MemberNode In)> CallsFrom(TypeNode type, MemberNode member, Func<MemberNode, bool>? follow = null)
    {
        var seen = new HashSet<MemberNode> { member };
        var queue = new Queue<MemberNode>([member]);
        while (queue.TryDequeue(out var current))
        {
            foreach (var call in current.Calls)
            {
                yield return (call, current);
                if (call.Receiver is null || call.Receiver == type.Name)
                {
                    foreach (var target in MembersOf(type).Where(x => x.Kind == "method" && x.Name == call.Name && (follow?.Invoke(x) ?? true) && seen.Add(x)))
                    {
                        queue.Enqueue(target);
                    }
                }
            }
        }
    }

    /// <summary>
    /// The in-scope type a reference names, seen from code in <paramref name="from"/>: of several with the name, the one in
    /// its namespace or an enclosing one. Null for a library's type, which isn't in the graph.
    /// </summary>
    public TypeNode? TypeFor(TypeRefNode reference, TypeNode from) => Resolve(reference, from);

    private TypeNode? Resolve(TypeRefNode reference, TypeNode from)
    {
        // A qualified base (Domain.SeedWork.Entity) names the end of the type's full name.
        var candidates = Types.Where(x => x.Name == reference.Name && (reference.QualifiedName == reference.Name || $"{x.Namespace}.{x.Name}".EndsWith("." + reference.QualifiedName, StringComparison.Ordinal))).ToList();
        return candidates.Count == 1
            ? candidates[0]
            : candidates.Where(x => from.Namespace == x.Namespace || from.Namespace.StartsWith(x.Namespace + ".", StringComparison.Ordinal))
                .OrderByDescending(x => x.Namespace.Length)
                .FirstOrDefault();
    }

    /// <summary>Keeps what a language or an extension's reader found, for the readers after it; one value per type.</summary>
    public void Set<T>(T value) where T : class => _extensions[typeof(T)] = value;

    public T? Get<T>() where T : class => _extensions.GetValueOrDefault(typeof(T)) as T;
}
