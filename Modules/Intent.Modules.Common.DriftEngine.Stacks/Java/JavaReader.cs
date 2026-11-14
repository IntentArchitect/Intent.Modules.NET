using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Stacks.Java.Grammar;
using static Intent.Modules.Common.DriftEngine.Stacks.Java.Grammar.JavaParser;

namespace Intent.Modules.Common.DriftEngine.Stacks.Java;

/// <summary>
/// The Java reader (code-reader.md): parses each file with ANTLR, syntax only. It never builds, loads or runs the code
/// (decision 3), so names aren't resolved; types compare by name.
/// </summary>
internal static class JavaReader
{
    // What says a type is nullable, from the Java profile: annotations by simple name, across the libraries that define
    // them (JSR 305, Jakarta Bean Validation, JetBrains, Lombok, the Checker Framework); boxed types; the nullable wrappers.
    // A framework's own, such as JPA's @Column(nullable = false), is that framework's extension's to read.
    private static readonly JsonNode Nullable = StockExtensions.LoadPack(JavaLanguage.LanguageName).Profile(JavaLanguage.LanguageName)!["nullable"]!;
    private static readonly HashSet<string> NonNull = Names(Nullable["nonNullAnnotations"]);
    private static readonly HashSet<string> MaybeNull = Names(Nullable["annotations"]);
    private static readonly HashSet<string> NullableTypes = [..(Nullable["boxed"] as JsonObject ?? []).Select(x => x.Value!.GetValue<string>()), ..Names(Nullable["wrappers"]).Select(x => x.Split('<')[0])];

    private static readonly Regex Whitespace = new(@"\s+", RegexOptions.CultureInvariant);
    private static readonly Regex Escape = new(@"\\(?:u+(?<hex>[0-9a-fA-F]{4})|(?<octal>[0-3][0-7]{0,2}|[4-7][0-7]?)|(?<line>\r\n|\r|\n)|(?<char>.))", RegexOptions.CultureInvariant);

    private static readonly Regex LineBreak = new(@"\r\n|\r|\n", RegexOptions.CultureInvariant);

    /// <param name="root">The output folder.</param>
    /// <param name="files">In-scope paths relative to <paramref name="root"/>, with <c>/</c> separators.</param>
    /// <param name="graph">The graph it adds the files, types and members to, and its <see cref="JavaSyntax"/>.</param>
    public static void Read(string root, IReadOnlyList<string> files, CodeGraph graph)
    {
        var syntax = new JavaSyntax();
        graph.Set(syntax);
        var bodies = new List<Body>();
        foreach (var file in files)
        {
            var (tree, errors) = Parse(File.ReadAllText(Path.Combine(root, file)));
            if (errors.Count > 0)
            {
                graph.Failures.Add((file, string.Join("; ", errors)));
            }

            syntax.Trees[file] = tree;
            var package = tree.packageDeclaration()?.qualifiedName().GetText() ?? "";
            graph.Files.Add(new FileNode
            {
                Path = file,
                Language = JavaLanguage.LanguageName,
                Namespaces = package.Length > 0 ? [package] : [],
                Imports = tree.importDeclaration().Select(x => Import(x, file)).ToList()
            });
            foreach (var declaration in tree.typeDeclaration())
            {
                Type(Declaration(declaration), declaration.classOrInterfaceModifier().Select(Modifier).ToList(), SpanOf(declaration, file), package, null, file, graph, bodies);
            }
        }

        // What a body's calls are made on, and what their arguments are, can depend on any type, so bodies are read last.
        foreach (var (member, body, owner, file) in bodies)
        {
            member.Body = new JavaBody(body, owner, file, graph).Member(member.Parameters);
        }
    }

    /// <summary>
    /// ANTLR's two-stage parse: SLL prediction with a bail-out error strategy, which is fast and right for almost every
    /// file, then the full LL parse with error recovery only when that fails.
    /// </summary>
    private static (CompilationunitContext Tree, IReadOnlyList<string> Errors) Parse(string text)
    {
        var errors = new Errors();
        var lexer = new JavaLexer(CharStreams.fromString(text));
        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(errors);
        var tokens = new CommonTokenStream(lexer);
        var parser = new JavaParser(tokens, TextWriter.Null, TextWriter.Null);
        parser.RemoveErrorListeners();
        parser.Interpreter.PredictionMode = PredictionMode.SLL;
        parser.ErrorHandler = new BailErrorStrategy();
        try
        {
            return (parser.compilationunit(), errors.Messages);
        }
        catch (ParseCanceledException)
        {
            tokens.Seek(0);
            parser.Reset();
            parser.AddErrorListener(errors);
            parser.ErrorHandler = new DefaultErrorStrategy();
            parser.Interpreter.PredictionMode = PredictionMode.LL;
            return (parser.compilationunit(), errors.Messages);
        }
    }

