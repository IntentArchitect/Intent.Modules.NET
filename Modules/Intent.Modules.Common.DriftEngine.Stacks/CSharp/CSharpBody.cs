using System.Text.RegularExpressions;
using Intent.Modules.Common.DriftEngine.Code;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intent.Modules.Common.DriftEngine.Stacks.CSharp;

/// <summary>
/// What a method's or constructor's body, or a file's top-level statements, do, read from their syntax once every type is
/// read: each call, the type of what it is made on, the method it reaches, and what its arguments are
/// (<see cref="ValueNode"/>), as far as the code says without running it; each lambda as a unit of its own.
/// </summary>
/// <param name="roots">The member's declaration, or the file's top-level statements.</param>
/// <param name="owner">The type whose members an unqualified call reaches: the member's type, or top-level code's <c>Program</c>.</param>
internal sealed class CSharpBody(IReadOnlyList<SyntaxNode> roots, TypeNode owner, string file, CodeGraph graph)
{
    /// <summary>How many locals deep a value is followed (<c>var a = b; var b = new X();</c>).</summary>
    private const int MaxDepth = 3;

    private static readonly Regex Whitespace = new(@"\s+", RegexOptions.CultureInvariant);

    private static readonly (List<TypeRefNode> Types, bool Known) Unknown = ([], false);

    /// <summary>The binary operators whose result is a bool: comparisons, the logical ones and a type test.</summary>
    private static readonly HashSet<SyntaxKind> Comparisons =
    [
        SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression, SyntaxKind.LessThanExpression, SyntaxKind.LessThanOrEqualExpression,
        SyntaxKind.GreaterThanExpression, SyntaxKind.GreaterThanOrEqualExpression, SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression,
        SyntaxKind.IsExpression
    ];

    /// <summary>Each call once, so a chain's call is the same node as the one the body has.</summary>
    private readonly Dictionary<InvocationExpressionSyntax, CallNode?> _calls = [];

    /// <summary>Each creation once, so a value's creation is the same node as the one the body has; null while it is read.</summary>
    private readonly Dictionary<ObjectCreationExpressionSyntax, CreationNode?> _creations = [];

    /// <summary>The type each creation creates, one node each, so a value's types say which creations it is.</summary>
    private readonly Dictionary<ObjectCreationExpressionSyntax, TypeRefNode> _created = [];

    private readonly Dictionary<TypeRefNode, ObjectCreationExpressionSyntax> _createdBy = new(ReferenceEqualityComparer.Instance);

    /// <summary>Each method group once, so a lambda's is the same node as the one the body has.</summary>
    private readonly Dictionary<ExpressionSyntax, CallNode?> _references = [];

    private readonly Dictionary<AnonymousFunctionExpressionSyntax, FunctionNode> _lambdas = [];

    private readonly Dictionary<AnonymousFunctionExpressionSyntax, IReadOnlyList<ParameterNode>> _lambdaParameters = [];

    // Every node of the body in order, and its locals and plain assignments by name, found once: a name is looked up at each
    // use, and a scaffolded OnModelCreating makes thousands of calls on its lambdas' parameters.
    private IReadOnlyList<SyntaxNode>? _nodes;
    private ILookup<string, VariableDeclaratorSyntax>? _locals;
    private ILookup<string, AssignmentExpressionSyntax>? _assignments;

    // The member whose body this is, and its parameters, which the values that name them are.
    private BaseMethodDeclarationSyntax? _declaration;
    private IReadOnlyList<ParameterNode> _declarationParameters = [];

    /// <summary>What a method's or constructor's body does; the unit is its block or expression body.</summary>
    public FunctionNode Member(BaseMethodDeclarationSyntax declaration, IReadOnlyList<ParameterNode> parameters)
    {
        (_declaration, _declarationParameters) = (declaration, parameters);
        var body = (SyntaxNode?)declaration.Body ?? (SyntaxNode?)declaration.ExpressionBody ?? declaration;
        return Function(declaration, [declaration], parameters,
            declaration is MethodDeclarationSyntax { ReturnType: PredefinedTypeSyntax { Keyword.Text: "void" } } or ConstructorDeclarationSyntax ? null : declaration.ExpressionBody?.Expression,
            declaration.Modifiers.Any(SyntaxKind.AsyncKeyword), Normalise(body), CSharpReader.SpanOf(body, file));
    }

    /// <summary>A field's or property's initializer, as a value of its type.</summary>
    public ValueNode Initializer(ExpressionSyntax initializer) => Value(initializer, initializer.SpanStart);

    /// <summary>A file's top-level statements, as one unit.</summary>
    public FunctionNode TopLevel()
    {
        var (first, last) = (CSharpReader.SpanOf(roots[0], file), CSharpReader.SpanOf(roots[^1], file));
        return Function(null, roots, [], null, roots.Any(x => x.DescendantNodes(IsOwn).OfType<AwaitExpressionSyntax>().Any()),
            string.Join(" ", roots.Select(Normalise)), first with { EndLine = last.EndLine, EndColumn = last.EndColumn });
    }

