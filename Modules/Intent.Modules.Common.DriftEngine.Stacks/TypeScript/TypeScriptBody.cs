using System.Text.RegularExpressions;
using Intent.Modules.Common.DriftEngine.Code;

namespace Intent.Modules.Common.DriftEngine.Stacks.TypeScript;

/// <summary>
/// What a function's body, or a module's top-level code, does, read from its syntax once every module is read: each call, the
/// type of what it is made on, the function it reaches, and what its arguments are (<see cref="ValueNode"/>), as far as the code
/// says without running it; each arrow function, function expression and object literal method as a unit of its own.
/// </summary>
/// <param name="owner">
/// The type it is in: a class's, an object's, or the module's (or component's) own for a module-level function and the
/// top-level code. A bare name is the module's in either case; a class's own members are reached through <c>this</c>.
/// </param>
internal sealed class TypeScriptBody(TypeScriptReader.Module module, TypeNode owner, IReadOnlyDictionary<string, TypeScriptReader.Module> modules, CodeGraph graph)
{
    /// <summary>How many locals deep a value is followed (<c>const a = b; const b = new X();</c>).</summary>
    private const int MaxDepth = 3;

    private static readonly (List<TypeRefNode> Types, bool Known) Unknown = ([], false);

    /// <summary>The nodes that are functions: each has its own parameters, and a <c>return</c> in it is its own.</summary>
    private static readonly HashSet<string> Functions = ["ArrowFunction", "FunctionExpression", "FunctionDeclaration", "MethodDeclaration", "Constructor", "GetAccessor", "SetAccessor"];

    /// <summary>Each call once, so a chain's call is the same node as the one the body has.</summary>
    private readonly Dictionary<TypeScriptNode, CallNode?> _calls = [];

    /// <summary>Each creation once, so a value's creation is the same node as the one the body has.</summary>
    private readonly Dictionary<TypeScriptNode, CreationNode?> _creations = [];

    private readonly Dictionary<TypeScriptNode, TypeRefNode?> _created = [];

    private readonly Dictionary<TypeRefNode, TypeScriptNode> _createdBy = [];

    private readonly Dictionary<TypeScriptNode, FunctionNode> _lambdas = [];

    /// <summary>Every node of the unit being read, for its locals.</summary>
    private IReadOnlyList<TypeScriptNode> _inside = [];

    private readonly Dictionary<TypeScriptNode, IReadOnlyList<ParameterNode>> _lambdaParameters = [];

    /// <summary>The compound assignments: each changes what it assigns other than with a plain <c>=</c>.</summary>
    private static readonly HashSet<string> CompoundAssignments =
    [
        "PlusEqualsToken", "MinusEqualsToken", "AsteriskEqualsToken", "AsteriskAsteriskEqualsToken", "SlashEqualsToken", "PercentEqualsToken",
        "LessThanLessThanEqualsToken", "GreaterThanGreaterThanEqualsToken", "GreaterThanGreaterThanGreaterThanEqualsToken", "AmpersandEqualsToken",
        "BarEqualsToken", "CaretEqualsToken", "BarBarEqualsToken", "AmpersandAmpersandEqualsToken", "QuestionQuestionEqualsToken"
    ];

    // The function being read, and its parameters, which the values that name them are.
    private TypeScriptNode? _function;
    private IReadOnlyList<ParameterNode> _functionParameters = [];

    /// <summary>What a function does: a function's, method's, constructor's or accessor's body.</summary>
    public FunctionNode Function(TypeScriptNode function, IReadOnlyList<ParameterNode> parameters)
    {
        (_function, _functionParameters) = (function, parameters);
        _inside = Inside(function).ToList();
        return Unit(function, parameters);
    }

    /// <summary>A field's, property's or module variable's initializer, as a value, read with the bodies so its names are followed.</summary>
    public ValueNode Initializer(TypeScriptNode initializer)
    {
        _inside = Inside(initializer).Prepend(initializer).ToList();
        return Value(initializer);
    }

    /// <summary>What a module does when it loads: its top-level statements (a <c>&lt;script setup&gt;</c>'s, when its component is set up).</summary>
    public FunctionNode TopLevel(IReadOnlyList<TypeScriptNode> statements)
    {
        _inside = statements.SelectMany(x => Inside(x).Prepend(x)).ToList();
        return Build(_inside, [], [], false, module.File.Canonical(statements[0].Start, statements[^1].End), module.File.SpanOf(statements[0].Start, statements[^1].End));
    }

