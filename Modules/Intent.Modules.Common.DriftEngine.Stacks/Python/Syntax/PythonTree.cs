namespace Intent.Modules.Common.DriftEngine.Stacks.Python.Syntax;

/// <summary>
/// A node of a Python file's syntax tree, as the engine's own parser reads it (<see cref="PythonParser"/>): its first and
/// last tokens, and the nodes inside it, in the order they are written.
/// </summary>
public abstract class PythonNode
{
    public required PythonToken First { get; init; }

    public required PythonToken Last { get; init; }

    public abstract IEnumerable<PythonNode> Children { get; }

    /// <summary>Every node inside it, depth first, in the order they are written.</summary>
    public IEnumerable<PythonNode> Descendants() => Children.SelectMany(x => x.Descendants().Prepend(x));

    protected static IEnumerable<PythonNode> Of(params object?[] items)
    {
        foreach (var item in items)
        {
            switch (item)
            {
                case PythonNode node:
                    yield return node;
                    break;
                case IEnumerable<PythonNode> nodes:
                    foreach (var node in nodes)
                    {
                        yield return node;
                    }

                    break;
            }
        }
    }
}

public abstract class PythonStatement : PythonNode;

public abstract class PythonExpression : PythonNode;

/// <summary>A file: its statements.</summary>
public sealed class PythonModule : PythonNode
{
    public required IReadOnlyList<PythonStatement> Body { get; init; }

    public override IEnumerable<PythonNode> Children => Body;
}

public sealed class PythonDecorator : PythonNode
{
    public required PythonExpression Expression { get; init; }

    public override IEnumerable<PythonNode> Children => [Expression];
}

public sealed class PythonClass : PythonStatement
{
    public required string Name { get; init; }

    /// <summary>Its own type parameters (<c>class Box[T]:</c>), by name.</summary>
    public IReadOnlyList<string> TypeParameters { get; init; } = [];

    /// <summary>Its bases and keywords as written in its parentheses: <c>Base</c>, <c>metaclass=ABCMeta</c>.</summary>
    public IReadOnlyList<PythonArgument> Bases { get; init; } = [];

    public IReadOnlyList<PythonDecorator> Decorators { get; init; } = [];

    public required IReadOnlyList<PythonStatement> Body { get; init; }

    public override IEnumerable<PythonNode> Children => Of(Decorators, Bases, Body);
}

public sealed class PythonFunction : PythonStatement
{
    public required string Name { get; init; }

    public bool IsAsync { get; init; }

    public IReadOnlyList<string> TypeParameters { get; init; } = [];

    public required IReadOnlyList<PythonParameter> Parameters { get; init; }

    public PythonExpression? Returns { get; init; }

    public IReadOnlyList<PythonDecorator> Decorators { get; init; } = [];

    public required IReadOnlyList<PythonStatement> Body { get; init; }

    public override IEnumerable<PythonNode> Children => Of(Decorators, Parameters, Returns, Body);
}

public enum PythonParameterKind
{
    /// <summary>Given by position or keyword.</summary>
    Normal,

    /// <summary>Before a <c>/</c>.</summary>
    PositionalOnly,

    /// <summary>After a <c>*</c> or <c>*args</c>.</summary>
    KeywordOnly,

    /// <summary><c>*args</c>.</summary>
    VarPositional,

    /// <summary><c>**kwargs</c>.</summary>
    VarKeyword
}

public sealed class PythonParameter : PythonNode
{
    public required string Name { get; init; }

    public PythonParameterKind Kind { get; init; }

    public PythonExpression? Annotation { get; init; }

    public PythonExpression? Default { get; init; }

    public override IEnumerable<PythonNode> Children => Of(Annotation, Default);
}

/// <summary><c>a = b = value</c>: each target, then the value.</summary>
public sealed class PythonAssign : PythonStatement
{
    public required IReadOnlyList<PythonExpression> Targets { get; init; }

    public required PythonExpression Value { get; init; }

    public override IEnumerable<PythonNode> Children => Of(Targets, Value);
}

/// <summary><c>name: int = 0</c>, or a declaration without a value: <c>name: int</c>.</summary>
public sealed class PythonAnnotatedAssign : PythonStatement
{
    public required PythonExpression Target { get; init; }

    public required PythonExpression Annotation { get; init; }

    public PythonExpression? Value { get; init; }

    public override IEnumerable<PythonNode> Children => Of(Target, Annotation, Value);
}

/// <summary><c>total += amount</c>.</summary>
public sealed class PythonAugmentedAssign : PythonStatement
{
    public required PythonExpression Target { get; init; }

    public required string Operator { get; init; }

    public required PythonExpression Value { get; init; }