    private static ParserRuleContext Declaration(TypeDeclarationContext declaration) =>
        (ParserRuleContext?)declaration.classDeclaration() ?? (ParserRuleContext?)declaration.interfaceDeclaration()
        ?? (ParserRuleContext?)declaration.enumDeclaration() ?? (ParserRuleContext?)declaration.recordDeclaration()
        ?? declaration.annotationTypeDeclaration();

    /// <param name="modifiers">Its modifiers and annotations, which the grammar keeps on the declaration's parent.</param>
    /// <param name="outer">The kind of the type it is declared in; null for a top-level type.</param>
    private static void Type(ParserRuleContext declaration, IReadOnlyList<Mod> modifiers, Span at, string package, string? outer, string file, CodeGraph graph, List<Body> bodies)
    {
        var (name, kind) = declaration switch
        {
            ClassDeclarationContext x => (x.identifier().GetText(), "class"),
            InterfaceDeclarationContext x => (x.identifier().GetText(), "interface"),
            EnumDeclarationContext x => (x.identifier().GetText(), "enum"),
            RecordDeclarationContext x => (x.identifier().GetText(), "record"),
            AnnotationTypeDeclarationContext x => (x.identifier().GetText(), "annotation"),
            _ => throw new InvalidOperationException($"Unexpected Java type declaration: {declaration.GetType().Name}")
        };

        // Without an access modifier, a type is package-private, unless an interface declares it.
        var type = new TypeNode
        {
            Name = name,
            Namespace = package,
            Kind = kind,
            At = at,
            IsAbstract = modifiers.Any(x => x.Keyword == "abstract"),
            IsStatic = modifiers.Any(x => x.Keyword == "static"),
            Access = Access(modifiers) ?? (outer is "interface" or "annotation" ? "public" : "package"),
            TextOf = () => Canonical(WithModifiers(declaration))
        };
        type.Files.Add(file);
        type.Attributes.AddRange(Annotations(modifiers, file));
        graph.Types.Add(type);

        switch (declaration)
        {
            case ClassDeclarationContext declared:
                type.TypeParameters = TypeParameters(declared.typeParameters());
                if (declared.typeType() is { } extends)
                {
                    type.BaseTypes.Add(TypeRef(extends));
                }

                type.BaseTypes.AddRange(ListAfter(declared, IMPLEMENTS)?.typeType().Select(TypeRef) ?? []);
                ClassMembers(type, declared.classBody().classBodyDeclaration(), package, file, graph, bodies);
                break;
            case InterfaceDeclarationContext declared:
                type.TypeParameters = TypeParameters(declared.typeParameters());
                type.BaseTypes.AddRange(ListAfter(declared, EXTENDS)?.typeType().Select(TypeRef) ?? []);
                InterfaceMembers(type, declared.interfaceBody().interfaceBodyDeclaration(), package, file, graph, bodies);
                break;
            case EnumDeclarationContext declared:
                type.BaseTypes.AddRange(declared.typeList()?.typeType().Select(TypeRef) ?? []);
                foreach (var constant in declared.enumConstants()?.enumConstant() ?? [])
                {
                    type.Members.Add(new MemberNode
                    {
                        Name = constant.identifier().GetText(),
                        Kind = "enumMember",
                        Attributes = constant.annotation().Select(x => Annotation(x, file)).ToList(),
                        At = SpanOf(constant, file),
                        TextOf = () => Canonical(constant)
                    });
                }

                ClassMembers(type, declared.enumBodyDeclarations()?.classBodyDeclaration() ?? [], package, file, graph, bodies);
                break;
            case RecordDeclarationContext declared:
                type.TypeParameters = TypeParameters(declared.typeParameters());
                type.BaseTypes.AddRange(declared.typeList()?.typeType().Select(TypeRef) ?? []);
                Record(type, declared, file);
                ClassMembers(type, declared.recordBody().classBodyDeclaration(), package, file, graph, bodies);
                break;
        }
    }

