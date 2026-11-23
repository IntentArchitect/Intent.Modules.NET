using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Stacks.Python.Syntax;

namespace Intent.Modules.Common.DriftEngine.Stacks.Python;

/// <summary>
/// What a function's or method's body does, read from its syntax once every module is read: each call, the type of what it
/// is made on, the function or method it reaches, and what its arguments are (<see cref="ValueNode"/>), as far as the code
/// says without running it; each lambda as a unit of its own. Python has no <c>new</c>: a call to a class is a creation,
/// a class being one the code declares or imports, or a name in CapWords (PEP 8), as a library's class is.
/// </summary>
/// <param name="roots">What the unit is written as: a function's statements, a module's top-level statements, a value.</param>
/// <param name="function">The function the unit is, for its parameters and its <c>self</c>; null for top-level code and a value.</param>
/// <param name="parameters">The function's parameters as the graph has them, <c>self</c> or <c>cls</c> left out.</param>
internal sealed class PythonBody(
    PythonFile file,
    TypeNode owner,
    PythonProject project,
    IReadOnlyList<PythonNode> roots,
    PythonFunction? function,
    IReadOnlyList<(PythonParameter Written, ParameterNode Node)> parameters)
{
    /// <summary>How many locals deep a value is followed (<c>a = b; b = Order()</c>).</summary>
    private const int MaxDepth = 3;

    private static readonly (List<TypeRefNode> Types, bool Known) Unknown = ([], false);

    private readonly CodeGraph _graph = project.Graph;

    private readonly Dictionary<PythonCall, CallNode?> _calls = new(ReferenceEqualityComparer.Instance);

    private readonly Dictionary<PythonCall, CreationNode?> _creations = new(ReferenceEqualityComparer.Instance);

    private readonly Dictionary<PythonCall, TypeRefNode?> _created = new(ReferenceEqualityComparer.Instance);

    private readonly Dictionary<TypeRefNode, PythonCall> _createdBy = new(ReferenceEqualityComparer.Instance);

    private readonly Dictionary<PythonLambda, FunctionNode> _lambdas = new(ReferenceEqualityComparer.Instance);

    private readonly Dictionary<PythonParameter, ParameterNode> _nested = new(ReferenceEqualityComparer.Instance);

    private Dictionary<PythonNode, PythonNode>? _parents;

    /// <summary>
    /// What a method's first parameter is called (<c>self</c>, <c>cls</c>), when the call binds it: the class's own, called on
    /// it, are the type's own members, as Java's <c>this</c> is.
    /// </summary>
    private readonly string? _self = function is { Parameters: [var first, ..] } && owner.Kind != "module" && !parameters.Any(x => ReferenceEquals(x.Written, first))
        ? first.Name
        : null;

    /// <summary>A classmethod's <c>cls</c> is the class, not an instance of it.</summary>
    private readonly bool _instance = function is null || !function.Decorators.Any(x => PythonReader.Dotted(x.Expression) == "classmethod");

    /// <summary>What the function's body does.</summary>
    public FunctionNode Function(PythonFunction declared) =>
        Unit(declared.Body, null, parameters.Select(x => x.Node).ToList(), declared.IsAsync, PythonReader.TextOf(file.Source, declared), PythonReader.SpanOf(declared, file.Path));

    /// <summary>What a module's top-level code does.</summary>
    public FunctionNode TopLevel() =>
        Unit(roots, null, [], false, string.Join(" ", roots.Select(x => PythonReader.TextOf(file.Source, x))),
            new Span(file.Path, roots[0].First.Line, roots[0].First.Column, roots[^1].Last.EndLine, roots[^1].Last.EndColumn));

    /// <param name="returned">A lambda's body, which is what it returns.</param>
    private FunctionNode Unit(IReadOnlyList<PythonNode> written, PythonExpression? returned, IReadOnlyList<ParameterNode> given, bool isAsync, string text, Span at)
    {
        var inside = written.SelectMany(x => x.Descendants().Prepend(x)).ToList();
        return new FunctionNode
        {
            Parameters = given,
            Calls = inside.OfType<PythonCall>().Select(Call).OfType<CallNode>().ToList(),
            Creations = inside.OfType<PythonCall>().Select(Creation).OfType<CreationNode>().ToList(),
            Assignments = Assignments(inside).ToList(),
            Changes = inside.OfType<PythonAugmentedAssign>().Select(x => TargetOf(x.Target)).ToList(),
            Returns = returned is not null
                ? [Value(returned)]
                : Own(written).OfType<PythonReturn>().Where(x => x.Value is not null).Select(x => Value(x.Value!)).ToList(),
            IsAsync = isAsync,
            Text = text,
            At = at
        };
    }

    /// <summary>The nodes that are the unit's own: not a nested function's, lambda's or class's.</summary>
    private static IEnumerable<PythonNode> Own(IEnumerable<PythonNode> nodes)
    {
        foreach (var node in nodes)
        {
            yield return node;
            if (node is not (PythonFunction or PythonLambda or PythonClass))
            {
                foreach (var inner in Own(node.Children))
                {
                    yield return inner;
                }
            }
        }
    }

    /// <summary>Each plain <c>=</c>, annotated assignment with a value and <c>:=</c>; unpacking several targets assigns no one value.</summary>
    private IEnumerable<AssignmentNode> Assignments(IEnumerable<PythonNode> inside)
    {
        foreach (var node in inside)
        {
            var (targets, value) = node switch
            {
                PythonAssign assign => (assign.Targets, assign.Value),
                PythonAnnotatedAssign { Value: { } given } annotated => ([annotated.Target], given),
                PythonNamedExpression named => ([named.Target], named.Value),
                _ => ((IReadOnlyList<PythonExpression>)[], (PythonExpression?)null)
            };

            foreach (var target in targets.Where(x => x is PythonName or PythonAttribute or PythonSubscript))
            {
                yield return new AssignmentNode(TargetOf(target), Value(value!), PythonReader.SpanOf(node, file.Path))
                {
                    Receiver = target is PythonAttribute attribute && !IsSelf(attribute.Value) ? Value(attribute.Value) : null
                };
            }
        }
    }

    /// <summary>What an assignment assigns: an attribute of <c>self</c> by its name, as the type's own member; anything else as written.</summary>
    private string TargetOf(PythonExpression target) =>
        target is PythonAttribute attribute && IsSelf(attribute.Value) ? attribute.Name : PythonReader.TextOf(file.Source, target);

    /// <summary>A lambda as its own unit, with its parameters as written: a lambda's have no types.</summary>
    private FunctionNode Lambda(PythonLambda lambda)
    {
        if (_lambdas.TryGetValue(lambda, out var known))
        {
            return known;
        }

        var given = lambda.Parameters.Select(Parameter).ToList();
        return _lambdas[lambda] = Unit([lambda.Body], lambda.Body, given, false, PythonReader.TextOf(file.Source, lambda), PythonReader.SpanOf(lambda, file.Path));
    }

    /// <summary>A lambda's or nested function's parameter, once.</summary>
    private ParameterNode Parameter(PythonParameter parameter)
    {
        if (_nested.TryGetValue(parameter, out var known))
        {
            return known;
        }

        var type = parameter.Annotation is null ? null : PythonTypes.TypeRef(parameter.Annotation);
        if (type is not null)
        {
            type.Nullable = PythonTypes.Nullability(type);
        }

        return _nested[parameter] = new ParameterNode
        {
            Name = parameter.Name,
            Type = type,
            Default = parameter.Default is null ? null : PythonReader.TextOf(file.Source, parameter.Default),
            IsOptional = parameter.Kind is PythonParameterKind.VarPositional or PythonParameterKind.VarKeyword,
            At = PythonReader.SpanOf(parameter, file.Path),
            TextOf = () => PythonReader.Canonical(file, parameter)
        };
    }

    /// <summary>A creation, with its arguments; null for any other call.</summary>
    private CreationNode? Creation(PythonCall call)
    {
        if (_creations.TryGetValue(call, out var known))
        {
            return known;
        }

        _creations[call] = null;
        if (Created(call) is not { } type)
        {
            return null;
        }

        return _creations[call] = new CreationNode
        {
            Type = type,
            Arguments = Arguments(call),
            Text = PythonReader.TextOf(file.Source, call),
            At = PythonReader.SpanOf(call, file.Path)
        };
    }

    /// <summary>
    /// The class a call creates, one node each, so a value's types say which creations it is: a class the code declares or
    /// imports, a classmethod's <c>cls</c>, or a name in CapWords that is no function the code declares. Null for any other call.
    /// </summary>
    private TypeRefNode? Created(PythonCall call)
    {
        if (_created.TryGetValue(call, out var known))
        {
            return known;
        }

        TypeRefNode? type = null;
        if (call.Function is PythonName self && IsSelf(self) && !_instance)
        {
            type = OwnerRef();
        }
        else if (call.Function is PythonName or PythonAttribute && !IsLocal(call.Function, call.First.Start))
        {
            var named = PythonReader.Dotted(call.Function);
            var creates = Lookup(call.Function) switch
            {
                { Type: { Kind: not ("module" or "typeAlias") }, Member: null } => true,
                { Module: not null } or { Member: not null } or { Type: not null } => false,
                // A library's or a builtin's: ValueError(...), HTTPException(...).
                _ => PythonReader.IsClassName(named.Split('.')[^1])
            };
            if (creates)
            {
                type = new TypeRefNode { Text = named, Name = named.Split('.')[^1], QualifiedName = named, Language = PythonLanguage.LanguageName };
            }
        }

        _created[call] = type;
        if (type is not null)
        {
            _createdBy[type] = call;
        }

        return type;
    }

    private List<ValueNode> Arguments(PythonCall call) => call.Arguments.Where(x => x.Stars == 0).Select(x => Value(x.Value, x.Name)).ToList();

    private CallNode? Call(PythonCall call)
    {
        if (_calls.TryGetValue(call, out var known))
        {
            return known;
        }

        if (Created(call) is not null || Resolve(call, 0) is not { } resolved)
        {
            return _calls[call] = null;
        }

        return _calls[call] = new CallNode
        {
            Name = resolved.Name,
            Receiver = resolved.Receiver,
            ReceiverType = resolved.ReceiverType,
            On = ReceiverOf(call) is { } receiver && Unawaited(receiver) is PythonCall on ? Call(on) : null,
            Arguments = Arguments(call),
            Target = resolved.Targets is [var target] ? target : null,
            Text = PythonReader.TextOf(file.Source, call),
            CanonicalOf = () => PythonReader.Canonical(file, call),
            At = PythonReader.SpanOf(call, file.Path)
        };
    }

    private static PythonExpression? ReceiverOf(PythonCall call) => call.Function is PythonAttribute attribute ? attribute.Value : null;

    private static PythonExpression Unawaited(PythonExpression expression) => expression is PythonAwait awaited ? awaited.Value : expression;

    /// <summary>
    /// A call's name, what it is made on (nothing for a function called by its name, or a method called on <c>self</c> or
    /// <c>cls</c>) and that's type, and the functions or methods it can reach: <c>self</c>'s through the class; a function by
    /// its name, through the module that declares or imports it; on a module, that module's; on a class, its static and
    /// class methods; on anything else, its type's.
    /// </summary>
    private (string Name, string? Receiver, TypeRefNode? ReceiverType, IReadOnlyList<CallTarget> Targets)? Resolve(PythonCall call, int depth)
    {
        var (name, receiver) = call.Function switch
        {
            PythonName named => (named.Id, null),
            PythonAttribute attribute => (attribute.Name, attribute.Value),
            _ => ((string?)null, (PythonExpression?)null)
        };
        if (name is null)
        {
            return null;
        }

        var arguments = call.Arguments.Any(x => x.Stars > 0) ? (int?)null : call.Arguments.Count;
        if (receiver is null)
        {
            // Python's unqualified name is a local, the module's or a builtin: never the class's own method.
            return (name, null, null, IsLocal(call.Function, call.First.Start) ? [] : FunctionsOf(Lookup(name), name));
        }

        if (IsSelf(receiver))
        {
            return (name, null, null, _graph.Targets(name, null, null, arguments, owner));
        }

        var written = PythonReader.TextOf(file.Source, receiver);
        var (types, known) = TypesOf(receiver, call.First.Start, depth);
        var typed = known && types is [var only] ? only : null;
        if (typed is not null)
        {
            return (name, written, typed, _graph.Targets(name, written, typed, arguments, owner));
        }

        var referent = IsLocal(receiver, call.First.Start) ? null : Lookup(receiver);
        return referent switch
        {
            { Module: { ModuleType: { } module } } => (name, written, null, FunctionsOf(new PythonReferent(null, module, module.Members.FirstOrDefault(x => x.Kind == "method" && x.Name == name)), name)),
            { Type: { Kind: not "module" } type, Member: null } => (name, written, null, _graph.Targets(name, written, OwnerRef(type), arguments, owner).Where(x => x.Member.IsStatic).ToList()),
            _ => (name, written, null, [])
        };
    }

    /// <summary>The module function a name is.</summary>
    private static IReadOnlyList<CallTarget> FunctionsOf(PythonReferent? referent, string name) =>
        referent is { Type: { Kind: "module" } module, Member: { Kind: "method" } function } && function.Name == name ? [new CallTarget(module, function)] : [];

    /// <summary>What the function or method a call reaches returns, as written; <c>None</c> is nothing.</summary>
    private (List<TypeRefNode> Types, bool Known) Returned(PythonCall call, int depth) =>
        Resolve(call, depth + 1) is { Targets: [var target] } && target.Returns is { Name: not "None" } returns ? ([returns], true) : Unknown;

    /// <param name="name">A keyword argument's name (<c>status="Paid"</c>).</param>
    public ValueNode Value(PythonExpression expression, string? name = null)
    {
        var before = expression.First.Start;
        var (types, known) = TypesOf(expression, before, 0);
        return new ValueNode
        {
            Text = PythonReader.TextOf(file.Source, expression),
            Name = name,
            Types = types,
            IsKnown = known && types.Count > 0,
            Literal = PythonReader.LiteralOf(expression, x => Value(x), file.Path),
            Creations = types.Select(x => _createdBy.TryGetValue(x, out var creation) ? Creation(creation) : null).OfType<CreationNode>().ToList(),
            Function = expression is PythonLambda lambda ? Lambda(lambda) : null,
            Members = expression is PythonDict dict
                ? dict.Entries.Select(x => new InitializerNode(x.Key is null ? "..." : x.Key is PythonString { Value: { } key } ? key : PythonReader.TextOf(file.Source, x.Key), Value(x.Value))).ToList()
                : [],
            Items = expression is PythonSequence sequence ? sequence.Items.Select(x => Value(x)).ToList() : [],
            Joined = expression is PythonBinary { Operator: "+" } ? Operands(expression).Select(x => Value(x)).ToList() : [],
            GivenOf = () => Given(expression),
            Parameter = expression is PythonName named && !IsLocal(named, before) ? ParameterFor(named) : null,
            Field = FieldRead(expression, before),
            Call = Unawaited(expression) is PythonCall call ? Call(call) : null,
            Trimmed = Trimmed(expression),
            At = PythonReader.SpanOf(expression, file.Path)
        };
    }

    private static IEnumerable<PythonExpression> Operands(PythonExpression expression) =>
        expression is PythonBinary { Operator: "+" } joined ? Operands(joined.Left).Concat(Operands(joined.Right)) : [expression];

    /// <summary>
    /// The values it is, one per way the code gives it, when it stands for another: what a local is given before the use, a
    /// conditional's two branches, both sides of an <c>or</c> (Python's <c>??</c>), and what <c>str(x)</c> is made from.
    /// </summary>
    private IReadOnlyList<ValueNode> Given(PythonExpression expression) => expression switch
    {
        PythonName named when IsLocal(named, named.First.Start) => Locals(named.Id, named.First.Start).Where(x => x.Value is not null).Select(x => Value(x.Value!)).ToList(),
        PythonConditional conditional => [Value(conditional.Body), Value(conditional.OrElse)],
        PythonBinary { Operator: "or" } either => [Value(either.Left), Value(either.Right)],
        PythonCall { Function: PythonName { Id: "str" }, Arguments: [{ Stars: 0, Name: null } only] } => [Value(only.Value)],
        _ => []
    };

    /// <summary><c>uri.strip()</c>, <c>uri.rstrip("/?")</c>: whitespace, or the characters a literal names.</summary>
    private TrimNode? Trimmed(PythonExpression expression) =>
        expression is PythonCall { Function: PythonAttribute { Name: "strip" or "lstrip" or "rstrip" } trim } call && call.Arguments.Count <= 1
                                                                                                                  && (call.Arguments.Count == 0 || call.Arguments[0].Value is PythonString { Value: not null })
            ? new TrimNode(Value(trim.Value), trim.Name != "rstrip", trim.Name != "lstrip", call.Arguments.Count == 0 ? [] : ((PythonString)call.Arguments[0].Value).Value!.ToList())
            : null;

    /// <summary>
    /// The field a value reads: an attribute of <c>self</c>, a class's attribute through its name (<c>Routes.CATALOG</c>), or a
    /// module's variable, by its name or through the module (<c>settings.BASE_URL</c>).
    /// </summary>
    private FieldRef? FieldRead(PythonExpression expression, int before) => expression switch
    {
        PythonAttribute attribute when IsSelf(attribute.Value) => _graph.FieldOf(owner, attribute.Name),
        PythonAttribute attribute when !IsLocal(attribute.Value, before) && Lookup(attribute.Value) is { Type: { Kind: not "module" } type, Member: null } => _graph.FieldOf(type, attribute.Name),
        PythonAttribute attribute when !IsLocal(attribute.Value, before) && Lookup(attribute) is { Type: { } module, Member: { Kind: "field" } field } => new FieldRef(module, field),
        PythonName named when !IsLocal(named, before) && ParameterFor(named) is null && Lookup(named.Id) is { Type: { } module, Member: { Kind: "field" } field } => new FieldRef(module, field),
        _ => null
    };

    /// <summary>The types an expression can be, and whether every way it is given is known (<see cref="ValueNode"/>).</summary>
    private (List<TypeRefNode> Types, bool Known) TypesOf(PythonExpression expression, int before, int depth)
    {
        switch (expression)
        {
            case PythonName named when IsSelf(named):
                return _instance ? ([OwnerRef()], true) : Unknown;
            case PythonName named when depth < MaxDepth:
                return Named(named, before, depth);
            case PythonCall call when Created(call) is { } created:
                return ([created], true);
            case PythonCall { Function: PythonName { Id: "cast" } or PythonAttribute { Name: "cast" }, Arguments: [{ Stars: 0 } cast, _] }:
                return ([PythonTypes.TypeRef(cast.Value)], true);
            case PythonCall call when depth < MaxDepth:
                return Returned(call, depth);
            case PythonAwait awaited:
                return TypesOf(awaited.Value, before, depth);
            case PythonNamedExpression named:
                return TypesOf(named.Value, before, depth);
            case PythonConditional conditional:
            {
                var body = TypesOf(conditional.Body, before, depth);
                var orElse = TypesOf(conditional.OrElse, before, depth);
                return ([..body.Types, ..orElse.Types], body.Known && orElse.Known);
            }
            case PythonAttribute attribute when IsSelf(attribute.Value):
                return FieldOf(owner, attribute.Name);
            case PythonAttribute attribute when depth < MaxDepth:
            {
                if (!IsLocal(attribute.Value, before) && Lookup(attribute) is { Member: { Kind: "field", Type: { } declared } })
                {
                    return ([declared], true);
                }

                // An attribute path through in-scope types: self.services.mediator is Services' mediator.
                var (types, known) = TypesOf(attribute.Value, before, depth + 1);
                return known && types is [var only] && _graph.TypeFor(only, owner) is { } type ? FieldOf(type, attribute.Name) : Unknown;
            }
            default:
                return Unknown;
        }
    }

    /// <summary>What an annotated local is: what it is given, when that's known, else its annotation.</summary>
    private static (List<TypeRefNode> Types, bool Known) OrDeclared((List<TypeRefNode> Types, bool Known) value, PythonExpression declared) =>
        value.Known && value.Types.Count > 0 ? value : ([PythonTypes.TypeRef(declared)], true);

    /// <summary>A name in the body: a local assigned before the use, then a parameter (a lambda's included), then the module's variable.</summary>
    private (List<TypeRefNode> Types, bool Known) Named(PythonName named, int before, int depth)
    {
        if (IsLocal(named, before))
        {
            var locals = Locals(named.Id, before).ToList();
            var given = locals.Where(x => x.Value is not null).Select(x => TypesOf(x.Value!, x.Value!.First.Start, depth + 1)).ToList();
            (List<TypeRefNode> Types, bool Known) value = (given.SelectMany(x => x.Types).ToList(), given.Count > 0 && given.Count == locals.Count && given.All(x => x.Known));
            return locals.LastOrDefault(x => x.Annotation is not null).Annotation is { } annotation ? OrDeclared(value, annotation) : value;
        }

        if (ParameterFor(named) is { } parameter)
        {
            return parameter.Type is null ? Unknown : ([parameter.Type], true);
        }

        return Lookup(named.Id) is { Member: { Kind: "field", Type: { } declared } } ? ([declared], true) : Unknown;
    }

    /// <summary>
    /// Where the unit binds a name before a place: each assignment with its value and annotation, and each <c>for</c>,
    /// <c>with</c>, <c>except</c> and comprehension target, whose value no expression says.
    /// </summary>
    private IEnumerable<(PythonExpression? Value, PythonExpression? Annotation)> Locals(string name, int before)
    {
        foreach (var node in roots.SelectMany(x => x.Descendants().Prepend(x)).Where(x => x.First.Start < before))
        {
            switch (node)
            {
                case PythonAssign assign when assign.Targets.Any(x => Binds(x, name)):
                    yield return (assign.Targets.Any(x => x is PythonName { } only && only.Id == name) ? assign.Value : null, null);
                    break;
                case PythonAnnotatedAssign { Target: PythonName { } target } annotated when target.Id == name:
                    yield return (annotated.Value, annotated.Annotation);
                    break;
                case PythonNamedExpression named when named.Target.Id == name:
                    yield return (named.Value, null);
                    break;
                case PythonFor loop when Binds(loop.Target, name):
                    yield return (null, null);
                    break;
                case PythonWithItem { Target: { } bound } when Binds(bound, name):
                    yield return (null, null);
                    break;
                case PythonComprehensionClause clause when Binds(clause.Target, name):
                    yield return (null, null);
                    break;
                case PythonExceptHandler handler when handler.Name == name:
                    yield return (null, null);
                    break;
            }
        }
    }

    private static bool Binds(PythonExpression target, string name) => target switch
    {
        PythonName named => named.Id == name,
        PythonSequence sequence => sequence.Items.Any(x => Binds(x, name)),
        PythonStarred starred => Binds(starred.Value, name),
        _ => false
    };

    /// <summary>A name the unit binds before the place, so isn't a parameter's or the module's there.</summary>
    private bool IsLocal(PythonExpression expression, int before) =>
        expression is PythonName named && !IsSelf(named) && Locals(named.Id, before).Any();

    /// <summary>The parameter a name is: the nearest lambda's or nested function's around it, then the unit's own.</summary>
    private ParameterNode? ParameterFor(PythonName named)
    {
        _parents ??= Parents();
        for (var at = (PythonNode)named; _parents.TryGetValue(at, out var parent); at = parent)
        {
            var declared = parent switch
            {
                PythonLambda lambda => lambda.Parameters,
                PythonFunction nested => nested.Parameters,
                _ => null
            };
            // The parameter's own node, which the lambda's unit has too: the unit may be being read.
            if (declared?.FirstOrDefault(x => x.Name == named.Id) is { } found)
            {
                return Parameter(found);
            }
        }

        return parameters.FirstOrDefault(x => x.Written.Name == named.Id).Node;
    }

    private Dictionary<PythonNode, PythonNode> Parents()
    {
        var parents = new Dictionary<PythonNode, PythonNode>(ReferenceEqualityComparer.Instance);
        var stack = new Stack<PythonNode>(roots);
        while (stack.TryPop(out var node))
        {
            foreach (var child in node.Children)
            {
                parents[child] = node;
                stack.Push(child);
            }
        }

        return parents;
    }

    /// <summary>A field or property of a type, or one it inherits.</summary>
    private (List<TypeRefNode> Types, bool Known) FieldOf(TypeNode type, string name) =>
        _graph.MembersOf(type).FirstOrDefault(x => x.Name == name && x.Kind is "field" or "property") is { Type: { } declared } ? ([declared], true) : Unknown;

    private bool IsSelf(PythonExpression expression) => _self is not null && expression is PythonName named && named.Id == _self;

    private TypeRefNode OwnerRef() => OwnerRef(owner);

    private static TypeRefNode OwnerRef(TypeNode type) =>
        new() { Text = type.Name, Name = type.Name, QualifiedName = type.Namespace.Length == 0 ? type.Name : $"{type.Namespace}.{type.Name}", Language = PythonLanguage.LanguageName };

    /// <summary>
    /// What a name is at the module's level: a class or type alias of the module, its function or variable, or what an import
    /// binds it to, through a wildcard import too.
    /// </summary>
    private PythonReferent? Lookup(string name)
    {
        if (file.Types.TryGetValue(name, out var type))
        {
            return new PythonReferent(null, type, null);
        }

        if (file.ModuleType?.Members.FirstOrDefault(x => x.Name == name) is { } member)
        {
            return new PythonReferent(null, file.ModuleType, member);
        }

        if (file.Bindings.TryGetValue(name, out var target))
        {
            return project.Resolve(target) ?? new PythonReferent(null, null, null, target);
        }

        return file.Wildcards.Select(x => project.Resolve($"{x}.{name}")).FirstOrDefault(x => x is not null);
    }

    /// <summary>What a name or a dotted name is: <c>models.Order</c> through the module <c>models</c>.</summary>
    private PythonReferent? Lookup(PythonExpression expression)
    {
        switch (expression)
        {
            case PythonName named:
                return Lookup(named.Id);
            case PythonAttribute attribute:
            {
                var inner = Lookup(attribute.Value);
                return inner switch
                {
                    { Module: { } module } => project.Resolve($"{module.Module}.{attribute.Name}") ?? new PythonReferent(null, null, null, $"{module.Module}.{attribute.Name}"),
                    { Library: { } library } => new PythonReferent(null, null, null, $"{library}.{attribute.Name}"),
                    _ => null
                };
            }
            default:
                return null;
        }
    }
}