    public override IEnumerable<PythonNode> Children => [Target, Value];
}

/// <summary><c>type Ids = list[int]</c>.</summary>
public sealed class PythonTypeAlias : PythonStatement
{
    public required string Name { get; init; }

    public IReadOnlyList<string> TypeParameters { get; init; } = [];

    public required PythonExpression Value { get; init; }

    public override IEnumerable<PythonNode> Children => [Value];
}

public sealed class PythonReturn : PythonStatement
{
    public PythonExpression? Value { get; init; }

    public override IEnumerable<PythonNode> Children => Of(Value);
}

public sealed class PythonExpressionStatement : PythonStatement
{
    public required PythonExpression Value { get; init; }

    public override IEnumerable<PythonNode> Children => [Value];
}

/// <summary>
/// A statement that is its keyword and the expressions after it: <c>pass</c>, <c>break</c>, <c>continue</c>, <c>del</c>,
/// <c>assert</c>, <c>raise</c> (its exception, then its <c>from</c>), <c>global</c> and <c>nonlocal</c> (their names).
/// </summary>
public sealed class PythonKeywordStatement : PythonStatement
{
    public required string Keyword { get; init; }

    public IReadOnlyList<PythonExpression> Values { get; init; } = [];

    public override IEnumerable<PythonNode> Children => Values;
}

/// <summary><c>import shop.domain as domain</c>: each module and the name it is bound to.</summary>
public sealed class PythonImport : PythonStatement
{
    public required IReadOnlyList<PythonAlias> Names { get; init; }

    public override IEnumerable<PythonNode> Children => Names;
}

/// <summary><c>from ..domain.order import Order as O</c>: the module as written after its dots, how many dots, and each name.</summary>
public sealed class PythonImportFrom : PythonStatement
{
    /// <summary>The module after its dots; null for <c>from . import x</c>.</summary>
    public string? Module { get; init; }

    /// <summary>How many dots it is relative by: 0 for an absolute import.</summary>
    public int Level { get; init; }

    public required IReadOnlyList<PythonAlias> Names { get; init; }

    /// <summary><c>from shop.domain import *</c>.</summary>
    public bool IsWildcard { get; init; }

    public override IEnumerable<PythonNode> Children => Names;
}

/// <summary>A name an import gives: dotted for <c>import</c>, plain for <c>from ... import</c>.</summary>
public sealed class PythonAlias : PythonNode
{
    public required string Name { get; init; }

    public string? AsName { get; init; }

    public override IEnumerable<PythonNode> Children => [];
}

/// <summary>An <c>if</c>; an <c>elif</c> is an <c>if</c> on its own in <see cref="OrElse"/>.</summary>
public sealed class PythonIf : PythonStatement
{
    public required PythonExpression Test { get; init; }

    public required IReadOnlyList<PythonStatement> Body { get; init; }

    public IReadOnlyList<PythonStatement> OrElse { get; init; } = [];

    public override IEnumerable<PythonNode> Children => Of(Test, Body, OrElse);
}

public sealed class PythonFor : PythonStatement
{
    public bool IsAsync { get; init; }

    public required PythonExpression Target { get; init; }

    public required PythonExpression Iterable { get; init; }

    public required IReadOnlyList<PythonStatement> Body { get; init; }

    public IReadOnlyList<PythonStatement> OrElse { get; init; } = [];

    public override IEnumerable<PythonNode> Children => Of(Target, Iterable, Body, OrElse);
}

public sealed class PythonWhile : PythonStatement
{
    public required PythonExpression Test { get; init; }

    public required IReadOnlyList<PythonStatement> Body { get; init; }

    public IReadOnlyList<PythonStatement> OrElse { get; init; } = [];

    public override IEnumerable<PythonNode> Children => Of(Test, Body, OrElse);
}

public sealed class PythonWith : PythonStatement
{
    public bool IsAsync { get; init; }

    public required IReadOnlyList<PythonWithItem> Items { get; init; }

    public required IReadOnlyList<PythonStatement> Body { get; init; }

    public override IEnumerable<PythonNode> Children => Of(Items, Body);
}

/// <summary><c>open(path) as file</c>.</summary>
public sealed class PythonWithItem : PythonNode
{
    public required PythonExpression Context { get; init; }

    public PythonExpression? Target { get; init; }

    public override IEnumerable<PythonNode> Children => Of(Context, Target);
}

public sealed class PythonTry : PythonStatement
{
    public required IReadOnlyList<PythonStatement> Body { get; init; }

    public IReadOnlyList<PythonExceptHandler> Handlers { get; init; } = [];

    public IReadOnlyList<PythonStatement> OrElse { get; init; } = [];

    public IReadOnlyList<PythonStatement> Finally { get; init; } = [];