    /// <summary>A record's components are its properties, and the parameters of its canonical constructor.</summary>
    private static void Record(TypeNode type, RecordDeclarationContext record, string file)
    {
        var parameters = new List<ParameterNode>();
        foreach (var component in record.recordHeader().recordComponentList()?.recordComponent() ?? [])
        {
            var annotations = component.annotation().Concat(component.typeType().annotation()).Select(x => Annotation(x, file)).ToList();
            var componentType = component.ELLIPSIS() is null ? TypeRef(component.typeType()) : ArrayOf(TypeRef(component.typeType()));
            componentType.Nullable = Nullability(componentType, annotations);
            var at = SpanOf(component, file);
            type.Members.Add(new MemberNode { Name = component.identifier().GetText(), Kind = "property", Type = componentType, Attributes = annotations, At = at, TextOf = () => Canonical(component) });
            parameters.Add(new ParameterNode { Name = component.identifier().GetText(), Type = componentType, Attributes = annotations, At = at, TextOf = () => Canonical(component) });
        }

        type.Members.Add(new MemberNode { Name = type.Name, Kind = "constructor", IsPrimary = true, Parameters = parameters, At = SpanOf(record.recordHeader(), file), TextOf = () => Canonical(record.recordHeader()) });
    }

    private static void ClassMembers(TypeNode type, IEnumerable<ClassBodyDeclarationContext> declarations, string package, string file, CodeGraph graph, List<Body> bodies)
    {
        foreach (var declaration in declarations)
        {
            // A ';', or an initializer block.
            if (declaration.memberDeclaration() is not { } member)
            {
                continue;
            }

            var modifiers = declaration.modifier().Select(Modifier).ToList();
            var at = SpanOf(declaration, file);
            if (member.fieldDeclaration() is { } field)
            {
                foreach (var variable in field.variableDeclarators().variableDeclarator())
                {
                    type.Members.Add(Member(variable.variableDeclaratorId().identifier().GetText(), "field", TypeRef(field.typeType()), modifiers, field.typeType().annotation(), "package", declaration, file,
                        value: ValueOf(variable.variableInitializer(), file)));
                }
            }
            else if ((member.methodDeclaration() ?? member.genericMethodDeclaration()?.methodDeclaration()) is { } method)
            {
                var body = method.methodBody().block();
                Add(type, Member(method.identifier().GetText(), "method", Returns(method.typeTypeOrVoid()), modifiers, method.typeTypeOrVoid().typeType()?.annotation() ?? [], "package", declaration, file,
                    Parameters(method.formalParameters(), file), typeParameters: TypeParameters(member.genericMethodDeclaration()?.typeParameters())), body, file, bodies);
            }
            else if ((member.constructorDeclaration() ?? member.genericConstructorDeclaration()?.constructorDeclaration()) is { } constructor)
            {
                // An enum's constructor is private, whether or not it says so.
                Add(type, Member(constructor.identifier().GetText(), "constructor", null, modifiers, [], type.Kind == "enum" ? "private" : "package", declaration, file,
                    Parameters(constructor.formalParameters(), file)), constructor.block(), file, bodies);
            }
            else if (Nested(member.classDeclaration(), member.interfaceDeclaration(), member.enumDeclaration(), member.recordDeclaration(), member.annotationTypeDeclaration()) is { } nested)
            {
                Type(nested, modifiers, at, package, type.Kind, file, graph, bodies);
            }
        }
    }

    /// <summary>A method or constructor, and its body to read once every type is read.</summary>
    private static void Add(TypeNode type, MemberNode member, BlockContext? body, string file, List<Body> bodies)
    {
        type.Members.Add(member);
        if (body is not null)
        {
            bodies.Add(new Body(member, body, type, file));
        }
    }

