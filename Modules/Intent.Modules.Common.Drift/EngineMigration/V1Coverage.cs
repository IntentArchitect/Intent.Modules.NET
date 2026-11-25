using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intent.Modules.Common.Drift.EngineMigration
{
    /// <summary>
    /// A type element of the model, with the names of its children, as the drift engine locates it in the code: by name. And
    /// whether it has a constructor among them, as v1 compared a type's constructors only then.
    /// </summary>
    internal sealed record ModelledType(string Name, string ElementType, IReadOnlyCollection<string> Members, bool HasConstructors = false)
    {
        /// <summary>For a handler, the names of the messages it subscribes to, which the engine finds its subscriptions' code by.</summary>
        public IReadOnlyCollection<string> Messages { get; init; } = Array.Empty<string>();
    }

    /// <summary>
    /// The Exceptions rows that keep the converted opt-in rows to what v1 reported (user decision, 2026-11-24: "Except what v1
    /// skipped"). A row is a glob, so it reports every unmodelled type in its files, where v1's <c>own</c> entry reported only
    /// what its find matched; and the member row reports public fields, which v1 never did. So, on the code as it is:
    /// <list type="bullet">
    /// <item>A type in a type row's files that the model doesn't name, and that no <c>own</c> find matched, is an exception. So
    /// is one a find matched in a contract file: v1 counted a file a contract template wrote as modelled.</item>
    /// <item>A type a find matched elsewhere stays reported, as v1 reported it: the move doesn't hide v1's own findings.</item>
    /// <item>A public field of a modelled type whose element states fields, which the model doesn't name, is an exception.</item>
    /// </list>
    /// A type added later is still reported, as the row's glob says. Types are read from C#, v1's code index; a Razor or Vue
    /// file is the one component of its file's name (<see cref="ComponentName"/>), as v1's file finds and the engine both see it.
    /// </summary>
    internal static class V1Coverage
    {
        /// <summary>The element types the drift engine's stock packs locate a type for (their conventions), by specialization name.</summary>
        public static readonly IReadOnlyCollection<string> TypeElements = new HashSet<string>(StringComparer.Ordinal)
        {
            "Class", "Enum", "Domain Event", "Value Object", "Repository", "Domain Service", "Data Contract",
            "DTO", "Command", "Query", "Service", "Service Proxy", "Message", "Integration Command", "Eventing DTO",
            "Integration Event Handler", "Domain Event Handler"
        };

        /// <summary>The element types whose conventions state fields, so <c>unmodelled.member</c> reports their public fields.</summary>
        private static readonly HashSet<string> FieldElements = new(StringComparer.Ordinal)
        {
            "Class", "Domain Event", "Value Object", "Data Contract", "DTO", "Command", "Query", "Message", "Integration Command", "Eventing DTO"
        };

        /// <summary>
        /// The member kinds each element type's convention states, as the stock packs give them (<c>intent.domain</c>,
        /// <c>intent.services</c>): the engine's <c>unmodelled.member</c> reports only those.
        /// </summary>
        private static readonly IReadOnlyDictionary<string, string[]> States = new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["Class"] = new[] { "property", "field", "constructor", "method" },
            ["Enum"] = new[] { "enumMember" },
            ["Repository"] = new[] { "method" },
            ["Domain Service"] = new[] { "method" },
            ["Service"] = new[] { "method" },
            ["Service Proxy"] = new[] { "method" },
            ["Integration Event Handler"] = new[] { "method" },
            ["Domain Event Handler"] = new[] { "method" }
        };

        private static readonly string[] Fields = { "property", "field" };

        /// <summary>
        /// The element types that may be a class or an interface: with no type of its name, the element is the interface the
        /// language names it (<c>I{Name}</c>).
        /// </summary>
        private static readonly HashSet<string> InterfaceElements = new(StringComparer.Ordinal) { "Service", "Service Proxy", "Repository", "Domain Service" };

        /// <summary>
        /// The element types whose conventions state methods where v1's contracts compared none: a handler stated its modifiers
        /// only, so v1 never reported its public methods.
        /// </summary>
        private static readonly HashSet<string> HandlerElements = new(StringComparer.Ordinal) { "Integration Event Handler", "Domain Event Handler" };

        public const string ContractFileReason = "Declared in a file v1 generated a modelled type into, which v1 counted as modelled";
        public const string FieldReason = "A public field, which v1 never reported";
        public const string CopyReason = "Another declaration of a modelled type's name, which v1 counted as modelled";
        public const string HandlerMethodReason = "A public method of a handler, whose methods v1 never compared";
        public const string UnreadReason = "Projects v1 never read: none of its recipes' files reach them, and it generated no contract file into them";
        public const string CopiesReason = "Code v1 never read that declares copies of modelled types: none of its recipes' files reach it, and in scope each copy would be another type of the modelled name";
        public const string ModelledUnreadReason = "Code v1 never read that declares a modelled type, read for the model's claims only: v1 never checked it, and out of scope the type couldn't be located";
        public const string UncomparedReason = "A public member of a modelled type v1 generated no contract for, so it never compared its members";
        public const string ConstructorReason = "A public constructor of a type the model states none for, which v1 never reported";

        /// <summary>
        /// One Out row for the projects v1 never read, or <see langword="null"/>: a folder with a <c>.csproj</c> and C# code, where
        /// no file is one a recipe's globs reach (<see cref="ConvertedRecipes.V1Read"/>) or a contract file. The <c>**</c> In row
        /// would bring them in, and the engine would report code v1 never checked against a model never drawn from it. A ClientB
        /// application keeps a legacy copy of its host project, the same classes in the same namespaces, which its recipe left
        /// out on purpose: read with the real project, each pair of copies was one type to the engine, and its v1-clean model had
        /// 44 errors. A project inside one already out isn't listed again; a project at the output folder's root never is.
        /// </summary>
        public static ScopeRowSpec UnreadProjects(CodeFiles code, IReadOnlyCollection<string> v1Read, IReadOnlyCollection<string> contractFiles)
        {
            var read = v1Read.Select(x => new Glob(x)).ToList();
            var contracts = new HashSet<string>(contractFiles, StringComparer.OrdinalIgnoreCase);
            var files = code.AllFiles();
            var projects = files.Where(x => x.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) && x.Contains('/'))
                .Select(x => x[..x.LastIndexOf('/')])
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x, StringComparer.Ordinal);
            var unread = new List<string>();
            foreach (var project in projects)
            {
                if (unread.Any(x => project.StartsWith(x + "/", StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                var inside = files.Where(x => x.StartsWith(project + "/", StringComparison.OrdinalIgnoreCase)).ToList();
                if (inside.Any(CodeFiles.IsCSharp) && !inside.Any(x => contracts.Contains(x) || read.Any(g => g.IsMatch(x))))
                {
                    unread.Add(project);
                }
            }

            return unread.Count == 0 ? null : new ScopeRowSpec(unread.Select(x => $"{x}/**").ToList(), "Out", UnreadReason);
        }

        /// <summary>
        /// One Out row for the code v1 never read, inside projects it did, that declares another type of a modelled name the code
        /// it read declares, or <see langword="null"/>. Each is the highest folder holding no file v1 read, else the file. A
        /// migrated EF6 application keeps legacy copies of its entities in a <c>Data/Models</c> folder beside the
        /// <c>Data/Contracts</c> its recipe read, which its recipe's note says were left out on purpose: in scope, each entity had
        /// two types, so none could be verified and the real ones read as unmodelled. Only copies go: a folder v1 didn't read
        /// may hold code the engine's readers need (a project's <c>Startup.cs</c>), and a modelled type declared only there is
        /// left in, so it can still be located.
        /// </summary>
        public static ScopeRowSpec UnreadCopies(CodeFiles code, IReadOnlyCollection<string> v1Read, IReadOnlyCollection<string> contractFiles,
            IReadOnlyList<ModelledType> model, IEnumerable<ScopeRowSpec> scope)
        {
            var read = v1Read.Select(x => new Glob(x)).ToList();
            var contracts = new HashSet<string>(contractFiles, StringComparer.OrdinalIgnoreCase);
            bool ReadByV1(string file) => contracts.Contains(file) || read.Any(g => g.IsMatch(file));

            var files = EngineRead(code, scope).ToList();
            var readFiles = files.Where(ReadByV1).ToList();
            var declared = readFiles.SelectMany(x => Declarations(code, x)).ToLookup(x => x.Identifier.Text, StringComparer.Ordinal);
            var modelled = Modelled(model, declared);

            var copies = files.Where(x => !ReadByV1(x) && Declarations(code, x).Any(d => modelled.Contains(d.Identifier.Text) && declared.Contains(d.Identifier.Text))).ToList();
            if (copies.Count == 0)
            {
                return null;
            }

            // A folder holds a file v1 read when one is in it or below it.
            var readFolders = new HashSet<string>(readFiles.SelectMany(Folders), StringComparer.OrdinalIgnoreCase);
            var globs = new List<string>();
            foreach (var file in copies)
            {
                var folder = Folders(file).TakeWhile(x => !readFolders.Contains(x)).LastOrDefault();
                globs.Add(folder is null ? file : $"{folder}/**");
            }

            var distinct = globs.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x, StringComparer.Ordinal).ToList();
            return new ScopeRowSpec(distinct, "Out", CopiesReason);
        }

        /// <summary>
        /// One Context row for the files an Out row takes out that declare a type the model names and no file in scope declares,
        /// or <see langword="null"/>. v1 never read them, so it never checked those types; out of scope the engine couldn't
        /// locate them and would say so for each. A migrated application's Eventing package models the commands it sends to and
        /// takes from a shared library project its recipes never reached. Read for context, the types are located and the
        /// model's claims on them checked, and nothing else in those files is reported, as v1 reported nothing there.
        /// </summary>
        public static ScopeRowSpec UnreadModelled(CodeFiles code, IReadOnlyList<ModelledType> model, IEnumerable<ScopeRowSpec> scope)
        {
            var inScope = new HashSet<string>(EngineRead(code, scope), StringComparer.OrdinalIgnoreCase);
            var all = code.AllFiles().Where(CodeFiles.IsCSharp)
                .SelectMany(file => Declarations(code, file).Select(x => (File: file, Declaration: x)))
                .ToList();
            var inScopeDeclarations = all.Where(x => inScope.Contains(x.File)).Select(x => x.Declaration).ToLookup(x => x.Identifier.Text, StringComparer.Ordinal);
            var allDeclarations = all.Select(x => x.Declaration).ToLookup(x => x.Identifier.Text, StringComparer.Ordinal);

            // The names the engine would locate with every file read, that no file in scope declares.
            var wanted = Modelled(model, allDeclarations).Where(x => !inScopeDeclarations.Contains(x)).ToHashSet(StringComparer.Ordinal);
            var files = all.Where(x => !inScope.Contains(x.File) && wanted.Contains(x.Declaration.Identifier.Text))
                .Select(x => x.File)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x, StringComparer.Ordinal)
                .ToList();
            return files.Count == 0 ? null : new ScopeRowSpec(files, "Context", ModelledUnreadReason);
        }

        /// <summary>The folders a file is in, innermost first, the output folder's root aside.</summary>
        private static IEnumerable<string> Folders(string file)
        {
            for (var slash = file.LastIndexOf('/'); slash > 0; slash = file.LastIndexOf('/', slash - 1))
            {
                yield return file[..slash];
            }
        }

        private static IEnumerable<BaseTypeDeclarationSyntax> Declarations(CodeFiles code, string file) =>
            code.Get(file)?.Syntax?.DescendantNodes().OfType<BaseTypeDeclarationSyntax>() ?? Enumerable.Empty<BaseTypeDeclarationSyntax>();

        /// <summary>The C# files the drift engine reads: those no Out row takes out of scope.</summary>
        private static IEnumerable<string> EngineRead(CodeFiles code, IEnumerable<ScopeRowSpec> scope)
        {
            var outGlobs = (scope ?? Enumerable.Empty<ScopeRowSpec>()).Where(x => x.Scope == "Out").SelectMany(x => x.Globs).Select(x => new Glob(x)).ToList();
            return code.AllFiles().Where(x => CodeFiles.IsCSharp(x) && !outGlobs.Any(g => g.IsMatch(x)));
        }

        /// <param name="code">The application's code, as v1 read it.</param>
        /// <param name="contractFiles">The contract files, relative to the code's root with <c>/</c> separators.</param>
        /// <param name="notes">Gets a line for each recipe file whose finds couldn't run, whose rows then get no exceptions.</param>
        /// <param name="components">The names of the User Interface designer's components (<see cref="ComponentElements"/>).</param>
        public static IReadOnlyList<ExceptionRowSpec> Exceptions(CodeFiles code, IEnumerable<RecipeSource> recipes, ConvertedRecipes rows,
            IReadOnlyCollection<string> contractFiles, IReadOnlyList<ModelledType> model, ICollection<string> notes, IReadOnlyCollection<string> components = null)
        {
            var contracts = new HashSet<string>(contractFiles, StringComparer.OrdinalIgnoreCase);
            // The engine never reports code a Context row decides as unmodelled, as it doesn't an Out row's.
            var outGlobs = rows.Scope.Where(x => x.Scope is "Out" or "Context").SelectMany(x => x.Globs).Select(x => new Glob(x)).ToList();
            bool Checked(string file) => CodeFiles.IsCSharp(file) && !outGlobs.Any(x => x.IsMatch(file));

            var declarations = EngineRead(code, rows.Scope)
                .SelectMany(x => code.Get(x)?.Syntax?.DescendantNodes().OfType<BaseTypeDeclarationSyntax>() ?? Enumerable.Empty<BaseTypeDeclarationSyntax>())
                .ToLookup(x => x.Identifier.Text, StringComparer.Ordinal);
            var modelled = Modelled(model, declarations);

            var (found, failed) = FoundByV1(code, recipes, notes);
            var byReason = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            var excepted = new HashSet<string>(StringComparer.Ordinal);
            void Except(string reason, string match)
            {
                if (excepted.Add(match))
                {
                    (byReason.TryGetValue(reason, out var matches) ? matches : byReason[reason] = new List<string>()).Add(match);
                }
            }

            // A recipe whose finds failed leaves its types reported: what v1 skipped there isn't known. A file several rows cover
            // names each row's conventions.
            var typeRows = rows.OptIn.Where(x => x.Rule == RecipeConversion.UnmodelledType && !x.Globs.Any(failed.Contains))
                .Select(x => (Row: x, Globs: x.Globs.Select(g => new Glob(g)).ToList()))
                .ToList();
            // A type nested in another is part of it, which the engine never reports on its own, so only top-level types count.
            var covered = code.AllFiles().Where(Checked)
                .Select(file => (File: file, Covering: typeRows.Where(x => x.Globs.Any(g => g.IsMatch(file))).Select(x => x.Row.Description).Distinct().ToList()))
                .Where(x => x.Covering.Count > 0)
                .Select(x => (x.File, x.Covering, Types: TopLevel(code, x.File)))
                .ToList();
            var coveredDeclarations = covered.SelectMany(x => x.Types).ToLookup(x => x.Identifier.Text, StringComparer.Ordinal);
            string Reason(IEnumerable<string> covering) => $"v1's conventions over these files ({string.Join(", ", covering)}) didn't report it";

            // A Razor or Vue file is one component of its file's name to the engine. v1 found it by a file find, so only its name
            // can be matched: a component the model names is located, and one no find matched v1 never reported (eShop10's UI
            // conventions left out App.razor and Routes.razor, the files holding <html> and <Router>).
            foreach (var file in code.AllFiles().Where(x => ComponentName(x) != null && !outGlobs.Any(g => g.IsMatch(x))))
            {
                var covering = typeRows.Where(x => x.Globs.Any(g => g.IsMatch(file))).Select(x => x.Row.Description).Distinct().ToList();
                var name = ComponentName(file);
                if (covering.Count > 0 && !modelled.Contains(name) && !(components?.Contains(name) ?? false) && !found.Contains(name))
                {
                    Except(Reason(covering), name);
                }
            }

            foreach (var (file, covering, types) in covered)
            {
                var reason = Reason(covering);
                // By name, as an exception matches: a partial type a find matched in one of its files was found.
                foreach (var type in types)
                {
                    var name = type.Identifier.Text;
                    if (modelled.Contains(name))
                    {
                        // The engine locates one type per element, and one interface per name a located class implements, so a
                        // further type of the name in these files is reported as unmodelled: a second DTO of a modelled DTO's
                        // name in another project, an unused interface of a located class's interface's name, or Result<T>
                        // beside a modelled Result.
                        if (Copies(coveredDeclarations[name]) > Math.Max(1, model.Count(x => x.Name == name)))
                        {
                            Except(CopyReason, name);
                        }
                    }
                    else if (!found.Contains(name))
                    {
                        Except(reason, name);
                    }
                    else if (contracts.Contains(file))
                    {
                        Except(ContractFileReason, name);
                    }
                }
            }

            if (rows.OptIn.FirstOrDefault(x => x.Rule == RecipeConversion.UnmodelledMember) is { } members)
            {
                var stateFields = model.Where(x => FieldElements.Contains(x.ElementType)).ToLookup(x => x.Name, StringComparer.Ordinal);
                var handlers = model.Where(x => HandlerElements.Contains(x.ElementType)).ToLookup(x => x.Name, StringComparer.Ordinal);
                var subscriptionCode = SubscriptionCode(code, rows.Scope, handlers);
                var byName = model.ToLookup(x => x.Name, StringComparer.Ordinal);
                var compared = new HashSet<string>(contractFiles.Where(CodeFiles.IsCSharp).SelectMany(x => Declarations(code, x)).Select(x => x.Identifier.Text), StringComparer.Ordinal);
                foreach (var file in code.Matching(members.Globs.Select(x => new Glob(x)).ToList()).Where(Checked))
                {
                    var syntax = code.Get(file)?.Syntax;
                    foreach (var type in syntax?.DescendantNodes().OfType<TypeDeclarationSyntax>() ?? Enumerable.Empty<TypeDeclarationSyntax>())
                    {
                        if (type is InterfaceDeclarationSyntax)
                        {
                            continue;
                        }

                        var elements = stateFields[type.Identifier.Text].ToList();
                        foreach (var field in PublicFields(type).Where(x => elements.Count > 0 && !elements.Any(e => e.Members.Contains(x))))
                        {
                            Except(FieldReason, $"{type.Identifier.Text}.{field}");
                        }

                        // A handler's method the engine finds a subscription in is located, so never reported (SubscriptionCode).
                        var handler = handlers[type.Identifier.Text].ToList();
                        foreach (var method in PublicMethods(type).Where(x => handler.Count > 0 && !handler.Any(e => e.Members.Contains(x.Identifier.Text))
                                     && !subscriptionCode.Contains(x)))
                        {
                            Except(HandlerMethodReason, $"{type.Identifier.Text}.{method.Identifier.Text}");
                        }
                    }

                    // v1 compared a modelled type's members only in the contract file it generated for it, so one it generated
                    // none for (a migrated EF6 application's T4-generated entities) had none reported. Where it compared them,
                    // a constructor counted only when the model stated one for the type. Members the engine locates match
                    // nothing, and the move's correction pass drops them as stale.
                    foreach (var type in syntax?.DescendantNodes().OfType<BaseTypeDeclarationSyntax>() ?? Enumerable.Empty<BaseTypeDeclarationSyntax>())
                    {
                        var name = type.Identifier.Text;
                        // A handler's methods are excepted above, whether v1 compared it or not.
                        var elements = byName[name].Where(x => TypeElements.Contains(x.ElementType) && !HandlerElements.Contains(x.ElementType)).ToList();
                        if (type is InterfaceDeclarationSyntax || elements.Count == 0)
                        {
                            continue;
                        }

                        if (!compared.Contains(name))
                        {
                            var kinds = elements.SelectMany(x => States.TryGetValue(x.ElementType, out var stated) ? stated : Fields).ToHashSet(StringComparer.Ordinal);
                            foreach (var member in PublicMembers(type, kinds).Where(x => !elements.Any(e => e.Members.Contains(x))))
                            {
                                Except(UncomparedReason, $"{name}.{member}");
                            }
                        }
                        else if (type is TypeDeclarationSyntax declared && !elements.Any(x => x.HasConstructors)
                                 && elements.Any(x => States.TryGetValue(x.ElementType, out var stated) && stated.Contains("constructor"))
                                 && declared.Members.OfType<ConstructorDeclarationSyntax>().Any(x => x.Modifiers.Any(SyntaxKind.PublicKeyword)))
                        {
                            Except(ConstructorReason, $"{name}.{name}");
                        }
                    }
                }
            }

            return byReason.Select(x => new ExceptionRowSpec(x.Key is FieldReason or HandlerMethodReason or UncomparedReason or ConstructorReason
                ? RecipeConversion.UnmodelledMember
                : RecipeConversion.UnmodelledType, x.Value, x.Key)).ToList();
        }

        /// <summary>
        /// The modelled type names the drift engine never reports as unmodelled: each element of the name has a type to locate,
        /// and the code declares no further one. A converted exception naming one matches nothing (ClientB's v1 exceptions for
        /// <c>Container</c>, which covered a relationship its conventions found, and <c>Applicant</c>, which covered a
        /// generation filter). Only the files the engine reads count: a copy in a project out of scope is never located.
        /// </summary>
        public static IReadOnlyCollection<string> NeverUnmodelled(CodeFiles code, IReadOnlyList<ModelledType> model, IEnumerable<ScopeRowSpec> scope = null)
        {
            var declarations = EngineRead(code, scope)
                .SelectMany(x => code.Get(x)?.Syntax?.DescendantNodes().OfType<BaseTypeDeclarationSyntax>() ?? Enumerable.Empty<BaseTypeDeclarationSyntax>())
                .ToLookup(x => x.Identifier.Text, StringComparer.Ordinal);
            return Modelled(model, declarations)
                .Where(x => declarations.Contains(x) && Copies(declarations[x].Where(d => d.Parent is not BaseTypeDeclarationSyntax)) <= Math.Max(1, model.Count(m => m.Name == x)))
                .ToHashSet(StringComparer.Ordinal);
        }

        /// <summary>The types a file declares outside any other type.</summary>
        private static List<BaseTypeDeclarationSyntax> TopLevel(CodeFiles code, string file) =>
            code.Get(file)?.Syntax?.DescendantNodes().OfType<BaseTypeDeclarationSyntax>().Where(x => x.Parent is not BaseTypeDeclarationSyntax).ToList()
            ?? new List<BaseTypeDeclarationSyntax>();

        /// <summary>
        /// The <c>Type.Member</c> matches the drift engine never reports as unmodelled: a member every type has
        /// (<c>Equals</c>, <c>GetHashCode</c>, <c>ToString</c>), and one the code declares only as overrides. A converted v1
        /// exception naming one (an entity's identity comparison) would match nothing and warn on the first run.
        /// </summary>
        public static IReadOnlyCollection<string> NeverUnmodelledMembers(CodeFiles code, IEnumerable<string> matches, IEnumerable<ScopeRowSpec> scope = null)
        {
            var members = EngineRead(code, scope)
                .SelectMany(x => code.Get(x)?.Syntax?.DescendantNodes().OfType<TypeDeclarationSyntax>() ?? Enumerable.Empty<TypeDeclarationSyntax>())
                .SelectMany(type => type.Members.Select(member => (Key: $"{type.Identifier.Text}.{MemberName(member)}", Override: member.Modifiers.Any(SyntaxKind.OverrideKeyword))))
                .ToLookup(x => x.Key, x => x.Override, StringComparer.Ordinal);
            return matches
                .Where(x => x.LastIndexOf('.') is var dot and > 0 && (EveryTypeHas.Contains(x.Substring(dot + 1)) || members.Contains(x) && members[x].All(o => o)))
                .ToHashSet(StringComparer.Ordinal);
        }

        /// <summary>The members every type has, which the engine's <c>unmodelled.member</c> never reports.</summary>
        private static readonly HashSet<string> EveryTypeHas = new(StringComparer.Ordinal) { "Equals", "GetHashCode", "ToString" };

        private static string MemberName(MemberDeclarationSyntax member) => member switch
        {
            MethodDeclarationSyntax method => method.Identifier.Text,
            PropertyDeclarationSyntax property => property.Identifier.Text,
            EventDeclarationSyntax @event => @event.Identifier.Text,
            _ => null
        };

        /// <summary>
        /// How many types the declarations are: a partial type's parts, in its namespace and enclosing types, are one; a generic
        /// type is another type than the one of its name without type parameters, as the engine reads them.
        /// </summary>
        private static int Copies(IEnumerable<BaseTypeDeclarationSyntax> declarations) =>
            declarations.Select(x => string.Join(".", x.AncestorsAndSelf().Select(a => a switch
            {
                BaseNamespaceDeclarationSyntax ns => ns.Name.ToString(),
                TypeDeclarationSyntax { TypeParameterList.Parameters.Count: > 0 } generic => $"{generic.Identifier.Text}`{generic.TypeParameterList.Parameters.Count}",
                BaseTypeDeclarationSyntax t => t.Identifier.Text,
                _ => null
            }).Where(n => n != null).Reverse())).Distinct(StringComparer.Ordinal).Count();

        private static string LastName(TypeSyntax type) => type switch
        {
            QualifiedNameSyntax qualified => qualified.Right.Identifier.Text,
            GenericNameSyntax generic => generic.Identifier.Text,
            SimpleNameSyntax simple => simple.Identifier.Text,
            NullableTypeSyntax nullable => LastName(nullable.ElementType),
            _ => type.ToString()
        };

        /// <summary>
        /// The names of the types the drift engine locates for the model's elements: a type of an element's name; for an element
        /// that may be a class or an interface, with no type of its name, the interface <c>I{Name}</c> and the classes
        /// implementing it; and the interfaces of every class located, which are that class's code.
        /// </summary>
        private static HashSet<string> Modelled(IReadOnlyList<ModelledType> model, ILookup<string, BaseTypeDeclarationSyntax> declarations)
        {
            var modelled = new HashSet<string>(model.Select(x => x.Name), StringComparer.Ordinal);
            var contracts = model.Where(x => InterfaceElements.Contains(x.ElementType) && !declarations.Contains(x.Name)).Select(x => $"I{x.Name}")
                .Concat(model.Where(x => InterfaceElements.Contains(x.ElementType) && declarations[x.Name].All(d => d is InterfaceDeclarationSyntax)).Select(x => x.Name))
                .Where(x => declarations[x].Any(d => d is InterfaceDeclarationSyntax))
                .ToHashSet(StringComparer.Ordinal);
            modelled.UnionWith(contracts);
            modelled.UnionWith(declarations.Where(x => x.Any(d => d is not InterfaceDeclarationSyntax && Bases(d).Any(contracts.Contains))).Select(x => x.Key));

            // A located class's interfaces, its base classes' and theirs in turn; a base class itself isn't located.
            var classes = modelled.Where(x => declarations[x].Any(d => d is not InterfaceDeclarationSyntax)).ToList();
            var visited = new HashSet<string>(classes, StringComparer.Ordinal);
            var pending = new Queue<string>(classes);
            while (pending.TryDequeue(out var name))
            {
                foreach (var @base in declarations[name].SelectMany(Bases).Where(x => declarations.Contains(x)))
                {
                    if (declarations[@base].Any(d => d is InterfaceDeclarationSyntax))
                    {
                        modelled.Add(@base);
                    }

                    if (visited.Add(@base))
                    {
                        pending.Enqueue(@base);
                    }
                }
            }

            return modelled;
        }

        /// <summary>The names a type's base list gives, without namespaces or type arguments.</summary>
        private static IEnumerable<string> Bases(BaseTypeDeclarationSyntax type) =>
            type.BaseList?.Types.Select(x => x.Type switch
            {
                QualifiedNameSyntax qualified => qualified.Right.Identifier.Text,
                SimpleNameSyntax simple => simple.Identifier.Text,
                _ => x.Type.ToString()
            }) ?? Enumerable.Empty<string>();

        /// <summary>
        /// The top-level types each <c>own</c> entry's find matched, by name, and the type rows' globs of the recipe files whose
        /// finds couldn't run. A find that ends inside a type matches that type; one that ends at a file matches its types.
        /// </summary>
        private static (HashSet<string> Found, HashSet<string> Failed) FoundByV1(CodeFiles code, IEnumerable<RecipeSource> recipes, ICollection<string> notes)
        {
            var found = new HashSet<string>(StringComparer.Ordinal);
            var failed = new HashSet<string>(StringComparer.Ordinal);
            foreach (var recipe in recipes)
            {
                var file = RecipeFile.Parse(recipe.Json, recipe.FileName, Array.Empty<FactKind>());
                var engine = new RecipeEngine(code, file.Files);
                foreach (var own in file.Own)
                {
                    try
                    {
                        foreach (var hit in engine.Own(own))
                        {
                            var type = hit.Hit.Node?.AncestorsAndSelf().OfType<BaseTypeDeclarationSyntax>().LastOrDefault();
                            found.UnionWith(type != null ? new[] { type.Identifier.Text }
                                : ComponentName(hit.Hit.File.RelativePath) is { } component ? new[] { component }
                                : TopLevelTypes(code, hit.Hit.File.RelativePath));
                        }
                    }
                    catch (Exception e) when (e is RegexMatchTimeoutException or ArgumentException or InvalidOperationException)
                    {
                        // Its row's globs, as RecipeConversion gives them: the entry's files, else its recipe's.
                        failed.UnionWith(RecipeConversion.Globs((own.Files ?? file.Files).Select(x => x.Pattern)));
                        notes.Add($"guardrails/{recipe.FileName}: the '{own.Kind}' convention's find couldn't run ({e.Message}), so what v1 didn't report in its files isn't excepted; the first run reports it.");
                    }
                }
            }

            return (found, failed);
        }

        /// <summary>The User Interface designer's element types the drift engine locates as a component (<c>intent.ui</c>).</summary>
        public static readonly IReadOnlyCollection<string> ComponentElements = new HashSet<string>(StringComparer.Ordinal) { "Page", "Dialog", "Component", "Layout" };

        /// <summary>
        /// The component a Razor or Vue file is to the drift engine, named for its file; null for any other file, and for an
        /// <c>_Imports.razor</c>, which declares none. A Razor name is the identifier Razor's compiler gives the class: anything
        /// an identifier can't hold becomes <c>_</c>, and a name that can't start one gets a leading <c>_</c>.
        /// </summary>
        public static string ComponentName(string file)
        {
            var name = Path.GetFileNameWithoutExtension(file);
            if (file.EndsWith(".vue", StringComparison.OrdinalIgnoreCase))
            {
                return name;
            }

            if (!file.EndsWith(".razor", StringComparison.OrdinalIgnoreCase) || string.Equals(name, "_Imports", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var identifier = new string(name.Select(x => SyntaxFacts.IsIdentifierPartCharacter(x) ? x : '_').ToArray());
            return identifier.Length > 0 && SyntaxFacts.IsIdentifierStartCharacter(identifier[0]) ? identifier : "_" + identifier;
        }

        private static IReadOnlyList<string> TopLevelTypes(CodeFiles code, string file) =>
            code.Get(file)?.Syntax is { } syntax ? CodeLocator.TopLevelTypes(syntax).Select(x => x.Identifier.Text).ToList() : new List<string>();

        private static IEnumerable<string> PublicFields(TypeDeclarationSyntax type) =>
            type.Members.OfType<FieldDeclarationSyntax>()
                .Where(x => x.Modifiers.Any(SyntaxKind.PublicKeyword))
                .SelectMany(x => x.Declaration.Variables.Select(v => v.Identifier.Text));

        /// <summary>
        /// The names of its own public members of the kinds its element states, which the engine's <c>unmodelled.member</c> may
        /// report: an enum's members, or a type's constructors (by the type's name), methods, properties and fields, overrides
        /// aside.
        /// </summary>
        private static IEnumerable<string> PublicMembers(BaseTypeDeclarationSyntax type, IReadOnlyCollection<string> kinds)
        {
            if (type is EnumDeclarationSyntax @enum)
            {
                return kinds.Contains("enumMember") ? @enum.Members.Select(x => x.Identifier.Text) : Enumerable.Empty<string>();
            }

            return ((TypeDeclarationSyntax)type).Members
                .Where(x => x.Modifiers.Any(SyntaxKind.PublicKeyword) && !x.Modifiers.Any(SyntaxKind.OverrideKeyword))
                .SelectMany(x => x switch
                {
                    ConstructorDeclarationSyntax when kinds.Contains("constructor") => new[] { type.Identifier.Text },
                    MethodDeclarationSyntax method when kinds.Contains("method") => new[] { method.Identifier.Text },
                    PropertyDeclarationSyntax property when kinds.Contains("property") => new[] { property.Identifier.Text },
                    FieldDeclarationSyntax field when kinds.Contains("field") => field.Declaration.Variables.Select(v => v.Identifier.Text).ToArray(),
                    _ => Array.Empty<string>()
                })
                .Distinct(StringComparer.Ordinal);
        }

        /// <summary>Its public methods, overrides aside, as the engine's <c>unmodelled.member</c> leaves them out.</summary>
        private static IEnumerable<MethodDeclarationSyntax> PublicMethods(TypeDeclarationSyntax type) =>
            type.Members.OfType<MethodDeclarationSyntax>()
                .Where(x => x.Modifiers.Any(SyntaxKind.PublicKeyword) && !x.Modifiers.Any(SyntaxKind.OverrideKeyword));

        /// <summary>
        /// The methods the drift engine finds handlers' subscriptions in, which it locates and so never reports, as its
        /// <c>subscription.handler</c> reading does over every part of a handler's class it reads (a partial handler's
        /// <c>Handle</c> in one file, the implementation method it hands the message to in another). For each message the
        /// handler subscribes to: the methods taking it, as a parameter's type or a type argument of one; with none, and no base
        /// over it, the methods making it, by a call given it as a type argument (<c>JsonConvert.DeserializeObject&lt;T&gt;(json)</c>).
        /// Only the handler's own messages count. A migrated NServiceBus application's handlers deserialise a modelled DTO
        /// (<c>JsonSerializer.Deserialize&lt;OperationData&gt;(json)</c>) without handling it, and one subscribes to another
        /// application's message with no link to it, so it has no subscription code and the engine reports every public method.
        /// By name, as the engine matches a message whose type it can't resolve.
        /// </summary>
        private static HashSet<MethodDeclarationSyntax> SubscriptionCode(CodeFiles code, IEnumerable<ScopeRowSpec> scope, ILookup<string, ModelledType> handlers)
        {
            var located = new HashSet<MethodDeclarationSyntax>();
            var parts = EngineRead(code, scope)
                .SelectMany(x => code.Get(x)?.Syntax?.DescendantNodes().OfType<TypeDeclarationSyntax>() ?? Enumerable.Empty<TypeDeclarationSyntax>())
                .Where(x => x is not InterfaceDeclarationSyntax && handlers.Contains(x.Identifier.Text))
                .ToLookup(x => x.Identifier.Text, StringComparer.Ordinal);
            foreach (var handler in parts)
            {
                var methods = handler.SelectMany(x => x.Members.OfType<MethodDeclarationSyntax>()).ToList();
                foreach (var message in handlers[handler.Key].SelectMany(x => x.Messages).Distinct(StringComparer.Ordinal))
                {
                    var taking = methods.Where(x => x.ParameterList.Parameters.Any(p => p.Type != null && Mentions(p.Type, message))).ToList();
                    if (taking.Count > 0)
                    {
                        located.UnionWith(taking);
                    }
                    else if (!handler.Any(x => x.BaseList?.Types.Any(b => Over(b.Type, message)) == true))
                    {
                        located.UnionWith(methods.Where(x => Makes(x, message)));
                    }
                }
            }

            return located;
        }

        /// <summary>The type is the message, or has it among its type arguments.</summary>
        private static bool Mentions(TypeSyntax type, string message) => LastName(type) == message || Over(type, message);

        /// <summary>The type has the message among its type arguments, at any depth: <c>IHandleMessages&lt;PlaceOrder&gt;</c>.</summary>
        private static bool Over(TypeSyntax type, string message) => type switch
        {
            QualifiedNameSyntax qualified => Over(qualified.Right, message),
            GenericNameSyntax generic => generic.TypeArgumentList.Arguments.Any(x => Mentions(x, message)),
            NullableTypeSyntax nullable => Over(nullable.ElementType, message),
            _ => false
        };

        /// <summary>The method calls something given the message as a type argument.</summary>
        private static bool Makes(MethodDeclarationSyntax method, string message) =>
            method.DescendantNodes().OfType<InvocationExpressionSyntax>().Any(x => (x.Expression switch
            {
                GenericNameSyntax generic => generic,
                MemberAccessExpressionSyntax { Name: GenericNameSyntax generic } => generic,
                MemberBindingExpressionSyntax { Name: GenericNameSyntax generic } => generic,
                _ => null
            })?.TypeArgumentList.Arguments.Any(a => LastName(a) == message) == true);
    }
}
