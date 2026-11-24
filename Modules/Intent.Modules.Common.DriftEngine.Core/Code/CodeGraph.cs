using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

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

    /// <summary>
    /// <c>true</c>, <c>false</c>, or <c>unknown</c> where the language doesn't state it: a C# reference type outside a
    /// nullable context, an unannotated Java one.
    /// </summary>
    public string Nullable { get; set; } = "unknown";

    /// <summary>A predefined value type keyword, such as <c>int</c>; other value types are known from the graph.</summary>
    public bool IsPredefinedValueType { get; init; }

    /// <summary>
    /// A union's members, in order, as written: <c>string</c> and <c>null</c> for TypeScript's <c>string | null</c>. Empty for
    /// a type that isn't a union. The profile's <c>nullable.unionMembers</c> (<c>null</c>, <c>undefined</c>) only make it
    /// nullable, so <c>string | null</c> compares as <c>string</c>.
    /// </summary>
    public IReadOnlyList<TypeRefNode> Union { get; init; } = [];

    /// <summary>The value a literal type is: <c>Draft</c> for TypeScript's <c>'Draft'</c>, <c>4</c> for <c>4</c>.</summary>
    public LiteralNode? Literal { get; init; }

    /// <summary>
    /// A type the code doesn't name, only shapes: C#'s <c>new { Id = id }</c>. It is its <see cref="Members"/>, and matches a
    /// modelled type of the same shape (<see cref="TypeComparer"/>).
    /// </summary>
    public bool IsAnonymous { get; init; }

    /// <summary>An anonymous type's members, in order, each with its type when the code says it; empty for any other type.</summary>
    public IReadOnlyList<TypeMemberNode> Members { get; init; } = [];

    /// <summary>
    /// The language it is written in, as its reader says: it is compared under that language's profile, so one run reads
    /// several languages. Null: the run's primary language.
    /// </summary>
    public string? Language { get; init; }
}

/// <summary>A member of an anonymous type: its name, and its type when the code says it.</summary>
public sealed record TypeMemberNode(string Name, TypeRefNode? Type);

/// <summary>
/// A member as written. A record only so a refiner (<see cref="Extensions.IMemberRefiner"/>) can give it as the house reads
/// it, with <c>with</c>; as a node of the graph, it is equal only to itself.
/// </summary>
public sealed record MemberNode
{
    public required string Name { get; init; }

    /// <summary><c>property</c>, <c>field</c>, <c>method</c>, <c>constructor</c> or <c>enumMember</c>; a Vue component's emits are <c>event</c>s.</summary>
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

    /// <summary>A method's own generic type parameters, in order: <c>T</c> for <c>T Find&lt;T&gt;(Guid id)</c>.</summary>
    public IReadOnlyList<string> TypeParameters { get; init; } = [];

    /// <summary>
    /// The value written with it: an enum member's (<c>Paid = 4</c>), a field's or property's initializer; null when none is.
    /// A language reader can set it again once it has read every type, as it does <see cref="Body"/>, so an initializer has
    /// what its names refer to (<see cref="ValueNode.Field"/>).
    /// </summary>
    public ValueNode? Value { get; set; }

    /// <summary>
    /// What a method's or constructor's body does, as a unit; null for a member without one. A language reader sets it once
    /// it has read every type, since what a call's receiver and arguments are can depend on other types.
    /// </summary>
    public FunctionNode? Body { get; set; }

    /// <summary>The calls in a method's or constructor's body (<see cref="Body"/>), so a rule can follow what it does without the syntax.</summary>
    public IReadOnlyList<CallNode> Calls => Body?.Calls ?? [];

    /// <summary>The creations in a method's or constructor's body (<c>new X(...)</c>), in the order they are written.</summary>
    public IReadOnlyList<CreationNode> Creations => Body?.Creations ?? [];

    /// <summary>The types a method's or constructor's body creates (<c>new X(...)</c>).</summary>
    public IReadOnlyList<TypeRefNode> Creates => Creations.Select(x => x.Type).ToList();

    public required Span At { get; init; }

    private string? _text;

    /// <summary>
    /// Its canonical text (code-reader.md, Canonical text): as written, attributes and body included, with comments dropped
    /// and only the spaces words need, so a find path's regular expression reads it whatever the layout.
    /// </summary>
    public string Text => _text ??= TextOf?.Invoke() ?? "";

    /// <summary>Gives <see cref="Text"/> the first time it is read; the language sets it.</summary>
    public Func<string>? TextOf { get; init; }

    public bool Equals(MemberNode? other) => ReferenceEquals(this, other);

    public override int GetHashCode() => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this);
}

/// <summary>Canonical text (code-reader.md): tokens joined with no whitespace, except one space between two words.</summary>
public static class CanonicalText
{
    /// <param name="tokens">Each token's text, and whether it joins the one before as written, as an interpolated string's parts do.</param>
    public static string Of(IEnumerable<(string Text, bool Joined)> tokens)
    {
        var text = new System.Text.StringBuilder();
        foreach (var (token, joined) in tokens)
        {
            if (token.Length == 0)
            {
                continue;
            }

            if (!joined && text.Length > 0 && IsWordEnd(text[^1]) && IsWordStart(token[0]))
            {
                text.Append(' ');
            }

            text.Append(token);
        }

        return text.ToString();
    }

    /// <summary>
    /// Text that isn't read as tokens, such as a Razor file's, as Common.Drift reads any file: each run of whitespace
    /// removed, or one space where it separates two words, so <c>@page "/orders"</c> reads <c>@page"/orders"</c>.
    /// </summary>
    public static string OfText(string source)
    {
        var text = new System.Text.StringBuilder();
        for (var i = 0; i < source.Length; i++)
        {
            if (!char.IsWhiteSpace(source[i]))
            {
                text.Append(source[i]);
                continue;
            }

            var end = i;
            while (end + 1 < source.Length && char.IsWhiteSpace(source[end + 1]))
            {
                end++;
            }

            if (text.Length > 0 && end + 1 < source.Length && IsWordChar(text[^1]) && IsWordChar(source[end + 1]))
            {
                text.Append(' ');
            }

            i = end;
        }

        return text.ToString();
    }

    /// <summary>
    /// Markup, such as an element's (code-reader.md, Canonical text): each run of whitespace reduced to one space, and none
    /// beside <c>&lt;</c> or <c>&gt;</c>, or before <c>/&gt;</c>, so text content is trimmed and
    /// <c>&lt;PageTitle&gt; Shopping Bag | AdventureWorks &lt;/PageTitle&gt;</c> reads
    /// <c>&lt;PageTitle&gt;Shopping Bag | AdventureWorks&lt;/PageTitle&gt;</c>.
    /// </summary>
    public static string OfMarkup(string source)
    {
        var text = new System.Text.StringBuilder();
        for (var i = 0; i < source.Length; i++)
        {
            if (!char.IsWhiteSpace(source[i]))
            {
                text.Append(source[i]);
                continue;
            }

            var end = i;
            while (end + 1 < source.Length && char.IsWhiteSpace(source[end + 1]))
            {
                end++;
            }

            var next = end + 1 < source.Length ? source[end + 1] : '\0';
            if (text.Length > 0 && next != '\0' && text[^1] is not ('<' or '>') && next is not ('<' or '>') &&
                !(next == '/' && end + 2 < source.Length && source[end + 2] == '>'))
            {
                text.Append(' ');
            }

            i = end;
        }

        return text.ToString();
    }

    private static bool IsWordEnd(char c) => IsWordChar(c) || c is '"' or '\'';

