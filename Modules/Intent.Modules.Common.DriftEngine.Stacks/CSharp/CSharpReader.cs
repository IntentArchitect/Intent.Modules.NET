using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Intent.Modules.Common.DriftEngine.Code;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intent.Modules.Common.DriftEngine.Stacks.CSharp;

/// <summary>
/// The C# reader (code-reader.md): parses each file with Roslyn, syntax only. It never builds, loads or runs the code
/// (decision 3), so names aren't resolved; types compare by name. A Razor component is the C# the Razor compiler generates
/// for it (<see cref="RazorFile"/>), one type with its code-behind.
/// </summary>
internal static class CSharpReader
{
    private static readonly HashSet<string> PredefinedValueTypes =
    [
        "bool", "byte", "sbyte", "char", "decimal", "double", "float", "int", "uint", "nint", "nuint", "long", "ulong", "short", "ushort"
    ];

    // Common value types of the base library. A type declared as a struct or enum in the code graph is known too.
    private static readonly HashSet<string> LibraryValueTypes =
    [
        "Guid", "DateTime", "DateTimeOffset", "DateOnly", "TimeOnly", "TimeSpan", "Decimal", "Int16", "Int32", "Int64", "Boolean", "Byte", "Double", "Single", "Char"
    ];

    private static readonly Regex Whitespace = new(@"\s+", RegexOptions.CultureInvariant);

    /// <param name="root">The output folder.</param>
    /// <param name="files">In-scope paths relative to <paramref name="root"/>, with <c>/</c> separators.</param>
    /// <param name="graph">The graph it adds the files, types and members to, and its <see cref="CSharpSyntax"/>.</param>
    /// <param name="cancellationToken">Checked file by file, and body by body.</param>
    public static void Read(string root, IReadOnlyList<string> files, CodeGraph graph, CancellationToken cancellationToken)
    {
        var syntax = new CSharpSyntax(graph);
        graph.Set(syntax);
        var projects = new Dictionary<string, Project>(StringComparer.Ordinal);
        foreach (var project in files.Where(x => x.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase)))
        {
            bool? nullable = null;
            string? rootNamespace = null;
            try
            {
                (nullable, rootNamespace) = ProjectSettings(Path.Combine(root, project));
            }
            catch (System.Xml.XmlException e)
            {
                graph.Failures.Add((project, e.Message));
            }

            projects[Path.GetDirectoryName(project)!.Replace('\\', '/')] = new Project(project, nullable, rootNamespace);
        }

        var parts = new Dictionary<string, TypeNode>(StringComparer.Ordinal);
        var referenceTypesNullable = new List<(TypeRefNode Ref, bool Context)>();
        var bodies = new List<(MemberNode Member, SyntaxNode Body, TypeNode Owner, string File)>();
        var topLevel = new List<(FileNode Node, IReadOnlyList<SyntaxNode> Statements, string File)>();
        var razorEngines = new Dictionary<string, RazorProjectEngine>(StringComparer.Ordinal);