    /// <summary>
    /// The markup JSX writes in what was just read (a function, an initializer or the top-level code), as the shape Razor and
    /// Vue markup have: each outermost element, and each element written in an attribute's value (react-router's
    /// <c>element={&lt;Page /&gt;}</c>), as a root. A fragment (<c>&lt;&gt;...&lt;/&gt;</c>) renders only its children, so they take its
    /// place; an element written in code among the children (<c>{items.map(x =&gt; &lt;Row /&gt;)}</c>, <c>{open &amp;&amp; &lt;Dialog /&gt;}</c>)
    /// is a child. A tag that starts with a capital letter or is qualified (<c>Ui.Grid</c>) names a component, as JSX
    /// compiles it. An attribute's value is a literal: a string as written, else <c>interpolated</c> with its code as parts
    /// (<c>to={`/orders/${id}`}</c> is <c>/orders/</c> and <c>id</c>; <c>onClick={save}</c> is <c>save</c> alone), as Razor's
    /// <c>href="/orders/@id"</c> is.
    /// </summary>
    /// <param name="scope">What was read.</param>
    /// <param name="built">The elements read so far, so code read twice (a variable's initializer that also runs) gives its markup once.</param>
    public IReadOnlyList<MarkupNode> Markup(TypeScriptNode scope, ISet<TypeScriptNode> built)
    {
        var roots = new List<MarkupNode>();
        var outermost = Nearest(scope, roots, built).ToList();
        roots.AddRange(outermost);
        return roots;
    }

    private static bool IsElement(TypeScriptNode node) => node.Kind is "JsxElement" or "JsxSelfClosingElement";

    /// <summary>The elements nearest a node: it, else those inside it, a fragment's children in its place.</summary>
    private IEnumerable<MarkupNode> Nearest(TypeScriptNode node, List<MarkupNode> roots, ISet<TypeScriptNode> built)
    {
        if (IsElement(node))
        {
            if (built.Add(node))
            {
                yield return Element(node, roots, built);
            }

            yield break;
        }

        var children = node.Is("JsxFragment") ? node.List("children") : node.Children;
        foreach (var child in children)
        {
            foreach (var element in Nearest(child, roots, built))
            {
                yield return element;
            }
        }
    }

    private MarkupNode Element(TypeScriptNode element, List<MarkupNode> roots, ISet<TypeScriptNode> built)
    {
        var opening = element.Is("JsxElement") ? element["openingElement"]! : element;
        var tag = opening["tagName"]!.Canonical;
        var attributes = new List<MarkupAttributeNode>();
        foreach (var attribute in opening["attributes"]?.List("properties") ?? [])
        {
            // An element written in an attribute's value is markup of its own.
            roots.AddRange(Nearest(attribute, roots, built));
            if (attribute.Is("JsxSpreadAttribute"))
            {
                var spread = attribute["expression"]!;
                attributes.Add(new MarkupAttributeNode { Name = "...", Value = spread.Source, Literal = Code(spread), At = attribute.Span });
                continue;
            }

            var initializer = attribute["initializer"];
            var expression = initializer?.Is("JsxExpression") == true ? initializer["expression"] : initializer;
            attributes.Add(new MarkupAttributeNode
            {
                Name = attribute["name"]!.Canonical,
                Value = expression is null ? null : expression.Is("StringLiteral") ? expression.Value : expression.Source,
                Literal = expression is null ? null : LiteralOf(expression, Value) ?? Code(expression),
                At = attribute.Span
            });
        }

        var children = element.Is("JsxElement") ? element.List("children").SelectMany(x => Nearest(x, roots, built)).ToList() : [];
        var source = element.File.Text[element.Start..element.End];
        var first = tag.Split('.')[0];
        return new MarkupNode
        {
            Tag = tag,
            Component = tag.Contains('.') || first.Length > 0 && char.IsUpper(first[0])
                ? new TypeRefNode { Text = tag, Name = tag[(tag.LastIndexOf('.') + 1)..], QualifiedName = tag, Language = TypeScriptLanguage.LanguageName }
                : null,
            Attributes = attributes,
            Children = children,
            Content = children.Count == 0 && element.Is("JsxElement") ? Content(element.List("children")) : null,
            At = element.Span,
            TextOf = () => CanonicalText.OfMarkup(source)
        };
    }

    /// <summary>Code given as an attribute's value: an <c>interpolated</c> literal of it alone.</summary>
    private LiteralNode Code(TypeScriptNode expression) => new() { Kind = "interpolated", Parts = [Value(expression)] };

    /// <summary>What an element holds when it holds only text and values: its text trimmed, each run of whitespace one space, and each <c>{value}</c>.</summary>
    private LiteralNode? Content(IReadOnlyList<TypeScriptNode> children)
    {
        var parts = new List<ValueNode>();
        foreach (var child in children)
        {
            if (child.Is("JsxText") && Regex.Replace(child.Value ?? "", @"\s+", " ").Trim() is { Length: > 0 } text)
            {
                parts.Add(new ValueNode { Text = text, Literal = new LiteralNode { Kind = "string", Value = text }, At = child.Span });
            }
            else if (child.Is("JsxExpression") && child["expression"] is { } expression)
            {
                parts.Add(Value(expression));
            }
        }

        return parts.Count switch
        {
            0 => null,
            _ when parts.All(x => x.Literal is { Kind: "string" }) => new LiteralNode { Kind = "string", Value = string.Join(" ", parts.Select(x => x.Literal!.Value)) },
            _ => new LiteralNode { Kind = "interpolated", Parts = parts }
        };
    }

