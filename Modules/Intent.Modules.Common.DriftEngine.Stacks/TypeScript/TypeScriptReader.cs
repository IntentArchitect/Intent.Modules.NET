using Intent.Modules.Common.DriftEngine.Code;

namespace Intent.Modules.Common.DriftEngine.Stacks.TypeScript;

/// <summary>
/// The TypeScript reader (code-reader.md, The TypeScript reader): TypeScript's own parser, syntax only. It never builds,
/// type-checks or runs the code. A module (a file) is a namespace named by its path without its extension; its classes,
/// interfaces, type aliases and enums are types; what it declares outside them, its functions and variables, are the members
/// of a <c>module</c> type named for the file (<c>client.ts</c>). A Vue single-file component is a <c>component</c> type
/// named for its file, whose members are its scripts' functions and variables, its props and its emits. A React component, a
/// capitalised function returning JSX or a class extending <c>Component</c>, is a <c>component</c> type of its name, and JSX
/// is its file's markup. JavaScript is read as TypeScript without its types.
/// </summary>
internal static class TypeScriptReader
{
    /// <summary>What one file declares, for its imports and bodies, which are read once every file's declarations are.</summary>
    internal sealed class Module
    {
        public required TypeScriptFile File { get; init; }

        public required string Namespace { get; init; }

        public required bool? Strict { get; init; }

        public required TsConfig? Config { get; init; }

        /// <summary>The <c>module</c> type, or a <c>.vue</c> file's component; null for a module that declares no functions or variables and runs nothing.</summary>
        public TypeNode? Owner { get; set; }

        /// <summary>The types it declares, by name.</summary>
        public Dictionary<string, TypeNode> Types { get; } = new(StringComparer.Ordinal);

        /// <summary>What its default export is, by name: a type's or a member's.</summary>
        public string? Default { get; set; }

        public List<(MemberNode Member, TypeScriptNode Function, TypeNode Owner)> Bodies { get; } = [];

        /// <summary>Each field's, property's and module variable's initializer, read again as a value with the bodies.</summary>
        public Dictionary<MemberNode, TypeScriptNode> Initializers { get; } = [];

        /// <summary>The statements that run when the module loads (a <c>&lt;script setup&gt;</c>'s, when its component is set up).</summary>
        public List<TypeScriptNode> TopLevel { get; } = [];

        /// <summary>What each local name it imports is: the import that binds it.</summary>
        public Dictionary<string, ImportNode> Bindings { get; } = new(StringComparer.Ordinal);

        /// <summary>The markup its JSX writes, in the order it is written: read with the code, after the file's node is made.</summary>
        public List<MarkupNode> Markup { get; } = [];

        public FileNode Node { get; set; } = null!;
    }

    /// <param name="root">The output folder.</param>
    /// <param name="files">In-scope paths relative to <paramref name="root"/>, with <c>/</c> separators.</param>
    /// <param name="graph">The graph it adds the files, types and members to, and its <see cref="TypeScriptSyntax"/>.</param>
    /// <param name="cancellationToken">
    /// Checked file by file and module by module; Node.js parses every file in one call, which runs to its end.
    /// </param>
    public static void Read(string root, IReadOnlyList<string> files, CodeGraph graph, INodeRunner node, CancellationToken cancellationToken)
    {
        // A Vue component's template is markup, read from the file as written.
        var templates = new Dictionary<string, IReadOnlyList<MarkupNode>>(StringComparer.Ordinal);
        var scripts = files.Select(file =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            var text = File.ReadAllText(Path.Combine(root, file));
            if (file.EndsWith(".vue", StringComparison.OrdinalIgnoreCase))
            {
                templates[file] = VueTemplate.Read(file, text);
                var (script, kind, _) = VueFile.Scripts(text);
                return (file, script, kind);
            }

            return (file, text, ScriptKind(file));
        }).ToList();

        var (parsed, failure) = TypeScriptWorker.Parse(scripts, node);
        if (failure is not null)
        {
            foreach (var file in files)
            {
                graph.Failures.Add((file, failure));
            }

            return;
        }

        var syntax = new TypeScriptSyntax();
        graph.Set(syntax);
        var configs = new Dictionary<string, object?>(StringComparer.Ordinal);
        var modules = new Dictionary<string, Module>(StringComparer.Ordinal);
        foreach (var file in parsed)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (file.Errors.Count > 0)
            {
                graph.Failures.Add((file.Path, string.Join("; ", file.Errors)));
            }

            if (file.Root is null)
            {
                continue;
            }