        // Razor components first, so a component's code-behind (X.razor.cs) is a part of the component.
        foreach (var file in files.Where(x => x.EndsWith(".razor", StringComparison.OrdinalIgnoreCase)).Concat(files.Where(x => x.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var parse = Stopwatch.GetTimestamp();
            var governing = Governing(file, projects);
            if (governing is { Nullable: null })
            {
                graph.UngovernedFiles[file] = governing.Path;
            }

            RazorFile? razor = null;
            SyntaxTree tree;
            if (file.EndsWith(".razor", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var rootNamespace = governing is null ? null : governing.RootNamespace ?? Path.GetFileNameWithoutExtension(governing.Path).Replace(' ', '_');
                    if (!razorEngines.TryGetValue(rootNamespace ?? "", out var engine))
                    {
                        razorEngines[rootNamespace ?? ""] = engine = RazorFile.Engine(root, rootNamespace);
                    }

                    razor = RazorFile.Generate(root, file, governing is null ? null : Path.GetDirectoryName(governing.Path)!.Replace('\\', '/'), engine);
                }
                catch (Exception e) when (e is IOException or UnauthorizedAccessException or InvalidOperationException or ArgumentException)
                {
                    graph.Failures.Add((file, e.Message));
                    continue;
                }

                tree = razor.Tree;
            }
            else
            {
                tree = CSharpSyntaxTree.ParseText(File.ReadAllText(Path.Combine(root, file)), path: file);
            }

            graph.Timed("parse", parse);
            var declarations = Stopwatch.GetTimestamp();
            var nullableContext = tree.GetText().ToString().Contains("#nullable enable", StringComparison.Ordinal) || governing?.Nullable == true;
            if (nullableContext)
            {
                syntax.NullableContexts.Add(file);
            }

            syntax.Trees[file] = tree;
            var fileNode = FileOf(tree.GetRoot(), file, governing?.Path, razor);
            graph.Files.Add(fileNode);
            if (tree.GetRoot() is CompilationUnitSyntax unit && unit.Members.OfType<GlobalStatementSyntax>().ToList() is { Count: > 0 } statements)
            {
                topLevel.Add((fileNode, statements, file));
            }

            // A component's generated code declares its class, and the types its @code declares; an _Imports.razor's none.
            foreach (var declaration in tree.GetRoot().DescendantNodes().OfType<BaseTypeDeclarationSyntax>()
                         .Where(x => razor is null || x == razor.Component || !razor.IsImports && razor.IsWritten(x)))
            {
                var component = razor is not null && declaration == razor.Component;
                var kind = component ? "component" : Kind(declaration);
                var ns = Namespace(declaration);
                var name = declaration.Identifier.Text;

                // With neither a project's root namespace nor an @namespace, Razor names a component by its checksum
                // (__GeneratedComponent.AspNetCore_<sha>); it is still the component its file names, in no known namespace.
                if (component && ns == "__GeneratedComponent")
                {
                    (ns, name) = ("", Path.GetFileNameWithoutExtension(file));
                }

                var key = $"{ns}.{name}";
                if (!parts.TryGetValue(key, out var type))
                {
                    type = new TypeNode
                    {
                        Name = name,
                        Namespace = ns,
                        Kind = kind,
                        At = SpanOf(declaration, file),
                        TextOf = component ? () => razor!.Canonical : () => Canonical(declaration)
                    };
                    parts.Add(key, type);
                    graph.Types.Add(type);
                }

                type.Files.Add(file);
                type.IsAbstract |= declaration.Modifiers.Any(SyntaxKind.AbstractKeyword);
                type.IsStatic |= declaration.Modifiers.Any(SyntaxKind.StaticKeyword);
                if (AccessWritten(declaration.Modifiers) is { } access)
                {
                    type.Access = access;
                }
                else if (type.Files.Count == 1)
                {
                    type.Access = declaration.Parent is BaseTypeDeclarationSyntax ? "private" : "internal";
                }

                if (declaration is TypeDeclarationSyntax { TypeParameterList: { } typeParameters })
                {
                    type.TypeParameters = typeParameters.Parameters.Select(x => x.Identifier.Text).ToList();
                }

                type.Attributes.AddRange(AttributesOf(declaration.AttributeLists, file));
                type.BaseTypes.AddRange((declaration.BaseList?.Types ?? []).Select(x => TypeRef(x.Type)));
                foreach (var (member, body) in Members(declaration, file, razor))
                {
                    type.Members.Add(member);
                    if (body is not null)
                    {
                        bodies.Add((member, body, type, file));
                    }

                    // Type arguments too: Task<Buyer?> returns a nullable Buyer.
                    foreach (var typeRef in member.Parameters.Select(x => x.Type).Prepend(member.Type).OfType<TypeRefNode>().SelectMany(WithArguments))
                    {
                        referenceTypesNullable.Add((typeRef, nullableContext));
                    }
                }
            }

            graph.Timed("declarations", declarations);
        }

        // What a body's calls are made on, and what their arguments are, can depend on any type, so bodies are read last, and
        // so are initializers, whose names can be another type's constants.
        var started = Stopwatch.GetTimestamp();
        foreach (var (member, body, owner, file) in bodies)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (body is EqualsValueClauseSyntax initializer)
            {
                member.Value = new CSharpBody([initializer], owner, file, graph).Initializer(initializer.Value);
            }
            else
            {
                member.Body = new CSharpBody([body], owner, file, graph).Member((BaseMethodDeclarationSyntax)body, member.Parameters);
            }
        }

        graph.Timed("bodies", started);

        // Top-level statements are the global namespace's Program's, which a partial Program class can add members to.
        started = Stopwatch.GetTimestamp();
        foreach (var (node, statements, file) in topLevel)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var program = parts.GetValueOrDefault(".Program") ?? new TypeNode { Name = "Program", Namespace = "", Kind = "class", At = SpanOf(statements[0], file) };
            node.TopLevel = new CSharpBody(statements, program, file, graph).TopLevel();
        }