    /// <param name="function">A function, method, constructor or accessor, whose body is a block, or an arrow function's an expression.</param>
    private FunctionNode Unit(TypeScriptNode function, IReadOnlyList<ParameterNode> parameters)
    {
        var inside = Inside(function).ToList();
        var body = function["body"];
        IReadOnlyList<ValueNode> returns = body is null ? []
            : body.Is("Block") ? inside.Where(x => x.Is("ReturnStatement") && x["expression"] is not null && Owns(function, x)).Select(x => Value(x["expression"]!)).ToList()
            : [Value(body)];
        return Build(inside, parameters, returns, function.Has("AsyncKeyword"), function.Canonical, function.Span);
    }

    private FunctionNode Build(IReadOnlyList<TypeScriptNode> inside, IReadOnlyList<ParameterNode> parameters, IReadOnlyList<ValueNode> returns, bool isAsync, string text, Span at) => new()
    {
        Parameters = parameters,
        Calls = inside.Where(x => x.Is("CallExpression")).Select(Call).OfType<CallNode>().ToList(),
        Creations = inside.Where(x => x.Is("NewExpression")).Select(Creation).OfType<CreationNode>().ToList(),
        Assignments = inside
            .Where(x => x.Is("BinaryExpression") && x["operatorToken"]?.Kind == "EqualsToken")
            .Select(x => new AssignmentNode(x["left"]!.Canonical, Value(x["right"]!), x.Span))
            .ToList(),
        Changes = inside.Select(Changed).OfType<TypeScriptNode>().Select(x => x.Canonical).ToList(),
        Returns = returns,
        IsAsync = isAsync,
        Text = text,
        At = at
    };

    /// <summary>What a node changes other than with a plain <c>=</c>: <c>x += y</c>, <c>x ??= y</c>, <c>x++</c>.</summary>
    private static TypeScriptNode? Changed(TypeScriptNode node) => node.Kind switch
    {
        "BinaryExpression" when node["operatorToken"]?.Kind is { } token && CompoundAssignments.Contains(token) => node["left"],
        "PrefixUnaryExpression" or "PostfixUnaryExpression" when node.Operator is "PlusPlusToken" or "MinusMinusToken" => node["operand"],
        _ => null
    };

    /// <summary>An arrow function, function expression or object literal method as its own unit, with its parameters as written.</summary>
    private FunctionNode Lambda(TypeScriptNode function)
    {
        if (!_lambdas.TryGetValue(function, out var unit))
        {
            _lambdas[function] = unit = Unit(function, LambdaParameters(function));
        }

        return unit;
    }

    /// <summary>A lambda's parameters, the same nodes for its unit and for the values that name them.</summary>
    private IReadOnlyList<ParameterNode> LambdaParameters(TypeScriptNode function)
    {
        if (!_lambdaParameters.TryGetValue(function, out var parameters))
        {
            _lambdaParameters[function] = parameters = function.List("parameters").Select(x => TypeScriptReader.Parameter(x, module.Strict)).ToList();
        }

        return parameters;
    }

    /// <summary>The nodes inside one, depth first; a decorator's aren't the code's.</summary>
    private static IEnumerable<TypeScriptNode> Inside(TypeScriptNode node)
    {
        foreach (var child in node.Children.Where(x => !x.Is("Decorator")))
        {
            yield return child;
            foreach (var nested in Inside(child))
            {
                yield return nested;
            }
        }
    }

    /// <summary>A node is the unit's own unless a nested function or class holds it.</summary>
    private static bool Owns(TypeScriptNode function, TypeScriptNode node)
    {
        for (var at = node.Parent; at is not null; at = at.Parent)
        {
            if (at == function)
            {
                return true;
            }

            if (Functions.Contains(at.Kind) || at.Kind is "ClassDeclaration" or "ClassExpression")
            {
                return false;
            }
        }

        return false;
    }

    /// <summary>A creation, with its arguments; null for one whose type is an expression, and while it is being read.</summary>
    private CreationNode? Creation(TypeScriptNode creation)
    {
        if (_creations.TryGetValue(creation, out var known))
        {
            return known;
        }

        _creations[creation] = null;
        if (Created(creation) is not { } type)
        {
            return null;
        }

        return _creations[creation] = new CreationNode
        {
            Type = type,
            Arguments = creation.List("arguments").Select(Value).ToList(),
            Text = creation.Canonical,
            At = creation.Span
        };
    }

    /// <summary>The type a creation creates, one node each, so a value's types say which creations it is.</summary>
    private TypeRefNode? Created(TypeScriptNode creation)
    {
        if (!_created.TryGetValue(creation, out var type))
        {
            _created[creation] = type = CreatedType(creation, module.Strict);
            if (type is not null)
            {
                _createdBy[type] = creation;
            }
        }

        return type;
    }

