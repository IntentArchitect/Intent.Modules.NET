using System.Text.RegularExpressions;
using System.Xml.Linq;
using Intent.Modules.Common.DriftEngine.Code;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intent.Modules.Common.DriftEngine.Stacks.CSharp;

/// <summary>
/// The C# reader (code-reader.md): parses each file with Roslyn, syntax only. It never builds, loads or runs the code
/// (decision 3), so names aren't resolved; types compare by name.
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
    public static void Read(string root, IReadOnlyList<string> files, CodeGraph graph)
    {
        var syntax = new CSharpSyntax(graph);
        graph.Set(syntax);
        var projects = new Dictionary<string, (string Path, bool? Nullable)>(StringComparer.Ordinal);
        foreach (var project in files.Where(x => x.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase)))
        {
            bool? nullable = null;
            try
            {
                nullable = NullableEnabled(Path.Combine(root, project));
            }
            catch (System.Xml.XmlException e)
            {
                graph.Failures.Add((project, e.Message));
            }

            projects[Path.GetDirectoryName(project)!.Replace('\\', '/')] = (project, nullable);
        }

        var parts = new Dictionary<string, TypeNode>(StringComparer.Ordinal);
        var referenceTypesNullable = new List<(TypeRefNode Ref, bool Context)>();
        var bodies = new List<(MemberNode Member, SyntaxNode Body, TypeNode Owner, string File)>();

        foreach (var file in files.Where(x => x.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)))
        {
            var text = File.ReadAllText(Path.Combine(root, file));
            var tree = CSharpSyntaxTree.ParseText(text, path: file);
            var governing = Governing(file, projects);
            if (governing is { Nullable: null })
            {
                graph.UngovernedFiles[file] = governing.Value.Path;
            }

            var nullableContext = text.Contains("#nullable enable", StringComparison.Ordinal) || governing?.Nullable == true;
            if (nullableContext)
            {
                syntax.NullableContexts.Add(file);
            }

            syntax.Trees[file] = tree;
            graph.Files.Add(FileOf(tree.GetRoot(), file, governing?.Path));
            foreach (var declaration in tree.GetRoot().DescendantNodes().OfType<BaseTypeDeclarationSyntax>())
            {
                var kind = Kind(declaration);
                var ns = Namespace(declaration);
                var key = $"{ns}.{declaration.Identifier.Text}";
                if (!parts.TryGetValue(key, out var type))
                {
                    type = new TypeNode { Name = declaration.Identifier.Text, Namespace = ns, Kind = kind, At = SpanOf(declaration, file) };
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
                foreach (var (member, body) in Members(declaration, file))
                {
                    type.Members.Add(member);
                    if (body is not null)
                    {
                        bodies.Add((member, body, type, file));
                    }

                    foreach (var typeRef in member.Parameters.Select(x => x.Type).Prepend(member.Type).OfType<TypeRefNode>())
                    {
                        referenceTypesNullable.Add((typeRef, nullableContext));
                    }
                }
            }
        }

        // What a body's calls are made on, and what their arguments are, can depend on any type, so bodies are read last.
        foreach (var (member, body, owner, file) in bodies)
        {
            member.Calls = new CSharpBody(body, owner, file, graph).Calls();
        }

        // Nullability needs to know which names are value types, which the whole graph tells.
        var valueTypes = ValueTypes(graph);
        foreach (var (typeRef, context) in referenceTypesNullable)
        {
            typeRef.Nullable = Nullability(typeRef, context, valueTypes);
        }
    }

    internal static TypeRefNode TypeOf(TypeSyntax syntax, string file, CSharpSyntax csharp, CodeGraph graph)
    {
        var type = TypeRef(syntax);
        type.Nullable = Nullability(type, csharp.NullableContexts.Contains(file), ValueTypes(graph));
        return type;
    }

    private static HashSet<string> ValueTypes(CodeGraph graph) => graph.Types.Where(x => x.Kind is "struct" or "enum").Select(x => x.Name).ToHashSet(StringComparer.Ordinal);

    private static string Nullability(TypeRefNode type, bool context, HashSet<string> valueTypes) =>
        type.NullableMarked || type.Name == "Nullable"
            ? "true"
            : !type.IsArray && (type.IsPredefinedValueType || LibraryValueTypes.Contains(type.Name) || valueTypes.Contains(type.Name))
                ? "false"
                : context ? "false" : "unknown";

    /// <summary>
    /// A file's declared namespaces and its <c>using</c> directives. A fully qualified name used inline isn't an import,
    /// so it isn't seen.
    /// </summary>
    private static FileNode FileOf(SyntaxNode root, string file, string? project) => new()
    {
        Path = file,
        Language = CSharpLanguage.LanguageName,
        Project = project,
        Namespaces = root.DescendantNodes().OfType<BaseNamespaceDeclarationSyntax>().Select(x => Namespace(x.Name)).Distinct().ToList(),
        Imports = root.DescendantNodes().OfType<UsingDirectiveSyntax>()
            .Where(x => x.NamespaceOrType is not null)
            .Select(x => new ImportNode
            {
                Target = Whitespace.Replace(x.NamespaceOrType!.ToString(), "").Replace("global::", ""),
                Text = Whitespace.Replace(x.ToString(), " ").Trim(),
                Alias = x.Alias?.Name.Identifier.Text,
                Static = x.StaticKeyword.IsKind(SyntaxKind.StaticKeyword),
                Global = x.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword),
                At = SpanOf(x, file)
            })
            .ToList()
    };

    /// <summary>A namespace declaration's qualified name, including the namespaces it is nested in.</summary>
    private static string Namespace(NameSyntax name) =>
        string.Join(".", name.AncestorsAndSelf().OfType<BaseNamespaceDeclarationSyntax>().Reverse().Select(x => x.Name.ToString()));

    /// <summary>The members a type declares, each with its declaration when it has a body to read.</summary>
    private static IEnumerable<(MemberNode Member, SyntaxNode? Body)> Members(BaseTypeDeclarationSyntax declaration, string file)
    {
        if (declaration is EnumDeclarationSyntax @enum)
        {
            foreach (var member in @enum.Members)
            {
                yield return (new MemberNode { Name = member.Identifier.Text, Kind = "enumMember", Value = ValueOf(member.EqualsValue, file), At = SpanOf(member, file) }, null);
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
                    yield return (new MemberNode { Name = parameter.Identifier.Text, Kind = "property", Type = parameter.Type is null ? null : TypeRef(parameter.Type), Attributes = AttributesOf(parameter.AttributeLists, file), At = SpanOf(parameter, file) }, null);
                }
            }

            yield return (new MemberNode { Name = type.Identifier.Text, Kind = "constructor", IsPrimary = true, Parameters = ParametersOf(primary, file), Access = type.Modifiers.Any(SyntaxKind.AbstractKeyword) ? "protected" : "public", At = SpanOf(primary, file) }, null);
        }

        foreach (var member in type.Members)
        {
            switch (member)
            {
                case PropertyDeclarationSyntax property:
                    yield return (new MemberNode { Name = property.Identifier.Text, Kind = "property", Type = TypeRef(property.Type), IsStatic = property.Modifiers.Any(SyntaxKind.StaticKeyword), Attributes = AttributesOf(property.AttributeLists, file), Access = AccessOf(property.Modifiers, type), Modifiers = ModifiersOf(property.Modifiers), Value = ValueOf(property.Initializer, file), At = SpanOf(property, file) }, null);
                    break;
                case FieldDeclarationSyntax field:
                    foreach (var variable in field.Declaration.Variables)
                    {
                        yield return (new MemberNode { Name = variable.Identifier.Text, Kind = "field", Type = TypeRef(field.Declaration.Type), IsStatic = field.Modifiers.Any(SyntaxKind.StaticKeyword), Attributes = AttributesOf(field.AttributeLists, file), Access = AccessOf(field.Modifiers, type), Modifiers = ModifiersOf(field.Modifiers), Value = ValueOf(variable.Initializer, file), At = SpanOf(field, file) }, null);
                    }

                    break;
                case MethodDeclarationSyntax method:
                    yield return (new MemberNode { Name = method.Identifier.Text, Kind = "method", Type = TypeRef(method.ReturnType), IsStatic = method.Modifiers.Any(SyntaxKind.StaticKeyword), Parameters = ParametersOf(method.ParameterList, file), Attributes = AttributesOf(method.AttributeLists, file), Access = AccessOf(method.Modifiers, type), Modifiers = ModifiersOf(method.Modifiers), Creates = CreatesIn(method), At = SpanOf(method, file) }, method);
                    break;
                case ConstructorDeclarationSyntax constructor:
                    yield return (new MemberNode { Name = constructor.Identifier.Text, Kind = "constructor", IsStatic = constructor.Modifiers.Any(SyntaxKind.StaticKeyword), Parameters = ParametersOf(constructor.ParameterList, file), Access = AccessOf(constructor.Modifiers, type), Modifiers = ModifiersOf(constructor.Modifiers), Creates = CreatesIn(constructor), At = SpanOf(constructor, file) }, constructor);
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

    private static ValueNode? ValueOf(EqualsValueClauseSyntax? clause, string file) =>
        clause is null ? null : new ValueNode { Text = Whitespace.Replace(clause.Value.ToString(), " ").Trim(), At = SpanOf(clause.Value, file) };

    /// <summary><c>new X(...)</c>; a target-typed <c>new(...)</c> names no type, so it isn't one.</summary>
    private static IReadOnlyList<TypeRefNode> CreatesIn(SyntaxNode member) =>
        member.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().Select(x => TypeRef(x.Type)).ToList();

    private static IReadOnlyList<AttributeNode> AttributesOf(SyntaxList<AttributeListSyntax> lists, string file) =>
        lists.SelectMany(x => x.Attributes).Select(attribute =>
        {
            var name = attribute.Name switch
            {
                QualifiedNameSyntax qualified => qualified.Right.Identifier.Text,
                AliasQualifiedNameSyntax alias => alias.Name.Identifier.Text,
                SimpleNameSyntax simple => simple.Identifier.Text,
                _ => attribute.Name.ToString()
            };
            static string Value(AttributeArgumentSyntax argument) =>
                argument.Expression is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.StringLiteralExpression) ? literal.Token.ValueText : argument.Expression.ToString();

            var all = attribute.ArgumentList?.Arguments ?? default;
            return new AttributeNode
            {
                Name = name.EndsWith("Attribute", StringComparison.Ordinal) ? name[..^"Attribute".Length] : name,
                Arguments = all.Where(x => x.NameEquals is null && x.NameColon is null).Select(Value).ToList(),
                Named = all.Where(x => x.NameEquals is not null).ToDictionary(x => x.NameEquals!.Name.Identifier.Text, Value),
                Values = all.OrderBy(x => x.NameEquals is not null || x.NameColon is not null)
                    .Select(x => AttributeValue(x.Expression, x.NameEquals?.Name.Identifier.Text ?? x.NameColon?.Name.Identifier.Text))
                    .ToList(),
                At = SpanOf(attribute, file)
            };
        }).ToList();

    private static AttributeValueNode AttributeValue(ExpressionSyntax expression, string? name = null)
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
            Literal = expression is LiteralExpressionSyntax literal && !literal.IsKind(SyntaxKind.NullLiteralExpression)
                ? literal.IsKind(SyntaxKind.StringLiteralExpression) ? literal.Token.ValueText : literal.Token.Text
                : null,
            Items = items.Select(x => AttributeValue(x)).ToList()
        };
    }

    private static IReadOnlyList<ParameterNode> ParametersOf(ParameterListSyntax list, string file) =>
        list.Parameters.Select(x => new ParameterNode
        {
            Name = x.Identifier.Text,
            Type = x.Type is null ? null : TypeRef(x.Type),
            Attributes = AttributesOf(x.AttributeLists, file),
            Default = x.Default is { } value ? Whitespace.Replace(value.Value.ToString(), " ").Trim() : null,
            At = SpanOf(x, file)
        }).ToList();

    internal static TypeRefNode TypeRef(TypeSyntax syntax)
    {
        var text = Whitespace.Replace(syntax.ToString(), " ").Replace(" <", "<").Replace("< ", "<").Replace(" >", ">");
        switch (syntax)
        {
            case NullableTypeSyntax nullable:
            {
                var inner = TypeRef(nullable.ElementType);
                return new TypeRefNode { Text = text, Name = inner.Name, QualifiedName = inner.QualifiedName, Arguments = inner.Arguments, IsArray = inner.IsArray, NullableMarked = true, IsPredefinedValueType = inner.IsPredefinedValueType };
            }
            case ArrayTypeSyntax array:
            {
                var element = TypeRef(array.ElementType);
                return new TypeRefNode { Text = text, Name = element.Name, QualifiedName = element.QualifiedName, Arguments = [element], IsArray = true };
            }
            case PredefinedTypeSyntax predefined:
                return new TypeRefNode { Text = text, Name = predefined.Keyword.Text, QualifiedName = predefined.Keyword.Text, IsPredefinedValueType = PredefinedValueTypes.Contains(predefined.Keyword.Text) };
            case QualifiedNameSyntax qualified:
            {
                var right = TypeRef(qualified.Right);
                return new TypeRefNode { Text = text, Name = right.Name, QualifiedName = text.Split('<')[0], Arguments = right.Arguments };
            }
            case AliasQualifiedNameSyntax alias:
                return TypeRef(alias.Name);
            case GenericNameSyntax generic:
                return new TypeRefNode { Text = text, Name = generic.Identifier.Text, QualifiedName = generic.Identifier.Text, Arguments = generic.TypeArgumentList.Arguments.Select(TypeRef).ToList() };
            case IdentifierNameSyntax identifier:
                return new TypeRefNode { Text = text, Name = identifier.Identifier.Text, QualifiedName = identifier.Identifier.Text };
            default:
                return new TypeRefNode { Text = text, Name = text, QualifiedName = text };
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
    internal static Span SpanOf(SyntaxNode node, string file)
    {
        var lines = node.GetLocation().GetLineSpan();
        return new Span(file, lines.StartLinePosition.Line + 1, lines.StartLinePosition.Character + 1, lines.EndLinePosition.Line + 1, lines.EndLinePosition.Character + 1);
    }

    /// <summary>The nearest project file in the file's folder or above governs it.</summary>
    private static (string Path, bool? Nullable)? Governing(string file, IReadOnlyDictionary<string, (string Path, bool? Nullable)> projects)
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

    private static bool NullableEnabled(string projectFile)
    {
        var nullable = XDocument.Load(projectFile).Descendants().FirstOrDefault(x => x.Name.LocalName == "Nullable")?.Value.Trim();
        return string.Equals(nullable, "enable", StringComparison.OrdinalIgnoreCase);
    }
}