        graph.Timed("top-level", started);

        // Nullability needs to know which names are value types, which the whole graph tells.
        started = Stopwatch.GetTimestamp();
        var valueTypes = ValueTypes(graph);
        foreach (var (typeRef, context) in referenceTypesNullable)
        {
            typeRef.Nullable = Nullability(typeRef, context, valueTypes);
        }

        graph.Timed("nullability", started);
    }

    internal static TypeRefNode TypeOf(TypeSyntax syntax, string file, CSharpSyntax csharp, CodeGraph graph)
    {
        var type = TypeRef(syntax);
        type.Nullable = Nullability(type, csharp.NullableContexts.Contains(file), ValueTypes(graph));
        return type;
    }

    private static HashSet<string> ValueTypes(CodeGraph graph) => graph.Types.Where(x => x.Kind is "struct" or "enum").Select(x => x.Name).ToHashSet(StringComparer.Ordinal);

    private static IEnumerable<TypeRefNode> WithArguments(TypeRefNode type) => type.Arguments.SelectMany(WithArguments).Prepend(type);

    private static string Nullability(TypeRefNode type, bool context, HashSet<string> valueTypes) =>
        type.NullableMarked || type.Name == "Nullable"
            ? "true"
            : !type.IsArray && (type.IsPredefinedValueType || LibraryValueTypes.Contains(type.Name) || valueTypes.Contains(type.Name))
                ? "false"
                : context ? "false" : "unknown";

    /// <summary>
    /// A file's declared namespaces and its <c>using</c> directives. A fully qualified name used inline isn't an import,
    /// so it isn't seen. A Razor file's are its own <c>@using</c>s; its <c>_Imports.razor</c> files' are theirs, which it
    /// has imports from.
    /// </summary>
    private static FileNode FileOf(SyntaxNode root, string file, string? project, RazorFile? razor) => new()
    {
        Path = file,
        Language = CSharpLanguage.LanguageName,
        Project = project,
        Namespaces = razor is { IsImports: true } ? [] : root.DescendantNodes().OfType<BaseNamespaceDeclarationSyntax>().Select(x => Namespace(x.Name)).Distinct().ToList(),
        ImportsFrom = razor?.ImportsFrom ?? [],
        Markup = razor?.Markup ?? [],
        // A Razor component's C# is generated, so its diagnostics don't say where the component's own markup is broken.
        Errors = razor is not null ? [] : root.SyntaxTree.GetDiagnostics().Where(x => x.Severity == DiagnosticSeverity.Error)
            .Select(x => (Diagnostic: x, Lines: x.Location.GetLineSpan()))
            .Select(x => new SyntaxError(new Span(file, x.Lines.StartLinePosition.Line + 1, x.Lines.StartLinePosition.Character + 1, x.Lines.EndLinePosition.Line + 1, x.Lines.EndLinePosition.Character + 1),
                $"{x.Diagnostic.Id}: {x.Diagnostic.GetMessage(System.Globalization.CultureInfo.InvariantCulture)}"))
            .ToList(),
        Imports = root.DescendantNodes().OfType<UsingDirectiveSyntax>()
            .Where(x => x.NamespaceOrType is not null && (razor is null || razor.IsOwn(x)))
            .Select(x => new ImportNode
            {
                Target = Whitespace.Replace(x.NamespaceOrType!.ToString(), "").Replace("global::", ""),
                Text = razor is null ? Whitespace.Replace(x.ToString(), " ").Trim() : "@" + Whitespace.Replace(x.ToString(), " ").Trim().TrimEnd(';'),
                Alias = x.Alias?.Name.Identifier.Text,
                Static = x.StaticKeyword.IsKind(SyntaxKind.StaticKeyword),
                Wildcard = x.Alias is null && !x.StaticKeyword.IsKind(SyntaxKind.StaticKeyword),
                Global = x.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword),
                At = SpanOf(x, file)
            })
            .ToList()
    };

    /// <summary>A namespace declaration's qualified name, including the namespaces it is nested in.</summary>
    private static string Namespace(NameSyntax name) =>
        string.Join(".", name.AncestorsAndSelf().OfType<BaseNamespaceDeclarationSyntax>().Reverse().Select(x => x.Name.ToString()));

    /// <summary>
    /// The members a type declares, each with its declaration when it has a body to read. A Razor component's are those
    /// written in its file: its <c>@code</c>'s and its <c>@inject</c>s', not what the compiler writes from its markup.
    /// </summary>
    /// <returns>Each member, with what its body pass reads: a method's or constructor's declaration, or a field's or property's initializer.</returns>
    private static IEnumerable<(MemberNode Member, SyntaxNode? Body)> Members(BaseTypeDeclarationSyntax declaration, string file, RazorFile? razor)
    {
        if (declaration is EnumDeclarationSyntax @enum)
        {
            foreach (var member in @enum.Members)
            {
                yield return (new MemberNode { Name = member.Identifier.Text, Kind = "enumMember", Value = ValueOf(member.EqualsValue, file), At = SpanOf(member, file), TextOf = () => Canonical(member) }, null);
            }

            yield break;
        }

        var type = (TypeDeclarationSyntax)declaration;

        // A primary constructor; a positional record's parameters are also its properties.
        if (type.ParameterList is { } primary)
        {
            if (type is RecordDeclarationSyntax)
            {
                foreach (var parameter in primary.Parameters)
                {
                    yield return (new MemberNode { Name = parameter.Identifier.Text, Kind = "property", Type = parameter.Type is null ? null : TypeRef(parameter.Type), Attributes = AttributesOf(parameter.AttributeLists, file), At = SpanOf(parameter, file), TextOf = () => Canonical(parameter) }, null);
                }
            }

            yield return (new MemberNode { Name = type.Identifier.Text, Kind = "constructor", IsPrimary = true, Parameters = ParametersOf(primary, file), Access = type.Modifiers.Any(SyntaxKind.AbstractKeyword) ? "protected" : "public", At = SpanOf(primary, file), TextOf = () => Canonical(primary) }, null);
        }

        foreach (var member in type.Members.Where(x => razor is null || razor.IsWritten(x)))
        {
            switch (member)
            {
                case PropertyDeclarationSyntax property:
                    yield return (new MemberNode { Name = property.Identifier.Text, Kind = "property", Type = TypeRef(property.Type), IsStatic = property.Modifiers.Any(SyntaxKind.StaticKeyword), Attributes = AttributesOf(property.AttributeLists, file), Access = AccessOf(property.Modifiers, type), Modifiers = ModifiersOf(property.Modifiers), Value = ValueOf(property.Initializer, file), At = SpanOf(property, file), TextOf = () => razor?.TextOf(property) ?? Canonical(property) }, property.Initializer);
                    break;
                case FieldDeclarationSyntax field:
                    foreach (var variable in field.Declaration.Variables)
                    {
                        yield return (new MemberNode { Name = variable.Identifier.Text, Kind = "field", Type = TypeRef(field.Declaration.Type), IsStatic = field.Modifiers.Any(SyntaxKind.StaticKeyword), Attributes = AttributesOf(field.AttributeLists, file), Access = AccessOf(field.Modifiers, type), Modifiers = ModifiersOf(field.Modifiers), Value = ValueOf(variable.Initializer, file), At = SpanOf(field, file), TextOf = () => Canonical(field) }, variable.Initializer);
                    }

                    break;
                case MethodDeclarationSyntax method:
                    yield return (new MemberNode { Name = method.Identifier.Text, Kind = "method", Type = TypeRef(method.ReturnType), IsStatic = method.Modifiers.Any(SyntaxKind.StaticKeyword), Parameters = ParametersOf(method.ParameterList, file), Attributes = AttributesOf(method.AttributeLists, file), Access = AccessOf(method.Modifiers, type), Modifiers = ModifiersOf(method.Modifiers), TypeParameters = method.TypeParameterList?.Parameters.Select(x => x.Identifier.Text).ToList() ?? [], At = SpanOf(method, file), TextOf = () => Canonical(method) }, method);
                    break;
                case ConstructorDeclarationSyntax constructor:
                    yield return (new MemberNode { Name = constructor.Identifier.Text, Kind = "constructor", IsStatic = constructor.Modifiers.Any(SyntaxKind.StaticKeyword), Parameters = ParametersOf(constructor.ParameterList, file), Access = AccessOf(constructor.Modifiers, type), Modifiers = ModifiersOf(constructor.Modifiers), At = SpanOf(constructor, file), TextOf = () => Canonical(constructor) }, constructor);
                    break;
            }
        }
    }

    /// <summary>The access modifiers as written; without any, an interface member is public and any other private.</summary>
    private static string AccessOf(SyntaxTokenList modifiers, TypeDeclarationSyntax type) =>
        AccessWritten(modifiers) ?? (type is InterfaceDeclarationSyntax ? "public" : "private");

    private static string? AccessWritten(SyntaxTokenList modifiers)
    {
        var written = modifiers.Where(IsAccess).Select(x => x.Text).ToList();
        return written.Count > 0 ? string.Join(" ", written) : null;
    }

    private static bool IsAccess(SyntaxToken token) => token.Kind() is SyntaxKind.PublicKeyword or SyntaxKind.ProtectedKeyword or SyntaxKind.InternalKeyword or SyntaxKind.PrivateKeyword or SyntaxKind.FileKeyword;

    /// <summary>The modifiers besides access and <c>static</c>, which have their own facts.</summary>
    private static IReadOnlyList<string> ModifiersOf(SyntaxTokenList modifiers) =>
        modifiers.Where(x => !IsAccess(x) && !x.IsKind(SyntaxKind.StaticKeyword)).Select(x => x.Text).ToList();

    private static ValueNode? ValueOf(EqualsValueClauseSyntax? clause, string file) => clause is null ? null : ValueOf(clause.Value, file);

    /// <summary><c>new X(...)</c>; a target-typed <c>new(...)</c> names no type, so it isn't one.</summary>
    private static IReadOnlyList<AttributeNode> AttributesOf(SyntaxList<AttributeListSyntax> lists, string file) =>
        lists.SelectMany(x => x.Attributes).Select(attribute =>
        {
            var simple = attribute.Name switch
            {
                QualifiedNameSyntax qualified => qualified.Right,
                AliasQualifiedNameSyntax alias => alias.Name,
                SimpleNameSyntax own => own,
                _ => null
            };
            var name = simple?.Identifier.Text ?? attribute.Name.ToString();
            string Value(AttributeArgumentSyntax argument) => LiteralOf(argument.Expression, file, x => ValueOf(x, file))?.Value ?? argument.Expression.ToString();

            var all = attribute.ArgumentList?.Arguments ?? default;
            return new AttributeNode
            {
                Name = name.EndsWith("Attribute", StringComparison.Ordinal) ? name[..^"Attribute".Length] : name,
                TypeArguments = simple is GenericNameSyntax generic ? generic.TypeArgumentList.Arguments.Select(TypeRef).ToList() : [],
                Arguments = all.Where(x => x.NameEquals is null && x.NameColon is null).Select(Value).ToList(),
                Named = all.Where(x => x.NameEquals is not null).ToDictionary(x => x.NameEquals!.Name.Identifier.Text, Value),
                Values = all.OrderBy(x => x.NameEquals is not null || x.NameColon is not null)
                    .Select(x => AttributeValue(x.Expression, file, x.NameEquals?.Name.Identifier.Text ?? x.NameColon?.Name.Identifier.Text))
                    .ToList(),
                At = SpanOf(attribute, file)
            };
        }).ToList();

    /// <summary>A value written in a declaration, such as an initializer: as written, with the literal it is.</summary>
    internal static ValueNode ValueOf(ExpressionSyntax expression, string file) =>
        new() { Text = Whitespace.Replace(expression.ToString(), " ").Trim(), Literal = LiteralOf(expression, file, x => ValueOf(x, file)), At = SpanOf(expression, file) };

    /// <summary>
    /// The literal an expression is (<see cref="LiteralNode"/>), or null. An interpolated string's holes are valued by
    /// <paramref name="hole"/>: a body's with the types the code says they are, a declaration's as written.
    /// </summary>
    /// <param name="formatted">
    /// A hole written with a format or an alignment (<c>{total:F2}</c>, <c>{name,10}</c>), whose text isn't its value's; the
    /// other holes' by default.
    /// </param>
    internal static LiteralNode? LiteralOf(ExpressionSyntax expression, string file, Func<ExpressionSyntax, ValueNode> hole, Func<ExpressionSyntax, ValueNode>? formatted = null)
    {
        switch (expression)
        {
            case ParenthesizedExpressionSyntax parenthesized:
                return LiteralOf(parenthesized.Expression, file, hole, formatted);
            case MemberAccessExpressionSyntax { Expression: PredefinedTypeSyntax { Keyword.Text: "string" } or IdentifierNameSyntax { Identifier.Text: "String" }, Name.Identifier.Text: "Empty" }:
                return new LiteralNode { Kind = "string", Value = "" };
            case LiteralExpressionSyntax literal:
                return literal.Kind() switch
                {
                    SyntaxKind.StringLiteralExpression or SyntaxKind.Utf8StringLiteralExpression => new LiteralNode { Kind = "string", Value = literal.Token.ValueText },
                    SyntaxKind.CharacterLiteralExpression => new LiteralNode { Kind = "char", Value = literal.Token.ValueText },
                    SyntaxKind.NumericLiteralExpression => new LiteralNode { Kind = "number", Value = literal.Token.Text },
                    SyntaxKind.TrueLiteralExpression or SyntaxKind.FalseLiteralExpression => new LiteralNode { Kind = "boolean", Value = literal.Token.Text },
                    SyntaxKind.NullLiteralExpression => new LiteralNode { Kind = "null" },
                    _ => null
                };
            case PrefixUnaryExpressionSyntax { Operand: LiteralExpressionSyntax number } signed
                when number.IsKind(SyntaxKind.NumericLiteralExpression) && (signed.IsKind(SyntaxKind.UnaryMinusExpression) || signed.IsKind(SyntaxKind.UnaryPlusExpression)):
                return new LiteralNode { Kind = "number", Value = (signed.IsKind(SyntaxKind.UnaryMinusExpression) ? "-" : "") + number.Token.Text };
            case InterpolatedStringExpressionSyntax interpolated:
                return new LiteralNode
                {
                    Kind = "interpolated",
                    Parts = interpolated.Contents
                        .Select(x => x is InterpolationSyntax interpolation
                            ? (interpolation.AlignmentClause is null && interpolation.FormatClause is null ? hole : formatted ?? hole)(interpolation.Expression)
                            : TextPart((InterpolatedStringTextSyntax)x, file))
                        .ToList()
                };
            case TypeOfExpressionSyntax typeOf:
                return new LiteralNode { Kind = "type", Type = TypeRef(typeOf.Type) };
            case InvocationExpressionSyntax { Expression: IdentifierNameSyntax { Identifier.Text: "nameof" }, ArgumentList.Arguments: [{ Expression: var named }] }:
                return new LiteralNode
                {
                    Kind = "string",
                    Value = named switch
                    {
                        MemberAccessExpressionSyntax access => access.Name.Identifier.Text,
                        QualifiedNameSyntax qualified => qualified.Right.Identifier.Text,
                        SimpleNameSyntax simple => simple.Identifier.Text,
                        _ => named.ToString()
                    }
                };
            default:
                return null;
        }
    }

    private static ValueNode TextPart(InterpolatedStringTextSyntax text, string file) => new()
    {
        Text = Whitespace.Replace(text.ToString(), " ").Trim(),
        Literal = new LiteralNode { Kind = "string", Value = text.TextToken.ValueText },
        At = SpanOf(text, file)
    };

    private static AttributeValueNode AttributeValue(ExpressionSyntax expression, string file, string? name = null)
    {
        IEnumerable<ExpressionSyntax> items = expression switch
        {
            ArrayCreationExpressionSyntax { Initializer: { } initializer } => initializer.Expressions,
            ImplicitArrayCreationExpressionSyntax array => array.Initializer.Expressions,
            CollectionExpressionSyntax collection => collection.Elements.OfType<ExpressionElementSyntax>().Select(x => x.Expression),
            _ => []
        };
        return new AttributeValueNode
        {
            Name = name,
            Text = Whitespace.Replace(expression.ToString(), " ").Trim(),
            Literal = LiteralOf(expression, file, x => ValueOf(x, file))?.Value,
            Items = items.Select(x => AttributeValue(x, file)).ToList()
        };
    }

    private static IReadOnlyList<ParameterNode> ParametersOf(ParameterListSyntax list, string file) => ParametersOf(list.Parameters, file);

    /// <summary>Parameters as written: a lambda's untyped ones have no type.</summary>
    internal static IReadOnlyList<ParameterNode> ParametersOf(IEnumerable<ParameterSyntax> parameters, string file) =>
        parameters.Select(x => new ParameterNode
        {
            Name = x.Identifier.Text,
            Type = x.Type is null ? null : TypeRef(x.Type),
            Attributes = AttributesOf(x.AttributeLists, file),
            Default = x.Default is { } value ? Whitespace.Replace(value.Value.ToString(), " ").Trim() : null,
            At = SpanOf(x, file),
            TextOf = () => Canonical(x)
        }).ToList();

    internal static TypeRefNode TypeRef(TypeSyntax syntax)
    {
        var text = Whitespace.Replace(syntax.ToString(), " ").Replace(" <", "<").Replace("< ", "<").Replace(" >", ">");
        switch (syntax)
        {
            case NullableTypeSyntax nullable:
            {
                var inner = TypeRef(nullable.ElementType);
                return new TypeRefNode { Text = text, Name = inner.Name, QualifiedName = inner.QualifiedName, Arguments = inner.Arguments, IsArray = inner.IsArray, NullableMarked = true, IsPredefinedValueType = inner.IsPredefinedValueType, Language = CSharpLanguage.LanguageName };
            }
            case ArrayTypeSyntax array:
            {
                // Only the outermost rank, the first written: a jagged T[][] is an array of T[], as the Java and TypeScript readers nest them.
                var element = TypeRef(array.RankSpecifiers.Count > 1 ? SyntaxFactory.ArrayType(array.ElementType, SyntaxFactory.List(array.RankSpecifiers.Skip(1))) : array.ElementType);
                return new TypeRefNode { Text = text, Name = element.Name, QualifiedName = element.QualifiedName, Arguments = [element], IsArray = true, Language = CSharpLanguage.LanguageName };
            }
            case PredefinedTypeSyntax predefined:
                return new TypeRefNode { Text = text, Name = predefined.Keyword.Text, QualifiedName = predefined.Keyword.Text, IsPredefinedValueType = PredefinedValueTypes.Contains(predefined.Keyword.Text), Language = CSharpLanguage.LanguageName };
            case QualifiedNameSyntax qualified:
            {
                var right = TypeRef(qualified.Right);
                return new TypeRefNode { Text = text, Name = right.Name, QualifiedName = text.Split('<')[0], Arguments = right.Arguments, Language = CSharpLanguage.LanguageName };
            }
            case AliasQualifiedNameSyntax alias:
                return TypeRef(alias.Name);
            case GenericNameSyntax generic:
                return new TypeRefNode { Text = text, Name = generic.Identifier.Text, QualifiedName = generic.Identifier.Text, Arguments = generic.TypeArgumentList.Arguments.Select(TypeRef).ToList(), Language = CSharpLanguage.LanguageName };
            case IdentifierNameSyntax identifier:
                return new TypeRefNode { Text = text, Name = identifier.Identifier.Text, QualifiedName = identifier.Identifier.Text, Language = CSharpLanguage.LanguageName };
            default:
                return new TypeRefNode { Text = text, Name = text, QualifiedName = text, Language = CSharpLanguage.LanguageName };
        }
    }

    private static string Kind(BaseTypeDeclarationSyntax declaration) => declaration switch
    {
        RecordDeclarationSyntax record => record.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword) ? "struct" : "record",
        ClassDeclarationSyntax => "class",
        StructDeclarationSyntax => "struct",
        InterfaceDeclarationSyntax => "interface",
        EnumDeclarationSyntax => "enum",
        _ => "class"
    };

    private static string Namespace(SyntaxNode node)
    {
        var names = node.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().Select(x => x.Name.ToString()).Reverse();
        return string.Join(".", names);
    }

    /// <summary>A node's span starts at its first attribute (code-reader.md, Locations).</summary>
    /// <summary>
    /// A node's canonical text, exactly as Common.Drift reads it (code-reader.md, Canonical text): an interpolated string's
    /// parts keep their text as written, so <c>$"orders/{id}"</c> reads as it is.
    /// </summary>
    internal static string Canonical(SyntaxNode node)
    {
        SyntaxToken previous = default;
        var tokens = new List<(string, bool)>();
        foreach (var token in node.DescendantTokens().Where(x => !x.IsKind(SyntaxKind.EndOfFileToken) && x.Span.Length > 0))
        {
            var joined = previous.Kind() is SyntaxKind.InterpolatedStringStartToken or SyntaxKind.InterpolatedVerbatimStringStartToken
                             or SyntaxKind.InterpolatedSingleLineRawStringStartToken or SyntaxKind.InterpolatedMultiLineRawStringStartToken
                             or SyntaxKind.InterpolatedStringTextToken ||
                         token.Kind() is SyntaxKind.InterpolatedStringTextToken or SyntaxKind.InterpolatedStringEndToken or SyntaxKind.InterpolatedRawStringEndToken;
            tokens.Add((token.Text, joined));
            previous = token;
        }

        return CanonicalText.Of(tokens);
    }

    internal static Span SpanOf(SyntaxNode node, string file)
    {
        if (RazorFile.Of(node.SyntaxTree) is { } razor)
        {
            return razor.SpanOf(node);
        }

        var lines = node.GetLocation().GetLineSpan();
        return new Span(file, lines.StartLinePosition.Line + 1, lines.StartLinePosition.Character + 1, lines.EndLinePosition.Line + 1, lines.EndLinePosition.Character + 1);
    }

    /// <summary>A project file, with its nullable context and root namespace; both null when it doesn't say or won't parse.</summary>
    private sealed record Project(string Path, bool? Nullable, string? RootNamespace);

    /// <summary>The nearest project file in the file's folder or above governs it.</summary>
    private static Project? Governing(string file, IReadOnlyDictionary<string, Project> projects)
    {
        var folder = Path.GetDirectoryName(file)?.Replace('\\', '/') ?? "";
        while (true)
        {
            if (projects.TryGetValue(folder, out var project))
            {
                return project;
            }

            if (folder.Length == 0)
            {
                return null;
            }

            folder = Path.GetDirectoryName(folder)?.Replace('\\', '/') ?? "";
        }
    }

    /// <summary>Whether the project turns nullable reference types on, and its root namespace when it sets one.</summary>
    private static (bool Nullable, string? RootNamespace) ProjectSettings(string projectFile)
    {
        var elements = XDocument.Load(projectFile).Descendants().ToList();
        var nullable = elements.FirstOrDefault(x => x.Name.LocalName == "Nullable")?.Value.Trim();
        var rootNamespace = elements.FirstOrDefault(x => x.Name.LocalName == "RootNamespace")?.Value.Trim();
        return (string.Equals(nullable, "enable", StringComparison.OrdinalIgnoreCase), string.IsNullOrEmpty(rootNamespace) ? null : rootNamespace);
    }
}