    /// <summary>
    /// An interface's members are public. A method's annotations can sit in three places: the declaration's modifiers, the
    /// method's own modifiers, and before its return type.
    /// </summary>
    private static void InterfaceMembers(TypeNode type, IEnumerable<InterfaceBodyDeclarationContext> declarations, string package, string file, CodeGraph graph, List<Body> bodies)
    {
        foreach (var declaration in declarations)
        {
            if (declaration.interfaceMemberDeclaration() is not { } member)
            {
                continue;
            }

            var modifiers = declaration.modifier().Select(Modifier).ToList();
            var at = SpanOf(declaration, file);
            if (member.constDeclaration() is { } constant)
            {
                foreach (var declarator in constant.constantDeclarator())
                {
                    type.Members.Add(Member(declarator.identifier().GetText(), "field", TypeRef(constant.typeType()), modifiers, constant.typeType().annotation(), "public", declaration, file,
                        value: ValueOf(declarator.variableInitializer(), file), isStatic: true));
                }
            }
            else if ((member.interfaceMethodDeclaration()?.interfaceCommonBodyDeclaration() ?? member.genericInterfaceMethodDeclaration()?.interfaceCommonBodyDeclaration()) is { } method)
            {
                var own = member.interfaceMethodDeclaration()?.interfaceMethodModifier() ?? member.genericInterfaceMethodDeclaration()!.interfaceMethodModifier();
                var all = modifiers.Concat(own.Select(Modifier)).Concat(method.annotation().Select(x => new Mod(null, x))).ToList();
                var body = method.methodBody().block();
                Add(type, Member(method.identifier().GetText(), "method", Returns(method.typeTypeOrVoid()), all, method.typeTypeOrVoid().typeType()?.annotation() ?? [], "public", declaration, file,
                    Parameters(method.formalParameters(), file), typeParameters: TypeParameters(member.genericInterfaceMethodDeclaration()?.typeParameters())), body, file, bodies);
            }
            else if (Nested(member.classDeclaration(), member.interfaceDeclaration(), member.enumDeclaration(), member.recordDeclaration(), member.annotationTypeDeclaration()) is { } nested)
            {
                Type(nested, modifiers, at, package, type.Kind, file, graph, bodies);
            }
        }
    }

    private static ParserRuleContext? Nested(params ParserRuleContext?[] declarations) => declarations.FirstOrDefault(x => x is not null);

    /// <param name="typeAnnotations">Annotations the grammar puts in the type, before it: the member's too.</param>
    /// <param name="access">Its access when none is written.</param>
    private static MemberNode Member(string name, string kind, TypeRefNode? type, IReadOnlyList<Mod> modifiers, IEnumerable<AnnotationContext> typeAnnotations, string access, ParserRuleContext declaration, string file,
        IReadOnlyList<ParameterNode>? parameters = null, ValueNode? value = null, bool isStatic = false, IReadOnlyList<string>? typeParameters = null)
    {
        var annotations = Annotations(modifiers.Concat(typeAnnotations.Select(x => new Mod(null, x))), file);
        if (type is not null && kind is "field" or "property")
        {
            type.Nullable = Nullability(type, annotations);
        }

        return new MemberNode
        {
            Name = name,
            Kind = kind,
            Type = type,
            IsStatic = isStatic || modifiers.Any(x => x.Keyword == "static"),
            Parameters = parameters ?? [],
            Attributes = annotations,
            Access = Access(modifiers) ?? access,
            Modifiers = modifiers.Select(x => x.Keyword).OfType<string>().Where(x => x is not ("public" or "protected" or "private" or "static")).ToList(),
            Value = value,
            TypeParameters = typeParameters ?? [],
            At = SpanOf(declaration, file),
            TextOf = () => Canonical(declaration)
        };
    }

    /// <summary>
    /// A nullness annotation says; else a primitive can't be null, and a boxed type or <c>Optional</c> can. Java doesn't say
    /// whether any other reference may be null, so it is <c>unknown</c> (code-reader.md, Type references).
    /// </summary>
    private static string Nullability(TypeRefNode type, IReadOnlyList<AttributeNode> annotations) =>
        annotations.Any(x => NonNull.Contains(x.Name)) ? "false"
        : annotations.Any(x => MaybeNull.Contains(x.Name)) ? "true"
        : type is { IsPredefinedValueType: true, IsArray: false } ? "false"
        : type is { IsArray: false } && NullableTypes.Contains(type.Name) ? "true"
        : "unknown";

    private static HashSet<string> Names(JsonNode? list) => (list as JsonArray ?? []).Select(x => x!.GetValue<string>()).ToHashSet(StringComparer.Ordinal);

    private static IReadOnlyList<ParameterNode> Parameters(FormalParametersContext parameters, string file) => FormalParameters(parameters).Select(x => Parameter(x, file)).ToList();