            syntax.Trees[file.Path] = file;
            var config = TsConfig.For(root, file.Path, configs);
            var module = new Module { File = file, Namespace = ModuleName(file.Path), Strict = config?.Strict, Config = config };
            modules[file.Path] = module;
            new Declarations(module, graph).Read();
        }

        // An import names what another module declares, so imports are read once every module's declarations are.
        foreach (var module in modules.Values)
        {
            module.Node = new FileNode
            {
                Path = module.File.Path,
                Language = TypeScriptLanguage.LanguageName,
                Namespaces = [module.Namespace],
                Imports = Imports(module, modules),
                // A Vue template's markup is read from its file; JSX's with the code, once every module is.
                Markup = templates.GetValueOrDefault(module.File.Path) ?? module.Markup
            };
            graph.Files.Add(module.Node);
        }

        // An initializer's names can be another module's, so initializers are read again once every module is, before the
        // bodies, whose values can be the fields they give. JSX is markup read with the code it is written in, so its values
        // are that code's.
        var built = new Dictionary<Module, HashSet<TypeScriptNode>>();
        foreach (var module in modules.Values)
        {
            built[module] = [];
            foreach (var owner in graph.Types.Where(x => x.Files.Contains(module.File.Path)))
            {
                foreach (var member in owner.Members)
                {
                    if (module.Initializers.TryGetValue(member, out var initializer))
                    {
                        var body = new TypeScriptBody(module, owner, modules, graph);
                        member.Value = body.Initializer(initializer);
                        module.Markup.AddRange(body.Markup(initializer, built[module]));
                    }
                }
            }
        }

        // What a body's calls reach, and what their arguments are, can depend on any module, so bodies are read last.
        foreach (var module in modules.Values)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach (var (member, function, owner) in module.Bodies)
            {
                var body = new TypeScriptBody(module, owner, modules, graph);
                member.Body = body.Function(function, member.Parameters);
                module.Markup.AddRange(body.Markup(function, built[module]));
            }

            if (module is { TopLevel.Count: > 0, Owner: { } top })
            {
                var body = new TypeScriptBody(module, top, modules, graph);
                module.Node.TopLevel = body.TopLevel(module.TopLevel);
                foreach (var statement in module.TopLevel)
                {
                    module.Markup.AddRange(body.Markup(statement, built[module]));
                }
            }

            module.Markup.Sort((a, b) => (a.At.Line, a.At.Column ?? 0).CompareTo((b.At.Line, b.At.Column ?? 0)));
        }
    }

    /// <summary>How TypeScript's parser reads a file: JSX in <c>.tsx</c> and <c>.jsx</c>, and JavaScript as JavaScript, which may also hold JSX.</summary>
    private static string ScriptKind(string path) => Path.GetExtension(path).ToLowerInvariant() switch
    {
        ".tsx" => "tsx",
        ".jsx" => "jsx",
        ".js" or ".mjs" or ".cjs" => "js",
        _ => "ts"
    };

    /// <summary>A module's name: its path without its extension (<c>src/api/client</c>; <c>.d.ts</c> as one extension).</summary>
    internal static string ModuleName(string path)
    {
        if (path.EndsWith(".d.ts", StringComparison.OrdinalIgnoreCase))
        {
            return path[..^5];
        }

        var dot = path.LastIndexOf('.');
        return dot > path.LastIndexOf('/') ? path[..dot] : path;
    }

    private static string FileName(string path) => path[(path.LastIndexOf('/') + 1)..];

    /// <summary>
    /// Each binding an import makes, as an import node: <c>import { A, B as C } from './m'</c> is two, <c>A</c> and <c>B</c>
    /// (as <c>C</c>), each <c>Target</c>ing the name in module <c>./m</c>'s namespace when it is in scope, else in the module
    /// as written. A default import names what the module's default export is; a namespace import (<c>* as m</c>) names the
    /// module, as a wildcard. A side-effect import (<c>import './styles.css'</c>) names the module.
    /// </summary>
    private static IReadOnlyList<ImportNode> Imports(Module module, IReadOnlyDictionary<string, Module> modules)
    {
        var imports = new List<ImportNode>();
        foreach (var statement in module.File.Root.List("statements"))
        {
            if (statement.Is("ImportEqualsDeclaration") && statement["moduleReference"] is { Kind: "ExternalModuleReference" } reference && reference["expression"]?.Value is { } required)
            {
                var (target, file) = Resolve(module, required, modules);
                Bind(new ImportNode
                {
                    Target = target?.Namespace ?? required, Alias = statement["name"]?.Value, Wildcard = true, Module = required, File = file,
                    TypeOnly = statement.Flags.Contains("isTypeOnly"), Text = statement.Canonical, At = statement.Span
                });
                continue;
            }

            if (!statement.Is("ImportDeclaration") || statement["moduleSpecifier"]?.Value is not { } specifier)
            {
                continue;
            }

            var (from, path) = Resolve(module, specifier, modules);
            var ns = from?.Namespace ?? specifier;
            var clause = statement["importClause"];
            var typeOnly = clause?.Flags.Contains("isTypeOnly") == true;
            ImportNode Import(string target, string? alias, bool wildcard, bool onlyTypes) => new()
            {
                Target = target, Alias = alias, Wildcard = wildcard, Module = specifier, File = path, TypeOnly = onlyTypes,
                Text = statement.Canonical, At = statement.Span
            };

            if (clause is null)
            {
                imports.Add(Import(ns, null, false, false));
                continue;
            }

            if (clause["name"]?.Value is { } local)
            {
                Bind(Import($"{ns}.{from?.Default ?? "default"}", local, false, typeOnly));
            }

            switch (clause["namedBindings"])
            {
                case { Kind: "NamespaceImport" } all when all["name"]?.Value is { } alias:
                    Bind(Import(ns, alias, true, typeOnly));
                    break;
                case { Kind: "NamedImports" } named:
                    foreach (var element in named.List("elements"))
                    {
                        var name = element["name"]!.Value!;
                        var imported = element["propertyName"]?.Value ?? name;
                        Bind(Import($"{ns}.{imported}", imported == name ? null : name, false, typeOnly || element.Flags.Contains("isTypeOnly")));
                    }

                    break;
            }
        }

        return imports;

        void Bind(ImportNode import)
        {
            imports.Add(import);
            module.Bindings[import.Alias ?? import.Target[(import.Target.LastIndexOf('.') + 1)..]] = import;
        }
    }

    /// <summary>
    /// The in-scope module an import's specifier names (code-reader.md, The TypeScript reader: resolution is declared): a
    /// relative path, or one <c>tsconfig.json</c>'s <c>paths</c> or <c>baseUrl</c> maps, with or without its extension, or a
    /// folder's <c>index</c>. A package's, or a file out of scope, is none.
    /// </summary>
    private static (Module? Module, string? File) Resolve(Module from, string specifier, IReadOnlyDictionary<string, Module> modules)
    {
        var bases = new List<string>();
        if (specifier.StartsWith("./", StringComparison.Ordinal) || specifier.StartsWith("../", StringComparison.Ordinal) || specifier is "." or "..")
        {
            bases.Add(TsConfig.Normalize(TsConfig.Join(TsConfig.Folder(from.File.Path), specifier)));
        }
        else if (from.Config is { } config)
        {
            foreach (var (pattern, targets) in config.Paths)
            {
                var star = pattern.IndexOf('*');
                if (star < 0 ? specifier == pattern : specifier.StartsWith(pattern[..star], StringComparison.Ordinal) && specifier.EndsWith(pattern[(star + 1)..], StringComparison.Ordinal) && specifier.Length >= pattern.Length - 1)
                {
                    var matched = star < 0 ? "" : specifier[star..(specifier.Length - (pattern.Length - star - 1))];
                    bases.AddRange(targets.Select(x => x.Replace("*", matched)));
                }
            }

            if (config.BaseUrl is { } baseUrl)
            {
                bases.Add(TsConfig.Normalize(TsConfig.Join(baseUrl, specifier)));
            }
        }

        foreach (var path in bases.SelectMany(Candidates))
        {
            if (modules.TryGetValue(path, out var module))
            {
                return (module, path);
            }
        }

        return (null, null);

        // As written; with each extension; an ESM import's .js written for its .ts; a folder's index.
        static IEnumerable<string> Candidates(string path)
        {
            yield return path;
            foreach (var extension in new[] { ".ts", ".tsx", ".d.ts", ".mts", ".cts", ".vue", ".js", ".jsx", ".mjs", ".cjs" })
            {
                yield return path + extension;
            }

            foreach (var (js, ts) in new[] { (".js", ".ts"), (".jsx", ".tsx"), (".mjs", ".mts"), (".cjs", ".cts") })
            {
                if (path.EndsWith(js, StringComparison.OrdinalIgnoreCase))
                {
                    yield return path[..^js.Length] + ts;
                }
            }

            foreach (var extension in new[] { ".ts", ".tsx", ".d.ts", ".js", ".jsx" })
            {
                yield return $"{path}/index{extension}";
            }
        }
    }

    /// <summary>A type as written (code-reader.md, Type references), its nullability as the file's <c>tsconfig.json</c> says.</summary>
    /// <param name="strict">Whether <c>strict</c> or <c>strictNullChecks</c> is on; null when the file has neither, so nullability is unknown.</param>
    internal static TypeRefNode TypeRef(TypeScriptNode type, bool? strict)
    {
        var text = type.Canonical;
        var stated = strict is null ? "unknown" : "false";
        switch (type.Kind)
        {
            case "ParenthesizedType":
                return TypeRef(type["type"]!, strict);
            case "TypeOperator" when type.Operator == "ReadonlyKeyword":
                // readonly string[] is a string[] the code doesn't change.
                return TypeRef(type["type"]!, strict);
            case "ArrayType":
            {
                var item = TypeRef(type["elementType"]!, strict);
                return new TypeRefNode { Text = text, Name = item.Name, QualifiedName = item.QualifiedName, Arguments = [item], IsArray = true, Nullable = stated, Language = TypeScriptLanguage.LanguageName };
            }
            case "TypeReference" or "ExpressionWithTypeArguments":
            {
                var name = type["typeName"] ?? type["expression"]!;
                var qualified = name.Canonical;
                return new TypeRefNode
                {
                    Text = text, Name = qualified[(qualified.LastIndexOf('.') + 1)..], QualifiedName = qualified,
                    Arguments = type.List("typeArguments").Select(x => TypeRef(x, strict)).ToList(), Nullable = stated, Language = TypeScriptLanguage.LanguageName
                };
            }
            case "UnionType":
            {
                var members = type.List("types").Select(x => TypeRef(x, strict)).ToList();
                return new TypeRefNode
                {
                    Text = text, Name = text, QualifiedName = text, Union = members,
                    Nullable = strict is null ? "unknown" : members.Any(x => x.Text is "null" or "undefined") ? "true" : "false",
                    Language = TypeScriptLanguage.LanguageName
                };
            }
            case "LiteralType":
                return new TypeRefNode { Text = text, Name = text, QualifiedName = text, Literal = TypeScriptBody.LiteralOf(type["literal"]!, null), Nullable = text == "null" ? "true" : stated, Language = TypeScriptLanguage.LanguageName };
            default:
                // A keyword (string, number, unknown, ...), or a type written in place: an object type, a function type, a
                // tuple, a mapped or conditional type. Each is as written.
                return new TypeRefNode { Text = text, Name = text, QualifiedName = text, Nullable = text == "undefined" ? "true" : stated, Language = TypeScriptLanguage.LanguageName };
        }
    }

    /// <summary>A name as written: an identifier's, a string or number's value, or a computed name's text (<c>[key]</c>).</summary>
    internal static string NameOf(TypeScriptNode name) => name.Kind is "Identifier" or "PrivateIdentifier" or "StringLiteral" or "NumericLiteral" or "NoSubstitutionTemplateLiteral" ? name.Value! : name.Canonical;

    /// <summary>A node with what TypeScript wraps around an expression without changing it unwrapped: parentheses, <c>!</c>, <c>satisfies</c>.</summary>
    internal static TypeScriptNode Unwrapped(TypeScriptNode expression) =>
        expression.Kind is "ParenthesizedExpression" or "NonNullExpression" or "SatisfiesExpression" ? Unwrapped(expression["expression"]!) : expression;

    /// <summary>What one module declares: its types, its module members or component, and what it runs.</summary>
    private sealed class Declarations(Module module, CodeGraph graph)
    {
        private readonly bool _vue = module.File.Path.EndsWith(".vue", StringComparison.OrdinalIgnoreCase);

        public void Read()
        {
            if (_vue)
            {
                var name = FileName(module.File.Path)[..^4];
                var statements = module.File.Root.List("statements");
                Add(module.Owner = new TypeNode
                {
                    Name = name,
                    Namespace = module.Namespace,
                    Kind = "component",
                    At = statements.Count > 0 ? module.File.SpanOf(statements[0].Start, statements[^1].End) : module.File.SpanOf(0, 0),
                    TextOf = () => module.File.Root.Canonical
                });
                module.Default = name;
            }

            foreach (var statement in module.File.Root.List("statements"))
            {
                Statement(statement, module.Namespace);
            }
        }

        private TypeNode Owner()
        {
            if (module.Owner is null)
            {
                var root = module.File.Root;
                Add(module.Owner = new TypeNode
                {
                    Name = FileName(module.File.Path),
                    Namespace = module.Namespace,
                    Kind = "module",
                    At = module.File.SpanOf(root.Start, root.End),
                    TextOf = () => root.Canonical
                });
            }

            return module.Owner;
        }

        private void Add(TypeNode type)
        {
            type.Files.Add(module.File.Path);
            graph.Types.Add(type);
            if (type.Kind is not ("module" or "component"))
            {
                module.Types.TryAdd(type.Name, type);
            }
        }

        /// <param name="ns">The namespace it declares types in: the module's, or a <c>namespace</c> block's inside it.</param>
        private void Statement(TypeScriptNode statement, string ns)
        {
            switch (statement.Kind)
            {
                case "ImportDeclaration" or "ImportEqualsDeclaration" or "ExportDeclaration" or "EmptyStatement":
                    return;
                case "ClassDeclaration":
                    Class(statement, ns);
                    return;
                case "InterfaceDeclaration":
                    Interface(statement, ns);
                    return;
                case "TypeAliasDeclaration":
                    Alias(statement, ns);
                    return;
                case "EnumDeclaration":
                    Enum(statement, ns);
                    return;
                case "ModuleDeclaration" when statement["body"] is { Kind: "ModuleBlock" } block && statement["name"]?.Value is { } name:
                    foreach (var inner in block.List("statements"))
                    {
                        Statement(inner, statement.Declared == "global" ? ns : $"{ns}.{name}");
                    }

                    return;
                case "FunctionDeclaration":
                    Function(statement);
                    return;
                case "VariableStatement":
                    Variables(statement);
                    return;
                case "ExportAssignment" when statement["expression"] is { Kind: "Identifier" } exported:
                    module.Default ??= exported.Value;
                    return;
                case "ExportAssignment" when Unwrapped(statement["expression"]!) is { Kind: "CallExpression" } wrapper:
                    // export default memo(Cart), connect(mapState)(Cart): what a wrapper is given, when it is a component's name.
                    if (wrapper.List("arguments").FirstOrDefault(x => x.Is("Identifier"))?.Value is { Length: > 0 } wrapped && char.IsUpper(wrapped[0]))
                    {
                        module.Default ??= wrapped;
                    }

                    Owner();
                    module.TopLevel.Add(statement);
                    return;
                case "ExpressionStatement" when _vue && statement["expression"] is { Kind: "CallExpression" } call && Macro(call, null):
                    return;
                default:
                    Owner();
                    module.TopLevel.Add(statement);
                    return;
            }
        }

        private static string Access(TypeScriptNode declaration) => declaration.Has("ExportKeyword") ? "public" : "private";

        private TypeNode Declare(TypeScriptNode declaration, string ns, string kind, TypeRefNode? aliases = null)
        {
            var type = new TypeNode
            {
                Name = declaration["name"]?.Value ?? "default",
                Namespace = ns,
                Kind = kind,
                Aliases = aliases,
                At = declaration.Span,
                IsAbstract = declaration.Has("AbstractKeyword"),
                Access = Access(declaration),
                TypeParameters = TypeParameters(declaration),
                TextOf = () => declaration.Canonical
            };
            type.Attributes.AddRange(Decorators(declaration));
            if (declaration.Has("DefaultKeyword"))
            {
                module.Default ??= type.Name;
            }

            Add(type);
            return type;
        }

        private void Class(TypeScriptNode declaration, string ns)
        {
            // A React class component: one that extends Component or PureComponent, whose props are its first type argument's.
            var react = _vue ? null : declaration.List("heritageClauses").SelectMany(x => x.List("types"))
                .FirstOrDefault(x => x["expression"]?.Canonical is "Component" or "PureComponent" or "React.Component" or "React.PureComponent");
            var type = Declare(declaration, ns, react is null ? "class" : "component");
            foreach (var clause in declaration.List("heritageClauses"))
            {
                type.BaseTypes.AddRange(clause.List("types").Select(x => TypeRef(x, module.Strict)));
            }

            if (react?.List("typeArguments").FirstOrDefault() is { } props)
            {
                type.Members.AddRange(Signatures(props).Select(Prop));
            }

            foreach (var member in declaration.List("members"))
            {
                switch (member.Kind)
                {
                    case "PropertyDeclaration":
                        type.Members.Add(Member(member, "property", Typed(member["type"], member), member.List("modifiers"), value: member["initializer"]));
                        break;
                    case "MethodDeclaration":
                        Body(Member(member, "method", Returns(member), member.List("modifiers"), Parameters(member), TypeParameters(member)), member, type);
                        break;
                    case "Constructor":
                        var parameters = Parameters(member);
                        Body(Member(member, "constructor", null, member.List("modifiers"), parameters, name: type.Name), member, type);

                        // A parameter with an access modifier or readonly is also a property (constructor(public readonly status: number)).
                        foreach (var (parameter, node) in parameters.Zip(member.List("parameters")))
                        {
                            if (node.List("modifiers").Any(x => x.Kind is "PublicKeyword" or "PrivateKeyword" or "ProtectedKeyword" or "ReadonlyKeyword"))
                            {
                                type.Members.Add(Member(node, "property", parameter.Type, node.List("modifiers")));
                            }
                        }

                        break;
                }
            }

            Accessors(type, declaration.List("members"));
        }

        private void Interface(TypeScriptNode declaration, string ns)
        {
            var type = Declare(declaration, ns, "interface");
            foreach (var clause in declaration.List("heritageClauses"))
            {
                type.BaseTypes.AddRange(clause.List("types").Select(x => TypeRef(x, module.Strict)));
            }

            Elements(type, declaration.List("members"));
        }

        /// <summary>
        /// A type alias, and the members its type states: an object type's, or each object type an intersection joins, whose
        /// named types are its bases (<c>type Paged&lt;T&gt; = Base &amp; { items: T[] }</c>).
        /// </summary>
        private void Alias(TypeScriptNode declaration, string ns)
        {
            var aliased = declaration["type"]!;
            var type = Declare(declaration, ns, "typeAlias", TypeRef(aliased, module.Strict));
            while (aliased.Is("ParenthesizedType"))
            {
                aliased = aliased["type"]!;
            }

            foreach (var part in aliased.Is("IntersectionType") ? aliased.List("types") : [aliased])
            {
                if (part.Is("TypeLiteral"))
                {
                    Elements(type, part.List("members"));
                }
                else if (aliased.Is("IntersectionType") && part.Is("TypeReference"))
                {
                    type.BaseTypes.Add(TypeRef(part, module.Strict));
                }
            }
        }

        private void Enum(TypeScriptNode declaration, string ns)
        {
            var type = Declare(declaration, ns, "enum");
            foreach (var member in declaration.List("members"))
            {
                type.Members.Add(new MemberNode
                {
                    Name = NameOf(member["name"]!),
                    Kind = "enumMember",
                    Value = member["initializer"] is { } value ? TypeScriptBody.Constant(value, module.Strict) : null,
                    At = member.Span,
                    TextOf = () => member.Canonical
                });
            }
        }

        /// <summary>An interface's or object type's members: its properties, methods and accessors. Index and call signatures aren't members.</summary>
        private void Elements(TypeNode type, IReadOnlyList<TypeScriptNode> members)
        {
            foreach (var member in members)
            {
                switch (member.Kind)
                {
                    case "PropertySignature":
                        type.Members.Add(Member(member, "property", Typed(member["type"], member), member.List("modifiers"), access: "public"));
                        break;
                    case "MethodSignature":
                        type.Members.Add(Member(member, "method", Returns(member), member.List("modifiers"), Parameters(member), TypeParameters(member), access: "public"));
                        break;
                }
            }

            Accessors(type, members);
        }

        /// <summary>
        /// A get and set accessor of one name are one property, its type the getter's, else the setter's parameter's; its body
        /// the getter's, else the setter's.
        /// </summary>
        private void Accessors(TypeNode type, IReadOnlyList<TypeScriptNode> members)
        {
            foreach (var accessors in members.Where(x => x.Kind is "GetAccessor" or "SetAccessor").GroupBy(x => NameOf(x["name"]!)))
            {
                var getter = accessors.FirstOrDefault(x => x.Is("GetAccessor"));
                var setter = accessors.FirstOrDefault(x => x.Is("SetAccessor"));
                var first = accessors.First();
                var written = getter?["type"] ?? setter?.List("parameters").FirstOrDefault()?["type"];
                var property = Member(first, "property", Typed(written, null), first.List("modifiers"),
                    extra: accessors.Select(x => x.Is("GetAccessor") ? "get" : "set").Distinct().ToList());
                type.Members.Add(property);
                if (new[] { getter, setter }.FirstOrDefault(x => x?["body"] is not null) is { } bodied)
                {
                    module.Bodies.Add((property, bodied, type));
                }
            }
        }

        /// <summary>A member's type, nullable when it is optional (<c>name?: string</c>) and the file's tsconfig.json says types state it.</summary>
        private TypeRefNode? Typed(TypeScriptNode? type, TypeScriptNode? member)
        {
            if (type is null)
            {
                return null;
            }

            var written = TypeRef(type, module.Strict);
            if (member?["questionToken"] is not null && module.Strict is not null)
            {
                written.Nullable = "true";
            }

            return written;
        }

        /// <summary>A function declared in the module: a method of the module, or of a component's script, or a React component.</summary>
        private void Function(TypeScriptNode declaration)
        {
            if (declaration["name"]?.Value is not { } name)
            {
                // export default function () { return <main />; } is a component named for its file.
                if (Component(ModuleName(FileName(module.File.Path)), declaration, declaration, "public", null))
                {
                    return;
                }

                // export default function () { }
                Owner();
                module.TopLevel.Add(declaration);
                return;
            }

            if (Component(name, declaration, declaration, Access(declaration), null))
            {
                return;
            }

            if (declaration.Has("DefaultKeyword"))
            {
                module.Default ??= name;
            }

            var owner = Owner();
            var member = Member(declaration, "method", Returns(declaration), declaration.List("modifiers"),
                Parameters(declaration), TypeParameters(declaration), access: Access(declaration), isStatic: true);
            Body(member, declaration, owner);
        }

        /// <summary>The function a variable holds that may render: an arrow function or function, also inside <c>memo(...)</c> or <c>forwardRef(...)</c>.</summary>
        private static TypeScriptNode? Rendering(TypeScriptNode? initializer) => initializer is null ? null : Unwrapped(initializer) switch
        {
            { Kind: "ArrowFunction" or "FunctionExpression" } function => function,
            { Kind: "CallExpression" } call when Unwrapped(call["expression"]!) is var callee
                                                 && (callee.Is("Identifier") ? callee.Value : callee["name"]?.Value) is "memo" or "forwardRef"
                => Rendering(call.List("arguments").FirstOrDefault()),
            _ => null
        };

        /// <summary>
        /// A React function component, when the function is one: named with a capital letter and returning JSX (React renders
        /// only a capitalised tag as a component). It is a <c>component</c> type of its name. Its members are its props (what its
        /// first parameter's type, or a <c>React.FC&lt;Props&gt;</c> variable's, states, the names it destructures from it, or
        /// those it reads from it), its state (each <c>useState</c> or <c>useReducer</c> value) and the functions it declares
        /// (also through <c>useCallback</c>), each private but its props. Its own body, which runs each time it renders, is a
        /// private constructor of its name, so its hooks, calls and JSX are its code. Not in a Vue component's script.
        /// </summary>
        /// <param name="declared">What declares it: the function, or the variable statement holding it.</param>
        /// <param name="variableType">A variable's type, which may give its props: <c>const Cart: React.FC&lt;CartProps&gt;</c>.</param>
        /// <returns>It is one.</returns>
        private bool Component(string name, TypeScriptNode function, TypeScriptNode declared, string access, TypeScriptNode? variableType)
        {
            if (_vue || name.Length == 0 || !char.IsUpper(name[0]) || !ReturnsJsx(function))
            {
                return false;
            }

            var type = new TypeNode { Name = name, Namespace = module.Namespace, Kind = "component", At = declared.Span, Access = access, TextOf = () => declared.Canonical };
            Add(type);
            if (declared.Has("DefaultKeyword"))
            {
                module.Default ??= name;
            }

            var props = function.List("parameters").FirstOrDefault();
            var stated = (props?["type"] ?? variableType?.List("typeArguments").FirstOrDefault()) is { } written ? Signatures(written).Select(Prop).ToList() : [];
            type.Members.AddRange(stated);
            if (props?["name"] is { Kind: "ObjectBindingPattern" } pattern)
            {
                // ({ title, onSave, compact = false }): each name it destructures is a prop, with its default.
                foreach (var element in pattern.List("elements").Where(x => x["dotDotDotToken"] is null))
                {
                    var prop = element["propertyName"] is { } renamed ? NameOf(renamed) : element["name"]?.Value;
                    if (prop is not null && type.Members.All(x => x.Name != prop))
                    {
                        type.Members.Add(Member(element, "property", null, [], name: prop, value: element["initializer"], access: "public"));
                    }
                }
            }
            else if (props?["name"] is { Kind: "Identifier" } named && stated.Count == 0)
            {
                // (props) => props.title: each name it reads from its props.
                foreach (var read in function.Descendants().Where(x => x.Is("PropertyAccessExpression") && x["expression"] is { Kind: "Identifier" } on && on.Value == named.Value)
                             .DistinctBy(x => x["name"]!.Value))
                {
                    type.Members.Add(Member(read, "property", null, [], name: read["name"]!.Value, access: "public"));
                }
            }

            var render = Member(declared, "constructor", null, [], Parameters(function), name: name, access: "private");
            type.Members.Add(render);
            if (function["body"] is not null)
            {
                module.Bodies.Add((render, function, type));
            }

            foreach (var statement in function["body"] is { Kind: "Block" } block ? block.List("statements") : [])
            {
                if (statement.Is("FunctionDeclaration") && statement["name"]?.Value is { } inner)
                {
                    Body(Member(statement, "method", Returns(statement), [], Parameters(statement), TypeParameters(statement), name: inner, access: "private"), statement, type);
                    continue;
                }

                foreach (var declaration in statement.Is("VariableStatement") ? statement["declarationList"]!.List("declarations") : [])
                {
                    var initializer = declaration["initializer"] is { } value ? Unwrapped(value) : null;
                    var hook = initializer is { Kind: "CallExpression" } call && Unwrapped(call["expression"]!) is var callee
                        ? (callee.Is("Identifier") ? callee.Value : callee["name"]?.Value)
                        : null;
                    if (declaration["name"] is { Kind: "ArrayBindingPattern" } state && hook is "useState" or "useReducer"
                        && state.List("elements").FirstOrDefault() is { Kind: "BindingElement" } first && first["name"] is { Kind: "Identifier" } held)
                    {
                        // const [count, setCount] = useState<number>(0): count is its state.
                        var stateType = initializer!.List("typeArguments").FirstOrDefault() is { } t && hook == "useState" ? TypeRef(t, module.Strict) : null;
                        type.Members.Add(Member(first, "property", stateType, [], name: held.Value, access: "private", at: declaration, extra: ["state"]));
                        continue;
                    }

                    var local = initializer is { Kind: "ArrowFunction" or "FunctionExpression" } ? initializer
                        : hook == "useCallback" ? initializer!.List("arguments").FirstOrDefault() is { } given ? Unwrapped(given) : null
                        : null;
                    if (declaration["name"] is { Kind: "Identifier" } functionName && local is { Kind: "ArrowFunction" or "FunctionExpression" })
                    {
                        Body(Member(declaration, "method", Returns(local), [], Parameters(local), TypeParameters(local), name: functionName.Value, access: "private",
                            extra: local.Has("AsyncKeyword") ? ["async"] : []), local, type);
                    }
                }
            }

            return true;
        }

        /// <summary>A function returns JSX: an arrow function's expression, or a <c>return</c> of its own, is an element, a fragment, or a condition giving one.</summary>
        private static bool ReturnsJsx(TypeScriptNode function)
        {
            return function["body"] switch
            {
                null => false,
                { Kind: "Block" } block => Returns(block).Any(x => x["expression"] is { } returned && IsJsx(returned)),
                var expression => IsJsx(expression)
            };

            static IEnumerable<TypeScriptNode> Returns(TypeScriptNode node) => node.Children
                .Where(x => x.Kind is not ("ArrowFunction" or "FunctionExpression" or "FunctionDeclaration" or "MethodDeclaration" or "ClassDeclaration" or "ClassExpression"))
                .SelectMany(x => x.Is("ReturnStatement") ? [x] : Returns(x));

            static bool IsJsx(TypeScriptNode expression)
            {
                expression = Unwrapped(expression);
                return expression.Kind switch
                {
                    "JsxElement" or "JsxSelfClosingElement" or "JsxFragment" => true,
                    "ConditionalExpression" => IsJsx(expression["whenTrue"]!) || IsJsx(expression["whenFalse"]!),
                    "BinaryExpression" when expression["operatorToken"]?.Kind is "AmpersandAmpersandToken" or "BarBarToken" or "QuestionQuestionToken"
                        => IsJsx(expression["left"]!) || IsJsx(expression["right"]!),
                    _ => false
                };
            }
        }

        /// <summary>
        /// The members a props type states: an object type's, an interface's or a type alias's the file declares by that name,
        /// or each part of an intersection's.
        /// </summary>
        private IEnumerable<TypeScriptNode> Signatures(TypeScriptNode type) => Stated(type).Where(x => x.Kind is "PropertySignature" or "MethodSignature");

        private IEnumerable<TypeScriptNode> Stated(TypeScriptNode type)
        {
            type = type.Is("ParenthesizedType") ? type["type"]! : type;
            switch (type.Kind)
            {
                case "TypeLiteral":
                    return type.List("members");
                case "IntersectionType":
                    return type.List("types").SelectMany(Stated);
                case "TypeReference" when type["typeName"]?.Value is { } name
                                          && module.File.Root.Descendants().FirstOrDefault(x => x.Kind is "InterfaceDeclaration" or "TypeAliasDeclaration" && x["name"]?.Value == name) is { } declared:
                    return declared.Is("InterfaceDeclaration") ? declared.List("members") : Stated(declared["type"]!);
                default:
                    return [];
            }
        }

        /// <summary>A prop its props type states, as a property: a callback prop (<c>onSave(): void</c>) too.</summary>
        private MemberNode Prop(TypeScriptNode signature) =>
            Member(signature, "property", signature.Is("PropertySignature") ? Typed(signature["type"], signature) : null, signature.List("modifiers"), access: "public");

        /// <summary>
        /// A module's variables: one holding an arrow function or function is a method; a <c>const</c> holding an object
        /// literal is an <c>object</c> type with its members, and a field; any other is a field, which runs when the module loads.
        /// In a component's script, Vue's compiler macros (<c>defineProps</c>, <c>defineEmits</c>, <c>defineModel</c>) give
        /// its props and emits instead.
        /// </summary>
        private void Variables(TypeScriptNode statement)
        {
            var list = statement["declarationList"]!;
            var runs = false;
            foreach (var declaration in list.List("declarations"))
            {
                var initializer = declaration["initializer"] is { } value ? Unwrapped(value) : null;
                if (declaration["name"] is not { Kind: "Identifier" } name)
                {
                    // const { a, b } = ...: a destructuring declares no member.
                    runs = true;
                    continue;
                }

                if (_vue && initializer is { Kind: "CallExpression" } call && Macro(call, declaration))
                {
                    continue;
                }

                if (Rendering(initializer) is { } rendering && Component(name.Value!, rendering, statement, Access(statement), declaration["type"]))
                {
                    continue;
                }

                var owner = Owner();
                if (initializer is { Kind: "ArrowFunction" or "FunctionExpression" } function)
                {
                    var member = Member(declaration, "method", Returns(function), statement.List("modifiers"),
                        Parameters(function), TypeParameters(function), name: name.Value, access: Access(statement), isStatic: true, at: statement, extra: function.Has("AsyncKeyword") ? ["async"] : []);
                    Body(member, function, owner);
                    continue;
                }

                owner.Members.Add(Member(declaration, "field", Typed(declaration["type"], null), statement.List("modifiers"), value: declaration["initializer"], access: Access(statement), isStatic: true, at: statement,
                    extra: list.Declared == "const" ? ["const"] : []));
                if (list.Declared == "const" && initializer is { Kind: "ObjectLiteralExpression" } literal)
                {
                    ObjectType(name.Value!, literal, statement);
                    continue;
                }

                runs |= initializer is not null;
            }

            if (runs)
            {
                module.TopLevel.Add(statement);
            }
        }

        /// <summary>
        /// A <c>const</c> holding an object literal, as a type of its own (<c>export const api = { getProducts() { ... } }</c>):
        /// its methods, and its functions' and values' properties.
        /// </summary>
        private void ObjectType(string name, TypeScriptNode literal, TypeScriptNode statement)
        {
            var type = new TypeNode { Name = name, Namespace = module.Namespace, Kind = "object", At = statement.Span, Access = Access(statement), TextOf = () => statement.Canonical };
            Add(type);
            foreach (var property in literal.List("properties"))
            {
                var value = property["initializer"] is { } initializer ? Unwrapped(initializer) : null;
                switch (property.Kind)
                {
                    case "MethodDeclaration":
                        Body(Member(property, "method", Returns(property), property.List("modifiers"), Parameters(property), TypeParameters(property), access: "public"), property, type);
                        break;
                    case "PropertyAssignment" when value is { Kind: "ArrowFunction" or "FunctionExpression" } function:
                        Body(Member(property, "method", Returns(function), function.List("modifiers"), Parameters(function), TypeParameters(function), access: "public",
                            extra: function.Has("AsyncKeyword") ? ["async"] : []), function, type);
                        break;
                    case "PropertyAssignment":
                        type.Members.Add(Member(property, "property", null, [], value: property["initializer"], access: "public"));
                        break;
                    case "ShorthandPropertyAssignment":
                        type.Members.Add(Member(property, "property", null, [], value: property["name"], access: "public"));
                        break;
                }
            }

            Accessors(type, literal.List("properties"));
        }

        /// <summary>
        /// Vue's compiler macros in a component's script: <c>defineProps</c> (also inside <c>withDefaults</c>) gives its props,
        /// as properties; <c>defineEmits</c> its emits, as events; <c>defineModel</c> a two-way prop.
        /// </summary>
        /// <returns>The call is one.</returns>
        private bool Macro(TypeScriptNode call, TypeScriptNode? declaration)
        {
            var component = module.Owner!;
            var (macro, defaults) = call["expression"]?.Value == "withDefaults" && call.List("arguments") is [{ Kind: "CallExpression" } inner, ..] && inner["expression"]?.Value == "defineProps"
                ? (inner, call.List("arguments").ElementAtOrDefault(1))
                : (call, null);
            var given = (defaults?.List("properties") ?? []).Where(x => x.Is("PropertyAssignment")).DistinctBy(x => NameOf(x["name"]!)).ToDictionary(x => NameOf(x["name"]!), x => x["initializer"]);
            switch (macro["expression"]?.Value)
            {
                case "defineProps":
                {
                    var props = new List<MemberNode>();
                    if (macro.List("typeArguments").FirstOrDefault() is { } declared)
                    {
                        var members = declared.Is("TypeLiteral") ? declared.List("members")
                            : declared.Is("TypeReference") && module.File.Root.List("statements").FirstOrDefault(x => x.Kind is "InterfaceDeclaration" or "TypeAliasDeclaration" && x["name"]?.Value == declared["typeName"]?.Value) is { } named
                                ? named.Is("InterfaceDeclaration") ? named.List("members") : named["type"]?.List("members") ?? []
                                : [];
                        props.AddRange(members.Where(x => x.Is("PropertySignature")).Select(x =>
                            Member(x, "property", Typed(x["type"], x), x.List("modifiers"), value: given.GetValueOrDefault(NameOf(x["name"]!)), access: "public")));
                    }
                    else if (macro.List("arguments").FirstOrDefault() is { } runtime)
                    {
                        // defineProps(['id']), or defineProps({ id: String, count: { type: Number, default: 1 } }).
                        props.AddRange(runtime.List("elements").Where(x => x.Is("StringLiteral")).Select(x => Member(x, "property", null, [], name: x.Value, access: "public")));
                        foreach (var prop in runtime.List("properties").Where(x => x.Is("PropertyAssignment")))
                        {
                            var options = prop["initializer"]!;
                            var type = options.Is("ObjectLiteralExpression") ? options.List("properties").FirstOrDefault(x => x["name"]?.Value == "type")?["initializer"] : options;
                            var value = options.Is("ObjectLiteralExpression") ? options.List("properties").FirstOrDefault(x => x["name"]?.Value == "default")?["initializer"] : null;
                            props.Add(Member(prop, "property", type is { Kind: "Identifier" } ? new TypeRefNode { Text = type.Value!, Name = type.Value!, QualifiedName = type.Value!, Language = TypeScriptLanguage.LanguageName } : null,
                                [], value: value, access: "public"));
                        }
                    }

                    component.Members.AddRange(props);
                    return true;
                }
                case "defineEmits":
                {
                    if (macro.List("typeArguments").FirstOrDefault() is { Kind: "TypeLiteral" } declared)
                    {
                        foreach (var member in declared.List("members"))
                        {
                            // (e: 'change', id: number): void, or change: [id: number].
                            if (member.Is("CallSignature") && member.List("parameters") is [var e, ..] && e["type"]?["literal"]?.Value is { } name)
                            {
                                component.Members.Add(Member(member, "event", null, [], ParametersOf(member.List("parameters").Skip(1)), name: name, access: "public"));
                            }
                            else if (member.Is("PropertySignature") && member["type"] is { Kind: "TupleType" } tuple)
                            {
                                var parameters = tuple.List("elements").Select((x, i) => new ParameterNode
                                {
                                    Name = x["name"]?.Value ?? $"arg{i}", Type = x["type"] is { } t ? TypeRef(t, module.Strict) : TypeRef(x, module.Strict), At = x.Span, TextOf = () => x.Canonical
                                }).ToList();
                                component.Members.Add(Member(member, "event", null, [], parameters, access: "public"));
                            }
                        }
                    }
                    else if (macro.List("arguments").FirstOrDefault() is { } runtime)
                    {
                        component.Members.AddRange(runtime.List("elements").Where(x => x.Is("StringLiteral")).Select(x => Member(x, "event", null, [], name: x.Value, access: "public")));
                        component.Members.AddRange(runtime.List("properties").Where(x => x.Kind is "PropertyAssignment" or "MethodDeclaration").Select(x => Member(x, "event", null, [], access: "public")));
                    }

                    return true;
                }
                case "defineModel":
                {
                    var name = macro.List("arguments").FirstOrDefault() is { Kind: "StringLiteral" } named ? named.Value! : "modelValue";
                    var type = macro.List("typeArguments").FirstOrDefault() is { } t ? TypeRef(t, module.Strict) : null;
                    component.Members.Add(Member(declaration ?? macro, "property", type, [], name: name, access: "public", extra: ["model"]));
                    return true;
                }
                case "defineExpose" or "defineOptions" or "defineSlots":
                    return true;
                default:
                    return false;
            }
        }

        private void Body(MemberNode member, TypeScriptNode function, TypeNode owner)
        {
            owner.Members.Add(member);
            if (function["body"] is not null)
            {
                module.Bodies.Add((member, function, owner));
            }
        }

        /// <param name="value">Its initializer, read as a value once every module is (see <see cref="TypeScriptBody.Constant"/>).</param>
        /// <param name="access">Its access when no modifier says: a class member's is <c>public</c>.</param>
        /// <param name="at">The node it is declared by, when it isn't <paramref name="declaration"/>: a variable's statement.</param>
        /// <param name="extra">Modifiers TypeScript doesn't write as one: <c>const</c>, <c>async</c> on an arrow function, <c>get</c> and <c>set</c>.</param>
        private MemberNode Member(TypeScriptNode declaration, string kind, TypeRefNode? type, IReadOnlyList<TypeScriptNode> modifiers, IReadOnlyList<ParameterNode>? parameters = null,
            IReadOnlyList<string>? typeParameters = null, TypeScriptNode? value = null, string? name = null, string access = "public", bool isStatic = false, TypeScriptNode? at = null, IReadOnlyList<string>? extra = null)
        {
            var written = modifiers.Where(x => !x.Is("Decorator")).Select(x => x.Source).ToList();
            var declared = at ?? declaration;
            var memberName = name ?? NameOf(declaration["name"]!);
            var member = new MemberNode
            {
                Name = memberName,
                Kind = kind,
                Type = type,
                IsStatic = isStatic || written.Contains("static"),
                Parameters = parameters ?? [],
                Attributes = Decorators(declaration),
                Access = memberName.StartsWith('#') ? "private" : written.FirstOrDefault(x => x is "public" or "private" or "protected") ?? access,
                Modifiers = written.Where(x => x is not ("public" or "private" or "protected" or "static" or "export" or "default")).Concat(extra ?? []).ToList(),
                Value = value is null ? null : TypeScriptBody.Constant(value, module.Strict),
                TypeParameters = typeParameters ?? [],
                At = declared.Span,
                TextOf = () => declared.Canonical
            };
            if (value is not null && kind is "field" or "property")
            {
                module.Initializers[member] = value;
            }

            return member;
        }

        /// <summary>A function's return type as written; null when it leaves TypeScript to infer it.</summary>
        private TypeRefNode? Returns(TypeScriptNode function) => function["type"] is { } type ? TypeRef(type, module.Strict) : null;

        private IReadOnlyList<ParameterNode> Parameters(TypeScriptNode function) => ParametersOf(function.List("parameters"));

        private IReadOnlyList<ParameterNode> ParametersOf(IEnumerable<TypeScriptNode> parameters) => parameters.Select(x => Parameter(x, module.Strict)).ToList();

        private static IReadOnlyList<string> TypeParameters(TypeScriptNode declaration) => declaration.List("typeParameters").Select(x => x["name"]!.Value!).ToList();

        private static IReadOnlyList<AttributeNode> Decorators(TypeScriptNode declaration) =>
            declaration.List("modifiers").Where(x => x.Is("Decorator")).Select(Decorator).ToList();
    }

    /// <summary>A parameter: its name (a destructuring pattern's as written), type, default and decorators.</summary>
    internal static ParameterNode Parameter(TypeScriptNode parameter, bool? strict)
    {
        var type = parameter["type"] is { } written ? TypeRef(written, strict) : null;
        if (type is not null && parameter["questionToken"] is not null && strict is not null)
        {
            type.Nullable = "true";
        }

        // ...items: T[] takes any number.
        return new ParameterNode
        {
            Name = parameter["name"] is { Kind: "Identifier" } name ? name.Value! : parameter["name"]!.Canonical,
            Type = type,
            Attributes = parameter.List("modifiers").Where(x => x.Is("Decorator")).Select(Decorator).ToList(),
            Default = parameter["initializer"]?.Canonical,
            IsOptional = parameter["questionToken"] is not null || parameter["dotDotDotToken"] is not null,
            At = parameter.Span,
            TextOf = () => parameter.Canonical
        };
    }

    /// <summary>
    /// A decorator as an attribute: <c>@Input()</c> is <c>Input</c>; <c>@Component({ selector: 'app-cart' })</c> is
    /// <c>Component</c>, an object literal's properties its named arguments.
    /// </summary>
    internal static AttributeNode Decorator(TypeScriptNode decorator)
    {
        var expression = decorator["expression"]!;
        var (callee, arguments) = expression.Is("CallExpression") ? (expression["expression"]!, expression.List("arguments")) : (expression, []);
        var values = new List<AttributeValueNode>();
        var named = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var argument in arguments)
        {
            if (argument.Is("ObjectLiteralExpression"))
            {
                foreach (var property in argument.List("properties").Where(x => x.Is("PropertyAssignment")))
                {
                    var value = AttributeValue(property["initializer"]!, NameOf(property["name"]!));
                    named[value.Name!] = value.Literal ?? value.Text;
                    values.Add(value);
                }
            }
            else
            {
                values.Add(AttributeValue(argument, null));
            }
        }

        return new AttributeNode
        {
            Name = callee.Is("PropertyAccessExpression") ? callee["name"]!.Value! : callee.Value ?? callee.Canonical,
            Arguments = values.Where(x => x.Name is null).Select(x => x.Literal ?? x.Text).ToList(),
            Named = named,
            Values = values,
            At = decorator.Span
        };
    }

    private static AttributeValueNode AttributeValue(TypeScriptNode value, string? name) => new()
    {
        Name = name,
        Text = value.Canonical,
        Literal = TypeScriptBody.LiteralOf(value, null)?.Value,
        Items = value.Is("ArrayLiteralExpression") ? value.List("elements").Select(x => AttributeValue(x, null)).ToList() : []
    };
}