    private static bool IsWordStart(char c) => IsWordChar(c) || c is '"' or '\'' or '@' or '$';

    private static bool IsWordChar(char c) => char.IsLetterOrDigit(c) || c == '_';
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

    /// <summary>
    /// The call it is made on, when what it is called on is a call's result: <c>build</c> in
    /// <c>ResponseEntity.noContent().build()</c> is made on <c>noContent</c>, which is made on <c>ResponseEntity</c>. Each
    /// call of a chain is also one of the body's calls.
    /// </summary>
    public CallNode? On { get; init; }

    /// <summary>The arguments, in order, each with what the code says it is.</summary>
    public IReadOnlyList<ValueNode> Arguments { get; init; } = [];

    /// <summary>
    /// The type arguments it is given, as written: <c>IOrderRepository</c> and <c>OrderRepository</c> for
    /// <c>AddScoped&lt;IOrderRepository, OrderRepository&gt;()</c>, <c>T</c> for Java's <c>this.&lt;T&gt;find(id)</c>. None when
    /// the code gives none, which leaves the compiler to infer them.
    /// </summary>
    public IReadOnlyList<TypeRefNode> TypeArguments { get; init; } = [];

    /// <summary>
    /// A method reference, not a call made here: Java's <c>orders::place</c>, a C# method group passed as a value. What it is
    /// passed to calls it, so it has no arguments, and it reaches any overload.
    /// </summary>
    public bool IsReference { get; init; }

    /// <summary>The in-scope method it reaches (<see cref="CodeGraph.Targets"/>), when exactly one fits; null for a library's.</summary>
    public CallTarget? Target { get; init; }

    /// <summary>The call as written, whitespace normalised, for a finding to quote.</summary>
    public string Text { get; init; } = "";

    private string? _canonical;

    /// <summary>
    /// Its canonical text (code-reader.md, Canonical text), which a find step's regular expression reads, whatever the layout:
    /// <c>AddSubscription&lt;A,B&gt;()</c>. <see cref="Text"/> when its language gives none.
    /// </summary>
    public string Canonical => _canonical ??= CanonicalOf?.Invoke() ?? Text;

    /// <summary>Gives <see cref="Canonical"/> the first time it is read; the language sets it.</summary>
    public Func<string>? CanonicalOf { get; init; }

    public required Span At { get; init; }
}

/// <summary>An in-scope method a call reaches, and the type that declares it.</summary>
public sealed record CallTarget(TypeNode Type, MemberNode Member)
{
    /// <summary>
    /// What a call to it gives, as written: null for <c>void</c>, or for a type parameter, which the call's own type
    /// arguments decide.
    /// </summary>
    public TypeRefNode? Returns =>
        Member.Type is { Name: not "void" } type && !Member.TypeParameters.Contains(type.Name) && !Type.TypeParameters.Contains(type.Name) ? type : null;

    /// <summary>
    /// What a call to it gives when the call passes the method's type arguments: a method that returns its own type parameter
    /// gives the type the call passes for it, so <c>Create&lt;Order&gt;()</c> gives <c>Order</c>. Otherwise as <see cref="Returns"/>.
    /// </summary>
    public TypeRefNode? ReturnsGiven(IReadOnlyList<TypeRefNode> typeArguments)
    {
        if (Returns is { } returns)
        {
            return returns;
        }

        var parameters = Member.TypeParameters;
        if (Member.Type is not { } type || typeArguments.Count != parameters.Count)
        {
            return null;
        }

        for (var i = 0; i < parameters.Count; i++)
        {
            if (parameters[i] == type.Name)
            {
                return typeArguments[i];
            }
        }

        return null;
    }
}

/// <summary>
/// A value in a body, such as a call's argument, and the types the code says it can be without running it: a creation is
/// its type; a local is what it is given before it is used (its initializer and each assignment, one type each), or its
/// declared type when one of those isn't known (C#'s <c>var</c>, its initializer's, when they disagree); a parameter, field
/// or property is its declared type; a cast is the type it
/// names, unless that's <c>object</c> or a type what it casts derives from (an upcast), when it is what it casts; a
/// conditional is both its branches, and a C# switch expression each of its arms.
/// </summary>
public sealed class ValueNode
{
    /// <summary>The value as written, whitespace normalised.</summary>
    public required string Text { get; init; }

    /// <summary>
    /// The parameter a named argument names: <c>status</c> for C#'s <c>status: "Paid"</c> and Python's <c>status="Paid"</c>.
    /// Null for a positional argument, and for any value that isn't an argument.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>The types it can be, one per way the code gives it; the known ones when <see cref="IsKnown"/> is false.</summary>
    public IReadOnlyList<TypeRefNode> Types { get; init; } = [];

    /// <summary>Every way the code gives it has its type in <see cref="Types"/>.</summary>
    public bool IsKnown { get; init; }

    /// <summary>The literal it is, when it is one; null for anything else, such as a name, a call or a concatenation.</summary>
    public LiteralNode? Literal { get; init; }

    /// <summary>
    /// The creations it can be, one for each way the code gives it that is a creation (<see cref="Types"/>), also through a
    /// local, a cast or a conditional: what <c>request</c> was created with in <c>SendAsync(request)</c>.
    /// </summary>
    public IReadOnlyList<CreationNode> Creations { get; init; } = [];

    /// <summary>
    /// The lambda it is, as a unit: a Minimal API handler (<c>app.MapGet("/{id}", ([FromRoute] Guid id) =&gt; ...)</c>), an
    /// options callback (<c>o =&gt; o.Title = "Orders"</c>).
    /// </summary>
    public FunctionNode? Function { get; init; }

    /// <summary>
    /// An object literal's members, in order, as an object initializer's are (<see cref="CreationNode.Initializers"/>):
    /// <c>path</c> and <c>component</c> for TypeScript's <c>{ path: '/', component: CatalogPage }</c>. A method's value is its
    /// unit (<see cref="Function"/>), a shorthand <c>{ routes }</c> is <c>routes</c> given <c>routes</c>, a computed key is
    /// as written (<c>[key]</c>), and a spread is <c>...</c>. Empty for anything else.
    /// </summary>
    public IReadOnlyList<InitializerNode> Members { get; init; } = [];

    /// <summary>An array literal's items, in order: <c>'a'</c> and <c>'b'</c> for <c>['a', 'b']</c>. Empty for anything else.</summary>
    public IReadOnlyList<ValueNode> Items { get; init; } = [];

    /// <summary>
    /// The operands of a <c>+</c>, in order, a chain of them flattened: <c>baseUrl + "items/" + id</c> is <c>baseUrl</c>,
    /// <c>"items/"</c> and <c>id</c>; C#'s <c>string.Concat</c> gives its arguments. Empty for anything else.
    /// </summary>
    public IReadOnlyList<ValueNode> Joined { get; init; } = [];

    private IReadOnlyList<ValueNode>? _given;

    /// <summary>
    /// The values it is, one per way the code gives it, when it stands for another: what a local is given before the use
    /// (its initializer and each plain assignment), a conditional's two branches, a switch expression's arms, both sides of
    /// a <c>??</c>, and what a conversion that keeps the text is made from (<c>x.ToString()</c>, <c>new Uri(x)</c>). Empty
    /// for anything else.
    /// </summary>
    public IReadOnlyList<ValueNode> Given => _given ??= GivenOf?.Invoke() ?? [];

    /// <summary>Gives <see cref="Given"/> the first time it is read: a local's values are read only when a rule asks.</summary>
    public Func<IReadOnlyList<ValueNode>>? GivenOf { get; init; }