    /// <summary>The type a <c>new</c> names, as written: <c>new Map&lt;string, number&gt;()</c> is <c>Map</c>; <c>new (factory())()</c> names none.</summary>
    private static TypeRefNode? CreatedType(TypeScriptNode creation, bool? strict)
    {
        var callee = TypeScriptReader.Unwrapped(creation["expression"]!);
        if (callee.Kind is not ("Identifier" or "PropertyAccessExpression"))
        {
            return null;
        }

        var qualified = callee.Canonical;
        var arguments = creation.List("typeArguments").Select(x => TypeScriptReader.TypeRef(x, strict)).ToList();
        return new TypeRefNode
        {
            Text = arguments.Count == 0 ? qualified : $"{qualified}<{string.Join(",", arguments.Select(x => x.Text))}>",
            Name = qualified[(qualified.LastIndexOf('.') + 1)..],
            QualifiedName = qualified,
            Arguments = arguments,
            Nullable = strict is null ? "unknown" : "false",
            Language = TypeScriptLanguage.LanguageName
        };
    }

    private CallNode? Call(TypeScriptNode call)
    {
        if (_calls.TryGetValue(call, out var known))
        {
            return known;
        }

        _calls[call] = null;
        if (Resolve(call, 0) is not { } resolved)
        {
            return null;
        }

        var on = call["expression"] is { Kind: "PropertyAccessExpression" } access ? TypeScriptReader.Unwrapped(access["expression"]!)
            : TypeScriptReader.Unwrapped(call["expression"]!) is { Kind: "CallExpression" } curried ? curried : null;
        if (on is { Kind: "AwaitExpression" })
        {
            on = TypeScriptReader.Unwrapped(on["expression"]!);
        }

        return _calls[call] = new CallNode
        {
            Name = resolved.Name,
            Receiver = resolved.Receiver,
            ReceiverType = resolved.ReceiverType,
            On = on is { Kind: "CallExpression" } ? Call(on) : null,
            Arguments = call.List("arguments").Select(Value).ToList(),
            TypeArguments = TypeArguments(call),
            Target = resolved.Targets is [var target] ? target : null,
            Text = call.Canonical,
            At = call.Span
        };
    }

    private IReadOnlyList<TypeRefNode> TypeArguments(TypeScriptNode call) => call.List("typeArguments").Select(x => TypeScriptReader.TypeRef(x, module.Strict)).ToList();

    /// <summary>
    /// A call's name, what it is made on (<c>api.getProducts()</c>; nothing for a bare call or one on <c>this</c>) and that's
    /// type, and the functions it can reach. A bare call reaches a function the module declares or imports, unless a local or a
    /// parameter has the name. A call of what a call returns has that call's name. A call whose callee isn't a name
    /// (<c>handlers[key]()</c>, <c>import('./x')</c>, <c>super()</c>) isn't one.
    /// </summary>
    private (string Name, string? Receiver, TypeRefNode? ReceiverType, IReadOnlyList<CallTarget> Targets)? Resolve(TypeScriptNode call, int depth)
    {
        var callee = TypeScriptReader.Unwrapped(call["expression"]!);
        var count = call.List("arguments").Count;
        switch (callee.Kind)
        {
            case "Identifier":
            {
                var name = callee.Value!;
                if (Local(name, call.Start) is not null || Parameter(callee, name) is not null)
                {
                    return (name, null, null, []);
                }

                return (name, null, null, Scoped(name) is { Whole: false } scoped && scoped.Module.Owner is { } holder ? graph.Targets(scoped.Name, null, null, count, holder) : []);
            }
            case "PropertyAccessExpression":
            {
                var name = callee["name"]!.Value!;
                var receiver = TypeScriptReader.Unwrapped(callee["expression"]!);
                if (receiver.Is("ThisKeyword"))
                {
                    return (name, null, null, graph.Targets(name, null, null, count, owner));
                }

                var (types, known) = TypesOf(receiver, call.Start, depth);
                var typed = known && types is [var only] ? only : null;
                var written = receiver.Canonical;
                return (name, written, typed, graph.Targets(name, written, typed, count, owner));
            }
            case "CallExpression" when depth < MaxDepth && Resolve(callee, depth + 1) is { } inner:
                // A call of what a call returns (Zustand's create<State>()(...), connect(map)(View)) is named as the call it is
                // made on, which is its On; what it reaches isn't known.
                return (inner.Name, inner.Receiver, inner.ReceiverType, []);
            default:
                return null;
        }
    }

    /// <summary>What the function a call reaches returns, with the type arguments the call passes.</summary>
    private (List<TypeRefNode> Types, bool Known) Returned(TypeScriptNode call, int depth) =>
        Resolve(call, depth + 1) is { Targets: [var target] } && target.ReturnsGiven(TypeArguments(call)) is { } returns ? ([returns], true) : Unknown;

