using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Intent.Modules.Common.DriftEngine.Code;
using static Intent.Modules.Common.DriftEngine.Stacks.Java.Grammar.JavaParser;

namespace Intent.Modules.Common.DriftEngine.Stacks.Java;

/// <summary>
/// What a method's or constructor's body does, read from its syntax once every type is read: each call, the type of what
/// it is made on, the method it reaches, and what its arguments are (<see cref="ValueNode"/>), as far as the code says
/// without running it; each lambda as a unit of its own.
/// </summary>
internal sealed class JavaBody(BlockContext body, TypeNode owner, string file, CodeGraph graph)
{
    /// <summary>How many locals deep a value is followed (<c>var a = b; var b = new X();</c>).</summary>
    private const int MaxDepth = 3;

    private static readonly (List<TypeRefNode> Types, bool Known) Unknown = ([], false);

    /// <summary>Each call once, so a chain's call is the same node as the one the body has.</summary>
    private readonly Dictionary<ParserRuleContext, CallNode?> _calls = [];

    /// <summary>Each creation once, so a value's creation is the same node as the one the body has.</summary>
    private readonly Dictionary<ObjectCreationExpressionContext, CreationNode?> _creations = [];

    private readonly Dictionary<ObjectCreationExpressionContext, TypeRefNode?> _created = [];

    private readonly Dictionary<TypeRefNode, ObjectCreationExpressionContext> _createdBy = new(ReferenceEqualityComparer.Instance);

    /// <summary>Each method reference once, so a lambda's is the same node as the one the body has.</summary>
    private readonly Dictionary<MethodReferenceExpressionContext, CallNode?> _references = [];

    private readonly Dictionary<LambdaExpressionContext, FunctionNode> _lambdas = [];

    /// <summary>What the method's or constructor's body does.</summary>
    public FunctionNode Member(IReadOnlyList<ParameterNode> parameters) => Function(body, parameters, null, JavaReader.SpanOf(body, file));

    /// <param name="function">The body or lambda, which a <c>return</c> in it belongs to unless a nested lambda or anonymous class is nearer.</param>
    /// <param name="returned">A lambda's expression body.</param>
    private FunctionNode Function(ParserRuleContext function, IReadOnlyList<ParameterNode> parameters, ExpressionContext? returned, Span at)
    {
        var inside = Descendants<ParserRuleContext>(function).ToList();
        return new FunctionNode
        {
            Parameters = parameters,
            Calls = inside
                .Select(x => x switch
                {
                    MethodCallContext or ExplicitGenericInvocationContext => Call(x),
                    MethodReferenceExpressionContext reference => Reference(reference),
                    _ => null
                })
                .OfType<CallNode>()
                .ToList(),

            // An array creation isn't one.
            Creations = inside.OfType<ObjectCreationExpressionContext>().Select(Creation).OfType<CreationNode>().ToList(),
            Assignments = inside.OfType<BinaryOperatorExpressionContext>()
                .Where(x => x.bop?.Text == "=")
                .Select(x => new AssignmentNode(JavaReader.TextOf(x.expression(0)), Value(x.expression(1), x.Start.StartIndex), JavaReader.SpanOf(x, file))
                {
                    Receiver = x.expression(0) is MemberReferenceExpressionContext { } access && access.identifier() is not null && !IsThis(access.expression())
                        ? Value(access.expression(), x.Start.StartIndex)
                        : null
                })
                .ToList(),
            Returns = returned is not null
                ? [Value(returned, returned.Start.StartIndex)]
                : inside.OfType<StatementContext>()
                    .Where(x => x.RETURN() is not null && x.expression(0) is not null && Owns(function, x))
                    .Select(x => Value(x.expression(0), x.expression(0).Start.StartIndex))
                    .ToList(),
            Text = JavaReader.TextOf(function),
            At = at
        };
    }