    /// <param name="function">The member or lambda; null for top-level statements.</param>
    /// <param name="nodes">What it is written in.</param>
    /// <param name="returned">Its expression body, when it returns that.</param>
    private FunctionNode Function(SyntaxNode? function, IReadOnlyList<SyntaxNode> nodes, IReadOnlyList<ParameterNode> parameters, ExpressionSyntax? returned, bool isAsync, string text, Span at)
    {
        var inside = nodes.SelectMany(x => x.DescendantNodes()).ToList();
        return new FunctionNode
        {
            Parameters = parameters,
            Calls = inside
                .Select(x => x switch
                {
                    InvocationExpressionSyntax invocation => Call(invocation),
                    ArgumentSyntax { Expression: IdentifierNameSyntax or MemberAccessExpressionSyntax } argument
                        when argument.Parent?.Parent is not InvocationExpressionSyntax named || !IsNameOf(named) => Reference(argument.Expression),
                    _ => null
                })
                .OfType<CallNode>()
                .ToList(),
            Creations = inside.OfType<ObjectCreationExpressionSyntax>().Select(Creation).OfType<CreationNode>().ToList(),
            Assignments = inside.OfType<AssignmentExpressionSyntax>()
                .Where(x => x.IsKind(SyntaxKind.SimpleAssignmentExpression) && x.Parent is not InitializerExpressionSyntax)
                .Select(x => new AssignmentNode(Normalise(x.Left), Value(x.Right, x.SpanStart), CSharpReader.SpanOf(x, file))
                {
                    Receiver = x.Left is MemberAccessExpressionSyntax { Expression: not ThisExpressionSyntax and not BaseExpressionSyntax } access ? Value(access.Expression, x.SpanStart) : null
                })
                .ToList(),
            Changes = inside.Select(Changed).OfType<ExpressionSyntax>().Select(Normalise).ToList(),
            // An expression body that throws (=> throw new NotSupportedException()) returns nothing.
            Returns = returned is ThrowExpressionSyntax ? []
                : returned is not null ? [Value(returned, returned.SpanStart)]
                : inside.OfType<ReturnStatementSyntax>()
                    .Where(x => x.Expression is not null && x.Ancestors().FirstOrDefault(IsFunction) == function)
                    .Select(x => Value(x.Expression!, x.Expression!.SpanStart))
                    .ToList(),
            IsAsync = isAsync,
            Text = text,
            At = at
        };
    }

    /// <summary>What a node changes other than with a plain <c>=</c>: <c>x += y</c>, <c>x++</c>, <c>ref x</c> or <c>out x</c>.</summary>
    private static ExpressionSyntax? Changed(SyntaxNode node) => node switch
    {
        AssignmentExpressionSyntax assignment when !assignment.IsKind(SyntaxKind.SimpleAssignmentExpression) => assignment.Left,
        PrefixUnaryExpressionSyntax prefix when prefix.IsKind(SyntaxKind.PreIncrementExpression) || prefix.IsKind(SyntaxKind.PreDecrementExpression) => prefix.Operand,
        PostfixUnaryExpressionSyntax postfix when postfix.IsKind(SyntaxKind.PostIncrementExpression) || postfix.IsKind(SyntaxKind.PostDecrementExpression) => postfix.Operand,
        ArgumentSyntax argument when argument.RefKindKeyword.IsKind(SyntaxKind.RefKeyword) || argument.RefKindKeyword.IsKind(SyntaxKind.OutKeyword) => argument.Expression,
        _ => null
    };

    /// <summary>A lambda or anonymous method as its own unit.</summary>
    private FunctionNode Lambda(AnonymousFunctionExpressionSyntax lambda)
    {
        if (_lambdas.TryGetValue(lambda, out var known))
        {
            return known;
        }

        return _lambdas[lambda] = Function(lambda, [lambda], LambdaParameters(lambda), lambda.ExpressionBody, lambda.AsyncKeyword.IsKind(SyntaxKind.AsyncKeyword),
            Normalise(lambda), CSharpReader.SpanOf(lambda, file));
    }

    /// <summary>A lambda's parameters, the same nodes for its unit and for the values that name them.</summary>
    private IReadOnlyList<ParameterNode> LambdaParameters(AnonymousFunctionExpressionSyntax lambda)
    {
        if (!_lambdaParameters.TryGetValue(lambda, out var parameters))
        {
            _lambdaParameters[lambda] = parameters = CSharpReader.ParametersOf(Parameters(lambda), file);
        }

        return parameters;
    }

    /// <summary>Code that runs as its own unit: a nested lambda's or local function's <c>return</c> isn't its container's.</summary>
    private static bool IsFunction(SyntaxNode node) => node is AnonymousFunctionExpressionSyntax or LocalFunctionStatementSyntax or BaseMethodDeclarationSyntax or AccessorDeclarationSyntax;

    private static bool IsOwn(SyntaxNode node) => !IsFunction(node);

    private CallNode? Call(InvocationExpressionSyntax invocation)
    {
        if (_calls.TryGetValue(invocation, out var known))
        {
            return known;
        }

        // nameof(x) is a string, not a call.
        if (IsNameOf(invocation) || Resolve(invocation.Expression, invocation.ArgumentList.Arguments.Count, invocation.SpanStart, 0) is not { } call)
        {
            return _calls[invocation] = null;
        }

        return _calls[invocation] = new CallNode
        {
            Name = call.Name,
            Receiver = call.Receiver,
            ReceiverType = call.ReceiverType,
            On = invocation.Expression is MemberAccessExpressionSyntax { Expression: InvocationExpressionSyntax on } ? Call(on) : null,
            Arguments = invocation.ArgumentList.Arguments.Select(x => Value(x.Expression, invocation.SpanStart, name: x.NameColon?.Name.Identifier.Text)).ToList(),
            TypeArguments = TypeArgumentsOf(invocation.Expression),
            Target = call.Targets is [var target] ? target : null,
            Text = Normalise(invocation),
            CanonicalOf = () => CSharpReader.Canonical(invocation),
            At = CSharpReader.SpanOf(invocation, file)
        };
    }

