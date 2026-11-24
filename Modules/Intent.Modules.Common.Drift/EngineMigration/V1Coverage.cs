using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intent.Modules.Common.Drift.EngineMigration
{
    /// <summary>A type element of the model, with the names of its children, as the drift engine locates it in the code: by name.</summary>
    internal sealed record ModelledType(string Name, string ElementType, IReadOnlyCollection<string> Members);

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
    /// A type added later is still reported, as the row's glob says. Only C# is read: v1's code index is C# only.
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
        /// The element types that may be a class or an interface: with no type of its name, the element is the interface the
        /// language names it (<c>I{Name}</c>).
        /// </summary>
        private static readonly HashSet<string> InterfaceElements = new(StringComparer.Ordinal) { "Service", "Service Proxy", "Repository", "Domain Service" };

        public const string ContractFileReason = "Declared in a file v1 generated a modelled type into, which v1 counted as modelled";
        public const string FieldReason = "A public field, which v1 never reported";

        /// <param name="code">The application's code, as v1 read it.</param>
        /// <param name="contractFiles">The contract files, relative to the code's root with <c>/</c> separators.</param>
        /// <param name="notes">Gets a line for each recipe file whose finds couldn't run, whose rows then get no exceptions.</param>
        public static IReadOnlyList<ExceptionRowSpec> Exceptions(CodeFiles code, IEnumerable<RecipeSource> recipes, ConvertedRecipes rows,
            IReadOnlyCollection<string> contractFiles, IReadOnlyList<ModelledType> model, ICollection<string> notes)
        {
            var contracts = new HashSet<string>(contractFiles, StringComparer.OrdinalIgnoreCase);
            var outGlobs = rows.Scope.Where(x => x.Scope == "Out").SelectMany(x => x.Globs).Select(x => new Glob(x)).ToList();
            bool Checked(string file) => CodeFiles.IsCSharp(file) && !outGlobs.Any(x => x.IsMatch(file));

            var declarations = code.AllFiles().Where(CodeFiles.IsCSharp)
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
            foreach (var file in code.AllFiles().Where(Checked))
            {
                var covering = typeRows.Where(x => x.Globs.Any(g => g.IsMatch(file))).Select(x => x.Row.Description).Distinct().ToList();
                if (covering.Count == 0)
                {
                    continue;
                }

                var reason = $"v1's conventions over these files ({string.Join(", ", covering)}) didn't report it";
                // By name, as an exception matches: a partial type a find matched in one of its files was found.
                foreach (var type in TopLevelTypes(code, file).Where(x => !modelled.Contains(x)))
                {
                    if (!found.Contains(type))
                    {
                        Except(reason, type);
                    }
                    else if (contracts.Contains(file))
                    {
                        Except(ContractFileReason, type);
                    }
                }
            }

            if (rows.OptIn.FirstOrDefault(x => x.Rule == RecipeConversion.UnmodelledMember) is { } members)
            {
                var stateFields = model.Where(x => FieldElements.Contains(x.ElementType)).ToLookup(x => x.Name, StringComparer.Ordinal);
                foreach (var file in code.Matching(members.Globs.Select(x => new Glob(x)).ToList()).Where(Checked))
                {
                    var syntax = code.Get(file)?.Syntax;
                    foreach (var type in syntax?.DescendantNodes().OfType<TypeDeclarationSyntax>() ?? Enumerable.Empty<TypeDeclarationSyntax>())
                    {
                        var elements = stateFields[type.Identifier.Text].ToList();
                        if (elements.Count == 0 || type is InterfaceDeclarationSyntax)
                        {
                            continue;
                        }

                        foreach (var field in PublicFields(type).Where(x => !elements.Any(e => e.Members.Contains(x))))
                        {
                            Except(FieldReason, $"{type.Identifier.Text}.{field}");
                        }
                    }
                }
            }

            return byReason.Select(x => new ExceptionRowSpec(x.Key == FieldReason ? RecipeConversion.UnmodelledMember : RecipeConversion.UnmodelledType, x.Value, x.Key)).ToList();
        }

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
                            found.UnionWith(type != null ? new[] { type.Identifier.Text } : TopLevelTypes(code, hit.Hit.File.RelativePath));
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

        private static IReadOnlyList<string> TopLevelTypes(CodeFiles code, string file) =>
            code.Get(file)?.Syntax is { } syntax ? CodeLocator.TopLevelTypes(syntax).Select(x => x.Identifier.Text).ToList() : new List<string>();

        private static IEnumerable<string> PublicFields(TypeDeclarationSyntax type) =>
            type.Members.OfType<FieldDeclarationSyntax>()
                .Where(x => x.Modifiers.Any(SyntaxKind.PublicKeyword))
                .SelectMany(x => x.Declaration.Variables.Select(v => v.Identifier.Text));
    }
}