    /// <summary>A lambda as its own unit, with its parameters as written: <c>x -&gt;</c> has no type.</summary>
    private FunctionNode Lambda(LambdaExpressionContext lambda)
    {
        if (_lambdas.TryGetValue(lambda, out var known))
        {
            return known;
        }

        var declared = lambda.lambdaParameters();
        var parameters = (declared.formalParameterList()?.formalParameter().Select(x => JavaReader.Parameter(x, file)) ?? [])
            .Concat(declared.identifier().Concat(declared.lambdaLVTIList()?.lambdaLVTIParameter().Select(x => x.identifier()) ?? [])
                .Select(x => new ParameterNode { Name = x.GetText(), At = JavaReader.SpanOf(x, file), TextOf = x.GetText }))
            .ToList();
        return _lambdas[lambda] = Function(lambda, parameters, lambda.lambdaBody().expression(), JavaReader.SpanOf(lambda, file));
    }

    /// <summary>A node is the unit's own unless a nested lambda or anonymous class holds it.</summary>
    private static bool Owns(ParserRuleContext function, ParserRuleContext node)
    {
        for (var at = node.Parent; at is not null; at = at.Parent)
        {
            if (at == function)
            {
                return true;
            }

            if (at is LambdaExpressionContext or ClassBodyContext)
            {
                return false;
            }
        }

        return false;
    }