    private ValueNode Value(TypeScriptNode expression)
    {
        var (types, known) = TypesOf(expression, expression.Start, 0);
        var unwrapped = TypeScriptReader.Unwrapped(expression);
        var bare = Bare(expression);
        return new ValueNode
        {
            Text = expression.Canonical,
            Types = types,
            IsKnown = known && types.Count > 0,
            Literal = LiteralOf(expression, Value),
            Function = unwrapped.Kind is "ArrowFunction" or "FunctionExpression" ? Lambda(unwrapped) : null,
            Creations = types.Select(x => _createdBy.TryGetValue(x, out var creation) ? Creation(creation) : null).OfType<CreationNode>().ToList(),
            Members = unwrapped.Is("ObjectLiteralExpression") ? unwrapped.List("properties").Select(Member).OfType<InitializerNode>().ToList() : [],
            Items = unwrapped.Is("ArrayLiteralExpression") ? unwrapped.List("elements").Select(Value).ToList() : [],
            Joined = Joined(bare),
            GivenOf = () => Given(bare),
            Parameter = bare.Is("Identifier") && Local(bare.Value!, bare.Start) is null ? ParameterOf(bare) : null,
            Field = FieldRead(bare),
            Call = bare.Is("CallExpression") ? Call(bare) : null,
            Trimmed = Trimmed(bare),
            At = expression.Span
        };
    }

    /// <summary>What a value is under its parentheses, <c>!</c>, <c>satisfies</c>, <c>as</c> and type assertions, none of which change its text.</summary>
    private static TypeScriptNode Bare(TypeScriptNode expression)
    {
        expression = TypeScriptReader.Unwrapped(expression);
        return expression.Kind is "AsExpression" or "TypeAssertionExpression" ? Bare(expression["expression"]!) : expression;
    }

    /// <summary>A <c>+</c> chain's operands, or <c>a.concat(b, c)</c>'s receiver and arguments.</summary>
    private IReadOnlyList<ValueNode> Joined(TypeScriptNode bare) => bare switch
    {
        { Kind: "BinaryExpression" } when bare["operatorToken"]?.Kind == "PlusToken" => Operands(bare).Select(Value).ToList(),
        { Kind: "CallExpression" } when TypeScriptReader.Unwrapped(bare["expression"]!) is { Kind: "PropertyAccessExpression" } concat && concat["name"]!.Value == "concat"
            => [Value(concat["expression"]!), ..bare.List("arguments").Select(Value)],
        _ => []
    };

    private static IEnumerable<TypeScriptNode> Operands(TypeScriptNode binary) =>
        (binary["left"] is { Kind: "BinaryExpression" } left && left["operatorToken"]?.Kind == "PlusToken" ? Operands(left) : [binary["left"]!]).Append(binary["right"]!);

    /// <summary>
    /// The values it stands for: what a local is given before the use (with any other change as a value whose text isn't
    /// known), a conditional's branches, both sides of <c>??</c> and <c>||</c>, what <c>x.toString()</c>, <c>String(x)</c> and
    /// <c>new URL(x)</c> are made from.
    /// </summary>
    private IReadOnlyList<ValueNode> Given(TypeScriptNode bare)
    {
        switch (bare.Kind)
        {
            case "Identifier" when Local(bare.Value!, bare.Start) is { } local && !local.Is("FunctionDeclaration"):
            {
                var name = bare.Value!;
                var values = new List<ValueNode>();
                if (local["initializer"] is { } initializer)
                {
                    values.Add(Value(initializer));
                }

                foreach (var node in _inside.Where(x => x.Start > local.Start && x.Start < bare.Start))
                {
                    if (node.Is("BinaryExpression") && node["operatorToken"]?.Kind == "EqualsToken" && node["left"] is { Kind: "Identifier" } left && left.Value == name)
                    {
                        values.Add(Value(node["right"]!));
                    }
                    else if (Changed(node) is { Kind: "Identifier" } changed && changed.Value == name)
                    {
                        values.Add(new ValueNode { Text = node.Canonical, At = node.Span });
                    }
                }

                return values;
            }
            case "ConditionalExpression":
                return [Value(bare["whenTrue"]!), Value(bare["whenFalse"]!)];
            case "BinaryExpression" when bare["operatorToken"]?.Kind is "QuestionQuestionToken" or "BarBarToken":
                return [Value(bare["left"]!), Value(bare["right"]!)];
            case "CallExpression" when TypeScriptReader.Unwrapped(bare["expression"]!) is { Kind: "PropertyAccessExpression" } access && access["name"]!.Value == "toString" && bare.List("arguments").Count == 0:
                return [Value(access["expression"]!)];
            case "CallExpression" when TypeScriptReader.Unwrapped(bare["expression"]!) is { Kind: "Identifier", Value: "String" } && bare.List("arguments") is [var converted]:
                return [Value(converted)];
            case "NewExpression" when TypeScriptReader.Unwrapped(bare["expression"]!) is { Kind: "Identifier", Value: "URL" } && bare.List("arguments") is [var url]:
                return [Value(url)];
            default:
                return [];
        }
    }