    internal static ParameterNode Parameter(FormalParameterContext parameter, string file)
    {
        var annotations = parameter.variableModifier().Select(m => m.annotation()).OfType<AnnotationContext>().Concat(parameter.typeType().annotation()).Select(a => Annotation(a, file)).ToList();
        var type = TypeOf(parameter);
        type.Nullable = Nullability(type, annotations);
        return new ParameterNode { Name = parameter.variableDeclaratorId().identifier().GetText(), Type = type, Attributes = annotations, At = SpanOf(parameter, file), TextOf = () => Canonical(parameter) };
    }

    /// <summary>The first parameter is the parameter list's own child, and the rest are in nested lists.</summary>
    internal static IEnumerable<FormalParameterContext> FormalParameters(FormalParametersContext parameters) =>
        (parameters.formalParameter() is { } first ? [first] : Array.Empty<FormalParameterContext>())
        .Concat(parameters.formalParameterList().SelectMany(x => x.formalParameter()));

    /// <summary>A parameter's type; a varargs parameter's is an array.</summary>
    internal static TypeRefNode TypeOf(FormalParameterContext parameter) =>
        parameter.ELLIPSIS() is null ? TypeRef(parameter.typeType()) : ArrayOf(TypeRef(parameter.typeType()));

    private static TypeRefNode Returns(TypeTypeOrVoidContext type) =>
        type.typeType() is { } returns ? TypeRef(returns) : new TypeRefNode { Text = "void", Name = "void", QualifiedName = "void", Language = JavaLanguage.LanguageName };

    internal static TypeRefNode TypeRef(TypeTypeContext type)
    {
        var element = type.primitiveType() is { } primitive
            ? new TypeRefNode { Text = primitive.GetText(), Name = primitive.GetText(), QualifiedName = primitive.GetText(), IsPredefinedValueType = true, Language = JavaLanguage.LanguageName }
            : ClassType(type.classOrInterfaceType().classType());
        return type.LBRACK().Aggregate(element, (x, _) => ArrayOf(x));
    }

    private static TypeRefNode ArrayOf(TypeRefNode element) =>
        new() { Text = element.Text + "[]", Name = element.Name, QualifiedName = element.QualifiedName, Arguments = [element], IsArray = true, Language = JavaLanguage.LanguageName };

    /// <summary>A class type's name is its last segment; its type arguments are that segment's (<c>Map.Entry&lt;K, V&gt;</c>).</summary>
    private static TypeRefNode ClassType(ClassTypeContext type)
    {
        var segments = type.packageName()?.identifier().Select(x => x.GetText()).ToList() ?? [];
        var written = segments.ToList();
        IReadOnlyList<TypeRefNode> arguments = [];
        foreach (var child in type.children)
        {
            switch (child)
            {
                case TypeIdentifierContext identifier:
                    segments.Add(identifier.GetText());
                    written.Add(identifier.GetText());
                    arguments = [];
                    break;
                case TypeArgumentsContext list:
                    arguments = list.typeArgument().Select(TypeArgument).ToList();
                    written[^1] += $"<{string.Join(", ", arguments.Select(x => x.Text))}>";
                    break;
            }
        }

        return new TypeRefNode { Text = string.Join(".", written), Name = segments[^1], QualifiedName = string.Join(".", segments), Arguments = arguments, Language = JavaLanguage.LanguageName };
    }

    /// <summary>A wildcard is named <c>?</c>, with its bound as its argument.</summary>
    internal static TypeRefNode TypeArgument(TypeArgumentContext argument)
    {
        if (argument.QUESTION() is null)
        {
            return TypeRef(argument.typeType());
        }

        var bound = argument.typeType() is { } type ? TypeRef(type) : null;
        var text = bound is null ? "?" : $"? {(argument.EXTENDS() is not null ? "extends" : "super")} {bound.Text}";
        return new TypeRefNode { Text = text, Name = "?", QualifiedName = "?", Arguments = bound is null ? [] : [bound], Language = JavaLanguage.LanguageName };
    }

    private static IReadOnlyList<string> TypeParameters(TypeParametersContext? parameters) =>
        parameters?.typeParameter().Select(x => x.identifier().GetText()).ToList() ?? [];

