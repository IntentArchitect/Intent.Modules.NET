using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intent.Modules.Services.Guardrails.Contracts.Drift
{
    /// <summary>What a contract template would write to a file, before it is merged into the code.</summary>
    internal sealed class GeneratedFile
    {
        public string RelativePath { get; init; }
        public string Text { get; init; }

        /// <summary>The model element each generated type (<c>Order</c>) and member (<c>Order.Total</c>) represents, by id.</summary>
        public IReadOnlyDictionary<string, string> ElementIds { get; init; }

        /// <summary>What the model can state about each generated type (<c>Order</c>); <see cref="Modellable.Everything"/> for a type not listed.</summary>
        public IReadOnlyDictionary<string, Modellable> ModellableTypes { get; init; }
    }

    /// <summary>
    /// What a model element can state about the type its template generates, recorded by the template that builds it. The
    /// comparison reads nothing else: the drift check keeps what is modelled in step with the code, so a difference the
    /// designer has no way to express is never a row and never needs an exception (user decision, 2026-10-23). Modifiers,
    /// and the base types the model names, can always be stated.
    /// </summary>
    internal sealed class Modellable
    {
        /// <summary>The metadata key a builder records it under, on the type it builds.</summary>
        public const string MetadataKey = "Guardrails.Modellable";

        /// <summary>A type whose template doesn't say: every part is compared.</summary>
        public static readonly Modellable Everything = new()
        {
            TypeParameters = true, ParameterList = true, Properties = true, Methods = true, Constructors = true
        };

        /// <summary>The kinds the type can be declared as (<c>class</c>, <c>record</c>, <c>interface</c>, <c>enum</c>), or <see langword="null"/> for any.</summary>
        public IReadOnlyCollection<string> Kinds { get; init; }

        public bool TypeParameters { get; init; }

        /// <summary>Parameters on the type itself: a positional record's, or a class's primary constructor's.</summary>
        public bool ParameterList { get; init; }

        /// <summary>Properties. No designer models a field, an event or an indexer.</summary>
        public bool Properties { get; init; }

        public bool Methods { get; init; }

        /// <summary>Constructors declared in the type's body.</summary>
        public bool Constructors { get; init; }
    }

    /// <summary>
    /// The code types some model describes: the declarations of the types the contract templates generate, and every
    /// type in a file any template of the run writes, whichever designer or module describes it.
    /// </summary>
    internal sealed class GeneratedTypes
    {
        private readonly HashSet<SyntaxNode> _declarations = new();
        private readonly HashSet<string> _fullPaths = new(StringComparer.Ordinal);
        private readonly HashSet<string> _outputFiles = new(StringComparer.OrdinalIgnoreCase);

        public void AddDeclarations(IEnumerable<BaseTypeDeclarationSyntax> declarations)
        {
            foreach (var declaration in declarations)
            {
                _declarations.Add(declaration);
                _fullPaths.Add(CodeLocator.FullTypePath(declaration));
            }
        }

        public void AddOutputFiles(IEnumerable<string> relativePaths) => _outputFiles.UnionWith(relativePaths);

        /// <summary>
        /// Whether the code type is described: a declaration the structure check matched, another part of it, or a type
        /// in a file a template writes.
        /// </summary>
        public bool Contains(BaseTypeDeclarationSyntax type) =>
            _declarations.Contains(type) || _fullPaths.Contains(CodeLocator.FullTypePath(type)) || _outputFiles.Contains(type.SyntaxTree.FilePath);
    }

    internal sealed class StructureResult
    {
        public IReadOnlyList<Finding> Findings { get; init; }
        public int TypeCount { get; init; }
        public GeneratedTypes Types { get; init; }

        /// <summary>
        /// The model's fingerprint of each generated type's declaration (<c>Order</c>) and member (<c>Order.Total(bool)</c>):
        /// what the template writes, so a change in the model changes it.
        /// </summary>
        public IReadOnlyDictionary<string, string> Fingerprints { get; init; }

        /// <summary>
        /// The code each aspect was compared with, by the aspect a line is about: the generated type or member, or the
        /// unmodelled code member. None where the code doesn't have it.
        /// </summary>
        public IReadOnlyDictionary<string, CodeAspect> CodeFingerprints { get; init; }

        /// <summary>The fingerprint of every member of each generated type found on disk, at any access level (<see cref="StructureComparer.CodeMembers"/>).</summary>
        public IReadOnlyDictionary<string, string> CodeMembers { get; init; }

        /// <summary>The aspects compared with the code: those of every generated type found on disk.</summary>
        public IReadOnlySet<string> Compared { get; init; }

        /// <summary>Each generated type's (<c>Order</c>) and member's (<c>Order.Total(bool)</c>) declaration in its template's output.</summary>
        public IReadOnlyDictionary<string, SyntaxNode> Declarations { get; init; }

        /// <summary>Each contract template's output, by its file's path relative to the codebase root.</summary>
        public IReadOnlyDictionary<string, string> Texts { get; init; }
    }

    /// <summary>
    /// A code type or member as the comparison reads it: its own key, which for a method the model changed the parameters of
    /// differs from the model's, and its fingerprint.
    /// </summary>
    internal sealed record CodeAspect(string Key, string Fingerprint);

    /// <summary>
    /// Compares each contract template's output with its file on disk: the types and members the model owns.
    /// Type spellings are compared by meaning (<c>Nullable&lt;int&gt;</c> is <c>int?</c>, <c>System.String</c> is
    /// <c>string</c>, a qualified name is its last part). Code members the model doesn't describe are reported at
    /// the access levels the model uses for that kind of member.
    /// </summary>
    internal static class StructureComparer
    {
        private static readonly CSharpParseOptions ParseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);

        private static readonly string[] ModifierOrder =
        {
            "public", "protected", "internal", "private", "file", "new", "static", "abstract", "virtual", "override", "sealed",
            "readonly", "required", "extern", "unsafe", "volatile", "async", "ref", "partial"
        };

        private static readonly Dictionary<string, string> Aliases = new(StringComparer.Ordinal)
        {
            ["Boolean"] = "bool", ["Byte"] = "byte", ["SByte"] = "sbyte", ["Char"] = "char", ["Decimal"] = "decimal",
            ["Double"] = "double", ["Single"] = "float", ["Int16"] = "short", ["UInt16"] = "ushort", ["Int32"] = "int",
            ["UInt32"] = "uint", ["Int64"] = "long", ["UInt64"] = "ulong", ["Object"] = "object", ["String"] = "string"
        };

        /// <param name="generated">The contract templates' output.</param>
        /// <param name="outputPaths">Every file any template of the run writes, relative to the codebase root: their types count as described.</param>
        /// <param name="code">The codebase.</param>
        public static StructureResult Compare(IReadOnlyList<GeneratedFile> generated, IReadOnlyCollection<string> outputPaths, CodeFiles code)
        {
            var findings = new List<Finding>();
            var generatedTypes = new GeneratedTypes();
            var fingerprints = new Dictionary<string, string>(StringComparer.Ordinal);
            var codeFingerprints = new Dictionary<string, CodeAspect>(StringComparer.Ordinal);
            var codeMembers = new Dictionary<string, string>(StringComparer.Ordinal);
            var compared = new HashSet<string>(StringComparer.Ordinal);
            var declarations = new Dictionary<string, SyntaxNode>(StringComparer.Ordinal);
            var texts = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var files = generated
                .Select(x => (
                    RelativePath: code.ToRelative(x.RelativePath),
                    Types: CodeLocator.TopLevelTypes((CompilationUnitSyntax)CSharpSyntaxTree.ParseText(x.Text, ParseOptions).GetRoot()).ToList(),
                    Ids: x.ElementIds ?? new Dictionary<string, string>(),
                    x.Text,
                    Modellable: x.ModellableTypes ?? new Dictionary<string, Modellable>()))
                .ToList();
            var levels = LevelsInPlay(files.SelectMany(x => x.Types));
            generatedTypes.AddOutputFiles((outputPaths ?? Array.Empty<string>()).Select(code.ToRelative).Concat(files.Select(x => x.RelativePath)));

            foreach (var (path, types, ids, text, modellable) in files.OrderBy(x => x.RelativePath, StringComparer.Ordinal))
            {
                var disk = code.Get(path);
                texts.TryAdd(path, text);
                foreach (var type in types)
                {
                    var typeName = CodeLocator.TypePath(type);
                    var members = Members(type, typeName);
                    fingerprints[typeName] = DeclarationFingerprint(type);
                    declarations[typeName] = type;
                    foreach (var member in members)
                    {
                        fingerprints[$"{typeName}.{member.Key}"] = member.Fingerprint;
                        declarations[$"{typeName}.{member.Key}"] = member.Node;
                    }

                    var diskTypes = disk?.Syntax == null ? new List<BaseTypeDeclarationSyntax>() : CodeLocator.FindTypes(disk.Syntax, type.Identifier.Text).ToList();
                    if (diskTypes.Count == 0)
                    {
                        findings.Add(Finding.Missing(
                            $"`{typeName}` ({KindOf(type)}): modelled, not in `{path}`",
                            path,
                            new DriftRow(typeName, Chain(ids.GetValueOrDefault(typeName)), path, 0, $"`{typeName}` ({KindOf(type)}) is modelled, but not in the code", FileExists: disk != null),
                            typeName));
                        continue;
                    }

                    compared.Add(typeName);
                    compared.UnionWith(members.Select(x => $"{typeName}.{x.Key}"));
                    codeFingerprints[typeName] = new CodeAspect(typeName, CodeDeclarationFingerprint(diskTypes));
                    codeMembers[typeName] = MembersFingerprint(diskTypes, typeName);
                    generatedTypes.AddDeclarations(diskTypes);
                    var context = new TypeContext(typeName, disk, ids, findings, codeFingerprints, modellable.GetValueOrDefault(typeName) ?? Modellable.Everything);
                    CompareDeclaration(type, diskTypes, context);
                    CompareMembers(type, members, diskTypes, context, levels);
                }
            }

            return new StructureResult
            {
                Findings = findings,
                TypeCount = files.Sum(x => x.Types.Count),
                Types = generatedTypes,
                Fingerprints = fingerprints,
                CodeFingerprints = codeFingerprints,
                CodeMembers = codeMembers,
                Compared = compared,
                Declarations = declarations,
                Texts = texts
            };
        }

        /// <summary>Whether two methods' or constructors' parameter lists read the same: modifiers, types, names and defaults.</summary>
        public static bool SameParameters(BaseMethodDeclarationSyntax first, BaseMethodDeclarationSyntax second) =>
            Parameters(first.ParameterList) == Parameters(second.ParameterList);

        /// <summary>
        /// The fingerprint of every member of a code type, at any access level, across its parts in the file: for a type the
        /// model no longer generates, to tell whether the code added, removed or changed a member since they last agreed.
        /// </summary>
        public static string CodeMembers(BaseTypeDeclarationSyntax type)
        {
            var parts = type.SyntaxTree.GetRoot() is CompilationUnitSyntax root ? CodeLocator.FindTypes(root, type.Identifier.Text) : new[] { type };
            return MembersFingerprint(parts, CodeLocator.TypePath(type));
        }

        private static string MembersFingerprint(IEnumerable<BaseTypeDeclarationSyntax> parts, string typeName)
        {
            var members = string.Join("\n", parts.SelectMany(x => Members(x, typeName)).Select(x => $"{x.Key}={x.Fingerprint}").OrderBy(x => x, StringComparer.Ordinal));
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(members)));
        }

        /// <summary>
        /// The fingerprints of a code type, its declaration (<c>Order</c>) and its members (<c>Order.Total(bool)</c>), as
        /// the comparison reads them: for a type the model no longer generates, to tell whether the code changed it.
        /// </summary>
        public static IReadOnlyDictionary<string, string> CodeFingerprints(BaseTypeDeclarationSyntax type)
        {
            var typeName = CodeLocator.TypePath(type);
            var parts = type.SyntaxTree.GetRoot() is CompilationUnitSyntax root ? CodeLocator.FindTypes(root, type.Identifier.Text) : new[] { type };
            var result = new Dictionary<string, string>(StringComparer.Ordinal) { [typeName] = CodeDeclarationFingerprint(parts) };
            foreach (var member in parts.SelectMany(x => Members(x, typeName)))
            {
                result[$"{typeName}.{member.Key}"] = member.Fingerprint;
            }

            return result;
        }

        private sealed record TypeContext(string TypeName, CodeFile File, IReadOnlyDictionary<string, string> Ids, List<Finding> Findings, Dictionary<string, CodeAspect> Code, Modellable Modellable)
        {
            /// <summary>The type's element, then the member's, as far as they are known.</summary>
            public IReadOnlyList<string> IdsOf(Member member) => Chain(Ids.GetValueOrDefault(TypeName), Ids.GetValueOrDefault($"{TypeName}.{member.Name}"));

            public IReadOnlyList<string> TypeIds => Chain(Ids.GetValueOrDefault(TypeName));
        }

        private static IReadOnlyList<string> Chain(params string[] ids) => ids.Where(x => x != null).Distinct(StringComparer.Ordinal).ToList();

        private static string DeclarationFingerprint(BaseTypeDeclarationSyntax type)
        {
            var typeDeclaration = type as TypeDeclarationSyntax;
            return string.Join("|", KindOf(type), Modifiers(type.Modifiers, exclude: null), TypeParameters(typeDeclaration?.TypeParameterList),
                Parameters(typeDeclaration?.ParameterList), string.Join(",", BaseTypes(type)));
        }

        /// <summary>
        /// A code type's declaration, across its parts in the file, read as <see cref="CompareDeclaration"/> reads it, including
        /// what the model can't state: a change the code made there still means the code changed since they last agreed.
        /// </summary>
        private static string CodeDeclarationFingerprint(IReadOnlyList<BaseTypeDeclarationSyntax> parts)
        {
            return string.Join("|", KindOf(parts[0]), Modifiers(parts.SelectMany(x => x.Modifiers), exclude: null),
                TypeParameters((parts[0] as TypeDeclarationSyntax)?.TypeParameterList),
                parts.OfType<TypeDeclarationSyntax>().Select(x => Parameters(x.ParameterList)).FirstOrDefault(x => x.Length > 0) ?? string.Empty,
                string.Join(",", parts.SelectMany(BaseTypes).Distinct(StringComparer.Ordinal).OrderBy(x => x, StringComparer.Ordinal)));
        }

        private static void CompareDeclaration(BaseTypeDeclarationSyntax generated, IReadOnlyList<BaseTypeDeclarationSyntax> disk, TypeContext context)
        {
            var (typeName, file) = (context.TypeName, context.File);
            var location = file.LineOf(disk[0].Identifier.SpanStart);
            void Contradiction(string aspect, string model, string code) =>
                context.Findings.Add(Finding.Contradicted(
                    $"`{typeName}` {aspect}: model {Code(model)}, code {Code(code)}. Location: `{file.RelativePath}:{location}`",
                    file.RelativePath,
                    disk[0],
                    new DriftRow(typeName, context.TypeIds, file.RelativePath, location, $"`{typeName}` {aspect}: model {Code(model)}, code {Code(code)}"),
                    typeName));

            // Only what the designer can state is compared (Modellable): a Service's primary constructor, which holds its
            // injected dependencies, or a handler declared as a record, is nothing the model could say. What the model does
            // state is always compared.
            var modellable = context.Modellable;
            var modelKind = KindOf(generated);
            var codeKind = KindOf(disk[0]);
            if (modelKind != codeKind && (modellable.Kinds == null || modellable.Kinds.Contains(codeKind)))
            {
                Contradiction("kind", modelKind, codeKind);
                return;
            }

            var modelModifiers = Modifiers(generated.Modifiers, exclude: null);
            var codeModifiers = Modifiers(disk.SelectMany(x => x.Modifiers), exclude: null);
            if (modelModifiers != codeModifiers)
            {
                Contradiction("modifiers", modelModifiers, codeModifiers);
            }

            if (generated is TypeDeclarationSyntax generatedType && disk[0] is TypeDeclarationSyntax diskType)
            {
                var modelTypeParameters = TypeParameters(generatedType.TypeParameterList);
                var codeTypeParameters = TypeParameters(diskType.TypeParameterList);
                if (modelTypeParameters != codeTypeParameters && (modellable.TypeParameters || modelTypeParameters.Length > 0))
                {
                    Contradiction("type parameters", modelTypeParameters, codeTypeParameters);
                }

                var modelParameters = Parameters(generatedType.ParameterList);
                var codeParameters = disk.OfType<TypeDeclarationSyntax>().Select(x => Parameters(x.ParameterList)).FirstOrDefault(x => x.Length > 0) ?? string.Empty;
                if (modelParameters != codeParameters && (modellable.ParameterList || modelParameters.Length > 0))
                {
                    Contradiction("parameters", modelParameters, codeParameters);
                }
            }

            var codeBases = new HashSet<string>(disk.SelectMany(BaseTypes), StringComparer.Ordinal);
            var missingBases = BaseTypes(generated).Where(x => !codeBases.Contains(x)).ToList();
            if (missingBases.Count > 0)
            {
                Contradiction("base types", string.Join(", ", BaseTypes(generated)), string.Join(", ", codeBases.OrderBy(x => x, StringComparer.Ordinal)));
            }
        }

        private static void CompareMembers(
            BaseTypeDeclarationSyntax generated,
            IReadOnlyList<Member> modelMembers,
            IReadOnlyList<BaseTypeDeclarationSyntax> disk,
            TypeContext context,
            AccessLevels levels)
        {
            var (typeName, file, findings) = (context.TypeName, context.File, context.Findings);
            var codeMembers = disk.SelectMany(x => Members(x, typeName)).ToList();
            var matched = new HashSet<Member>();
            var unmatchedModel = new List<Member>();

            foreach (var member in modelMembers)
            {
                var candidates = codeMembers.Where(x => !matched.Contains(x) && x.Group == member.Group && x.Name == member.Name).ToList();
                var match = candidates.FirstOrDefault(x => x.Signature == member.Signature) ?? (member.Group is "method" or "constructor" ? null : candidates.FirstOrDefault());
                if (match == null)
                {
                    unmatchedModel.Add(member);
                    continue;
                }

                matched.Add(match);
                CompareMember(member, match, context);
            }

            foreach (var member in unmatchedModel)
            {
                var candidates = codeMembers.Where(x => !matched.Contains(x) && x.Group == member.Group && x.Name == member.Name).ToList();
                var others = unmatchedModel.Count(x => x.Group == member.Group && x.Name == member.Name);
                if (candidates.Count == 1 && others == 1)
                {
                    matched.Add(candidates[0]);
                    CompareMember(member, candidates[0], context);
                    continue;
                }

                findings.Add(Finding.Missing(
                    $"`{typeName}.{member.Key}` ({member.Kind}): modelled, not in the code. Expected in `{file.RelativePath}`",
                    file.RelativePath,
                    new DriftRow($"{typeName}.{member.Key}", context.IdsOf(member), file.RelativePath, 0, $"`{typeName}.{member.Key}` ({member.Kind}) is modelled, but not in the code"),
                    $"{typeName}.{member.Key}"));
            }

            // A code member the model doesn't describe is only reported when the element could model it: a DTO's ToString()
            // or a handler's Handle method is nothing its designer can state, and neither is any field, event or indexer.
            var modellable = context.Modellable;
            var typeHasConstructors = modelMembers.Any(x => x.Group == "constructor");
            foreach (var member in codeMembers.Where(x => !matched.Contains(x)))
            {
                var inPlay = member.Group switch
                {
                    "method" => modellable.Methods && levels.Methods.Contains(member.Access),
                    "property" when member.Kind == "property" => modellable.Properties && levels.Properties.Contains(member.Access),
                    "constructor" => modellable.Constructors && typeHasConstructors && levels.Constructors.Contains(member.Access),
                    "literal" => true,
                    _ => false
                };
                if (inPlay)
                {
                    var line = file.LineOf(member.Node.SpanStart);
                    context.Code[$"{typeName}.{member.Key}"] = new CodeAspect($"{typeName}.{member.Key}", member.Fingerprint);
                    findings.Add(Finding.Undescribed(
                        $"`{typeName}.{member.Key}` ({member.Access} {member.Kind}): not modelled. Location: `{file.RelativePath}:{line}`",
                        file.RelativePath, member.Node, typeName, member.Name, null,
                        new DriftRow(typeName, context.TypeIds, file.RelativePath, line, $"`{typeName}.{member.Key}` ({member.Access} {member.Kind}) isn't modelled"),
                        $"{typeName}.{member.Key}"));
                }
            }
        }

        private static void CompareMember(Member model, Member code, TypeContext context)
        {
            var (typeName, file) = (context.TypeName, context.File);
            var line = file.LineOf(code.Node.SpanStart);
            context.Code[$"{typeName}.{model.Key}"] = new CodeAspect($"{typeName}.{code.Key}", code.Fingerprint);
            void Contradiction(string aspect, string modelValue, string codeValue) =>
                context.Findings.Add(Finding.Contradicted(
                    $"`{typeName}.{model.Key}` {aspect}: model {Code(modelValue)}, code {Code(codeValue)}. Location: `{file.RelativePath}:{line}`",
                    file.RelativePath,
                    code.Node,
                    new DriftRow($"{typeName}.{model.Key}", context.IdsOf(model), file.RelativePath, line, $"`{typeName}.{model.Key}` {aspect}: model {Code(modelValue)}, code {Code(codeValue)}"),
                    $"{typeName}.{model.Key}"));

            if (model.Kind != code.Kind)
            {
                Contradiction("kind", model.Kind, code.Kind);
                return;
            }

            if (model.Type != code.Type)
            {
                Contradiction(model.Group == "method" ? "return type" : "type", model.Type, code.Type);
            }

            if (model.Parameters != code.Parameters)
            {
                Contradiction("parameters", model.Parameters, code.Parameters);
            }

            if (model.TypeParameters != code.TypeParameters)
            {
                Contradiction("type parameters", model.TypeParameters, code.TypeParameters);
            }

            if (model.Modifiers != code.Modifiers)
            {
                Contradiction("modifiers", model.Modifiers, code.Modifiers);
            }

            if (model.Value != code.Value)
            {
                Contradiction("value", model.Value, code.Value);
            }
        }

        private static IReadOnlyList<Member> Members(BaseTypeDeclarationSyntax type, string typeName)
        {
            var result = new List<Member>();
            if (type is EnumDeclarationSyntax @enum)
            {
                foreach (var literal in @enum.Members)
                {
                    result.Add(new Member("literal", "literal", literal.Identifier.Text, literal.Identifier.Text, CodeLocator.Public, literal)
                    {
                        Value = literal.EqualsValue == null ? string.Empty : CodeText.Canonicalize(literal.EqualsValue.Value.ToString())
                    });
                }

                return result;
            }

            if (type is not TypeDeclarationSyntax declaration)
            {
                return result;
            }

            var isInterface = declaration is InterfaceDeclarationSyntax;
            foreach (var member in declaration.Members)
            {
                var access = CodeLocator.AccessOf(member.Modifiers, isInterface);
                switch (member)
                {
                    case MethodDeclarationSyntax { ExplicitInterfaceSpecifier: null } method:
                        var methodKey = $"{method.Identifier.Text}({CodeLocator.ParameterKeys(method.ParameterList)})";
                        result.Add(new Member("method", "method", method.Identifier.Text, methodKey, access, method)
                        {
                            Signature = TypeKeys(method.ParameterList),
                            Type = TypeKey(method.ReturnType),
                            Parameters = Parameters(method.ParameterList),
                            TypeParameters = TypeParameters(method.TypeParameterList),
                            Modifiers = Modifiers(method.Modifiers, exclude: "partial")
                        });
                        break;
                    case ConstructorDeclarationSyntax constructor when !constructor.Modifiers.Any(SyntaxKind.StaticKeyword):
                        result.Add(new Member("constructor", "constructor", typeName, $"{constructor.Identifier.Text}({CodeLocator.ParameterKeys(constructor.ParameterList)})", access, constructor)
                        {
                            Signature = TypeKeys(constructor.ParameterList),
                            Parameters = Parameters(constructor.ParameterList),
                            Modifiers = Modifiers(constructor.Modifiers, exclude: "partial")
                        });
                        break;
                    case PropertyDeclarationSyntax { ExplicitInterfaceSpecifier: null } property:
                        result.Add(new Member("property", "property", property.Identifier.Text, property.Identifier.Text, access, property)
                        {
                            Type = TypeKey(property.Type),
                            Modifiers = Modifiers(property.Modifiers, exclude: "virtual partial")
                        });
                        break;
                    case IndexerDeclarationSyntax { ExplicitInterfaceSpecifier: null } indexer:
                        result.Add(new Member("indexer", "indexer", "this", $"this[{CodeLocator.ParameterKeys(indexer.ParameterList)}]", access, indexer)
                        {
                            Signature = TypeKeys(indexer.ParameterList),
                            Type = TypeKey(indexer.Type),
                            Parameters = Parameters(indexer.ParameterList),
                            Modifiers = Modifiers(indexer.Modifiers, exclude: "virtual partial")
                        });
                        break;
                    case EventDeclarationSyntax { ExplicitInterfaceSpecifier: null } @event:
                        result.Add(new Member("property", "event", @event.Identifier.Text, @event.Identifier.Text, access, @event)
                        {
                            Type = TypeKey(@event.Type),
                            Modifiers = Modifiers(@event.Modifiers, exclude: "virtual partial")
                        });
                        break;
                    case BaseFieldDeclarationSyntax field:
                        var kind = field is EventFieldDeclarationSyntax ? "event" : "field";
                        foreach (var variable in field.Declaration.Variables)
                        {
                            result.Add(new Member("property", kind, variable.Identifier.Text, variable.Identifier.Text, access, field)
                            {
                                Type = TypeKey(field.Declaration.Type),
                                Modifiers = Modifiers(field.Modifiers, exclude: "virtual partial")
                            });
                        }

                        break;
                }
            }

            return result;
        }

        private static AccessLevels LevelsInPlay(IEnumerable<BaseTypeDeclarationSyntax> generatedTypes)
        {
            var levels = new AccessLevels();
            foreach (var member in generatedTypes.SelectMany(x => Members(x, CodeLocator.TypePath(x))))
            {
                switch (member.Group)
                {
                    case "method":
                        levels.Methods.Add(member.Access);
                        break;
                    case "constructor":
                        levels.Constructors.Add(member.Access);
                        break;
                    case "property" when member.Kind == "property":
                        levels.Properties.Add(member.Access);
                        break;
                }
            }

            return levels;
        }

        private static string KindOf(BaseTypeDeclarationSyntax type) => type switch
        {
            RecordDeclarationSyntax record => record.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword) ? "record struct" : "record",
            ClassDeclarationSyntax => "class",
            StructDeclarationSyntax => "struct",
            InterfaceDeclarationSyntax => "interface",
            EnumDeclarationSyntax => "enum",
            _ => type.Kind().ToString()
        };

        private static string Modifiers(IEnumerable<SyntaxToken> modifiers, string exclude)
        {
            var excluded = new HashSet<string>((exclude ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries), StringComparer.Ordinal);
            return string.Join(" ", modifiers.Select(x => x.Text)
                .Where(x => !excluded.Contains(x))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(x => Array.IndexOf(ModifierOrder, x) is var index and >= 0 ? index : int.MaxValue)
                .ThenBy(x => x, StringComparer.Ordinal));
        }

        private static string TypeParameters(TypeParameterListSyntax list)
        {
            return list == null ? string.Empty : $"<{string.Join(",", list.Parameters.Select(x => x.Identifier.Text))}>";
        }

        /// <summary>A parameter list as it reads: modifiers, type, name and default, e.g. <c>(ref int id,string area="x")</c>.</summary>
        private static string Parameters(BaseParameterListSyntax list)
        {
            if (list == null)
            {
                return string.Empty;
            }

            var parameters = list.Parameters.Select(x =>
            {
                var modifiers = string.Join(" ", x.Modifiers.Select(m => m.Text));
                var text = $"{(modifiers.Length > 0 ? modifiers + " " : string.Empty)}{(x.Type == null ? string.Empty : TypeKey(x.Type) + " ")}{x.Identifier.Text}";
                return x.Default == null ? text : $"{text}={CodeText.Canonicalize(x.Default.Value.ToString())}";
            });
            return list is BracketedParameterListSyntax ? $"[{string.Join(",", parameters)}]" : $"({string.Join(",", parameters)})";
        }

        private static string TypeKeys(BaseParameterListSyntax list)
        {
            return list == null ? string.Empty : string.Join(",", list.Parameters.Select(x => string.Join(" ", x.Modifiers.Select(m => m.Text).Append(x.Type == null ? string.Empty : TypeKey(x.Type)))));
        }

        private static IReadOnlyList<string> BaseTypes(BaseTypeDeclarationSyntax type)
        {
            return type.BaseList?.Types.Select(x => TypeKey(x.Type)).ToList() ?? new List<string>();
        }

        /// <summary>A type as the model means it, whatever its spelling.</summary>
        public static string TypeKey(TypeSyntax type) => type switch
        {
            null => string.Empty,
            PredefinedTypeSyntax x => x.Keyword.Text,
            NullableTypeSyntax x => $"{TypeKey(x.ElementType)}?",
            ArrayTypeSyntax x => TypeKey(x.ElementType) + string.Concat(x.RankSpecifiers.Select(r => $"[{new string(',', r.Rank - 1)}]")),
            QualifiedNameSyntax x => TypeKey(x.Right),
            AliasQualifiedNameSyntax x => TypeKey(x.Name),
            GenericNameSyntax { Identifier.Text: "Nullable", TypeArgumentList.Arguments.Count: 1 } x => $"{TypeKey(x.TypeArgumentList.Arguments[0])}?",
            GenericNameSyntax x => $"{x.Identifier.Text}<{string.Join(",", x.TypeArgumentList.Arguments.Select(TypeKey))}>",
            IdentifierNameSyntax x => Aliases.TryGetValue(x.Identifier.Text, out var alias) ? alias : x.Identifier.Text,
            TupleTypeSyntax x => $"({string.Join(",", x.Elements.Select(e => $"{TypeKey(e.Type)}{(e.Identifier.IsKind(SyntaxKind.None) ? string.Empty : " " + e.Identifier.Text)}"))})",
            _ => new string(type.ToString().Where(c => !char.IsWhiteSpace(c)).ToArray())
        };

        private static string Code(string value) => string.IsNullOrEmpty(value) ? "(none)" : $"`{value}`";

        private sealed class AccessLevels
        {
            public HashSet<string> Methods { get; } = new(StringComparer.Ordinal) { CodeLocator.Public };
            public HashSet<string> Properties { get; } = new(StringComparer.Ordinal) { CodeLocator.Public };
            public HashSet<string> Constructors { get; } = new(StringComparer.Ordinal) { CodeLocator.Public };
        }

        private sealed class Member
        {
            public Member(string group, string kind, string name, string key, string access, SyntaxNode node)
            {
                Group = group;
                Kind = kind;
                Name = name;
                Key = key;
                Access = access;
                Node = node;
            }

            /// <summary>Members of one group can stand for each other: a modelled property may be a field in code.</summary>
            public string Group { get; }
            public string Kind { get; }
            public string Name { get; }
            public string Key { get; }
            public string Access { get; }
            public SyntaxNode Node { get; }
            public string Signature { get; init; } = string.Empty;
            public string Type { get; init; } = string.Empty;
            public string Parameters { get; init; } = string.Empty;
            public string TypeParameters { get; init; } = string.Empty;
            public string Modifiers { get; init; } = string.Empty;
            public string Value { get; init; } = string.Empty;

            /// <summary>Everything the comparison reads, so it changes whenever the template's output for the member does.</summary>
            public string Fingerprint => string.Join("|", Kind, Access, Type, Parameters, TypeParameters, Modifiers, Value);
        }
    }
}