    public override IEnumerable<PythonNode> Children => Of(Body, Handlers, OrElse, Finally);
}

/// <summary><c>except ValueError as error:</c>.</summary>
public sealed class PythonExceptHandler : PythonNode
{
    public PythonExpression? Type { get; init; }

    public string? Name { get; init; }

    public required IReadOnlyList<PythonStatement> Body { get; init; }

    public override IEnumerable<PythonNode> Children => Of(Type, Body);
}

public sealed class PythonMatch : PythonStatement
{
    public required PythonExpression Subject { get; init; }

    public required IReadOnlyList<PythonCase> Cases { get; init; }

    public override IEnumerable<PythonNode> Children => Of(Subject, Cases);
}

/// <summary>
/// A <c>case</c>: its pattern read as the expression it is written like (<c>Point(x=0)</c> is a call, <c>[a, *rest]</c> a
/// list), null where it can't be; its guard; its body.
/// </summary>
public sealed class PythonCase : PythonNode
{
    public PythonExpression? Pattern { get; init; }

    public PythonExpression? Guard { get; init; }

    public required IReadOnlyList<PythonStatement> Body { get; init; }

    public override IEnumerable<PythonNode> Children => Of(Pattern, Guard, Body);
}

public sealed class PythonName : PythonExpression
{
    public required string Id { get; init; }

    public override IEnumerable<PythonNode> Children => [];
}

/// <summary>A number as written: <c>1_000</c>, <c>0x1F</c>, <c>18.4</c>, <c>2j</c>.</summary>
public sealed class PythonNumber : PythonExpression
{
    public required string Text { get; init; }

    public override IEnumerable<PythonNode> Children => [];
}

/// <summary><c>True</c>, <c>False</c>, <c>None</c> or <c>...</c>.</summary>
public sealed class PythonConstant : PythonExpression
{
    public required string Value { get; init; }

    public override IEnumerable<PythonNode> Children => [];
}

/// <summary>
/// A string, its adjacent strings joined to it as Python joins them (<c>"a" "b"</c> is <c>ab</c>): its text with its
/// escapes resolved, and an f-string's replacement fields between.
/// </summary>
public sealed class PythonString : PythonExpression
{
    public required IReadOnlyList<PythonStringPart> Parts { get; init; }

    /// <summary>A bytes literal (<c>b"..."</c>).</summary>
    public bool IsBytes { get; init; }

    /// <summary>An f-string (or t-string) is one of the joined strings.</summary>
    public bool IsFormatted { get; init; }

    /// <summary>Its text, when it has no replacement fields; null otherwise.</summary>
    public string? Value => Parts.All(x => x.Text is not null) ? string.Concat(Parts.Select(x => x.Text)) : null;

    public override IEnumerable<PythonNode> Children => Parts.Select(x => x.Expression).OfType<PythonNode>();
}

/// <summary>Text, or a replacement field's code with its conversion (<c>r</c> for <c>!r</c>) and format spec as written.</summary>
public sealed record PythonStringPart(string? Text, PythonExpression? Expression = null, string? Conversion = null, string? FormatSpec = null);

/// <summary><c>order.lines</c>.</summary>
public sealed class PythonAttribute : PythonExpression
{
    public required PythonExpression Value { get; init; }

    public required string Name { get; init; }

    public override IEnumerable<PythonNode> Children => [Value];
}

public sealed class PythonCall : PythonExpression
{
    public required PythonExpression Function { get; init; }

    public required IReadOnlyList<PythonArgument> Arguments { get; init; }

    public override IEnumerable<PythonNode> Children => Of(Function, Arguments);
}

/// <summary>A call's argument, or a class's base: positional, a keyword's (<c>status="Paid"</c>), <c>*items</c> or <c>**options</c>.</summary>
public sealed class PythonArgument : PythonNode
{
    public string? Name { get; init; }

    public required PythonExpression Value { get; init; }

    /// <summary>1 for <c>*items</c>, 2 for <c>**options</c>.</summary>
    public int Stars { get; init; }

    public override IEnumerable<PythonNode> Children => [Value];
}

/// <summary><c>items[0]</c>, <c>dict[str, int]</c>: several indexes are a tuple.</summary>
public sealed class PythonSubscript : PythonExpression
{
    public required PythonExpression Value { get; init; }

    public required PythonExpression Index { get; init; }

    public override IEnumerable<PythonNode> Children => [Value, Index];
}

public sealed class PythonSlice : PythonExpression
{
    public PythonExpression? Lower { get; init; }

    public PythonExpression? Upper { get; init; }

    public PythonExpression? Step { get; init; }

    public override IEnumerable<PythonNode> Children => Of(Lower, Upper, Step);
}