    /// <summary>The parameter of the function being read, or of an arrow function in it, that a name is, as the unit's own node.</summary>
    private ParameterNode? ParameterOf(TypeScriptNode identifier)
    {
        var name = identifier.Value!;
        foreach (var function in identifier.Ancestors().Where(x => Functions.Contains(x.Kind)))
        {
            var index = function.List("parameters").ToList().FindIndex(x => x["name"] is { Kind: "Identifier" } declared && declared.Value == name);
            if (index < 0)
            {
                continue;
            }

            var parameters = function == _function ? _functionParameters : function.Kind is "ArrowFunction" or "FunctionExpression" or "MethodDeclaration" ? LambdaParameters(function) : [];
            return index < parameters.Count ? parameters[index] : null;
        }

        return null;
    }

    /// <summary>
    /// The field or property a value reads: a module variable, by its name or through an import; one of the type's own, on
    /// <c>this</c>; or one of an object's, a class's or a whole module's, through its name (<c>config.baseUrl</c>).
    /// </summary>
    private FieldRef? FieldRead(TypeScriptNode bare)
    {
        switch (bare.Kind)
        {
            case "Identifier" when Local(bare.Value!, bare.Start) is null && Parameter(bare, bare.Value!) is null:
                return Scoped(bare.Value!) is { Whole: false } variable && variable.Module.Owner is { } holder ? graph.FieldOf(holder, variable.Name) : null;
            case "PropertyAccessExpression":
            {
                var receiver = TypeScriptReader.Unwrapped(bare["expression"]!);
                var name = bare["name"]!.Value!;
                if (receiver.Is("ThisKeyword"))
                {
                    return graph.FieldOf(owner, name);
                }

                if (!receiver.Is("Identifier") || Local(receiver.Value!, receiver.Start) is not null || Parameter(receiver, receiver.Value!) is not null || Scoped(receiver.Value!) is not { } scoped)
                {
                    return null;
                }

                var type = scoped.Whole ? scoped.Module.Owner : scoped.Module.Types.GetValueOrDefault(scoped.Name);
                return type is null ? null : graph.FieldOf(type, name);
            }
            default:
                return null;
        }
    }

    /// <summary>A trim of whitespace: <c>trim()</c>, <c>trimStart()</c>, <c>trimEnd()</c>.</summary>
    private TrimNode? Trimmed(TypeScriptNode bare) =>
        bare.Is("CallExpression") && bare.List("arguments").Count == 0 && TypeScriptReader.Unwrapped(bare["expression"]!) is { Kind: "PropertyAccessExpression" } access
        && access["name"]!.Value is "trim" or "trimStart" or "trimEnd" or "trimLeft" or "trimRight"
            ? new TrimNode(Value(access["expression"]!), access["name"]!.Value is "trim" or "trimStart" or "trimLeft", access["name"]!.Value is "trim" or "trimEnd" or "trimRight", [])
            : null;

    /// <summary>An object literal's member: <c>path: '/'</c>, a shorthand <c>routes</c>, a method, or a spread as <c>...</c>.</summary>
    private InitializerNode? Member(TypeScriptNode property) => property.Kind switch
    {
        "PropertyAssignment" => new InitializerNode(TypeScriptReader.NameOf(property["name"]!), Value(property["initializer"]!)),
        "ShorthandPropertyAssignment" => new InitializerNode(property["name"]!.Value!, Value(property["name"]!)),
        "SpreadAssignment" => new InitializerNode("...", Value(property["expression"]!)),
        "MethodDeclaration" or "GetAccessor" or "SetAccessor" => new InitializerNode(TypeScriptReader.NameOf(property["name"]!),
            new ValueNode { Text = property.Canonical, Function = Lambda(property), At = property.Span }),
        _ => null
    };

    /// <summary>The types an expression can be, and whether every way it is given is known (<see cref="ValueNode"/>).</summary>
    /// <param name="before">Where it is used: a local's assignments after it don't give it.</param>
    private (List<TypeRefNode> Types, bool Known) TypesOf(TypeScriptNode expression, int before, int depth)
    {
        expression = TypeScriptReader.Unwrapped(expression);
        switch (expression.Kind)
        {
            case "ThisKeyword":
                return ([RefTo(owner)], true);
            case "Identifier" when depth < MaxDepth:
                return Named(expression, before, depth);
            case "NewExpression":
                return Created(expression) is { } created ? ([created], true) : Unknown;
            case "CallExpression" when depth < MaxDepth:
                return Returned(expression, depth);
            case "AwaitExpression":
            {
                // await gives what a promise resolves to.
                var (types, known) = TypesOf(expression["expression"]!, before, depth);
                return (types.Select(x => x is { Name: "Promise" or "PromiseLike", Arguments: [var resolved] } ? resolved : x).ToList(), known);
            }
            case "AsExpression" or "TypeAssertionExpression" when expression["type"]!.Canonical == "const":
                return TypesOf(expression["expression"]!, before, depth);
            case "AsExpression" or "TypeAssertionExpression":
                return OrDeclared(TypesOf(expression["expression"]!, before, depth), expression["type"]!);
            case "ConditionalExpression":
            {
                var whenTrue = TypesOf(expression["whenTrue"]!, before, depth);
                var whenFalse = TypesOf(expression["whenFalse"]!, before, depth);
                return ([..whenTrue.Types, ..whenFalse.Types], whenTrue.Known && whenFalse.Known);
            }
            case "PropertyAccessExpression" when TypeScriptReader.Unwrapped(expression["expression"]!).Is("ThisKeyword"):
                return FieldOf(owner, expression["name"]!.Value!);
            case "PropertyAccessExpression" when depth < MaxDepth:
            {
                // A property path through in-scope types: this.services.api is OrderServices' api.
                var (types, known) = TypesOf(expression["expression"]!, before, depth + 1);
                return known && types is [var only] && graph.TypeFor(only, owner) is { } type ? FieldOf(type, expression["name"]!.Value!) : Unknown;
            }
            default:
                return Unknown;
        }
    }