    /// <summary>The parameter of its unit it names, as the unit's own <see cref="ParameterNode"/>; null for anything else.</summary>
    public ParameterNode? Parameter { get; init; }

    /// <summary>
    /// The field or property it reads, of its own type (<c>remoteServiceBaseUrl</c>, <c>this.baseUrl</c>) or, through the
    /// type's name, another in scope (<c>Routes.Catalog</c>); null for anything else.
    /// </summary>
    public FieldRef? Field { get; init; }

    /// <summary>The call it is the result of (<c>GetAllCatalogItemsUri(...)</c>), the same node as the unit's; null for anything else.</summary>
    public CallNode? Call { get; init; }

    /// <summary>The value it trims: C#'s <c>uri.TrimEnd('&amp;', '?')</c>, Java's and TypeScript's <c>trim()</c>.</summary>
    public TrimNode? Trimmed { get; init; }

    public required Span At { get; init; }
}

/// <summary>A field or property a value reads, and the type that declares it.</summary>
public sealed record FieldRef(TypeNode Type, MemberNode Member);

/// <summary>A value with characters trimmed from its start, its end or both; whitespace when <see cref="Characters"/> is empty.</summary>
public sealed record TrimNode(ValueNode Value, bool Start, bool End, IReadOnlyList<char> Characters);

/// <summary>
/// An object creation in a body, <c>new HttpRequestMessage(HttpMethod.Post, uri) { Content = body }</c>: what it creates and
/// what it is given. A target-typed <c>new()</c> names no type, so isn't one, and neither is an array creation.
/// </summary>
public sealed class CreationNode
{
    /// <summary>The type it creates, as written: Java's <c>new ArrayList&lt;&gt;()</c> is <c>ArrayList</c>.</summary>
    public required TypeRefNode Type { get; init; }

    /// <summary>The constructor's arguments, in order, each with what the code says it is.</summary>
    public IReadOnlyList<ValueNode> Arguments { get; init; } = [];

    /// <summary>
    /// What an object initializer assigns, in order: <c>Method</c> and <c>Content</c> for
    /// <c>new HttpRequestMessage { Method = HttpMethod.Post, Content = body }</c>. Java has none.
    /// </summary>
    public IReadOnlyList<InitializerNode> Initializers { get; init; } = [];

    /// <summary>
    /// A collection initializer's items, in order: <c>"a"</c> and <c>"b"</c> for <c>new List&lt;string&gt; { "a", "b" }</c>.
    /// A dictionary's <c>{ "Accept", "json" }</c> is one item, as written. Java has none.
    /// </summary>
    public IReadOnlyList<ValueNode> Items { get; init; } = [];

    /// <summary>The creation as written, whitespace normalised, for a finding to quote.</summary>
    public string Text { get; init; } = "";

    public required Span At { get; init; }
}

/// <summary>
/// What an object initializer assigns: the member by its name (<c>Content</c>), or an indexer as written
/// (<c>["Accept"]</c>), and the value it is given. A nested initializer (<c>Headers = { ... }</c>) is a value as written.
/// </summary>
public sealed record InitializerNode(string Member, ValueNode Value);

/// <summary>
/// A literal as its value, the same whatever the language's quoting and escapes, so a rule compares <see cref="Value"/>
/// rather than the text: <c>"Orders"</c>, <c>@"Orders"</c> and Java's <c>"Orders"</c> are all <c>Orders</c>.
/// </summary>
public sealed class LiteralNode
{
    /// <summary>
    /// <c>string</c>, <c>char</c>, <c>number</c>, <c>boolean</c>, <c>null</c>, <c>interpolated</c> (C#'s <c>$"..."</c>) or
    /// <c>type</c> (C#'s <c>typeof(Order)</c>, Java's <c>Order.class</c>). C#'s <c>nameof(Order.Total)</c> is the string
    /// <c>Total</c>.
    /// </summary>
    public required string Kind { get; init; }

    /// <summary>
    /// A string's or char's content with its escapes resolved (a verbatim or raw string's, a Java text block's with its
    /// indentation stripped), a number as written with its sign (<c>-1</c>, <c>18.4m</c>), <c>true</c> or <c>false</c>. Null
    /// for <c>null</c>, an interpolated string and a type.
    /// </summary>
    public string? Value { get; init; }

    /// <summary>
    /// An interpolated string's parts, in order: its text, each a <c>string</c> literal, and each hole's value, with what the
    /// code says it is. <c>$"orders/{id}/lines"</c> is <c>orders/</c>, <c>id</c> and <c>/lines</c>.
    /// </summary>
    public IReadOnlyList<ValueNode> Parts { get; init; } = [];

    /// <summary>A type literal's type.</summary>
    public TypeRefNode? Type { get; init; }
}

public sealed class AttributeNode
{
    /// <summary>The name as used, without a namespace or the <c>Attribute</c> suffix: <c>HttpGet</c>.</summary>
    public required string Name { get; init; }

    /// <summary>A generic attribute's type arguments: <c>OrderDto</c> for C#'s <c>[ProducesResponseType&lt;OrderDto&gt;(200)]</c>.</summary>
    public IReadOnlyList<TypeRefNode> TypeArguments { get; init; } = [];

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

    /// <summary>A literal's <see cref="LiteralNode.Value"/>: a string's or char's content, a number or boolean as written; null for anything else.</summary>
    public string? Literal { get; init; }

    /// <summary>
    /// A list's items: C#'s <c>new[] { ... }</c> or <c>[ ... ]</c>, Java's <c>{ ... }</c>. A Protobuf option's message value
    /// has its fields, each named: <c>get</c> for <c>{ get: "/v1/orders" }</c>.
    /// </summary>
    public IReadOnlyList<AttributeValueNode> Items { get; init; } = [];

    /// <summary>An annotation given as the value.</summary>
    public AttributeNode? Annotation { get; init; }
}

/// <summary>A parameter as written; a record, equal only to itself, as <see cref="MemberNode"/> is.</summary>
public sealed record ParameterNode
{
    public required string Name { get; init; }

    public TypeRefNode? Type { get; init; }

    public IReadOnlyList<AttributeNode> Attributes { get; init; } = [];

    /// <summary>The default value as written (<c>pageSize = 10</c> is <c>10</c>); null when none is.</summary>
    public string? Default { get; init; }

    /// <summary>A call may leave it out without a default: TypeScript's <c>id?: string</c>.</summary>
    public bool IsOptional { get; init; }

    public required Span At { get; init; }

    private string? _text;

    /// <summary>Its canonical text, as <see cref="MemberNode.Text"/>: attributes, type, name and default.</summary>
    public string Text => _text ??= TextOf?.Invoke() ?? "";

    /// <summary>Gives <see cref="Text"/> the first time it is read; the language sets it.</summary>
    public Func<string>? TextOf { get; init; }

    public bool Equals(ParameterNode? other) => ReferenceEquals(this, other);

    public override int GetHashCode() => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this);
}

public sealed class TypeNode
{
    public required string Name { get; init; }

    public required string Namespace { get; init; }

    /// <summary>
    /// <c>class</c>, <c>record</c>, <c>struct</c>, <c>interface</c> or <c>enum</c>; <c>component</c> for a Razor component
    /// or a Vue single-file component; TypeScript's also <c>typeAlias</c> (<c>type X = ...</c>), <c>object</c> (a
    /// <c>const</c> holding an object literal) and <c>module</c> (what a file declares outside its types: its functions and
    /// variables).
    /// </summary>
    public required string Kind { get; init; }

