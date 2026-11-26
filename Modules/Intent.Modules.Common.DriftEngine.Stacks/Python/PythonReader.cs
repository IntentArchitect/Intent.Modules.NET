using System.Text.RegularExpressions;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Stacks.Python.Syntax;

namespace Intent.Modules.Common.DriftEngine.Stacks.Python;

/// <summary>
/// The Python reader (code-reader.md): parses each file with the engine's own parser, syntax only. It never runs or imports
/// the code (decision 3), so names are resolved only through what the code declares and imports; types compare by name.
/// A file's namespace is its module's dotted name, its classes are its types, and what it declares outside them (its
/// functions and variables) is a <c>module</c> type of its own, as TypeScript's are.
/// </summary>
internal static class PythonReader
{
    // The bases that make a class an enum, from the enum module.
    private static readonly HashSet<string> EnumBases = ["Enum", "IntEnum", "StrEnum", "Flag", "IntFlag", "ReprEnum"];

    // What a class derives from only to be a kind of type, or generic: its kind and type parameters say it, not its bases.
    private static readonly HashSet<string> MarkerBases = ["object", "Generic", "Protocol", "ABC"];

    private static readonly Regex Whitespace = new(@"\s+", RegexOptions.CultureInvariant);

    /// <param name="root">The output folder.</param>
    /// <param name="files">In-scope paths relative to <paramref name="root"/>, with <c>/</c> separators.</param>
    /// <param name="graph">The graph it adds the files, types and members to, and its <see cref="PythonSyntax"/>.</param>
    /// <param name="cancellationToken">Checked file by file, and body by body.</param>
    public static void Read(string root, IReadOnlyList<string> files, CodeGraph graph, CancellationToken cancellationToken)
    {
        var syntax = new PythonSyntax();
        graph.Set(syntax);
        var parsed = new List<PythonFile>();
        foreach (var path in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var source = File.ReadAllText(Path.Combine(root, path));
            var (tree, issues, tokens) = PythonParser.Parse(source);
            if (issues.Count > 0)
            {
                graph.Failures.Add((path, string.Join("; ", issues)));
            }

            syntax.Trees[path] = tree;
            syntax.Sources[path] = source;
            parsed.Add(new PythonFile(path, source, tree, tokens));
        }

        var project = new PythonProject(graph, ModuleNames(root, parsed));

        // What each module's imports bind, and its Annotated aliases (ProjectServiceDep = Annotated[ProjectService,
        // Depends(...)]), which stand for their type wherever they annotate, are known before any module's declarations are read.
        foreach (var file in parsed)
        {
            Bind(file);
            foreach (var statement in file.Tree.Body)
            {
                if (statement is PythonAssign { Targets: [PythonName name], Value: PythonSubscript value } && PythonTypes.TypeRef(value.Value).Name == "Annotated")
                {
                    project.Aliases.TryAdd($"{file.Module}.{name.Id}", (value, file));
                }
            }
        }

        foreach (var file in parsed)
        {
            syntax.Modules[file.Path] = file.Module;
            new Declarations(file, project).Read();
        }

        // An import names another module's file, so imports are read once every module's name is known.
        foreach (var file in parsed)
        {
            graph.Files.Add(new FileNode
            {
                Path = file.Path,
                Language = PythonLanguage.LanguageName,
                Namespaces = [file.Module],
                Imports = Imports(file, project)
            });
        }

        // What a body's calls are made on, and what their arguments are, can depend on any module, so bodies and
        // initializers are read last: initializers first, as a body's values can be the fields they give.
        foreach (var file in parsed)
        {
            foreach (var (member, value, owner) in file.Initializers)
            {
                member.Value = new PythonBody(file, owner, project, [value], null, []).Value(value);
            }
        }

        var nodes = graph.Files.Where(x => x.Language == PythonLanguage.LanguageName).ToDictionary(x => x.Path, StringComparer.Ordinal);
        foreach (var file in parsed)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach (var (member, function, owner, parameters) in file.Bodies)
            {
                member.Body = new PythonBody(file, owner, project, function.Body, function, parameters).Function(function);
            }

            if (file.TopLevel.Count > 0)
            {
                nodes[file.Path].TopLevel = new PythonBody(file, file.ModuleOwner, project, file.TopLevel, null, []).TopLevel();
            }
        }
    }

    /// <summary>Imports bind names in the module, wherever they are written: under <c>if TYPE_CHECKING:</c>, in a try, in a function.</summary>
    private static void Bind(PythonFile file)
    {
        foreach (var node in file.Tree.Descendants())
        {
            switch (node)
            {
                case PythonImport import:
                    foreach (var name in import.Names)
                    {
                        file.Bindings.TryAdd(name.AsName ?? name.Name.Split('.')[0], name.AsName is null ? name.Name.Split('.')[0] : name.Name);
                    }

                    break;
                case PythonImportFrom importFrom when file.Absolute(importFrom) is { } origin:
                    if (importFrom.IsWildcard)
                    {
                        file.Wildcards.Add(origin);
                    }

                    foreach (var name in importFrom.Names)
                    {
                        file.Bindings.TryAdd(name.AsName ?? name.Name, $"{origin}.{name.Name}");
                    }

                    break;
            }
        }
    }

    /// <summary>
    /// Each file's module name. A regular package says it: the folders above a file that hold an <c>__init__.py</c>. So does
    /// what the code imports, which also names a namespace package's (PEP 420), whose folders hold none, and a module under a
    /// source folder (<c>src/</c>). Of the two, the longer; a file neither names is its own top-level module.
    /// </summary>
    private static Dictionary<PythonFile, string> ModuleNames(string root, IReadOnlyList<PythonFile> files)
    {
        var imported = new HashSet<string>(StringComparer.Ordinal);
        foreach (var statement in files.SelectMany(x => x.Tree.Descendants()))
        {
            switch (statement)
            {
                case PythonImport import:
                    imported.UnionWith(import.Names.Select(x => x.Name));
                    break;
                case PythonImportFrom { Level: 0, Module: { } module } importFrom:
                    imported.Add(module);
                    imported.UnionWith(importFrom.Names.Select(x => $"{module}.{x.Name}"));
                    break;
            }
        }

        var names = new Dictionary<PythonFile, string>();
        foreach (var file in files)
        {
            var segments = file.Path[..file.Path.LastIndexOf('.')].Split('/').ToList();
            var package = segments[^1] == "__init__";
            if (package)
            {
                segments.RemoveAt(segments.Count - 1);
            }

            // The folders above that are regular packages.
            var regular = package ? 1 : 0;
            for (var folder = segments.Count - (package ? 1 : 2); folder >= 0; folder--)
            {
                var path = Path.Combine([root, ..segments.Take(folder + 1)]);
                if (!File.Exists(Path.Combine(path, "__init__.py")) && !File.Exists(Path.Combine(path, "__init__.pyi")))
                {
                    break;
                }

                regular = segments.Count - folder;
            }

            var named = Enumerable.Range(1, segments.Count).LastOrDefault(x => imported.Contains(string.Join(".", segments.TakeLast(x))));
            var length = Math.Max(Math.Max(regular, named), segments.Count == 0 ? 0 : 1);
            names[file] = string.Join(".", segments.TakeLast(length));
            file.Module = names[file];
            file.IsPackage = package;
        }

        return names;
    }

    /// <summary>
    /// A file's imports, each by what it names, qualified: <c>shop.domain.order.Order</c> for
    /// <c>from .order import Order</c> in <c>shop.domain</c>. A module it imports from is its <see cref="ImportNode.Module"/>,
    /// as written, and the in-scope file that is that module its <see cref="ImportNode.File"/>; null for a library's.
    /// </summary>
    private static List<ImportNode> Imports(PythonFile file, PythonProject project)
    {
        var imports = new List<ImportNode>();
        foreach (var node in file.Tree.Descendants())
        {
            switch (node)
            {
                case PythonImport import:
                    foreach (var name in import.Names)
                    {
                        imports.Add(new ImportNode
                        {
                            Target = name.Name,
                            Text = TextOf(file.Source, name),
                            Alias = name.AsName,
                            Module = name.Name,
                            File = project.FileOf(name.Name)?.Path,
                            At = SpanOf(name, file.Path)
                        });
                    }

                    break;
                case PythonImportFrom importFrom when file.Absolute(importFrom) is { } module:
                {
                    var written = new string('.', importFrom.Level) + importFrom.Module;
                    if (importFrom.IsWildcard)
                    {
                        imports.Add(new ImportNode { Target = module, Text = TextOf(file.Source, importFrom), Wildcard = true, Module = written, File = project.FileOf(module)?.Path, At = SpanOf(importFrom, file.Path) });
                        break;
                    }

                    foreach (var name in importFrom.Names)
                    {
                        // A name is a submodule, or what the module declares.
                        var target = $"{module}.{name.Name}";
                        imports.Add(new ImportNode
                        {
                            Target = target,
                            Text = TextOf(file.Source, name),
                            Alias = name.AsName,
                            Module = written,
                            File = (project.FileOf(target) ?? project.FileOf(module))?.Path,
                            At = SpanOf(name, file.Path)
                        });
                    }

                    break;
                }
            }
        }

        return imports;
    }

    /// <summary>What a file declares: its classes, its module's functions and variables, its type aliases.</summary>
    private sealed class Declarations(PythonFile file, PythonProject project)
    {
        private CodeGraph Graph => project.Graph;

        public void Read()
        {
            var module = file.ModuleOwner = new TypeNode
            {
                Name = FileName(file.Path),
                Namespace = file.Module,
                Kind = "module",
                At = SpanOf(file.Tree, file.Path),
                TextOf = () => Canonical(file, file.Tree)
            };
            module.Files.Add(file.Path);
            foreach (var statement in file.Tree.Body)
            {
                switch (statement)
                {
                    case PythonClass declared:
                        file.Types.TryAdd(declared.Name, Class(declared));
                        break;
                    case PythonFunction function:
                        if (Function(function, module, inClass: false) is { } added)
                        {
                            module.Members.Add(added);
                        }

                        break;
                    case PythonTypeAlias alias:
                        Alias(alias.Name, alias.TypeParameters, alias.Value, alias);
                        break;
                    case PythonAnnotatedAssign { Target: PythonName name } annotated when PythonTypes.TypeRef(annotated.Annotation).Name == "TypeAlias" && annotated.Value is { } aliased:
                        Alias(name.Id, [], aliased, annotated);
                        break;
                    case PythonAnnotatedAssign or PythonAssign:
                        module.Members.AddRange(Variables(statement, module, inClass: false));
                        file.TopLevel.Add(statement);
                        break;
                    case PythonImport or PythonImportFrom:
                        break;
                    case PythonExpressionStatement { Value: PythonString }:
                        // A docstring.
                        break;
                    default:
                        file.TopLevel.Add(statement);
                        break;
                }
            }

            // A module that declares nothing outside its classes is no type.
            if (module.Members.Count > 0)
            {
                Graph.Types.Add(module);
                file.ModuleType = module;
                Protocol(module);
            }
        }

        /// <summary>
        /// A dunder (<c>__repr__</c>, <c>__eq__</c>, <c>__tablename__</c>, <c>__all__</c>) is the language's or a framework's
        /// protocol, which no designer states, as C#'s <c>ToString</c> override isn't: <c>unmodelled.member</c> passes over it.
        /// <c>__init__</c> is the constructor, which a designer does state.
        /// </summary>
        private void Protocol(TypeNode type)
        {
            foreach (var member in type.Members.Where(x => x.Kind != "constructor" && x.Name.Length > 4 && x.Name.StartsWith("__", StringComparison.Ordinal) && x.Name.EndsWith("__", StringComparison.Ordinal)))
            {
                Graph.Unmodellable.TryAdd(member, $"`{member.Name}` is a Python dunder: the language's or a framework's protocol, not a member a designer states");
            }
        }

        /// <summary>
        /// What an annotation stands for: an <c>Annotated</c> alias (<c>service: ProjectServiceDep</c>) is the alias's value,
        /// <c>Annotated[ProjectService, Depends(get_project_service)]</c>, with its names as the alias's module binds them, so
        /// its type is <c>ProjectService</c> and its metadata the member's attributes. Any other annotation is itself.
        /// </summary>
        private (PythonExpression? Annotation, PythonFile Origin) Effective(PythonExpression? annotation)
        {
            var origin = file;
            for (var depth = 0; depth < 3 && annotation is PythonName or PythonAttribute; depth++)
            {
                var written = Dotted(annotation);
                var head = written.Split('.')[0];
                var qualified = origin.Bindings.TryGetValue(head, out var bound) ? bound + written[head.Length..] : $"{origin.Module}.{written}";
                if (!project.Aliases.TryGetValue(qualified, out var alias))
                {
                    break;
                }

                (annotation, origin) = alias;
            }

            return (annotation, origin);
        }

        /// <summary>An annotation as the graph's type, read through an alias: a name the alias's module imports is qualified as it binds it.</summary>
        private TypeRefNode TypeOf((PythonExpression? Annotation, PythonFile Origin) effective)
        {
            var type = PythonTypes.TypeRef(effective.Annotation!);
            if (ReferenceEquals(effective.Origin, file))
            {
                return type;
            }

            var head = type.QualifiedName.Split('.')[0];
            var qualified = effective.Origin.Bindings.TryGetValue(head, out var bound) ? bound + type.QualifiedName[head.Length..]
                : effective.Origin.Tree.Body.Any(x => x is PythonClass declared && declared.Name == head) ? $"{effective.Origin.Module}.{type.QualifiedName}"
                : type.QualifiedName;
            return new TypeRefNode
            {
                Text = type.Text,
                Name = type.Name,
                QualifiedName = qualified,
                Arguments = type.Arguments,
                Union = type.Union,
                IsArray = type.IsArray,
                Literal = type.Literal,
                Language = type.Language,
                Nullable = type.Nullable
            };
        }

        private void Alias(string name, IReadOnlyList<string> typeParameters, PythonExpression value, PythonStatement statement)
        {
            var type = new TypeNode
            {
                Name = name,
                Namespace = file.Module,
                Kind = "typeAlias",
                Aliases = PythonTypes.TypeRef(value),
                At = SpanOf(statement, file.Path),
                Access = Access(name, member: false),
                TypeParameters = typeParameters,
                TextOf = () => Canonical(file, statement)
            };
            type.Files.Add(file.Path);
            Graph.Types.Add(type);
            file.Types.TryAdd(name, type);
        }

        private TypeNode Class(PythonClass declared)
        {
            var bases = declared.Bases.Where(x => x is { Name: null, Stars: 0 }).Select(x => x.Value).ToList();
            var baseTypes = bases.Select(PythonTypes.TypeRef).ToList();
            var kind = baseTypes.Any(x => EnumBases.Contains(x.Name)) ? "enum"
                : baseTypes.Any(x => x.Name == "Protocol") ? "interface"
                : "class";
            var metaclass = declared.Bases.FirstOrDefault(x => x.Name == "metaclass")?.Value;
            var functions = declared.Body.OfType<PythonFunction>().ToList();
            var type = new TypeNode
            {
                Name = declared.Name,
                Namespace = file.Module,
                Kind = kind,
                At = SpanOf(declared, file.Path),
                IsAbstract = baseTypes.Any(x => x.Name == "ABC") || metaclass is not null && PythonTypes.TypeRef(metaclass).Name == "ABCMeta"
                             || functions.Any(x => Decorators(x).Contains("abstractmethod")),
                Access = Access(declared.Name, member: false),
                // A Generic[T] or Protocol[T] base declares its type parameters, as class Box[T]: does.
                TypeParameters = declared.TypeParameters
                    .Concat(baseTypes.Where(x => x.Name is "Generic" or "Protocol").SelectMany(x => x.Arguments).Select(x => x.Name))
                    .Distinct()
                    .ToList(),
                TextOf = () => Canonical(file, declared)
            };
            type.Files.Add(file.Path);
            type.Attributes.AddRange(declared.Decorators.Select(x => Attribute(x.Expression, x)));
            if (kind != "enum")
            {
                type.BaseTypes.AddRange(baseTypes.Where(x => !MarkerBases.Contains(x.Name)));
            }

            Graph.Types.Add(type);

            foreach (var statement in declared.Body)
            {
                switch (statement)
                {
                    case PythonFunction function:
                        if (Function(function, type, inClass: true) is { } member)
                        {
                            type.Members.Add(member);
                        }

                        break;
                    case PythonClass nested:
                        Class(nested);
                        break;
                    case PythonAssign or PythonAnnotatedAssign when kind == "enum":
                        type.Members.AddRange(EnumMembers(statement, type));
                        break;
                    case PythonAssign or PythonAnnotatedAssign:
                        type.Members.AddRange(Variables(statement, type, inClass: true));
                        break;
                }
            }

            // What __init__ assigns to self is an attribute of every instance, unless the class declares it.
            if (functions.FirstOrDefault(x => x.Name == "__init__") is { } init)
            {
                type.Members.AddRange(SelfAttributes(init, type));
            }

            Protocol(type);
            return type;
        }

        private IEnumerable<MemberNode> EnumMembers(PythonStatement statement, TypeNode type)
        {
            var (targets, value) = statement switch
            {
                PythonAssign assign => (assign.Targets, assign.Value),
                PythonAnnotatedAssign annotated => ([annotated.Target], annotated.Value),
                _ => ((IReadOnlyList<PythonExpression>)[], (PythonExpression?)null)
            };

            // An enum's _ignore_ and other sunder names aren't its members, and neither is a declaration without a value.
            foreach (var name in targets.OfType<PythonName>().Where(x => !x.Id.StartsWith('_') && value is not null))
            {
                var member = new MemberNode
                {
                    Name = name.Id,
                    Kind = "enumMember",
                    IsStatic = true,
                    Value = Initial(value!),
                    At = SpanOf(statement, file.Path),
                    TextOf = () => Canonical(file, statement)
                };
                file.Initializers.Add((member, value!, type));
                yield return member;
            }
        }

        /// <summary>
        /// A class's or module's variables: <c>name: str</c> and <c>name: str = ""</c> declare an instance's attribute in a
        /// class, unless the annotation is <c>ClassVar</c>; a plain <c>NAME = 1</c> is the class's own, or the module's.
        /// </summary>
        /// <param name="owner">The class, or the module.</param>
        /// <param name="inClass">The owner is a class.</param>
        private IEnumerable<MemberNode> Variables(PythonStatement statement, TypeNode owner, bool inClass)
        {
            var (targets, annotation, value) = statement switch
            {
                PythonAssign assign => (assign.Targets, null, assign.Value),
                PythonAnnotatedAssign annotated => ((IReadOnlyList<PythonExpression>)[annotated.Target], annotated.Annotation, annotated.Value),
                _ => ([], (PythonExpression?)null, (PythonExpression?)null)
            };

            var effective = Effective(annotation);
            annotation = effective.Annotation;
            var declared = annotation is null ? null : TypeOf(effective);
            var classVar = declared?.Name == "ClassVar";
            var final = declared?.Name == "Final";
            if (declared is { Name: "ClassVar" or "Final" })
            {
                declared = declared.Arguments is [var only] ? only : null;
            }

            foreach (var name in targets.OfType<PythonName>())
            {
                var type = declared ?? Created(value);
                if (type is not null && declared is not null)
                {
                    type.Nullable = PythonTypes.Nullability(type);
                }

                var member = new MemberNode
                {
                    Name = name.Id,
                    Kind = "field",
                    Type = type,
                    IsStatic = !inClass || annotation is null || classVar,
                    Attributes = PythonTypes.Metadata(annotation).Select(x => Attribute(x, x)).ToList(),
                    Access = Access(name.Id, member: inClass),
                    Modifiers = final ? ["final"] : [],
                    Value = value is null ? null : Initial(value),
                    At = SpanOf(statement, file.Path),
                    TextOf = () => Canonical(file, statement)
                };
                if (value is not null)
                {
                    file.Initializers.Add((member, value, owner));
                }

                yield return member;
            }
        }

        /// <summary>
        /// The attributes <c>__init__</c> gives an instance (<c>self.repository = repository</c>), each with its type: its
        /// annotation, the annotated parameter it is given, or the class it is created as.
        /// </summary>
        private IEnumerable<MemberNode> SelfAttributes(PythonFunction init, TypeNode type)
        {
            var self = init.Parameters.FirstOrDefault()?.Name;
            var seen = type.Members.Select(x => x.Name).ToHashSet(StringComparer.Ordinal);
            foreach (var statement in init.Body.SelectMany(x => x.Descendants().Prepend(x)).OfType<PythonStatement>())
            {
                var (targets, annotation, value) = statement switch
                {
                    PythonAssign assign => (assign.Targets, null, assign.Value),
                    PythonAnnotatedAssign annotated => ((IReadOnlyList<PythonExpression>)[annotated.Target], annotated.Annotation, annotated.Value),
                    _ => ([], (PythonExpression?)null, (PythonExpression?)null)
                };

                foreach (var target in targets.OfType<PythonAttribute>().Where(x => x.Value is PythonName { } owner && owner.Id == self))
                {
                    if (!seen.Add(target.Name))
                    {
                        continue;
                    }

                    var given = value is PythonName { } name ? init.Parameters.FirstOrDefault(x => x.Name == name.Id)?.Annotation : null;
                    var written = annotation ?? given;
                    var typeRef = written is null ? Created(value) : PythonTypes.TypeRef(written);
                    if (typeRef is not null && written is not null)
                    {
                        typeRef.Nullable = PythonTypes.Nullability(typeRef);
                    }

                    var member = new MemberNode
                    {
                        Name = target.Name,
                        Kind = "field",
                        Type = typeRef,
                        Attributes = PythonTypes.Metadata(annotation).Select(x => Attribute(x, x)).ToList(),
                        Access = Access(target.Name, member: true),
                        At = SpanOf(statement, file.Path),
                        TextOf = () => Canonical(file, statement)
                    };
                    yield return member;
                }
            }
        }

        /// <summary>
        /// The type an unannotated value has, as a type checker infers it: the class it is created as, when it is a call to a class's
        /// name (<c>APIRouter(prefix="/orders")</c>), or a literal's builtin type (<c>status_code = 409</c> is an <c>int</c>).
        /// </summary>
        private static TypeRefNode? Created(PythonExpression? value)
        {
            if (value is PythonCall { Function: PythonName or PythonAttribute } call && PythonTypes.TypeRef(call.Function) is var type && IsClassName(type.Name))
            {
                return type;
            }

            var number = value is PythonUnary { Operator: "-" or "+", Operand: PythonNumber signed } ? signed : value as PythonNumber;
            var builtin = value switch
            {
                PythonString { IsBytes: true } => "bytes",
                PythonString => "str",
                PythonConstant { Value: "True" or "False" } => "bool",
                _ when number?.Text is { } text => text.EndsWith('j') || text.EndsWith('J') ? null
                    : text.StartsWith("0x", StringComparison.OrdinalIgnoreCase) || text.StartsWith("0o", StringComparison.OrdinalIgnoreCase) || text.StartsWith("0b", StringComparison.OrdinalIgnoreCase) || !text.Any(x => x is '.' or 'e' or 'E') ? "int"
                    : "float",
                _ => null
            };
            return builtin is null ? null : new TypeRefNode { Text = builtin, Name = builtin, QualifiedName = builtin, Nullable = "false", Language = PythonLanguage.LanguageName };
        }

        /// <param name="owner">The class, or the module.</param>
        /// <param name="inClass">The owner is a class.</param>
        private MemberNode? Function(PythonFunction function, TypeNode owner, bool inClass)
        {
            var decorators = Decorators(function);

            // A property's setter and deleter are the property's.
            if (decorators.Any(x => x.EndsWith(".setter", StringComparison.Ordinal) || x.EndsWith(".deleter", StringComparison.Ordinal)))
            {
                return null;
            }

            var isStatic = !inClass || decorators.Contains("staticmethod") || decorators.Contains("classmethod");
            var bound = inClass && !decorators.Contains("staticmethod");
            var kind = !inClass ? "method" : function.Name == "__init__" ? "constructor" : decorators.Contains("property") || decorators.Contains("cached_property") ? "property" : "method";

            // self or cls is bound by the call, not given.
            var written = function.Parameters.ToList();
            if (bound && written is [{ Kind: PythonParameterKind.Normal or PythonParameterKind.PositionalOnly }, ..])
            {
                written.RemoveAt(0);
            }

            var parameters = kind == "property" ? [] : written.Select(Parameter).ToList();
            var returns = function.Returns is null || kind == "constructor" ? null : TypeOf(Effective(function.Returns));
            if (returns is not null && kind == "property")
            {
                returns.Nullable = PythonTypes.Nullability(returns);
            }

            var modifiers = new List<string>();
            if (function.IsAsync)
            {
                modifiers.Add("async");
            }

            modifiers.AddRange(decorators.Where(x => x is "abstractmethod" or "classmethod" or "final" or "override").Select(x => x == "abstractmethod" ? "abstract" : x));

            var member = new MemberNode
            {
                Name = kind == "constructor" ? owner.Name : function.Name,
                Kind = kind,
                Type = returns,
                IsStatic = isStatic && kind != "constructor",
                Parameters = parameters,
                Attributes = function.Decorators.Select(x => Attribute(x.Expression, x)).ToList(),
                Access = kind == "constructor" ? "public" : Access(function.Name, member: inClass),
                Modifiers = modifiers,
                TypeParameters = function.TypeParameters,
                At = SpanOf(function, file.Path),
                TextOf = () => Canonical(file, function)
            };
            file.Bodies.Add((member, function, owner, written.Zip(parameters).ToList()));
            return member;
        }

        private ParameterNode Parameter(PythonParameter parameter)
        {
            var effective = Effective(parameter.Annotation);
            var type = effective.Annotation is null ? null : TypeOf(effective);
            if (type is not null)
            {
                type.Nullable = PythonTypes.Nullability(type);
            }

            return new ParameterNode
            {
                Name = parameter.Name,
                // *args is the arguments' tuple.
                Type = parameter.Kind == PythonParameterKind.VarPositional && type is not null
                    ? new TypeRefNode { Text = $"tuple[{type.Text}, ...]", Name = type.Name, QualifiedName = type.QualifiedName, Arguments = [type], IsArray = true, Nullable = "false", Language = PythonLanguage.LanguageName }
                    : type,
                Attributes = PythonTypes.Metadata(effective.Annotation).Select(x => Attribute(x, x)).ToList(),
                Default = parameter.Default is null ? null : TextOf(file.Source, parameter.Default),
                IsOptional = parameter.Kind is PythonParameterKind.VarPositional or PythonParameterKind.VarKeyword,
                At = SpanOf(parameter, file.Path),
                TextOf = () => Canonical(file, parameter)
            };
        }

        /// <summary>A value as the reader first has it; it is read again with the code around it once every module is.</summary>
        private ValueNode Initial(PythonExpression value) =>
            new() { Text = TextOf(file.Source, value), Literal = LiteralOf(value, null, file.Path), At = SpanOf(value, file.Path) };

        private static List<string> Decorators(PythonFunction function) => function.Decorators.Select(x => Dotted(x.Expression is PythonCall call ? call.Function : x.Expression)).ToList();

        /// <summary>
        /// A decorator, or an <c>Annotated</c> type's metadata, as an attribute: named by its last name (<c>get</c> for
        /// <c>@router.get("/orders")</c>), with its call's arguments; a keyword argument is a named one.
        /// </summary>
        private AttributeNode Attribute(PythonExpression expression, PythonNode at)
        {
            var call = expression as PythonCall;
            var values = call?.Arguments.Where(x => x.Stars == 0).Select(x => Value(x.Value, x.Name)).ToList() ?? [];
            return new AttributeNode
            {
                Name = Dotted(call?.Function ?? expression).Split('.')[^1],
                Arguments = values.Where(x => x.Name is null).Select(x => x.Literal ?? x.Text).ToList(),
                Named = values.Where(x => x.Name is not null).GroupBy(x => x.Name!).ToDictionary(x => x.Key, x => x.First().Literal ?? x.First().Text),
                Values = values,
                At = SpanOf(at, file.Path)
            };
        }

        private AttributeValueNode Value(PythonExpression value, string? name) => new()
        {
            Name = name,
            Text = TextOf(file.Source, value),
            Literal = LiteralOf(value, null, file.Path)?.Value,
            Items = value is PythonSequence sequence ? sequence.Items.Select(x => Value(x, null)).ToList() : [],
            Annotation = value is PythonCall nested ? Attribute(nested, nested) : null
        };
    }

    /// <summary>A name as written, its attributes joined: <c>router.get</c>; anything else as Python prints it.</summary>
    internal static string Dotted(PythonExpression expression) => expression switch
    {
        PythonName name => name.Id,
        PythonAttribute attribute => $"{Dotted(attribute.Value)}.{attribute.Name}",
        _ => PythonTypes.Print(expression)
    };

    /// <summary>PEP 8 names a class in CapWords: a capital first, and a lower-case letter, so <c>MAX_SIZE</c> is no class.</summary>
    internal static bool IsClassName(string name) => name.Length > 0 && char.IsUpper(name[0]) && name.Any(char.IsLower);

    /// <summary>
    /// Python has no access modifiers, only its naming convention: <c>__name</c> is private (the class mangles it), a class's
    /// <c>_name</c> is protected (for itself and its subclasses), a module's <c>_name</c> private to the module, and
    /// anything else, <c>__init__</c> and the other dunder names included, public.
    /// </summary>
    internal static string Access(string name, bool member) =>
        name.StartsWith("__", StringComparison.Ordinal) && !name.EndsWith("__", StringComparison.Ordinal) ? "private"
        : name.StartsWith('_') && !name.EndsWith("__", StringComparison.Ordinal) ? member ? "protected" : "private"
        : "public";

    /// <summary>
    /// The literal an expression is (<see cref="LiteralNode"/>), or null: a string with its escapes resolved and its
    /// neighbours joined, a number with its sign, <c>True</c> and <c>False</c> as <c>true</c> and <c>false</c>, <c>None</c> as
    /// null. An f-string is <c>interpolated</c>, each field's code a part, as <paramref name="hole"/> gives it; a bytes literal
    /// is no string.
    /// </summary>
    internal static LiteralNode? LiteralOf(PythonExpression expression, Func<PythonExpression, ValueNode>? hole, string file)
    {
        switch (expression)
        {
            case PythonString { IsBytes: false, Value: { } text }:
                return new LiteralNode { Kind = "string", Value = text };
            case PythonString { IsBytes: false } formatted:
                return new LiteralNode
                {
                    Kind = "interpolated",
                    Parts = formatted.Parts.Select(x => x.Text is { } text
                            ? new ValueNode { Text = text, Literal = new LiteralNode { Kind = "string", Value = text }, At = SpanOf(formatted, file) }
                            : hole?.Invoke(x.Expression!) ?? new ValueNode { Text = PythonTypes.Print(x.Expression!), At = SpanOf(x.Expression!, file) })
                        .ToList()
                };
            case PythonNumber number:
                return new LiteralNode { Kind = "number", Value = number.Text };
            case PythonUnary { Operator: "-" or "+", Operand: PythonNumber number } signed:
                return new LiteralNode { Kind = "number", Value = (signed.Operator == "-" ? "-" : "") + number.Text };
            case PythonConstant { Value: "True" or "False" } boolean:
                return new LiteralNode { Kind = "boolean", Value = boolean.Value == "True" ? "true" : "false" };
            case PythonConstant { Value: "None" }:
                return new LiteralNode { Kind = "null" };
            default:
                return null;
        }
    }

    internal static Span SpanOf(PythonNode node, string file) => new(file, node.First.Line, node.First.Column, node.Last.EndLine, node.Last.EndColumn);

    internal static string TextOf(string source, PythonNode node) =>
        node.Last.End <= node.First.Start ? "" : Whitespace.Replace(source[node.First.Start..node.Last.End], " ").Trim();

    /// <summary>
    /// A node's canonical text (code-reader.md, Canonical text): its tokens joined, with only the spaces words need. Comments
    /// and line breaks aren't tokens. Code inside an f-string is part of its string's token, so is its source text.
    /// </summary>
    internal static string Canonical(PythonFile file, PythonNode node)
    {
        var start = file.TokenAt(node.First.Start);
        var tokens = new List<(string, bool)>();
        for (var i = start; i >= 0 && i < file.Tokens.Count && file.Tokens[i].Start < node.Last.End; i++)
        {
            if (file.Tokens[i].IsWritten)
            {
                tokens.Add((file.Tokens[i].Text, false));
            }
        }

        return tokens.Count > 0 ? CanonicalText.Of(tokens) : CanonicalText.OfText(file.Source[node.First.Start..node.Last.End]);
    }

    private static string FileName(string path) => path[(path.LastIndexOf('/') + 1)..];
}