    /// <summary>What a cast or a typed local wraps says more than its declared type, which is what it is when that isn't known.</summary>
    private (List<TypeRefNode> Types, bool Known) OrDeclared((List<TypeRefNode> Types, bool Known) value, TypeScriptNode declared) =>
        value.Known && value.Types.Count > 0 ? value : ([TypeScriptReader.TypeRef(declared, module.Strict)], true);

    /// <summary>A name in the code: a local declared before the use, then a parameter (an arrow function's included), then the module's.</summary>
    private (List<TypeRefNode> Types, bool Known) Named(TypeScriptNode identifier, int before, int depth)
    {
        var name = identifier.Value!;
        if (Local(name, before) is { } local)
        {
            if (local.Is("FunctionDeclaration"))
            {
                return Unknown;
            }

            // Its initializer and every plain assignment to it before the use (one in each branch of an if is both).
            var values = new List<TypeScriptNode>();
            if (local["initializer"] is { } initializer)
            {
                values.Add(initializer);
            }

            values.AddRange(_inside
                .Where(x => x.Is("BinaryExpression") && x["operatorToken"]?.Kind == "EqualsToken" && x["left"] is { Kind: "Identifier" } left && left.Value == name
                            && x.Start > local.Start && x.Start < before)
                .Select(x => x["right"]!));
            var given = values.Select(x => TypesOf(x, x.Start, depth + 1)).ToList();
            (List<TypeRefNode> Types, bool Known) value = (given.SelectMany(x => x.Types).ToList(), given.Count > 0 && given.All(x => x.Known));
            return local["type"] is { } declared ? OrDeclared(value, declared) : value;
        }

        if (Parameter(identifier, name) is { } parameter)
        {
            return parameter["type"] is { } type ? ([TypeScriptReader.TypeRef(type, module.Strict)], true) : Unknown;
        }

        return ModuleScoped(name);
    }

    /// <summary>
    /// A local the unit declares with the name: a variable before the position, or a function. A variable or function declared
    /// outside every function is the module's, not a local.
    /// </summary>
    private TypeScriptNode? Local(string name, int before) =>
        _inside.LastOrDefault(x => (x.Is("VariableDeclaration") && x.Start < before || x.Is("FunctionDeclaration")) && x["name"] is { Kind: "Identifier" } declared && declared.Value == name
                                   && x.Ancestors().Any(a => Functions.Contains(a.Kind)));

    /// <summary>The parameter with the name of the function a node is in, or of one around it.</summary>
    private static TypeScriptNode? Parameter(TypeScriptNode node, string name) =>
        node.Ancestors().Where(x => Functions.Contains(x.Kind))
            .Select(x => x.List("parameters").FirstOrDefault(p => p["name"] is { Kind: "Identifier" } declared && declared.Value == name))
            .FirstOrDefault(x => x is not null);

    /// <summary>
    /// The module a module-level name is in, and its name there: the module's own, or the one an import binds it to; a
    /// namespace import (<c>import * as api</c>) is the whole module. Null for an import of a package or a file out of scope.
    /// </summary>
    private (TypeScriptReader.Module Module, string Name, bool Whole)? Scoped(string name)
    {
        if (!module.Bindings.TryGetValue(name, out var import))
        {
            return (module, name, false);
        }

        if (import.File is null || !modules.TryGetValue(import.File, out var from))
        {
            return null;
        }

        return import.Wildcard ? (from, "", true) : (from, import.Target[(import.Target.LastIndexOf('.') + 1)..], false);
    }

    /// <summary>A module-level name's types: a whole module's, an object's, or a module variable's.</summary>
    private (List<TypeRefNode> Types, bool Known) ModuleScoped(string name)
    {
        if (Scoped(name) is not { } scoped)
        {
            return Unknown;
        }

        if (scoped.Whole)
        {
            return scoped.Module.Owner is { } whole ? ([RefTo(whole)], true) : Unknown;
        }

        if (scoped.Module.Types.TryGetValue(scoped.Name, out var type) && type.Kind == "object")
        {
            return ([RefTo(type)], true);
        }

        return scoped.Module.Owner is { } holder ? FieldOf(holder, scoped.Name) : Unknown;
    }