    /// <summary>The type a type alias names, as written: <c>'Draft' | 'Paid'</c> for <c>type Status = 'Draft' | 'Paid'</c>.</summary>
    public TypeRefNode? Aliases { get; init; }

    /// <summary>Where the type is declared; a partial type's first part.</summary>
    public required Span At { get; init; }

    /// <summary>Every file a part of the type is in.</summary>
    public List<string> Files { get; } = [];

    /// <summary>
    /// Where each part of a partial type is declared, the first being <see cref="At"/>, as a reader that merges parts records
    /// them. Empty from a reader that doesn't, so <see cref="Declarations"/> is <see cref="At"/>.
    /// </summary>
    public List<Span> Parts { get; } = [];

    /// <summary>Where each part of the type is declared: <see cref="Parts"/>, else <see cref="At"/>.</summary>
    public IReadOnlyList<Span> Declarations => Parts.Count > 0 ? Parts : [At];

    public bool IsAbstract { get; set; }

    public bool IsStatic { get; set; }

    /// <summary>
    /// A type whose parts may be in several files, as a C# <c>partial</c> type's are. Only such a type can have a member in a
    /// file other than the ones it was found in.
    /// </summary>
    public bool IsPartial { get; set; }

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

    private string? _text;

    /// <summary>Its canonical text, as <see cref="MemberNode.Text"/>: its first part's declaration and body, nested types included.</summary>
    public string Text => _text ??= TextOf?.Invoke() ?? "";

    /// <summary>Gives <see cref="Text"/> the first time it is read; the language sets it.</summary>
    public Func<string>? TextOf { get; init; }
}

/// <summary>An import: a C# <c>using</c>, a Java or TypeScript <c>import</c> (code-reader.md, import nodes).</summary>
public sealed class ImportNode
{
    /// <summary>
    /// What it imports, as written without whitespace or a wildcard: <c>Ordering.Domain.AggregatesModel</c>,
    /// <c>com.shop.common</c> for <c>import com.shop.common.*;</c>. <see cref="CodeGraph.TypeOf(ImportNode)"/> gives the
    /// in-scope type it names.
    /// </summary>
    public required string Target { get; init; }

    /// <summary>The import as written, whitespace normalised: <c>using static System.Math;</c>.</summary>
    public required string Text { get; init; }

    /// <summary>The name it gives what it imports: <c>OrderEntity</c> for <c>using OrderEntity = Shop.Domain.Order;</c>.</summary>
    public string? Alias { get; init; }

    /// <summary>It imports a type's members (<c>using static</c>, Java's <c>import static</c>).</summary>
    public bool Static { get; init; }

    /// <summary>
    /// It imports everything <see cref="Target"/> holds: a C# <c>using</c> of a namespace, Java's
    /// <c>import com.shop.common.*;</c>. An import that is none of wildcard, static or aliased names one type, as Java's
    /// <c>import com.shop.Order;</c> does.
    /// </summary>
    public bool Wildcard { get; init; }

    /// <summary>It applies to every file of the project (<c>global using</c>).</summary>
    public bool Global { get; init; }

    /// <summary>
    /// The module it imports from, as written: <c>./types</c> for TypeScript's <c>import { ProductDto } from './types'</c>,
    /// whose <see cref="Target"/> is <c>src/api/types.ProductDto</c> when that module is in scope. Null for a language whose
    /// imports name namespaces.
    /// </summary>
    public string? Module { get; init; }

    /// <summary>The in-scope file <see cref="Module"/> resolves to; null for a package, or a file out of scope.</summary>
    public string? File { get; init; }

    /// <summary>It imports only types: TypeScript's <c>import type { ... }</c>, or <c>import { type X }</c>.</summary>
    public bool TypeOnly { get; init; }

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

    /// <summary>
    /// The other files whose imports its code has too, by path: the <c>_Imports.razor</c> files in a Razor component's folder
    /// and the folders above it. <see cref="CodeGraph.ImportsOf"/> includes the imports of those that are in scope.
    /// </summary>
    public IReadOnlyList<string> ImportsFrom { get; init; } = [];

    /// <summary>
    /// Its top-level statements as a unit: C#'s Program.cs, where an application registers its services and maps its
    /// endpoints. Null when it has none. A language reader sets it with the members' bodies.
    /// </summary>
    public FunctionNode? TopLevel { get; set; }

    /// <summary>
    /// The markup elements written at its top level, each with the elements inside it: a Razor component's markup, a Vue
    /// single-file component's template's. Empty for a file without markup.
    /// </summary>
    public IReadOnlyList<MarkupNode> Markup { get; init; } = [];

    /// <summary>
    /// Where its reader couldn't parse it cleanly, each with the parser's diagnostic (code-reader.md, Failures): a missing
    /// token the parser inserted, a token it skipped. The file is still read; a value read from code there may not be what
    /// was meant, so its claim can't be verified (<c>readerFailed</c>).
    /// </summary>
    public IReadOnlyList<SyntaxError> Errors { get; init; } = [];
}

/// <summary>A span of a file its reader couldn't parse cleanly, and the parser's diagnostic.</summary>
public sealed record SyntaxError(Span At, string Message);

/// <summary>
/// An element written in markup, the same whatever the language: a Razor component's <c>&lt;CatalogListItem Item="@item" /&gt;</c>,
/// a Vue template's <c>&lt;stock-pill :in-stock="product.InStock" /&gt;</c>. What its attributes mean to a framework (a
/// component's parameter, a link's route) is a stack's to say.
/// </summary>
public sealed class MarkupNode
{
    private string? _text;

    /// <summary>The tag as written: <c>div</c>, <c>CatalogListItem</c>, <c>Ui.Grid</c>, <c>stock-pill</c>.</summary>
    public required string Tag { get; init; }

    /// <summary>
    /// The component the tag names, when the language reads it as one: in Razor, a tag that starts with a capital letter or
    /// is qualified (<c>Ui.Grid</c>); in Vue, one that isn't an HTML, SVG or MathML element, by its PascalCase name
    /// (<c>stock-pill</c> names <c>StockPill</c>). <see cref="CodeGraph.TypeFor"/> from the type the markup is in resolves it
    /// as the language does: through the file's imports, then its namespace. Null for an element.
    /// </summary>
    public TypeRefNode? Component { get; init; }

    /// <summary>Its attributes, in the order they are written.</summary>
    public IReadOnlyList<MarkupAttributeNode> Attributes { get; init; } = [];

    /// <summary>
    /// The elements directly inside it, in the order they are written, those inside code included: a Razor
    /// <c>@foreach</c>'s, an <c>@if</c>'s.
    /// </summary>
    public IReadOnlyList<MarkupNode> Children { get; init; } = [];

    /// <summary>
    /// What it holds, when it holds only text and values: a <c>string</c> literal for text alone (<c>Shopping Bag |
    /// AdventureWorks</c>), else an <c>interpolated</c> one whose parts are its text and each value's code (Razor's
    /// <c>@item.Name</c>, Vue's <c>{{ item.name }}</c>). Text is trimmed, each run of whitespace in it reduced to one space,
    /// and its character references resolved (<c>&amp;amp;</c> is <c>&amp;</c>). Null when it holds an element or other code,
    /// such as a Razor <c>@if</c>.
    /// </summary>
    public LiteralNode? Content { get; init; }

    public required Span At { get; init; }