/// <summary>A Python file as the reader works with it: its parse, its module, what it binds and declares, and what is read last.</summary>
internal sealed class PythonFile(string path, string source, PythonModule tree, IReadOnlyList<PythonToken> tokens)
{
    public string Path { get; } = path;

    public string Source { get; } = source;

    public PythonModule Tree { get; } = tree;

    public IReadOnlyList<PythonToken> Tokens { get; } = tokens;

    /// <summary>Its module's dotted name.</summary>
    public string Module { get; set; } = "";

    /// <summary>A package's <c>__init__.py</c>.</summary>
    public bool IsPackage { get; set; }

    /// <summary>What it declares outside its classes; null when it declares nothing there.</summary>
    public TypeNode? ModuleType { get; set; }

    /// <summary>
    /// The module as a type, whether or not it declares anything outside its classes: what its functions, variables and
    /// top-level code belong to. In the graph only as <see cref="ModuleType"/>.
    /// </summary>
    public TypeNode ModuleOwner { get; set; } = null!;

    /// <summary>Its top-level classes and type aliases, by name.</summary>
    public Dictionary<string, TypeNode> Types { get; } = new(StringComparer.Ordinal);

    /// <summary>
    /// The names its imports bind, each to what it names, qualified: <c>Order</c> to <c>shop.domain.order.Order</c>,
    /// <c>shop</c> to <c>shop</c> for <c>import shop.domain</c>.
    /// </summary>
    public Dictionary<string, string> Bindings { get; } = new(StringComparer.Ordinal);