    /// <summary>
    /// A method group passed as a value (<c>items.ForEach(Audit)</c>). C# writes it as any other name, so a name is one only
    /// when the code doesn't type it as a value and an in-scope method has it.
    /// </summary>
    private CallNode? Reference(ExpressionSyntax expression)
    {
        if (_references.TryGetValue(expression, out var known))
        {
            return known;
        }

        if (TypesOf(expression, expression.SpanStart, 0).Known || Resolve(expression, null, expression.SpanStart, 0) is not { Targets.Count: > 0 } call)
        {
            return _references[expression] = null;
        }

        return _references[expression] = new CallNode
        {
            Name = call.Name,
            Receiver = call.Receiver,
            ReceiverType = call.ReceiverType,
            TypeArguments = TypeArgumentsOf(expression),
            IsReference = true,
            Target = call.Targets is [var target] ? target : null,
            Text = Normalise(expression),
            CanonicalOf = () => CSharpReader.Canonical(expression),
            At = CSharpReader.SpanOf(expression, file)
        };
    }

    /// <summary>A call's or method group's name, what it is made on and that's type, and the methods it can reach.</summary>
    /// <param name="arguments">Its number of arguments; null for a method group.</param>
    private (string Name, string? Receiver, TypeRefNode? ReceiverType, IReadOnlyList<CallTarget> Targets)? Resolve(ExpressionSyntax expression, int? arguments, int before, int depth)
    {
        var (name, receiver) = expression switch
        {
            SimpleNameSyntax simple => (simple.Identifier.Text, null),
            MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax } access => (access.Name.Identifier.Text, null),
            MemberAccessExpressionSyntax access => (access.Name.Identifier.Text, access.Expression),
            // item?.RemoveStock(units): made on what the ?. is applied to.
            MemberBindingExpressionSyntax binding => (binding.Name.Identifier.Text, binding.FirstAncestorOrSelf<ConditionalAccessExpressionSyntax>()?.Expression),
            _ => ((string?)null, (ExpressionSyntax?)null)
        };
        if (name is null)
        {
            return null;
        }