    /// <summary>A class's implements and permits lists are the same rule, so the keyword before one says which it is.</summary>
    private static TypeListContext? ListAfter(ParserRuleContext declaration, int keyword)
    {
        for (var i = 0; i < declaration.ChildCount - 1; i++)
        {
            if (declaration.GetChild(i) is ITerminalNode terminal && terminal.Symbol.Type == keyword && declaration.GetChild(i + 1) is TypeListContext list)
            {
                return list;
            }
        }

        return null;
    }

    private static ImportNode Import(ImportDeclarationContext import, string file) => new()
    {
        Target = import.qualifiedName().GetText(),
        Text = TextOf(import),
        Static = import.STATIC() is not null,
        Wildcard = import.MUL() is not null,
        At = SpanOf(import, file)
    };

    private static ValueNode? ValueOf(VariableInitializerContext? initializer, string file) =>
        initializer is null ? null : new ValueNode { Text = TextOf(initializer), Literal = initializer.expression() is { } expression ? LiteralOf(expression) : null, At = SpanOf(initializer, file) };

    private static IReadOnlyList<AttributeNode> Annotations(IEnumerable<Mod> modifiers, string file) =>
        modifiers.Select(x => x.Annotation).OfType<AnnotationContext>().Select(x => Annotation(x, file)).ToList();

    private static AttributeNode Annotation(AnnotationContext annotation, string file)
    {
        var values = annotation.annotationFieldValues()?.annotationFieldValue().Select(x => Value(x.annotationValue(), x.identifier()?.GetText(), file)).ToList() ?? [];
        return new AttributeNode
        {
            Name = annotation.qualifiedName().identifier().Last().GetText(),
            Arguments = values.Where(x => x.Name is null).Select(x => x.Literal ?? x.Text).ToList(),
            Named = values.Where(x => x.Name is not null).ToDictionary(x => x.Name!, x => x.Literal ?? x.Text),
            Values = values.OrderBy(x => x.Name is not null).ToList(),
            At = SpanOf(annotation, file)
        };
    }

    private static AttributeValueNode Value(AnnotationValueContext value, string? name, string file) => new()
    {
        Name = name,
        Text = TextOf(value),
        Literal = value.expression() is { } expression ? LiteralOf(expression)?.Value : null,
        Items = value.LBRACE() is null ? [] : value.annotationValue().Select(x => Value(x, null, file)).ToList(),
        Annotation = value.annotation() is { } nested ? Annotation(nested, file) : null
    };

    /// <summary>The literal an expression is (<see cref="LiteralNode"/>), or null. Java has no interpolated strings.</summary>
    internal static LiteralNode? LiteralOf(ExpressionContext expression)
    {
        switch (expression)
        {
            case PrimaryExpressionContext { } primary when primary.primary().expression() is { } parenthesized:
                return LiteralOf(parenthesized);
            case PrimaryExpressionContext { } primary when primary.primary().literal() is { } literal:
                return literal.STRING_LITERAL() is { } text ? new LiteralNode { Kind = "string", Value = Unescape(text.GetText()[1..^1]) }
                    : literal.TEXT_BLOCK() is { } block ? new LiteralNode { Kind = "string", Value = TextBlock(block.GetText()) }
                    : literal.CHAR_LITERAL() is { } character ? new LiteralNode { Kind = "char", Value = Unescape(character.GetText()[1..^1]) }
                    : literal.BOOL_LITERAL() is not null ? new LiteralNode { Kind = "boolean", Value = literal.GetText() }
                    : literal.NULL_LITERAL() is not null ? new LiteralNode { Kind = "null" }
                    : new LiteralNode { Kind = "number", Value = literal.GetText() };
            case PrimaryExpressionContext { } primary when primary.primary().CLASS() is not null:
                return new LiteralNode { Kind = "type", Type = Returns(primary.primary().typeTypeOrVoid()) };
            case UnaryOperatorExpressionContext { prefix.Text: "-" or "+" } signed
                when signed.expression() is PrimaryExpressionContext { } operand && operand.primary().literal() is { } number && (number.integerLiteral() is not null || number.floatLiteral() is not null):
                return new LiteralNode { Kind = "number", Value = (signed.prefix.Text == "-" ? "-" : "") + number.GetText() };
            default:
                return null;
        }
    }