    /// <summary>The modules it imports everything from (<c>from shop.domain import *</c>).</summary>
    public List<string> Wildcards { get; } = [];

    /// <summary>Each method's and function's body to read once every module is, with its owner and its parameters as the graph has them.</summary>
    public List<(MemberNode Member, PythonFunction Function, TypeNode Owner, IReadOnlyList<(PythonParameter, ParameterNode)> Parameters)> Bodies { get; } = [];

    /// <summary>Each variable's value to read again once every module is.</summary>
    public List<(MemberNode Member, PythonExpression Value, TypeNode Owner)> Initializers { get; } = [];

    /// <summary>Its top-level code: what isn't a class, a function, an import or a docstring.</summary>
    public List<PythonStatement> TopLevel { get; } = [];

    /// <summary>The package a relative import is relative to.</summary>
    public string Package => IsPackage || !Module.Contains('.') ? IsPackage ? Module : "" : Module[..Module.LastIndexOf('.')];

    /// <summary>The module a <c>from</c> import imports from, qualified: <c>from ..domain import x</c> in <c>shop.api.orders</c> is <c>shop.domain</c>.</summary>
    public string? Absolute(PythonImportFrom importFrom)
    {
        if (importFrom.Level == 0)
        {
            return importFrom.Module;
        }

        var package = Package.Length == 0 ? [] : Package.Split('.').ToList();
        if (importFrom.Level - 1 > package.Count)
        {
            return null;
        }

        var parts = package.Take(package.Count - (importFrom.Level - 1)).ToList();
        if (importFrom.Module is not null)
        {
            parts.Add(importFrom.Module);
        }

        return parts.Count == 0 ? null : string.Join(".", parts);
    }