        var receiverType = receiver is null ? Unknown : TypesOf(receiver, before, depth);
        var typed = receiverType is { Known: true, Types: [var only] } ? only : null;
        var written = receiver is null ? null : Normalise(receiver);
        return (name, written, typed, graph.Targets(name, written, typed, arguments, owner));
    }

    /// <summary>The type arguments a call or method group is given: <c>AddScoped&lt;IOrderRepository, OrderRepository&gt;</c>.</summary>
    private static IReadOnlyList<TypeRefNode> TypeArgumentsOf(ExpressionSyntax expression) =>
        expression switch
        {
            GenericNameSyntax generic => generic.TypeArgumentList.Arguments.Select(CSharpReader.TypeRef).ToList(),
            MemberAccessExpressionSyntax { Name: GenericNameSyntax generic } => generic.TypeArgumentList.Arguments.Select(CSharpReader.TypeRef).ToList(),
            _ => []
        };

    private static bool IsNameOf(InvocationExpressionSyntax invocation) =>
        invocation is { Expression: IdentifierNameSyntax { Identifier.Text: "nameof" }, ArgumentList.Arguments.Count: 1 };

    /// <param name="sourced">
    /// Whether it says where it comes from (<see cref="ValueNode.Joined"/>, <see cref="ValueNode.Given"/> and the like); not for
    /// an interpolation hole with a format, whose text isn't its value's.
    /// </param>
    /// <param name="name">A named argument's name (<c>status: "Paid"</c>).</param>
    private ValueNode Value(ExpressionSyntax expression, int before, bool sourced = true, string? name = null)
    {
        var (types, known) = TypesOf(expression, before, 0);
        var bare = Bare(expression);
        return new ValueNode
        {
            Text = Normalise(expression),
            Name = name,
            Types = types,
            IsKnown = known && types.Count > 0,
            Literal = CSharpReader.LiteralOf(expression, file, x => Value(x, before), x => Value(x, before, sourced: false)),
            Creations = types.Select(x => _createdBy.TryGetValue(x, out var creation) ? Creation(creation) : null).OfType<CreationNode>().ToList(),
            Function = Unwrapped(expression) is AnonymousFunctionExpressionSyntax lambda ? Lambda(lambda) : null,
            Joined = sourced ? Joined(bare, before) : [],
            GivenOf = sourced ? () => Given(bare, before) : null,
            Parameter = sourced && bare is IdentifierNameSyntax named && Local(named, before) is null ? ParameterOf(named) : null,
            Field = sourced ? FieldRead(bare, before) : null,
            // An awaited call's value is what the call gives once awaited: await factory.MakeAsync(id) is MakeAsync's, and so is
            // await factory.MakeAsync(id).ConfigureAwait(false).
            Call = sourced && (bare is AwaitExpressionSyntax awaited ? Bare(Configured(awaited.Expression)) : bare) is InvocationExpressionSyntax invocation ? Call(invocation) : null,
            Trimmed = sourced ? Trimmed(bare, before) : null,
            At = CSharpReader.SpanOf(expression, file)
        };
    }

    private static ExpressionSyntax Unwrapped(ExpressionSyntax expression) => expression is ParenthesizedExpressionSyntax parenthesized ? Unwrapped(parenthesized.Expression) : expression;

    /// <summary>What is awaited under <c>ConfigureAwait(...)</c>, which changes only where the code resumes: <c>MakeAsync(id)</c> in <c>MakeAsync(id).ConfigureAwait(false)</c>.</summary>
    private static ExpressionSyntax Configured(ExpressionSyntax awaited) =>
        Unwrapped(awaited) is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax { Name.Identifier.Text: "ConfigureAwait" } access, ArgumentList.Arguments.Count: 1 }
            ? access.Expression
            : awaited;

    /// <summary>What a value is under its parentheses, casts, <c>as</c> and <c>!</c>, none of which change its text.</summary>
    private static ExpressionSyntax Bare(ExpressionSyntax expression) => expression switch
    {
        ParenthesizedExpressionSyntax parenthesized => Bare(parenthesized.Expression),
        CastExpressionSyntax cast => Bare(cast.Expression),
        PostfixUnaryExpressionSyntax suppressed when suppressed.IsKind(SyntaxKind.SuppressNullableWarningExpression) => Bare(suppressed.Operand),
        BinaryExpressionSyntax converted when converted.IsKind(SyntaxKind.AsExpression) => Bare(converted.Left),
        _ => expression
    };

    /// <summary>A <c>+</c> chain's operands, or <c>string.Concat</c>'s arguments.</summary>
    private IReadOnlyList<ValueNode> Joined(ExpressionSyntax bare, int before) => bare switch
    {
        BinaryExpressionSyntax binary when binary.IsKind(SyntaxKind.AddExpression) => Operands(binary).Select(x => Value(x, before)).ToList(),
        InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax { Expression: PredefinedTypeSyntax { Keyword.Text: "string" } or IdentifierNameSyntax { Identifier.Text: "String" }, Name.Identifier.Text: "Concat" } } concat
            => concat.ArgumentList.Arguments.Select(x => Value(x.Expression, before)).ToList(),
        _ => []
    };

    private static IEnumerable<ExpressionSyntax> Operands(BinaryExpressionSyntax binary) =>
        (binary.Left is BinaryExpressionSyntax left && left.IsKind(SyntaxKind.AddExpression) ? Operands(left) : [binary.Left]).Append(binary.Right);

    /// <summary>
    /// The values it stands for: a local's (<see cref="LocalValues"/>), a conditional's branches, a switch expression's arms,
    /// both sides of a <c>??</c>, what <c>x.ToString()</c> and <c>new Uri(x)</c> are made from.
    /// </summary>
    private IReadOnlyList<ValueNode> Given(ExpressionSyntax bare, int before) => bare switch
    {
        // A change other than a plain = (filterQs += "&") is a way it is given whose text isn't known.
        IdentifierNameSyntax identifier when Local(identifier, before) is { } local =>
        [
            ..LocalValues(local, identifier.Identifier.Text, before).Select(x => Value(x, x.SpanStart)),
            ..Nodes.Where(x => x.SpanStart > local.SpanStart && x.SpanStart < before)
                .Where(x => Changed(x) is IdentifierNameSyntax changed && changed.Identifier.Text == identifier.Identifier.Text)
                .Select(x => new ValueNode { Text = Normalise(x), At = CSharpReader.SpanOf(x, file) })
        ],
        // A loop's variable is given each item of what it loops over: foreach (var order in orders).
        IdentifierNameSyntax item when item.Ancestors().OfType<ForEachStatementSyntax>().FirstOrDefault(x => x.Identifier.Text == item.Identifier.Text) is { } loop =>
            [Value(loop.Expression, loop.Expression.SpanStart)],
        ConditionalExpressionSyntax conditional => [Value(conditional.WhenTrue, before), Value(conditional.WhenFalse, before)],
        SwitchExpressionSyntax @switch => Arms(@switch).Select(x => Value(x, before)).ToList(),
        BinaryExpressionSyntax coalesce when coalesce.IsKind(SyntaxKind.CoalesceExpression) => [Value(coalesce.Left, before), Value(coalesce.Right, before)],
        InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax { Name.Identifier.Text: "ToString" } access, ArgumentList.Arguments.Count: 0 } => [Value(access.Expression, before)],
        ObjectCreationExpressionSyntax { ArgumentList.Arguments: [var uri, ..] } creation when creation.Type.ToString() is "Uri" or "System.Uri" => [Value(uri.Expression, before)],
        _ => []
    };

    /// <summary>What a switch expression's arms give; an arm that throws gives nothing.</summary>
    private static IEnumerable<ExpressionSyntax> Arms(SwitchExpressionSyntax @switch) =>
        @switch.Arms.Select(x => x.Expression).Where(x => x is not ThrowExpressionSyntax);

    /// <summary>The parameter of this member or of a lambda in it that a name is, as the unit's own node.</summary>
    private ParameterNode? ParameterOf(IdentifierNameSyntax identifier)
    {
        var name = identifier.Identifier.Text;
        foreach (var ancestor in identifier.Ancestors())
        {
            var index = Parameters(ancestor).ToList().FindIndex(x => x.Identifier.Text == name);
            if (index < 0)
            {
                continue;
            }

            var parameters = ancestor switch
            {
                BaseMethodDeclarationSyntax method when method == _declaration => _declarationParameters,
                AnonymousFunctionExpressionSyntax lambda => LambdaParameters(lambda),
                _ => []
            };
            return index < parameters.Count ? parameters[index] : null;
        }

        return null;
    }

    /// <summary>
    /// The field or property a value reads: the type's own, by name or on <c>this</c>, unless a local or parameter has the
    /// name; or, through a type's name, another in-scope type's (<c>Routes.Catalog</c>).
    /// </summary>
    private FieldRef? FieldRead(ExpressionSyntax bare, int before) => bare switch
    {
        IdentifierNameSyntax identifier when Local(identifier, before) is null && !IsParameter(identifier) => graph.FieldOf(owner, identifier.Identifier.Text),
        MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax } access => graph.FieldOf(owner, access.Name.Identifier.Text),
        MemberAccessExpressionSyntax access when NamedType(access.Expression, before) is { } type => graph.FieldOf(type, access.Name.Identifier.Text),
        _ => null
    };

    /// <summary>The in-scope type a name or qualified name is, when it isn't a value the code has under that name.</summary>
    private TypeNode? NamedType(ExpressionSyntax expression, int before)
    {
        var first = expression.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().FirstOrDefault();
        if (first is null || expression.DescendantNodesAndSelf().Any(x => x is not (IdentifierNameSyntax or MemberAccessExpressionSyntax))
            || Local(first, before) is not null || IsParameter(first) || graph.FieldOf(owner, first.Identifier.Text) is not null)
        {
            return null;
        }

        var written = Normalise(expression);
        return graph.TypeFor(new TypeRefNode { Text = written, Name = written[(written.LastIndexOf('.') + 1)..], QualifiedName = written }, owner);
    }

    private static bool IsParameter(IdentifierNameSyntax identifier) => identifier.Ancestors().SelectMany(Parameters).Any(x => x.Identifier.Text == identifier.Identifier.Text);

    /// <summary>A trim, when the characters it trims are written: <c>uri.TrimEnd('&amp;', '?')</c>, <c>name.Trim()</c>.</summary>
    private TrimNode? Trimmed(ExpressionSyntax bare, int before) =>
        bare is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax { Name.Identifier.Text: "Trim" or "TrimStart" or "TrimEnd" } access } invocation
        && invocation.ArgumentList.Arguments.All(x => x.Expression is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.CharacterLiteralExpression))
            ? new TrimNode(Value(access.Expression, before), access.Name.Identifier.Text != "TrimEnd", access.Name.Identifier.Text != "TrimStart",
                invocation.ArgumentList.Arguments.Select(x => (char)((LiteralExpressionSyntax)x.Expression).Token.Value!).ToList())
            : null;

    /// <summary>A creation, with what it is given; null while it is being read, as when invalid code passes a local to its own initializer.</summary>
    private CreationNode? Creation(ObjectCreationExpressionSyntax creation)
    {
        if (_creations.TryGetValue(creation, out var known))
        {
            return known;
        }

        _creations[creation] = null;
        var initializer = creation.Initializer;
        return _creations[creation] = new CreationNode
        {
            Type = Created(creation),
            Arguments = creation.ArgumentList?.Arguments.Select(x => Value(x.Expression, creation.SpanStart, name: x.NameColon?.Name.Identifier.Text)).ToList() ?? [],
            Initializers = initializer is not null && initializer.IsKind(SyntaxKind.ObjectInitializerExpression)
                ? initializer.Expressions.OfType<AssignmentExpressionSyntax>()
                    .Select(x => new InitializerNode(x.Left is IdentifierNameSyntax name ? name.Identifier.Text : Normalise(x.Left), Value(x.Right, creation.SpanStart)))
                    .ToList()
                : [],
            Items = initializer is not null && initializer.IsKind(SyntaxKind.CollectionInitializerExpression)
                ? initializer.Expressions.Select(x => Value(x, creation.SpanStart)).ToList()
                : [],
            Text = Normalise(creation),
            At = CSharpReader.SpanOf(creation, file)
        };
    }

    private TypeRefNode Created(ObjectCreationExpressionSyntax creation)
    {
        if (!_created.TryGetValue(creation, out var type))
        {
            _created[creation] = type = CSharpReader.TypeRef(creation.Type);
            _createdBy[type] = creation;
        }

        return type;
    }

    /// <summary>The types an expression can be, and whether every way it is given is known (<see cref="ValueNode"/>).</summary>
    private (List<TypeRefNode> Types, bool Known) TypesOf(ExpressionSyntax expression, int before, int depth)
    {
        switch (expression)
        {
            case ParenthesizedExpressionSyntax parenthesized:
                return TypesOf(parenthesized.Expression, before, depth);
            case ObjectCreationExpressionSyntax creation:
                return ([Created(creation)], true);
            case CastExpressionSyntax cast:
                return Cast(TypesOf(cast.Expression, before, depth), cast.Type);
            case BinaryExpressionSyntax { Right: TypeSyntax type } binary when binary.IsKind(SyntaxKind.AsExpression):
                return Cast(TypesOf(binary.Left, before, depth), type);
            // A comparison, a logical operator and a type test are a bool, whatever they compare: x.ToLowerInvariant() == "y".
            case BinaryExpressionSyntax when Comparisons.Contains(expression.Kind()):
            case PrefixUnaryExpressionSyntax when expression.IsKind(SyntaxKind.LogicalNotExpression):
            case IsPatternExpressionSyntax:
                return ([Predefined("bool")], true);
            // A signed number is the number's type, as the compiler gives it: -1.00m is a decimal. Only the types C# keeps
            // under a sign; a smaller one is promoted, so isn't its own.
            case PrefixUnaryExpressionSyntax signed when signed.IsKind(SyntaxKind.UnaryMinusExpression) || signed.IsKind(SyntaxKind.UnaryPlusExpression):
            {
                var operand = TypesOf(signed.Operand, before, depth);
                return operand is { Known: true, Types: [{ Name: "int" or "long" or "float" or "double" or "decimal" }] } ? operand : Unknown;
            }
            // string.Empty, the one constant of string's own it reads.
            case MemberAccessExpressionSyntax { Expression: PredefinedTypeSyntax { Keyword.Text: "string" } or IdentifierNameSyntax { Identifier.Text: "String" }, Name.Identifier.Text: "Empty" }:
                return ([Predefined("string")], true);
            case ConditionalExpressionSyntax conditional:
            {
                var whenTrue = TypesOf(conditional.WhenTrue, before, depth);
                var whenFalse = TypesOf(conditional.WhenFalse, before, depth);
                return ([..whenTrue.Types, ..whenFalse.Types], whenTrue.Known && whenFalse.Known);
            }
            case SwitchExpressionSyntax @switch:
            {
                var arms = Arms(@switch).Select(x => TypesOf(x, before, depth)).ToList();
                return (arms.SelectMany(x => x.Types).ToList(), arms.All(x => x.Known));
            }
            case ThisExpressionSyntax:
                return ([OwnerRef()], true);
            case InvocationExpressionSyntax invocation when depth < MaxDepth && !IsNameOf(invocation):
            {
                if (Resolve(invocation.Expression, invocation.ArgumentList.Arguments.Count, invocation.SpanStart, depth + 1) is not { } resolved)
                {
                    return Unknown;
                }

                // What the method it reaches returns, with the type arguments the call passes; of several overloads, the
                // ones its arguments' types fit, when they all return the same.
                var typeArguments = TypeArgumentsOf(invocation.Expression);
                if (resolved.Targets.Count > 0)
                {
                    return Fitting(resolved.Targets, invocation, depth + 1).Select(x => x.ReturnsGiven(typeArguments)).ToList() is [{ } returns, ..] given
                           && given.All(x => x?.Text == returns.Text)
                        ? ([returns], true)
                        : Unknown;
                }

                if (Tasks(resolved.Name, resolved.Receiver, invocation, typeArguments, before, depth + 1) is { } task)
                {
                    return task;
                }

                // A library's method: what a well-known one gives, or what a query chain does.
                var library = LibraryResults.Of(resolved.Name, resolved.Receiver, resolved.ReceiverType, typeArguments, invocation.ArgumentList.Arguments.Count)
                              ?? Query(invocation, before, depth);
                return library is null ? Unknown : ([library], true);
            }
            case AnonymousObjectCreationExpressionSyntax anonymous:
                return ([Anonymous(anonymous, before, depth)], true);
            case LiteralExpressionSyntax literal when LiteralType(literal) is { } literalType:
                return ([literalType], true);
            case InterpolatedStringExpressionSyntax:
                return ([Predefined("string")], true);
            case AwaitExpressionSyntax awaited:
            {
                // What a Task<T> or ValueTask<T> gives when awaited, ConfigureAwait or not; it's part of the await, so costs no depth.
                var (types, known) = TypesOf(Configured(awaited.Expression), before, depth);
                return known && types.All(x => x is { Name: "Task" or "ValueTask", Arguments.Count: 1 }) ? (types.Select(x => x.Arguments[0]).ToList(), true) : Unknown;
            }
            case IdentifierNameSyntax identifier:
                return Named(identifier, before, depth);
            case MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax } access:
                return FieldOf(owner, access.Name.Identifier.Text);
            case MemberAccessExpressionSyntax access when depth < MaxDepth:
            {
                // A property path through in-scope types: services.Mediator is OrderServices' Mediator.
                var (types, known) = TypesOf(access.Expression, before, depth + 1);
                return known && types is [var only] && graph.TypeFor(only, owner) is { } type ? FieldOf(type, access.Name.Identifier.Text) : Unknown;
            }
            default:
                return Unknown;
        }
    }

    /// <summary>
    /// Of a call's overloads, the ones its arguments' types fit, position by position: an argument whose type the code doesn't
    /// say, or a parameter of <c>object</c> or a type parameter, fits any. A call with named arguments, or none that fit,
    /// keeps them all.
    /// </summary>
    private IReadOnlyList<CallTarget> Fitting(IReadOnlyList<CallTarget> targets, InvocationExpressionSyntax invocation, int depth)
    {
        var arguments = invocation.ArgumentList.Arguments;
        if (targets.Count < 2 || arguments.Any(x => x.NameColon is not null))
        {
            return targets;
        }

        var types = arguments.Select(x => ArgumentType(x.Expression, invocation.SpanStart, depth)).ToList();
        var fitting = targets.Where(target => types.Select((type, i) => type is null || i >= target.Member.Parameters.Count
                                                                          || Fits(target.Member.Parameters[i].Type, type, target)).All(x => x)).ToList();
        return fitting.Count > 0 ? fitting : targets;
    }

    private bool Fits(TypeRefNode? parameter, TypeRefNode argument, CallTarget target)
    {
        if (parameter is null || parameter.Name is "object" or "Object" || target.Member.TypeParameters.Contains(parameter.Name) || target.Type.TypeParameters.Contains(parameter.Name))
        {
            return true;
        }

        if (parameter.IsArray != argument.IsArray)
        {
            return false;
        }

        // The argument's own type, or one it derives from or implements.
        return parameter.Name == argument.Name || graph.TypeFor(argument, owner) is { } type && graph.Supertypes(type).Contains(parameter.Name);
    }

    /// <summary>An argument's type: as the code says, or the enum a member of it names (<c>BidTypeGroup.MarketPlace</c>).</summary>
    private TypeRefNode? ArgumentType(ExpressionSyntax expression, int before, int depth)
    {
        if (TypesOf(expression, before, depth) is { Known: true, Types: [var known] })
        {
            return known;
        }

        return expression is MemberAccessExpressionSyntax { Expression: IdentifierNameSyntax or QualifiedNameSyntax or MemberAccessExpressionSyntax } access
               && graph.TypeFor(CSharpReader.TypeRef(SyntaxFactory.ParseTypeName(access.Expression.ToString())), owner) is { Kind: "enum" } named
            ? new TypeRefNode { Text = named.Name, Name = named.Name, QualifiedName = named.Namespace.Length == 0 ? named.Name : $"{named.Namespace}.{named.Name}" }
            : null;
    }

    /// <summary>
    /// What a query chain gives (<see cref="Sequences"/>): its operators read down to what it starts on, then each applied in
    /// turn, a <c>Select</c> projecting to what its lambda returns and a <c>GroupBy</c> to groups whose type isn't read. A chain
    /// is walked, not recursed into, so however long it is it costs no more stack: a fluent chain can run to hundreds of calls.
    /// </summary>
    private TypeRefNode? Query(InvocationExpressionSyntax invocation, int before, int depth)
    {
        var operators = new List<(string Name, InvocationExpressionSyntax Call)>();
        ExpressionSyntax start = invocation;
        while (start is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax access } call && Sequences.IsOperator(access.Name.Identifier.Text))
        {
            operators.Add((access.Name.Identifier.Text, call));
            start = access.Expression;
        }

        if (operators.Count == 0)
        {
            return null;
        }

        operators.Reverse();

        // A chain that projects first doesn't need what it starts on. What it starts on is part of the call, not a value the
        // call follows, so reading it spends no depth.
        TypeRefNode? element = null;
        if (operators[0].Name != "Select")
        {
            if (TypesOf(start, before, depth) is not { Known: true, Types: [var source] } || Sequences.ElementOf(source) is not { } first)
            {
                return null;
            }

            element = first;
        }

        TypeRefNode? result = null;
        foreach (var (name, call) in operators)
        {
            if (name == "GroupBy")
            {
                // Its groups' type isn't read, so only a Select after it can say what the chain gives.
                (element, result) = (null, null);
                continue;
            }

            if (name == "Select")
            {
                element = Projection(call, depth);
            }
            else if (result is not null)
            {
                element = Sequences.ElementOf(result);
            }

            if (element is null || Sequences.Given(name, element) is not { } given)
            {
                return null;
            }

            result = given;
        }

        return result;
    }

    /// <summary>What a <c>Select</c>'s lambda returns, when its expression body's type is known: <c>x =&gt; new OrderDto { ... }</c>.</summary>
    private TypeRefNode? Projection(InvocationExpressionSyntax select, int depth) =>
        select.ArgumentList.Arguments is [{ Expression: LambdaExpressionSyntax { ExpressionBody: { } body } }]
        && TypesOf(body, body.SpanStart, depth + 1) is { Known: true, Types: [var projected] }
            ? projected
            : null;

    /// <summary>
    /// An anonymous type, <c>new { Id = id, x.Key.Silo }</c>: each member by the name it is given, or the name it is read by,
    /// with its type where the code says it.
    /// </summary>
    private TypeRefNode Anonymous(AnonymousObjectCreationExpressionSyntax anonymous, int before, int depth)
    {
        var members = anonymous.Initializers.Select(x => new TypeMemberNode(
            x.NameEquals?.Name.Identifier.Text ?? x.Expression switch
            {
                MemberAccessExpressionSyntax access => access.Name.Identifier.Text,
                IdentifierNameSyntax identifier => identifier.Identifier.Text,
                _ => Normalise(x.Expression)
            },
            depth < MaxDepth && TypesOf(x.Expression, before, depth + 1) is { Known: true, Types: [var type] } ? type : null)).ToList();
        var text = $"new {{ {string.Join(", ", members.Select(x => x.Name))} }}";
        return new TypeRefNode { Text = text, Name = text, QualifiedName = text, IsAnonymous = true, Members = members, Language = CSharpLanguage.LanguageName };
    }

    /// <summary>A literal's type, as the compiler gives it: <c>"x"</c> a string, <c>18.4m</c> a decimal; null for <c>null</c> and <c>default</c>.</summary>
    private static TypeRefNode? LiteralType(LiteralExpressionSyntax literal) =>
        literal.Token.Value switch
        {
            string => Predefined("string"),
            char => Predefined("char"),
            bool => Predefined("bool"),
            int => Predefined("int"),
            uint => Predefined("uint"),
            long => Predefined("long"),
            ulong => Predefined("ulong"),
            float => Predefined("float"),
            double => Predefined("double"),
            decimal => Predefined("decimal"),
            _ => null
        };

    private static TypeRefNode Predefined(string keyword) =>
        new() { Text = keyword, Name = keyword, QualifiedName = keyword, IsPredefinedValueType = keyword != "string", Language = CSharpLanguage.LanguageName };

    /// <summary>What a typed local is given says more than its declared type, which is what it is when that isn't known.</summary>
    private static (List<TypeRefNode> Types, bool Known) OrDeclared((List<TypeRefNode> Types, bool Known) value, TypeSyntax declared) =>
        value.Known && value.Types.Count > 0 ? value : ([CSharpReader.TypeRef(declared)], true);

    /// <summary>
    /// A cast or <c>as</c> is the type it names: <c>(CrmDataEntities)_context</c> is a <c>CrmDataEntities</c>, so its calls reach
    /// that class's methods. Not when it names <c>object</c>, or a type what it casts is or derives from: an upcast keeps what the
    /// value is, so <c>(IEvent)new OrderPlaced()</c> is still that creation.
    /// </summary>
    private (List<TypeRefNode> Types, bool Known) Cast((List<TypeRefNode> Types, bool Known) value, TypeSyntax target)
    {
        var type = CSharpReader.TypeRef(target);
        return value.Known && value.Types.Count > 0 && (type.Name is "object" or "Object" || value.Types.All(x => IsA(x, type))) ? value : ([type], true);
    }

    /// <summary>A type is another, or derives from it or implements it: an in-scope one by its qualified name, a library's by its name.</summary>
    private bool IsA(TypeRefNode type, TypeRefNode other)
    {
        if (type.Name == other.Name && type.IsArray == other.IsArray)
        {
            return true;
        }

        if (type.IsArray || graph.TypeFor(type, owner) is not { } declared)
        {
            return false;
        }

        var named = graph.TypeFor(other, owner);
        return graph.Supertypes(declared).Contains(named is null ? other.Name : named.Namespace.Length > 0 ? $"{named.Namespace}.{named.Name}" : named.Name);
    }

    /// <summary>
    /// What <c>Task.FromResult(x)</c> and <c>ValueTask.FromResult(x)</c> give, a task of <c>x</c>'s type, and <c>Task.Run(() =&gt; e)</c>,
    /// a task of what its lambda gives (a task it gives is the one it runs, unwrapped, as <c>Task.Run</c>'s overloads do); a type
    /// argument they're given says it outright. Null for any other call.
    /// </summary>
    private (List<TypeRefNode> Types, bool Known)? Tasks(string name, string? receiver, InvocationExpressionSyntax invocation, IReadOnlyList<TypeRefNode> typeArguments, int before, int depth)
    {
        var task = receiver switch
        {
            "Task" or "System.Threading.Tasks.Task" => "Task",
            "ValueTask" or "System.Threading.Tasks.ValueTask" => "ValueTask",
            _ => null
        };
        if (task is null || name is not ("FromResult" or "Run") || invocation.ArgumentList.Arguments is not [var argument] || name == "Run" && task != "Task")
        {
            return null;
        }

        if (typeArguments is [var stated])
        {
            return ([TaskOf(task, stated)], true);
        }

        if (name == "FromResult")
        {
            var (types, known) = TypesOf(argument.Expression, before, depth);
            return known && types.Count > 0 ? (types.Select(x => TaskOf(task, x)).ToList(), true) : Unknown;
        }

        if (argument.Expression is not LambdaExpressionSyntax { ExpressionBody: { } body })
        {
            return Unknown;
        }

        var (given, all) = TypesOf(body, body.SpanStart, depth);
        return all && given.Count > 0 ? (given.Select(x => x.Name == "Task" && x.Arguments.Count <= 1 ? x : TaskOf(task, x)).ToList(), true) : Unknown;
    }

    private static TypeRefNode TaskOf(string task, TypeRefNode result) =>
        new() { Text = $"{task}<{result.Text}>", Name = task, QualifiedName = $"System.Threading.Tasks.{task}", Arguments = [result], Language = CSharpLanguage.LanguageName };

    /// <summary>
    /// A name in the body: a local declared before the use, then a parameter (the type's primary constructor's included), then
    /// a field or property of the type.
    /// </summary>
    private (List<TypeRefNode> Types, bool Known) Named(IdentifierNameSyntax identifier, int before, int depth)
    {
        var name = identifier.Identifier.Text;
        if (Local(identifier, before) is { } local)
        {
            // Only following a local's values recurses, so only that stops at the depth; a parameter or field is its declared type.
            if (depth >= MaxDepth)
            {
                return Unknown;
            }

            var given = LocalValues(local, name, before).Select(x => TypesOf(x, x.SpanStart, depth + 1)).ToList();
            (List<TypeRefNode> Types, bool Known) value = (given.SelectMany(x => x.Types).ToList(), given.Count > 0 && given.All(x => x.Known));
            if (local.Parent is VariableDeclarationSyntax { Type.IsVar: false } declaration)
            {
                return OrDeclared(value, declaration.Type);
            }

            // A var local whose values disagree, or aren't all known, is its initializer's type, which is C#'s own static type
            // for it: var model = new object(); model = "..."; is an object.
            return local.Initializer is not null && given is [{ Known: true, Types: [var initial] }, _, ..] && !(value.Known && value.Types.All(x => x.Text == initial.Text))
                ? ([initial], true)
                : value;
        }

        var parameter = identifier.Ancestors().SelectMany(Parameters).FirstOrDefault(x => x.Identifier.Text == name);
        if (parameter is not null)
        {
            return parameter.Type is null ? Unknown : ([CSharpReader.TypeRef(parameter.Type)], true);
        }

        return FieldOf(owner, name);
    }

    /// <summary>The local a name is: the last one of the name declared before the use.</summary>
    private VariableDeclaratorSyntax? Local(IdentifierNameSyntax identifier, int before) =>
        (_locals ??= Nodes.OfType<VariableDeclaratorSyntax>().ToLookup(x => x.Identifier.Text, StringComparer.Ordinal))[identifier.Identifier.Text]
        .LastOrDefault(x => x.SpanStart < before);

    private IReadOnlyList<SyntaxNode> Nodes => _nodes ??= roots.SelectMany(x => x.DescendantNodes()).ToList();

    /// <summary>What a local is given before the use: its initializer and every plain assignment to it (one in each branch of an if is both).</summary>
    private List<ExpressionSyntax> LocalValues(VariableDeclaratorSyntax local, string name, int before)
    {
        var values = new List<ExpressionSyntax>();
        if (local.Initializer?.Value is { } initializer)
        {
            values.Add(initializer);
        }

        _assignments ??= Nodes.OfType<AssignmentExpressionSyntax>()
            .Where(x => x.IsKind(SyntaxKind.SimpleAssignmentExpression) && x.Left is IdentifierNameSyntax)
            .ToLookup(x => ((IdentifierNameSyntax)x.Left).Identifier.Text, StringComparer.Ordinal);
        values.AddRange(_assignments[name].Where(x => x.SpanStart > local.SpanStart && x.SpanStart < before).Select(x => x.Right));
        return values;
    }

    private static IEnumerable<ParameterSyntax> Parameters(SyntaxNode node) => node switch
    {
        BaseMethodDeclarationSyntax method => method.ParameterList.Parameters,
        LocalFunctionStatementSyntax function => function.ParameterList.Parameters,
        ParenthesizedLambdaExpressionSyntax lambda => lambda.ParameterList.Parameters,
        SimpleLambdaExpressionSyntax lambda => [lambda.Parameter],
        AnonymousMethodExpressionSyntax { ParameterList: { } list } => list.Parameters,
        TypeDeclarationSyntax { ParameterList: { } list } => list.Parameters,
        _ => []
    };

    /// <summary>A field or property of a type, or one it inherits (a positional record's parameters are its properties).</summary>
    private (List<TypeRefNode> Types, bool Known) FieldOf(TypeNode type, string name) =>
        graph.MembersOf(type).FirstOrDefault(x => x.Name == name && x.Kind is "field" or "property") is { Type: { } declared } ? ([declared], true) : Unknown;

    private TypeRefNode OwnerRef() => new() { Text = owner.Name, Name = owner.Name, QualifiedName = owner.Namespace.Length == 0 ? owner.Name : $"{owner.Namespace}.{owner.Name}" };

    private static string Normalise(SyntaxNode node) => Whitespace.Replace(node.ToString(), " ").Trim();
}