    /// <summary>A creation, with its arguments; null for an array creation, and while it is being read.</summary>
    private CreationNode? Creation(ObjectCreationExpressionContext creation)
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
            Arguments = creation.creator().classCreatorRest().arguments().expressionList()?.expression().Select(x => Value(x, creation.Start.StartIndex)).ToList() ?? [],
            Text = JavaReader.TextOf(creation),
            At = JavaReader.SpanOf(creation, file)
        };
    }

    /// <summary>The type a creation creates, one node each, so a value's types say which creations it is.</summary>
    private TypeRefNode? Created(ObjectCreationExpressionContext creation)
    {
        if (!_created.TryGetValue(creation, out var type))
        {
            _created[creation] = type = Created(creation.creator());
            if (type is not null)
            {
                _createdBy[type] = creation;
            }
        }

        return type;
    }

    /// <param name="call">A <see cref="MethodCallContext"/>, or an <see cref="ExplicitGenericInvocationContext"/>.</param>
    private CallNode? Call(ParserRuleContext call)
    {
        if (_calls.TryGetValue(call, out var known))
        {
            return known;
        }

        if (SiteOf(call) is not { } site || Resolve(site, 0) is not { } resolved)
        {
            return _calls[call] = null;
        }

        return _calls[call] = new CallNode
        {
            Name = resolved.Name,
            Receiver = resolved.Receiver,
            ReceiverType = resolved.ReceiverType,
            On = site.Whole is MemberReferenceExpressionContext access ? Inner(access.expression()) is { } on ? Call(on) : null : null,
            Arguments = site.Arguments.expressionList()?.expression().Select(x => Value(x, site.Whole.Start.StartIndex)).ToList() ?? [],
            TypeArguments = site.TypeArguments,
            Target = resolved.Targets is [var target] ? target : null,
            Text = JavaReader.TextOf(site.Whole),
            CanonicalOf = () => JavaReader.Canonical(site.Whole),
            At = JavaReader.SpanOf(site.Whole, file)
        };
    }

    /// <summary>A call as written: its name, arguments and type arguments, and the expression it is.</summary>
    private sealed record Site(IdentifierContext Name, ArgumentsContext Arguments, IReadOnlyList<TypeRefNode> TypeArguments, ParserRuleContext Whole);

    /// <summary>
    /// The call a node is: <c>find(id)</c> and <c>orders.find(id)</c>, or with type arguments, <c>this.&lt;T&gt;find(id)</c>.
    /// <c>this(...)</c> and <c>super(...)</c> call a constructor, so aren't calls.
    /// </summary>
    private static Site? SiteOf(ParserRuleContext node) => node switch
    {
        MethodCallContext call when call.identifier() is { } name => new Site(name, call.arguments(), [], (ParserRuleContext)call.Parent),
        ExplicitGenericInvocationContext generic when generic.explicitGenericInvocationSuffix().identifier() is { } name && generic.Parent is MemberReferenceExpressionContext access =>
            new Site(name, generic.explicitGenericInvocationSuffix().arguments(), generic.nonWildcardTypeArguments().typeList().typeType().Select(JavaReader.TypeRef).ToList(), access),
        _ => null
    };

    /// <summary>
    /// A method reference: <c>orders::place</c>, <c>this::audit</c>, or on a type, <c>Order::markAsPaid</c>. A constructor
    /// reference (<c>Order::new</c>) isn't one.
    /// </summary>
    private CallNode? Reference(MethodReferenceExpressionContext reference)
    {
        if (_references.TryGetValue(reference, out var known))
        {
            return known;
        }

        if (reference.identifier() is not { } name)
        {
            return _references[reference] = null;
        }

        var (receiver, typed) = reference.expression() switch
        {
            { } expression when IsThis(expression) => (null, null),
            { } expression => (JavaReader.TextOf(expression), TypesOf(expression, reference.Start.StartIndex, 0) is { Known: true, Types: [var only] } ? only : null),
            null when reference.typeType() is { } type => (JavaReader.TextOf(type), JavaReader.TypeRef(type)),
            _ => ((string?)null, (TypeRefNode?)null)
        };
        var targets = graph.Targets(name.GetText(), receiver, typed, null, owner);
        return _references[reference] = new CallNode
        {
            Name = name.GetText(),
            Receiver = receiver,
            ReceiverType = typed,
            TypeArguments = reference.typeArguments()?.typeArgument().Select(JavaReader.TypeArgument).ToList() ?? [],
            IsReference = true,
            Target = targets is [var target] ? target : null,
            Text = JavaReader.TextOf(reference),
            CanonicalOf = () => JavaReader.Canonical(reference),
            At = JavaReader.SpanOf(reference, file)
        };
    }

    /// <summary>
    /// A call's name, what it is made on (<c>x.call()</c>; nothing for the type's own member, called unqualified or on
    /// <c>this</c>) and that's type, and the methods it can reach.
    /// </summary>
    private (string Name, string? Receiver, TypeRefNode? ReceiverType, IReadOnlyList<CallTarget> Targets)? Resolve(Site site, int depth)
    {
        var receiver = site.Whole is MemberReferenceExpressionContext access && !IsThis(access.expression()) ? access.expression() : null;
        var receiverType = receiver is null ? Unknown : TypesOf(receiver, site.Whole.Start.StartIndex, depth);
        var typed = receiverType is { Known: true, Types: [var only] } ? only : null;
        var written = receiver is null ? null : JavaReader.TextOf(receiver);
        var arguments = site.Arguments.expressionList()?.expression().Length ?? 0;
        var name = site.Name.GetText();
        return (name, written, typed, graph.Targets(name, written, typed, arguments, owner));
    }

    /// <summary>The call an expression is, unqualified (<c>find(id)</c>) or made on something (<c>orders.find(id)</c>).</summary>
    private static ParserRuleContext? Inner(ExpressionContext expression) => expression switch
    {
        MethodCallExpressionContext call => call.methodCall(),
        MemberReferenceExpressionContext access => (ParserRuleContext?)access.methodCall() ?? access.explicitGenericInvocation(),
        _ => null
    };

    /// <summary>What the method a call reaches returns, with the type arguments the call passes.</summary>
    private (List<TypeRefNode> Types, bool Known) Returned(ParserRuleContext call, int depth) =>
        SiteOf(call) is { } site && Resolve(site, depth + 1) is { Targets: [var target] } && target.ReturnsGiven(site.TypeArguments) is { } returns ? ([returns], true) : Unknown;

    private ValueNode Value(ExpressionContext expression, int before)
    {
        var (types, known) = TypesOf(expression, before, 0);
        return new ValueNode
        {
            Text = JavaReader.TextOf(expression),
            Types = types,
            IsKnown = known && types.Count > 0,
            Literal = JavaReader.LiteralOf(expression),
            Function = Unwrapped(expression) is ExpressionLambdaContext lambda ? Lambda(lambda.lambdaExpression()) : null,
            Creations = types.Select(x => _createdBy.TryGetValue(x, out var creation) ? Creation(creation) : null).OfType<CreationNode>().ToList(),
            Call = Inner(Unwrapped(expression)) is { } call ? Call(call) : null,
            At = JavaReader.SpanOf(expression, file)
        };
    }

    private static ExpressionContext Unwrapped(ExpressionContext expression) =>
        expression is PrimaryExpressionContext { } primary && primary.primary().expression() is { } parenthesized ? Unwrapped(parenthesized) : expression;

    /// <summary>The types an expression can be, and whether every way it is given is known (<see cref="ValueNode"/>).</summary>
    private (List<TypeRefNode> Types, bool Known) TypesOf(ExpressionContext expression, int before, int depth)
    {
        switch (expression)
        {
            case PrimaryExpressionContext { } primary when primary.primary().expression() is { } parenthesized:
                return TypesOf(parenthesized, before, depth);
            case PrimaryExpressionContext primary when IsThis(primary):
                return ([OwnerRef()], true);
            case PrimaryExpressionContext primary when primary.primary().identifier() is { } identifier && depth < MaxDepth:
                return Named(identifier, before, depth);
            case ObjectCreationExpressionContext creation:
                return Created(creation) is { } created ? ([created], true) : Unknown;
            case MethodCallExpressionContext invocation when depth < MaxDepth:
                return Returned(invocation.methodCall(), depth);
            case MemberReferenceExpressionContext access when Inner(access) is { } call && depth < MaxDepth:
                return Returned(call, depth);
            case CastExpressionContext cast:
                return OrDeclared(TypesOf(cast.expression(), before, depth), cast.typeType(0));
            case TernaryExpressionContext conditional:
            {
                var whenTrue = TypesOf(conditional.expression(1), before, depth);
                var whenFalse = TypesOf(conditional.expression(2), before, depth);
                return ([..whenTrue.Types, ..whenFalse.Types], whenTrue.Known && whenFalse.Known);
            }
            case MemberReferenceExpressionContext access when access.identifier() is { } field && IsThis(access.expression()):
                return FieldOf(owner, field.GetText());
            case MemberReferenceExpressionContext access when access.identifier() is { } field && depth < MaxDepth:
            {
                // A field path through in-scope types: services.mediator is OrderServices' mediator.
                var (types, known) = TypesOf(access.expression(), before, depth + 1);
                return known && types is [var only] && graph.TypeFor(only, owner) is { } type ? FieldOf(type, field.GetText()) : Unknown;
            }
            default:
                return Unknown;
        }
    }

    /// <summary>What a cast or a typed local wraps says more than its declared type, which is what it is when that isn't known.</summary>
    private static (List<TypeRefNode> Types, bool Known) OrDeclared((List<TypeRefNode> Types, bool Known) value, TypeTypeContext declared) =>
        value.Known && value.Types.Count > 0 ? value : ([JavaReader.TypeRef(declared)], true);

    /// <summary>A name in the body: a local declared before the use, then a parameter (a lambda's included), then a field of the type.</summary>
    private (List<TypeRefNode> Types, bool Known) Named(IdentifierContext identifier, int before, int depth)
    {
        var name = identifier.GetText();
        var local = Descendants<LocalVariableDeclarationContext>(body).Where(x => x.Start.StartIndex < before).SelectMany(Locals).LastOrDefault(x => x.Name == name);
        if (local.Name is not null)
        {
            // Its initializer and every plain assignment to it before the use (one in each branch of an if is both).
            var values = new List<ExpressionContext>();
            if (local.Value is { } initializer)
            {
                values.Add(initializer);
            }

            values.AddRange(Descendants<BinaryOperatorExpressionContext>(body)
                .Where(x => x.bop?.Text == "=" && x.expression(0) is PrimaryExpressionContext { } left && left.primary().identifier()?.GetText() == name
                            && x.Start.StartIndex > local.Start && x.Start.StartIndex < before)
                .Select(x => x.expression(1)));
            var given = values.Select(x => TypesOf(x, x.Start.StartIndex, depth + 1)).ToList();
            (List<TypeRefNode> Types, bool Known) value = (given.SelectMany(x => x.Types).ToList(), given.Count > 0 && given.All(x => x.Known));
            return local.Type is { } declared ? OrDeclared(value, declared) : value;
        }

        for (RuleContext? scope = identifier.Parent; scope is not null; scope = scope.Parent)
        {
            var parameter = scope switch
            {
                MethodDeclarationContext method => Parameter(method.formalParameters(), name),
                ConstructorDeclarationContext constructor => Parameter(constructor.formalParameters(), name),
                InterfaceCommonBodyDeclarationContext method => Parameter(method.formalParameters(), name),
                LambdaExpressionContext lambda => Parameter(lambda.lambdaParameters(), name),
                _ => null
            };
            if (parameter is { } found)
            {
                return found.Type is null ? Unknown : ([found.Type], true);
            }
        }

        return FieldOf(owner, name);
    }

    /// <summary>Each local a declaration declares: <c>var x = ...</c>, or <c>T a = ..., b;</c>.</summary>
    private static IEnumerable<(string Name, ExpressionContext? Value, TypeTypeContext? Type, int Start)> Locals(LocalVariableDeclarationContext declaration) =>
        declaration.VAR() is not null
            ? [(declaration.identifier().GetText(), declaration.expression(), null, declaration.Start.StartIndex)]
            : declaration.variableDeclarators().variableDeclarator()
                .Select(x => (x.variableDeclaratorId().identifier().GetText(), x.variableInitializer()?.expression(), (TypeTypeContext?)declaration.typeType(), declaration.Start.StartIndex));

    private static Declared? Parameter(FormalParametersContext parameters, string name) =>
        JavaReader.FormalParameters(parameters).FirstOrDefault(x => x.variableDeclaratorId().identifier().GetText() == name) is { } parameter
            ? new Declared(JavaReader.TypeOf(parameter))
            : null;

    /// <summary>A lambda's typed parameters have their types; <c>x -&gt;</c>, <c>(a, b) -&gt;</c> and <c>(var x) -&gt;</c> don't say.</summary>
    private static Declared? Parameter(LambdaParametersContext parameters, string name)
    {
        if (parameters.formalParameterList()?.formalParameter().FirstOrDefault(x => x.variableDeclaratorId().identifier().GetText() == name) is { } typed)
        {
            return new Declared(JavaReader.TypeOf(typed));
        }

        var untyped = parameters.identifier().Concat(parameters.lambdaLVTIList()?.lambdaLVTIParameter().Select(x => x.identifier()) ?? []);
        return untyped.Any(x => x.GetText() == name) ? new Declared(null) : null;
    }

    /// <summary>A parameter found by its name, and its type when the code writes one.</summary>
    private sealed record Declared(TypeRefNode? Type);

    /// <summary>A field or property of a type, or one it inherits (a record's components are its properties).</summary>
    private (List<TypeRefNode> Types, bool Known) FieldOf(TypeNode type, string name) =>
        graph.MembersOf(type).FirstOrDefault(x => x.Name == name && x.Kind is "field" or "property") is { Type: { } declared } ? ([declared], true) : Unknown;

    private TypeRefNode OwnerRef() => new() { Text = owner.Name, Name = owner.Name, QualifiedName = owner.Namespace.Length == 0 ? owner.Name : $"{owner.Namespace}.{owner.Name}" };

    private static bool IsThis(ExpressionContext expression) => expression is PrimaryExpressionContext { } primary && primary.primary() is { ChildCount: 1 } only && only.THIS() is not null;

    /// <summary>The type a <c>new</c> creates, as written: <c>new ArrayList&lt;&gt;()</c> is <c>ArrayList</c>. An array creation isn't one.</summary>
    private static TypeRefNode? Created(CreatorContext creator)
    {
        if (creator.classCreatorRest() is null || creator.createdName() is not { } created || created.primitiveType() is not null)
        {
            return null;
        }

        var segments = new List<string>();
        var written = new List<string>();
        IReadOnlyList<TypeRefNode> arguments = [];
        foreach (var child in created.children)
        {
            switch (child)
            {
                case IdentifierContext identifier:
                    segments.Add(identifier.GetText());
                    written.Add(identifier.GetText());
                    arguments = [];
                    break;
                case TypeArgumentsOrDiamondContext { } list when list.typeArguments() is { } typeArguments:
                    arguments = typeArguments.typeArgument().Select(JavaReader.TypeArgument).ToList();
                    written[^1] += $"<{string.Join(", ", arguments.Select(x => x.Text))}>";
                    break;
            }
        }

        return new TypeRefNode { Text = string.Join(".", written), Name = segments[^1], QualifiedName = string.Join(".", segments), Arguments = arguments, Language = JavaLanguage.LanguageName };
    }

    private static IEnumerable<T> Descendants<T>(IParseTree node) where T : IParseTree
    {
        for (var i = 0; i < node.ChildCount; i++)
        {
            var child = node.GetChild(i);
            if (child is T match)
            {
                yield return match;
            }

            foreach (var nested in Descendants<T>(child))
            {
                yield return nested;
            }
        }
    }
}