    /// <summary>The index of the first token at or after an offset.</summary>
    public int TokenAt(int offset)
    {
        int low = 0, high = Tokens.Count - 1;
        while (low < high)
        {
            var middle = (low + high) / 2;
            if (Tokens[middle].Start < offset)
            {
                low = middle + 1;
            }
            else
            {
                high = middle;
            }
        }

        return low;
    }
}

/// <summary>Every in-scope module, by its name, for what a name another module declares is.</summary>
internal sealed class PythonProject(CodeGraph graph, Dictionary<PythonFile, string> modules)
{
    private readonly Dictionary<string, PythonFile> _byModule = modules
        .GroupBy(x => x.Value, StringComparer.Ordinal)
        .ToDictionary(x => x.Key, x => x.First().Key, StringComparer.Ordinal);

    public CodeGraph Graph { get; } = graph;

    /// <summary>Each module's <c>Annotated</c> aliases, by qualified name: what the alias stands for, and the module that declares it.</summary>
    public Dictionary<string, (PythonExpression Value, PythonFile File)> Aliases { get; } = new(StringComparer.Ordinal);

    public PythonFile? FileOf(string module) => _byModule.GetValueOrDefault(module);

    /// <summary>
    /// What a qualified name is: a module, a class or type alias it declares, or a function or variable of its module.
    /// Null when no in-scope module declares it: a library's.
    /// </summary>
    public PythonReferent? Resolve(string qualified)
    {
        if (FileOf(qualified) is { } module)
        {
            return new PythonReferent(module, null, null);
        }

        var dot = qualified.LastIndexOf('.');
        if (dot < 0 || FileOf(qualified[..dot]) is not { } owner)
        {
            return null;
        }

        var name = qualified[(dot + 1)..];
        if (owner.Types.TryGetValue(name, out var type))
        {
            return new PythonReferent(null, type, null);
        }

        // A name the module imports is the module's too: a package's __init__.py re-exports.
        if (owner.ModuleType?.Members.FirstOrDefault(x => x.Name == name) is { } member)
        {
            return new PythonReferent(null, owner.ModuleType, member);
        }

        return owner.Bindings.TryGetValue(name, out var reexported) && reexported != qualified ? Resolve(reexported) : null;
    }
}

/// <summary>
/// What a name is: a module, a type, or a member of a module (its function or variable, with the module as its type); or,
/// for a name an import gives that no in-scope module declares, the library's name it is (<c>fastapi.HTTPException</c>).
/// </summary>
internal sealed record PythonReferent(PythonFile? Module, TypeNode? Type, MemberNode? Member, string? Library = null);