    /// <summary>Java's escapes resolved: <c>\n</c> and the other single characters, octal, <c>\uXXXX</c>, and a text block's <c>\</c> at a line's end.</summary>
    private static string Unescape(string text) =>
        Escape.Replace(text, x =>
            x.Groups["hex"].Success ? ((char)Convert.ToInt32(x.Groups["hex"].Value, 16)).ToString()
            : x.Groups["octal"].Success ? ((char)Convert.ToInt32(x.Groups["octal"].Value, 8)).ToString()
            : x.Groups["line"].Success ? ""
            : x.Groups["char"].Value switch { "n" => "\n", "t" => "\t", "r" => "\r", "b" => "\b", "f" => "\f", "s" => " ", var other => other });

    /// <summary>
    /// A text block's content (JLS 3.10.6): the lines after its opening delimiter, without their common indentation (the
    /// closing delimiter's line counts) or trailing spaces, then its escapes resolved.
    /// </summary>
    private static string TextBlock(string written)
    {
        var lines = LineBreak.Split(written[3..^3]).Skip(1).ToList();
        var indent = lines.Where((x, i) => x.Trim().Length > 0 || i == lines.Count - 1).Select(x => x.Length - x.TrimStart().Length).DefaultIfEmpty(0).Min();
        return Unescape(string.Join("\n", lines.Select(x => x.Trim().Length == 0 ? "" : x[indent..].TrimEnd())));
    }

    private static string? Access(IEnumerable<Mod> modifiers) => modifiers.Select(x => x.Keyword).FirstOrDefault(x => x is "public" or "protected" or "private");

    private static Mod Modifier(ClassOrInterfaceModifierContext modifier) => modifier.annotation() is { } annotation ? new Mod(null, annotation) : new Mod(modifier.GetText(), null);

    private static Mod Modifier(ModifierContext modifier) => modifier.classOrInterfaceModifier() is { } inner ? Modifier(inner) : new Mod(modifier.GetText(), null);

    private static Mod Modifier(InterfaceMethodModifierContext modifier) => modifier.annotation() is { } annotation ? new Mod(null, annotation) : new Mod(modifier.GetText(), null);

    internal static Span SpanOf(ParserRuleContext node, string file)
    {
        var stop = node.Stop ?? node.Start;
        return new Span(file, node.Start.Line, node.Start.Column + 1, stop.Line, stop.Column + (stop.Text?.Length ?? 0) + 1);
    }

    /// <summary>
    /// A node's canonical text (code-reader.md, Canonical text): its tokens joined, with only the spaces words need.
    /// Comments and whitespace are on hidden channels, so the tree's own tokens are the code.
    /// </summary>
    internal static string Canonical(IParseTree node) =>
        CanonicalText.Of(Terminals(node).Where(x => x.Symbol.Type != TokenConstants.EOF).Select(x => (x.GetText(), false)));

    /// <summary>A type's declaration with the modifiers and annotations the grammar keeps on the node around it.</summary>
    private static ParserRuleContext WithModifiers(ParserRuleContext declaration) => declaration.Parent switch
    {
        TypeDeclarationContext x => x,
        MemberDeclarationContext { Parent: ClassBodyDeclarationContext x } => x,
        InterfaceMemberDeclarationContext { Parent: InterfaceBodyDeclarationContext x } => x,
        _ => declaration
    };

    private static IEnumerable<ITerminalNode> Terminals(IParseTree node) =>
        node is ITerminalNode terminal ? [terminal] : Enumerable.Range(0, node.ChildCount).SelectMany(x => Terminals(node.GetChild(x)));

    internal static string TextOf(ParserRuleContext node) =>
        node.Start is null || node.Stop is null || node.Stop.StopIndex < node.Start.StartIndex
            ? ""
            : Whitespace.Replace(node.Start.InputStream.GetText(Interval.Of(node.Start.StartIndex, node.Stop.StopIndex)), " ").Trim();

    /// <summary>A modifier keyword, or an annotation: the grammar mixes them.</summary>
    private readonly record struct Mod(string? Keyword, AnnotationContext? Annotation);

    private readonly record struct Body(MemberNode Member, BlockContext Block, TypeNode Owner, string File);

    private sealed class Errors : BaseErrorListener, IAntlrErrorListener<int>
    {
        public List<string> Messages { get; } = [];

        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e) =>
            Messages.Add($"line {line}:{charPositionInLine + 1} {msg}");

        public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e) =>
            Messages.Add($"line {line}:{charPositionInLine + 1} {msg}");
    }
}