    /// <summary>A field or property of a type, or one it inherits: its declared type, else what its initializer creates.</summary>
    private (List<TypeRefNode> Types, bool Known) FieldOf(TypeNode type, string name) =>
        graph.MembersOf(type).FirstOrDefault(x => x.Name == name && x.Kind is "field" or "property") switch
        {
            { Type: { } declared } => ([declared], true),
            { Value: { IsKnown: true, Types: [var created] } } => ([created], true),
            _ => Unknown
        };

    /// <summary>A reference to an in-scope type by its full name, which the graph resolves to it.</summary>
    private static TypeRefNode RefTo(TypeNode type) => new()
    {
        Text = type.Name,
        Name = type.Name,
        QualifiedName = type.Namespace.Length == 0 ? type.Name : $"{type.Namespace}.{type.Name}",
        Language = TypeScriptLanguage.LanguageName
    };

    /// <summary>
    /// A declaration's initializer as a value, read with the declarations: what it is as written, the type a creation or a
    /// cast gives it, and an object or array literal's members and items. What its names and calls are is read with the
    /// bodies.
    /// </summary>
    internal static ValueNode Constant(TypeScriptNode value, bool? strict)
    {
        var unwrapped = TypeScriptReader.Unwrapped(value);
        var type = unwrapped.Kind switch
        {
            "NewExpression" => CreatedType(unwrapped, strict),
            "AsExpression" or "TypeAssertionExpression" when unwrapped["type"]!.Canonical != "const" => TypeScriptReader.TypeRef(unwrapped["type"]!, strict),
            _ => null
        };
        return new ValueNode
        {
            Text = value.Canonical,
            Types = type is null ? [] : [type],
            IsKnown = type is not null,
            Literal = LiteralOf(value, x => Constant(x, strict)),
            Creations = type is not null && unwrapped.Is("NewExpression")
                ? [new CreationNode { Type = type, Arguments = unwrapped.List("arguments").Select(x => Constant(x, strict)).ToList(), Text = unwrapped.Canonical, At = unwrapped.Span }]
                : [],
            Members = unwrapped.Is("ObjectLiteralExpression")
                ? unwrapped.List("properties").Select(x => x.Kind switch
                {
                    "PropertyAssignment" => new InitializerNode(TypeScriptReader.NameOf(x["name"]!), Constant(x["initializer"]!, strict)),
                    "ShorthandPropertyAssignment" => new InitializerNode(x["name"]!.Value!, Constant(x["name"]!, strict)),
                    "SpreadAssignment" => new InitializerNode("...", Constant(x["expression"]!, strict)),
                    "MethodDeclaration" or "GetAccessor" or "SetAccessor" => new InitializerNode(TypeScriptReader.NameOf(x["name"]!), new ValueNode { Text = x.Canonical, At = x.Span }),
                    _ => null
                }).OfType<InitializerNode>().ToList()
                : [],
            Items = unwrapped.Is("ArrayLiteralExpression") ? unwrapped.List("elements").Select(x => Constant(x, strict)).ToList() : [],
            At = value.Span
        };
    }

    /// <summary>
    /// The literal an expression is: a string (a template without substitutions included), a number with its sign, a boolean,
    /// <c>null</c>, or a template with substitutions as <c>interpolated</c>, with its parts.
    /// </summary>
    /// <param name="value">What a template's substitutions are; as written when null.</param>
    internal static LiteralNode? LiteralOf(TypeScriptNode expression, Func<TypeScriptNode, ValueNode>? value)
    {
        expression = TypeScriptReader.Unwrapped(expression);
        switch (expression.Kind)
        {
            case "StringLiteral" or "NoSubstitutionTemplateLiteral":
                return new LiteralNode { Kind = "string", Value = expression.Value };
            case "NumericLiteral" or "BigIntLiteral":
                return new LiteralNode { Kind = "number", Value = expression.Value };
            case "PrefixUnaryExpression" when expression.Operator is "MinusToken" or "PlusToken" && expression["operand"] is { Kind: "NumericLiteral" or "BigIntLiteral" } operand:
                return new LiteralNode { Kind = "number", Value = (expression.Operator == "MinusToken" ? "-" : "") + operand.Value };
            case "TrueKeyword":
                return new LiteralNode { Kind = "boolean", Value = "true" };
            case "FalseKeyword":
                return new LiteralNode { Kind = "boolean", Value = "false" };
            case "NullKeyword":
                return new LiteralNode { Kind = "null" };
            case "TemplateExpression":
            {
                var parts = new List<ValueNode>();
                Text(expression["head"]!);
                foreach (var span in expression.List("templateSpans"))
                {
                    var hole = span["expression"]!;
                    parts.Add(value?.Invoke(hole) ?? new ValueNode { Text = hole.Canonical, At = hole.Span });
                    Text(span["literal"]!);
                }

                return new LiteralNode { Kind = "interpolated", Parts = parts };

                void Text(TypeScriptNode part)
                {
                    if (!string.IsNullOrEmpty(part.Value))
                    {
                        parts.Add(new ValueNode { Text = part.Value, Literal = new LiteralNode { Kind = "string", Value = part.Value }, At = part.Span });
                    }
                }
            }
            default:
                return null;
        }
    }
}