/// <summary>A binary operator, <c>and</c> and <c>or</c> included: <c>a + b</c>.</summary>
public sealed class PythonBinary : PythonExpression
{
    public required PythonExpression Left { get; init; }

    public required string Operator { get; init; }

    public required PythonExpression Right { get; init; }

    public override IEnumerable<PythonNode> Children => [Left, Right];
}

/// <summary><c>-x</c>, <c>+x</c>, <c>~x</c> or <c>not x</c>.</summary>
public sealed class PythonUnary : PythonExpression
{
    public required string Operator { get; init; }

    public required PythonExpression Operand { get; init; }

    public override IEnumerable<PythonNode> Children => [Operand];
}

/// <summary><c>a &lt; b &lt;= c</c>: the first operand, then each operator (<c>not in</c>, <c>is not</c> as written) with the operand after it.</summary>
public sealed class PythonCompare : PythonExpression
{
    public required PythonExpression Left { get; init; }

    public required IReadOnlyList<string> Operators { get; init; }

    public required IReadOnlyList<PythonExpression> Comparators { get; init; }

    public override IEnumerable<PythonNode> Children => Of(Left, Comparators);
}

/// <summary><c>body if test else orElse</c>.</summary>
public sealed class PythonConditional : PythonExpression
{
    public required PythonExpression Body { get; init; }

    public required PythonExpression Test { get; init; }

    public required PythonExpression OrElse { get; init; }

    public override IEnumerable<PythonNode> Children => [Body, Test, OrElse];
}

public sealed class PythonLambda : PythonExpression
{
    public required IReadOnlyList<PythonParameter> Parameters { get; init; }

    public required PythonExpression Body { get; init; }

    public override IEnumerable<PythonNode> Children => Of(Parameters, Body);
}

/// <summary>A tuple, list or set display: <c>(a, b)</c>, <c>a, b</c>, <c>[a, b]</c>, <c>{a, b}</c>.</summary>
public sealed class PythonSequence : PythonExpression
{
    /// <summary><c>tuple</c>, <c>list</c> or <c>set</c>.</summary>
    public required string Kind { get; init; }

    public required IReadOnlyList<PythonExpression> Items { get; init; }

    public override IEnumerable<PythonNode> Children => Items;
}

public sealed class PythonDict : PythonExpression
{
    public required IReadOnlyList<PythonDictEntry> Entries { get; init; }

    public override IEnumerable<PythonNode> Children => Entries.SelectMany(x => Of(x.Key, x.Value));
}

/// <summary>A dict display's entry; a <c>**spread</c> has no key.</summary>
public sealed record PythonDictEntry(PythonExpression? Key, PythonExpression Value);

/// <summary>A list, set or dict comprehension, or a generator expression: <c>[line.total for line in lines if line.paid]</c>.</summary>
public sealed class PythonComprehension : PythonExpression
{
    /// <summary><c>list</c>, <c>set</c>, <c>dict</c> or <c>generator</c>.</summary>
    public required string Kind { get; init; }

    /// <summary>What each item is; a dict comprehension's key.</summary>
    public required PythonExpression Element { get; init; }

    /// <summary>A dict comprehension's value.</summary>
    public PythonExpression? Value { get; init; }

    public required IReadOnlyList<PythonComprehensionClause> Clauses { get; init; }

    public override IEnumerable<PythonNode> Children => Of(Element, Value, Clauses);
}

public sealed class PythonComprehensionClause : PythonNode
{
    public bool IsAsync { get; init; }

    public required PythonExpression Target { get; init; }

    public required PythonExpression Iterable { get; init; }

    public IReadOnlyList<PythonExpression> Conditions { get; init; } = [];

    public override IEnumerable<PythonNode> Children => Of(Target, Iterable, Conditions);
}

public sealed class PythonAwait : PythonExpression
{
    public required PythonExpression Value { get; init; }

    public override IEnumerable<PythonNode> Children => [Value];
}

public sealed class PythonYield : PythonExpression
{
    public PythonExpression? Value { get; init; }

    public bool IsFrom { get; init; }

    public override IEnumerable<PythonNode> Children => Of(Value);
}

/// <summary><c>*items</c> where a sequence or target takes it.</summary>
public sealed class PythonStarred : PythonExpression
{
    public required PythonExpression Value { get; init; }

    public override IEnumerable<PythonNode> Children => [Value];
}

/// <summary><c>(order := find(id))</c>.</summary>
public sealed class PythonNamedExpression : PythonExpression
{
    public required PythonName Target { get; init; }

    public required PythonExpression Value { get; init; }

    public override IEnumerable<PythonNode> Children => [Target, Value];
}