    /// <summary>The element as written, the elements inside it included, as markup's canonical text (<see cref="CanonicalText.OfMarkup"/>).</summary>
    public string Text => _text ??= TextOf?.Invoke() ?? "";

    public Func<string>? TextOf { get; init; }

    /// <summary>It and every element inside it, depth first, in the order they are written.</summary>
    public IEnumerable<MarkupNode> DescendantsAndSelf() => Children.SelectMany(x => x.DescendantsAndSelf()).Prepend(this);
}

/// <summary>
/// An attribute of a markup element, as its language writes it: a plain one (<c>class="catalog"</c>, <c>href="/orders/@id"</c>,
/// <c>Item="@item"</c>), or a directive whose value is code: Razor's <c>@onclick</c>, <c>@bind-Value</c> or <c>@key</c>, Vue's
/// <c>v-if</c>, <c>v-model:title</c>, <c>:to</c> (<c>v-bind:to</c>) or <c>@click.prevent</c> (<c>v-on:click.prevent</c>).
/// </summary>
public sealed class MarkupAttributeNode
{
    /// <summary>As written: <c>class</c>, <c>Item</c>, <c>@bind-Value</c>, <c>:to</c>, <c>@click.prevent</c>.</summary>
    public required string Name { get; init; }

    /// <summary>
    /// The directive it is, as the language names it, when it is one: Razor's <c>@onclick</c>, <c>@bind-Value</c> and
    /// <c>@key</c> are <c>onclick</c>, <c>bind</c> and <c>key</c>; Vue's <c>v-if</c>, <c>v-model</c>, <c>:to</c> and
    /// <c>@click</c> are <c>if</c>, <c>model</c>, <c>bind</c> and <c>on</c>. Null for a plain attribute.
    /// </summary>
    public string? Directive { get; init; }

    /// <summary>
    /// What a directive applies to: <c>Value</c> for Razor's <c>@bind-Value</c>; <c>to</c> for Vue's <c>:to</c>, <c>click</c>
    /// for <c>@click</c>, <c>title</c> for <c>v-model:title</c>. Null when it has none, and for a plain attribute.
    /// </summary>
    public string? Argument { get; init; }

    /// <summary>A directive's modifiers, in order: Vue's <c>@click.prevent</c>'s <c>prevent</c>, Razor's <c>@onclick:preventDefault</c>'s <c>preventDefault</c>.</summary>
    public IReadOnlyList<string> Modifiers { get; init; } = [];

    /// <summary>Its value as written, without its quotes; null when it has none (<c>disabled</c>).</summary>
    public string? Value { get; init; }

    /// <summary>
    /// A plain attribute's value as a literal: a <c>string</c> for text alone, with its character references resolved; an
    /// <c>interpolated</c> one for text with Razor's values in it (<c>href="/orders/@id"</c> is <c>/orders/</c> and
    /// <c>id</c>; <c>Item="@item"</c> is <c>item</c> alone). Null for a directive, whose value is code, and when it has no value.
    /// </summary>
    public LiteralNode? Literal { get; init; }

    public required Span At { get; init; }
}

/// <summary>
/// Code that runs as a unit: a method's or constructor's body, a lambda (<c>(Guid id, ISender sender) =&gt; ...</c>), or a
/// file's top-level statements. Each has what it calls, creates and assigns, and what it returns. Everything written
/// inside it counts, its own lambdas' code included, as the same nodes the lambda's own unit has.
/// </summary>
public sealed class FunctionNode
{
    /// <summary>Its parameters: a member's, or a lambda's with their attributes and, where written, their types. Top-level code has none.</summary>
    public IReadOnlyList<ParameterNode> Parameters { get; init; } = [];

    /// <summary>Its calls and the methods it passes as values, in the order they are written.</summary>
    public IReadOnlyList<CallNode> Calls { get; init; } = [];

    /// <summary>Its creations (<c>new X(...)</c>), in the order they are written.</summary>
    public IReadOnlyList<CreationNode> Creations { get; init; } = [];

    /// <summary>
    /// What it assigns with a plain <c>=</c>, in order: <c>o.Title = "Orders"</c> in an options lambda. An object
    /// initializer's are its creation's <see cref="CreationNode.Initializers"/>.
    /// </summary>
    public IReadOnlyList<AssignmentNode> Assignments { get; init; } = [];

    /// <summary>
    /// What it changes other than with a plain <c>=</c>, as written: a compound assignment's target (<c>filterQs</c> in
    /// <c>filterQs += "&amp;"</c>), an increment's or decrement's operand, and what it passes as <c>ref</c> or <c>out</c>. A field
    /// one of these changes has no one value (<see cref="CodeGraph.StringOf"/>).
    /// </summary>
    public IReadOnlyList<string> Changes { get; init; } = [];

    /// <summary>
    /// What it returns: an expression body's value (a lambda's <c>=&gt; Results.Ok(order)</c>, not a <c>void</c> method's),
    /// or each <c>return</c>'s, not a nested lambda's.
    /// </summary>
    public IReadOnlyList<ValueNode> Returns { get; init; } = [];

    public bool IsAsync { get; init; }

    /// <summary>The unit as written, whitespace normalised, for a finding to quote.</summary>
    public string Text { get; init; } = "";

    public required Span At { get; init; }
}

/// <summary>A plain assignment in a unit: what it assigns, as written (<c>o.Title</c>), and the value.</summary>
public sealed record AssignmentNode(string Target, ValueNode Value, Span At)
{
    /// <summary>
    /// What the member it assigns belongs to, when that is written: <c>order</c> in <c>order.Status = status</c>, with what the
    /// code says it is. Null for a local, or the type's own member (<c>Status = status</c>, <c>this.Status = status</c>).
    /// </summary>
    public ValueNode? Receiver { get; init; }
}

/// <summary>
/// What the language readers produced from the in-scope files, in no language's terms. A language keeps its own syntax
/// here too (<see cref="Set{T}"/>), for the extensions that read it.
/// </summary>
public sealed class CodeGraph
{
    private readonly Dictionary<Type, object> _extensions = [];

    public CodeGraph()
    {
        Files = new GraphList<FileNode>(Changed);
        Types = new GraphList<TypeNode>(Changed);
    }

    /// <summary>Every in-scope source file a reader read, in path order.</summary>
    public GraphList<FileNode> Files { get; }

    public GraphList<TypeNode> Types { get; }

    private Indexes? _indexes;

    /// <summary>
    /// The lookups by name, path and project, and what is worked out once per type, each built on its first use and all of
    /// them dropped when <see cref="Types"/> or <see cref="Files"/> change. What they read of a node (a type's files and base
    /// types) is complete once its reader has declared it, and no lookup runs before that.
    /// </summary>
    private Indexes Index => _indexes ??= new Indexes(this);

    private void Changed() => _indexes = null;

    /// <summary>Files a reader failed on, with its diagnostic (code-reader.md, Failures).</summary>
    public List<(string File, string Error)> Failures { get; } = [];

    /// <summary>Source files whose governing project file failed to read, so its settings are unknown.</summary>
    public Dictionary<string, string> UngovernedFiles { get; } = new(StringComparer.Ordinal);

    /// <summary>
    /// What a file's code can name without qualifying it: its own imports, those of the files it has imports from
    /// (<see cref="FileNode.ImportsFrom"/>), and the global ones of its project.
    /// </summary>
    public IEnumerable<ImportNode> ImportsOf(FileNode file) => Index.ImportsOf(file);

    /// <summary>
    /// Every markup element written in a type's files, depth first, in the order they are written: a Razor component's
    /// markup, a Vue component's template's.
    /// </summary>
    public IEnumerable<MarkupNode> MarkupOf(TypeNode type) =>
        // JSX is read with its code, after its file is in the graph, so whether a file has markup is looked at here, not indexed.
        Index.FilesAt(type.Files).Where(x => x.Markup.Count > 0).SelectMany(x => x.Markup).SelectMany(x => x.DescendantsAndSelf());

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
        var memo = Index.Supertypes;
        if (memo.TryGetValue(type, out var known))
        {
            return known;
        }

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

        return memo[type] = names.Distinct(StringComparer.Ordinal).ToList();
    }

    /// <summary>
    /// The in-scope types a type derives from or implements, nearest first. A base type is found by its name; of several
    /// with the name, the one in the type's namespace or an enclosing one. A library's base type isn't in the graph.
    /// </summary>
    public IReadOnlyList<TypeNode> Bases(TypeNode type)
    {
        var memo = Index.Bases;
        if (memo.TryGetValue(type, out var known))
        {
            return known;
        }

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

        return memo[type] = bases;
    }

    /// <summary>
    /// The in-scope types that derive from or implement a type, directly or through others, in the graph's order: the
    /// classes implementing an interface, and the interfaces extending it. The types whose <see cref="Bases"/> have it.
    /// </summary>
    public IReadOnlyList<TypeNode> ImplementationsOf(TypeNode type) => Index.Implementations.GetValueOrDefault(type) ?? [];

    /// <summary>
    /// A type's members, then the ones it inherits, nearest first: <c>Id</c> declared on an <c>Entity</c> base is a member
    /// of every entity. Constructors aren't inherited; an interface's members are inherited only by an interface.
    /// </summary>
    public IEnumerable<MemberNode> MembersOf(TypeNode type) =>
        type.Members.Concat(Bases(type)
            .Where(x => x.Kind != "interface" || type.Kind == "interface")
            .SelectMany(x => x.Members.Where(m => m.Kind != "constructor")))
            .Concat(LibraryMembersOf(type));

    private readonly Dictionary<string, LibraryType> _libraryTypes = new(StringComparer.Ordinal);

    /// <summary>
    /// Tells the graph the members a library type declares, by its bare name, as a stack knows them: ASP.NET Core Identity's
    /// <c>IdentityUser&lt;TKey&gt;</c> declares <c>Id</c> of <c>TKey</c>. The library's code isn't read, so without this a
    /// type deriving from it has none of them. A member's type naming one of <paramref name="typeParameters"/> is the type
    /// argument the code gives it, or with none, the one <paramref name="defaults"/> gives (<c>IdentityUser</c> is
    /// <c>IdentityUser&lt;string&gt;</c>).
    /// </summary>
    public void AddLibraryType(string name, IReadOnlyList<string> typeParameters, IReadOnlyList<TypeRefNode> defaults, IReadOnlyList<MemberNode> members)
    {
        _libraryTypes[name] = new LibraryType(typeParameters, defaults, members);
        Changed();
    }

    private sealed record LibraryType(IReadOnlyList<string> TypeParameters, IReadOnlyList<TypeRefNode> Defaults, IReadOnlyList<MemberNode> Members);

    /// <summary>
    /// The members a type inherits from the library types it or its in-scope bases derive from that a stack told the graph
    /// of, each placed at the declaration of the type that derives from it, since that is where the code states it.
    /// </summary>
    private IReadOnlyList<MemberNode> LibraryMembersOf(TypeNode type)
    {
        if (_libraryTypes.Count == 0)
        {
            return [];
        }

        var memo = Index.LibraryMembers;
        if (memo.TryGetValue(type, out var known))
        {
            return known;
        }

        var members = new List<MemberNode>();
        foreach (var declaring in new[] { type }.Concat(Bases(type)))
        {
            foreach (var reference in declaring.BaseTypes.Where(x => Resolve(x, declaring) is null))
            {
                if (!_libraryTypes.TryGetValue(reference.Name, out var library))
                {
                    continue;
                }

                var arguments = reference.Arguments.Count > 0 ? reference.Arguments : library.Defaults;

                TypeRefNode? Bound(TypeRefNode? memberType) =>
                    memberType is not null && library.TypeParameters.ToList().IndexOf(memberType.Name) is var index and >= 0 && index < arguments.Count
                        ? arguments[index]
                        : memberType;

                members.AddRange(library.Members.Where(x => members.All(m => m.Name != x.Name))
                    .Select(x => x with { Type = Bound(x.Type), At = declaring.At }));
            }
        }

        return memo[type] = members;
    }

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
    /// The in-scope methods a call in <paramref name="from"/> can reach, by name: on the type of what it is called on; on the
    /// type a static call names (<c>Http.created(..)</c>); with nothing to call it on, on <paramref name="from"/> itself. A
    /// method is found with the ones it inherits, and an override hides what it overrides. A language reader sets
    /// <see cref="CallNode.Target"/> from this once every type is read.
    /// </summary>
    /// <param name="receiver">What it is called on, as written; null for unqualified or <c>this</c>.</param>
    /// <param name="receiverType">The type of what it is called on, when the code says.</param>
    /// <param name="arguments">
    /// How many arguments it has, which the method's parameters must take: a parameter with a default, or an optional one, may be left out, and a
    /// last array parameter may take any number (<c>params</c>, Java's varargs). Null for a method reference, which reaches
    /// any overload, and through a type name an instance method too (<c>Order::markAsPaid</c>).
    /// </param>
    public IReadOnlyList<CallTarget> Targets(string name, string? receiver, TypeRefNode? receiverType, int? arguments, TypeNode from)
    {
        var (type, onlyStatic) = receiver is null ? (from, false)
            : receiverType is not null ? (TypeFor(receiverType, from), false)
            // Called on a name the code doesn't type: a type's name, for a static call.
            : (TypeFor(new TypeRefNode { Text = receiver, Name = receiver.Split('.')[^1], QualifiedName = receiver }, from), arguments is not null);
        if (type is null)
        {
            return [];
        }

        return MembersOf(type).Where(x => x.Kind == "method" && x.Name == name && (!onlyStatic || x.IsStatic) && (arguments is not { } count || Takes(x, count)))
            .GroupBy(x => string.Join(",", x.Parameters.Select(p => p.Type?.Name)))
            .Select(x => new CallTarget(DeclaringType(type, x.First()), x.First()))
            .ToList();
    }

    private static bool Takes(MemberNode method, int count)
    {
        var parameters = method.Parameters;
        var required = parameters.Count(x => x.Default is null && !x.IsOptional);
        return count >= required && count <= parameters.Count || parameters is [.., { Type.IsArray: true }] && count >= parameters.Count - 1;
    }

    private TypeNode DeclaringType(TypeNode type, MemberNode member) =>
        type.Members.Contains(member) ? type : Bases(type).FirstOrDefault(x => x.Members.Contains(member)) ?? type;

    /// <summary>A field or property of a type, or one it inherits, with the type that declares it; null when it has none.</summary>
    public FieldRef? FieldOf(TypeNode type, string name) =>
        MembersOf(type).FirstOrDefault(x => x.Name == name && x.Kind is "field" or "property") is { } member ? new FieldRef(DeclaringType(type, member), member) : null;

    /// <summary>
    /// The string a value is, as far as the code says without running it (code-reader.md, String values): a <c>string</c>
    /// literal when the code gives all of it, otherwise <c>interpolated</c>, its text and, as holes, the values whose text it
    /// doesn't give. It follows literals and interpolation, <c>+</c>, what a local is given, fields and constants, conversions
    /// that keep the text, trims, and what a method of the code returns, its parameters bound to the call's arguments, so
    /// <c>GetFromJsonAsync(uri)</c> after <c>var uri = $"{remoteServiceBaseUrl}items/{id}";</c> is <c>api/catalog/items/</c> and
    /// <c>id</c>. A value given several ways is known only when they agree.
    /// </summary>
    public LiteralNode StringOf(ValueNode value) => StringValues.Of(value);

    /// <summary>
    /// The string a value written in a method of the code is when the calls in <paramref name="through"/> lead to it, outermost
    /// first, each binding its method's parameters to its arguments: a request helper's <c>fetch(`${baseUrl}${path}`)</c>, reached
    /// by <c>request('GET', `/api/orders/${id}`)</c>, is <c>/api/orders/</c> and <c>id</c> after the base URL.
    /// </summary>
    public LiteralNode StringOf(ValueNode value, IReadOnlyList<CallNode> through) => StringValues.Of(value, through);

    /// <summary>
    /// The string a constant stands for, by its name as the code writes it where only text is kept, as an attribute's
    /// argument is: <c>AuthorizationRoles.Reader</c> for a type's field, a bare name for one of <paramref name="from"/>'s
    /// own. Null when it isn't a name, or the code doesn't give it one string.
    /// </summary>
    public string? ConstantOf(string name, TypeNode from) => LiteralOf(name, from) is { Kind: "string" } literal ? literal.Value : null;

    /// <summary>
    /// The literal a constant stands for, named as <see cref="ConstantOf"/>'s are: a string, or a number or boolean a
    /// <c>const</c> or <c>readonly</c> (Java's <c>final</c>) field is initialised with (<c>OrderConstraints.DescriptionMaxLength</c>).
    /// Null when it isn't a name, or the code doesn't give it one value: a field set in a constructor has none until it runs.
    /// </summary>
    public LiteralNode? LiteralOf(string name, TypeNode from)
    {
        if (!Regex.IsMatch(name, @"^[A-Za-z_]\w*(\.[A-Za-z_]\w*)*$"))
        {
            return null;
        }

        var dot = name.LastIndexOf('.');
        var owner = dot < 0 ? from : TypeFor(new TypeRefNode { Text = name[..dot], Name = name[..dot].Split('.')[^1], QualifiedName = name[..dot] }, from);
        if (owner is null || FieldOf(owner, name[(dot + 1)..]) is not { Member: { Value: { } value } member })
        {
            return null;
        }

        return StringOf(value) is { Kind: "string" } text ? text
            : value.Literal is { Kind: "number" or "boolean" } literal && member.Modifiers.Any(x => x is "const" or "readonly" or "final") ? literal
            : null;
    }

    /// <summary>
    /// The in-scope type a reference names, seen from code in <paramref name="from"/>: of several with the name, the one in
    /// its namespace or an enclosing one. Null for a library's type, which isn't in the graph.
    /// </summary>
    public TypeNode? TypeFor(TypeRefNode reference, TypeNode from) => Resolve(reference, from);

    /// <summary>
    /// The in-scope type an import names: a single-type import's (<c>import com.shop.Order;</c>), a static import's
    /// (<c>using static Shop.Orders;</c>, Java's <c>import static com.shop.Orders.place;</c>) or an alias's
    /// (<c>using OrderEntity = Shop.Domain.Order;</c>). Null for a namespace, and for a library's type, which isn't in the graph.
    /// </summary>
    public TypeNode? TypeOf(ImportNode import)
    {
        var type = TypeNamed(import.Target);
        if (type is not null || !import.Static || import.Wildcard)
        {
            return type;
        }

        // A static import of one member names its type first.
        var dot = import.Target.LastIndexOf('.');
        return dot < 0 ? null : TypeNamed(import.Target[..dot]);
    }

    /// <summary>The in-scope types of a name, without namespace, in the graph's order.</summary>
    public IReadOnlyList<TypeNode> TypesNamed(string name) => Index.TypesByName.GetValueOrDefault(name) ?? [];

    /// <summary>The in-scope types of any of these names, in the graph's order.</summary>
    public IReadOnlyList<TypeNode> TypesNamed(IEnumerable<string> names)
    {
        var distinct = names.Distinct(StringComparer.Ordinal).ToList();
        return distinct.Count == 1 ? TypesNamed(distinct[0]) : distinct.SelectMany(TypesNamed).OrderBy(Index.PositionOf).ToList();
    }

    private TypeNode? TypeNamed(string qualifiedName) => Index.TypesByQualifiedName.GetValueOrDefault(qualifiedName);

    private TypeNode? Resolve(TypeRefNode reference, TypeNode from)
    {
        // A qualified base (Domain.SeedWork.Entity) names the end of the type's full name, or all of it.
        var candidates = (Index.TypesByName.GetValueOrDefault(reference.Name) ?? [])
            .Where(x => reference.QualifiedName == reference.Name || $".{x.Namespace}.{x.Name}".EndsWith("." + reference.QualifiedName, StringComparison.Ordinal))
            .ToList();
        var imports = Index.ImportsOf(from);
        if (reference.QualifiedName != reference.Name)
        {
            // Qualified by an alias of what an import names (C#'s using D = Shop.Domain;, TypeScript's import * as api),
            // it is that's type.
            var dot = reference.QualifiedName.IndexOf('.');
            if (imports.FirstOrDefault(x => x.Alias == reference.QualifiedName[..dot]) is { } aliased)
            {
                return TypeNamed($"{aliased.Target}{reference.QualifiedName[dot..]}");
            }

            return candidates.Count == 1 ? candidates[0] : Nearest(candidates, from);
        }

        // An unqualified name an alias or a single-type import gives is what it imports, even over a type of its own
        // namespace, as Java's single-type import is; null when that is a library's type.
        if (imports.FirstOrDefault(x => x.Alias == reference.Name || (x is { Alias: null, Static: false, Wildcard: false } && x.Target.EndsWith("." + reference.Name, StringComparison.Ordinal))) is { } named)
        {
            return TypeNamed(named.Target);
        }

        // Of several, the one in its namespace or an enclosing one, then the one only a namespace it imports holds.
        if (candidates.Count == 1)
        {
            return candidates[0];
        }

        return Nearest(candidates, from)
               ?? (candidates.Where(x => imports.Any(i => i is { Alias: null, Static: false, Wildcard: true } && i.Target == x.Namespace)).ToList() is [var only] ? only : null);
    }

    private static TypeNode? Nearest(IEnumerable<TypeNode> candidates, TypeNode from) =>
        candidates.Where(x => from.Namespace == x.Namespace || from.Namespace.StartsWith(x.Namespace + ".", StringComparison.Ordinal))
            .OrderByDescending(x => x.Namespace.Length)
            .FirstOrDefault();

    private readonly List<(string What, long Ticks)> _timed = [];

    /// <summary>
    /// Adds the time since <paramref name="started"/> (a <see cref="Stopwatch.GetTimestamp"/>) to a part of a reader's work,
    /// for the run's timings in the Software Factory log: C#'s parse, declarations and bodies. A reader times a part file by
    /// file, and the engine logs the parts under the reader that timed them, each in the order it was first timed.
    /// </summary>
    public void Timed(string what, long started)
    {
        var ticks = Stopwatch.GetTimestamp() - started;
        var index = _timed.FindIndex(x => x.What == what);
        if (index < 0)
        {
            _timed.Add((what, ticks));
        }
        else
        {
            _timed[index] = (what, _timed[index].Ticks + ticks);
        }
    }

    /// <summary>The parts timed since the last call, in milliseconds, for the engine to log under the reader that timed them.</summary>
    internal IReadOnlyList<(string What, double Milliseconds)> TakeTimed()
    {
        var timed = _timed.Select(x => (x.What, x.Ticks * 1000d / Stopwatch.Frequency)).ToList();
        _timed.Clear();
        return timed;
    }

    /// <summary>Keeps what a language or an extension's reader found, for the readers after it; one value per type.</summary>
    public void Set<T>(T value) where T : class => _extensions[typeof(T)] = value;

    public T? Get<T>() where T : class => _extensions.GetValueOrDefault(typeof(T)) as T;

    /// <summary>What is kept on the graph (<see cref="Set{T}"/>), by its type.</summary>
    internal IReadOnlyCollection<Type> Stored => _extensions.Keys.ToList();

    /// <summary>Drops what was kept on the graph since <paramref name="stored"/> was: what claims kept, as against what readers did.</summary>
    internal void KeepOnly(IReadOnlyCollection<Type> stored)
    {
        foreach (var type in _extensions.Keys.Except(stored).ToList())
        {
            _extensions.Remove(type);
        }
    }

    /// <summary>
    /// The graph's lookups, each built on its first use. Each keeps the graph's order, so a lookup gives what a scan of
    /// <see cref="Types"/> or <see cref="Files"/> gave, in the same order, and ties resolve as they did.
    /// </summary>
    private sealed class Indexes(CodeGraph graph)
    {
        private Dictionary<string, List<TypeNode>>? _typesByName;
        private Dictionary<string, TypeNode>? _typesByQualifiedName;
        private Dictionary<TypeNode, int>? _positions;
        private Dictionary<string, List<(int Position, FileNode File)>>? _filesByPath;
        private Dictionary<string, List<FileNode>>? _globalImporters;
        private Dictionary<TypeNode, List<TypeNode>>? _implementations;
        private readonly Dictionary<FileNode, IReadOnlyList<ImportNode>> _fileImports = [];
        private readonly Dictionary<TypeNode, (int Files, IReadOnlyList<ImportNode> Imports)> _typeImports = [];

        public Dictionary<TypeNode, IReadOnlyList<TypeNode>> Bases { get; } = [];

        public Dictionary<TypeNode, IReadOnlyList<string>> Supertypes { get; } = [];

        public Dictionary<TypeNode, IReadOnlyList<MemberNode>> LibraryMembers { get; } = [];

        /// <summary>The types of each name, in the graph's order.</summary>
        public Dictionary<string, List<TypeNode>> TypesByName => _typesByName ??= graph.Types
            .GroupBy(x => x.Name, StringComparer.Ordinal)
            .ToDictionary(x => x.Key, x => x.ToList(), StringComparer.Ordinal);

        /// <summary>Where a type first is in <see cref="CodeGraph.Types"/>.</summary>
        public int PositionOf(TypeNode type)
        {
            if (_positions is null)
            {
                _positions = [];
                foreach (var (node, position) in graph.Types.Select((x, i) => (x, i)))
                {
                    _positions.TryAdd(node, position);
                }
            }

            return _positions[type];
        }

        /// <summary>The first type of each qualified name: <c>Shop.Domain.Order</c>, or a bare name in no namespace.</summary>
        public Dictionary<string, TypeNode> TypesByQualifiedName
        {
            get
            {
                if (_typesByQualifiedName is null)
                {
                    _typesByQualifiedName = new Dictionary<string, TypeNode>(StringComparer.Ordinal);
                    foreach (var type in graph.Types)
                    {
                        _typesByQualifiedName.TryAdd(type.Namespace.Length == 0 ? type.Name : $"{type.Namespace}.{type.Name}", type);
                    }
                }

                return _typesByQualifiedName;
            }
        }

        /// <summary>The types whose bases have each type, in the graph's order (<see cref="CodeGraph.ImplementationsOf"/>).</summary>
        public Dictionary<TypeNode, List<TypeNode>> Implementations
        {
            get
            {
                if (_implementations is null)
                {
                    _implementations = [];
                    foreach (var type in graph.Types)
                    {
                        foreach (var @base in graph.Bases(type))
                        {
                            if (!_implementations.TryGetValue(@base, out var implementations))
                            {
                                _implementations[@base] = implementations = [];
                            }

                            implementations.Add(type);
                        }
                    }
                }

                return _implementations;
            }
        }

        /// <summary>The graph's files at these paths, in the graph's order, each once.</summary>
        public IEnumerable<FileNode> FilesAt(IReadOnlyCollection<string> paths)
        {
            _filesByPath ??= graph.Files.Select((file, position) => (Position: position, File: file))
                .GroupBy(x => x.File.Path, StringComparer.Ordinal)
                .ToDictionary(x => x.Key, x => x.ToList(), StringComparer.Ordinal);
            return paths.Count == 1
                ? _filesByPath.GetValueOrDefault(paths.First())?.Select(x => x.File) ?? []
                : paths.Distinct(StringComparer.Ordinal).SelectMany(x => _filesByPath.GetValueOrDefault(x) ?? []).OrderBy(x => x.Position).Select(x => x.File);
        }

        public IReadOnlyList<ImportNode> ImportsOf(FileNode file)
        {
            if (_fileImports.TryGetValue(file, out var known))
            {
                return known;
            }

            // A project's global imports are of each file in it but their own.
            _globalImporters ??= graph.Files.Where(x => x.Project is not null && x.Imports.Any(i => i.Global))
                .GroupBy(x => x.Project!, StringComparer.Ordinal)
                .ToDictionary(x => x.Key, x => x.ToList(), StringComparer.Ordinal);
            return _fileImports[file] = file.Imports
                .Concat(file.ImportsFrom.Count == 0 ? [] : FilesAt(file.ImportsFrom).SelectMany(x => x.Imports))
                .Concat(file.Project is null
                    ? []
                    : (_globalImporters.GetValueOrDefault(file.Project) ?? []).Where(x => x != file).SelectMany(x => x.Imports.Where(i => i.Global)))
                .ToList();
        }

        /// <summary>The imports of every file a type is in, in the graph's order of those files.</summary>
        public IReadOnlyList<ImportNode> ImportsOf(TypeNode type)
        {
            // A type's files are complete once it is declared, before any lookup; the count only guards that.
            if (_typeImports.TryGetValue(type, out var known) && known.Files == type.Files.Count)
            {
                return known.Imports;
            }

            var imports = FilesAt(type.Files).SelectMany(ImportsOf).ToList();
            _typeImports[type] = (type.Files.Count, imports);
            return imports;
        }
    }
}

/// <summary>
/// A list of the graph's nodes, <see cref="CodeGraph.Types"/> or <see cref="CodeGraph.Files"/>, that tells the graph when it
/// changes, so its lookups are built again.
/// </summary>
public sealed class GraphList<T> : Collection<T>
{
    private readonly Action _changed;

    internal GraphList(Action changed) => _changed = changed;

    public void AddRange(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            Items.Add(item);
        }

        _changed();
    }

    protected override void InsertItem(int index, T item)
    {
        base.InsertItem(index, item);
        _changed();
    }

    protected override void SetItem(int index, T item)
    {
        base.SetItem(index, item);
        _changed();
    }

    protected override void RemoveItem(int index)
    {
        base.RemoveItem(index);
        _changed();
    }

    protected override void ClearItems()
    {
        base.ClearItems();
        _changed();
    }
}
