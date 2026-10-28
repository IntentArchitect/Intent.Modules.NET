using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Intent.Modules.Services.Guardrails.Contracts.Drift
{
    internal enum DriftSection
    {
        Contradicted,
        Unverifiable,
        Undescribed,
        Missing
    }

    /// <summary>
    /// A row of the report's drift table: the model element (the ids of it and the elements it belongs to, outermost
    /// first, for live links: a class, then its attribute), the code file and line, and what the drift is.
    /// </summary>
    internal sealed record DriftRow(string Element, IReadOnlyList<string> ElementIds, string Path, int Line, string Drift, bool FileExists = true);


    /// <summary>One line of the report.</summary>
    internal sealed class Finding
    {
        public DriftSection Section { get; init; }

        /// <summary>The line as plain text, for the Software Factory's error list.</summary>
        public string Line { get; init; }

        public string File { get; init; }
        public SyntaxNode Node { get; init; }

        /// <summary>For code the model doesn't describe: what an exception can name.</summary>
        public string TypeName { get; init; }

        public string MemberName { get; init; }
        public string Name { get; init; }

        /// <summary>The line as a row of the report's drift table; the plain line when <see langword="null"/>.</summary>
        public DriftRow Row { get; init; }

        /// <summary>
        /// The generated type (<c>Order</c>) or member (<c>Order.Total(bool)</c>) a structure line is about, which the
        /// model baseline tracks. <see langword="null"/> for facts and ownership.
        /// </summary>
        public string Aspect { get; init; }

        public static Finding Contradicted(string line, string file, SyntaxNode node, DriftRow row = null, string aspect = null) =>
            new() { Section = DriftSection.Contradicted, Line = line, File = file, Node = node, Row = row, Aspect = aspect };

        public static Finding Unverifiable(string line, DriftRow row = null) => new() { Section = DriftSection.Unverifiable, Line = line, Row = row };

        /// <summary>
        /// For a structure line: the model element of the type or member it is about, when its own id is known (a member's
        /// row also names its type's).
        /// </summary>
        public string OwnElementId =>
            Row?.ElementIds is { } ids && ids.Count == (Aspect?.Contains('.') == true ? 2 : 1) ? ids[^1] : null;

        /// <summary>The line, saying that the model and the code have both changed since they last agreed.</summary>
        public Finding ChangedOnBothSides()
        {
            const string note = " (the model and the code have both changed since they last agreed)";
            return new Finding
            {
                Section = Section,
                Line = Line + note,
                File = File,
                Node = Node,
                TypeName = TypeName,
                MemberName = MemberName,
                Name = Name,
                Row = Row == null ? null : Row with { Drift = Row.Drift + note },
                Aspect = Aspect
            };
        }

        public static Finding Missing(string line, string file, DriftRow row = null, string aspect = null) =>
            new() { Section = DriftSection.Missing, Line = line, File = file, Row = row, Aspect = aspect };

        public static Finding Undescribed(string line, string file, SyntaxNode node, string typeName, string memberName, string name, DriftRow row = null, string aspect = null) => new()
        {
            Section = DriftSection.Undescribed,
            Line = line,
            File = file,
            Node = node,
            TypeName = typeName,
            MemberName = memberName,
            Name = name,
            Row = row,
            Aspect = aspect
        };
    }

    /// <summary>
    /// What the model and the code looked like, per generated type and member, when they last agreed, kept per machine
    /// in the solution's .cache folder. A difference only the model made since then is the Software Factory's to apply,
    /// so it isn't reported until the code agrees. A difference the code made is reported, whether or not the model
    /// changed too: applying the model's change would overwrite it.
    /// </summary>
    internal sealed class ModelBaseline
    {
        private static readonly System.Text.Json.JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow
        };

        public Dictionary<string, BaselineEntry> Aspects { get; } = new(StringComparer.Ordinal);

        /// <summary>
        /// The baseline, or <see langword="null"/> when the text isn't one, including one an earlier version wrote without
        /// the code's side (then nothing is skipped).
        /// </summary>
        public static ModelBaseline Parse(string json)
        {
            try
            {
                var aspects = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, BaselineEntry>>(json, JsonOptions);
                if (aspects == null)
                {
                    return null;
                }

                var baseline = new ModelBaseline();
                foreach (var (aspect, entry) in aspects.Where(x => x.Value != null))
                {
                    baseline.Aspects[aspect] = entry;
                }

                return baseline;
            }
            catch (System.Text.Json.JsonException)
            {
                return null;
            }
        }

        public string ToJson() => System.Text.Json.JsonSerializer.Serialize(new SortedDictionary<string, BaselineEntry>(Aspects, StringComparer.Ordinal), JsonOptions);
    }

    /// <summary>The structure lines left to the Software Factory this run, because the model made them.</summary>
    internal sealed class PendingChanges
    {
        public HashSet<(SyntaxTree, TextSpan)> Nodes { get; } = new();

        /// <summary>Types the model removed, whose files the Software Factory is deleting.</summary>
        public HashSet<string> RemovedTypes { get; } = new(StringComparer.Ordinal);

        /// <summary>Types and members the model added since the code last agreed, which the Software Factory is writing.</summary>
        public List<PendingAddition> Additions { get; } = new();

        /// <summary>Methods and constructors whose parameters the model changed, whose parameter lists the Software Factory is replacing.</summary>
        public List<PendingParameters> ParameterLists { get; } = new();

        public int Count { get; private set; }

        public void Add(IReadOnlyCollection<Finding> findings)
        {
            Count += findings.Count;
            Nodes.UnionWith(findings.Where(x => x.Node != null).Select(x => (x.Node.SyntaxTree, x.Node.Span)));
        }
    }

    /// <summary>A type (<c>Order</c>) or member (<c>Order.Total(bool)</c>) the model added: its code file, and its model element when its id is known.</summary>
    internal sealed record PendingAddition(string Aspect, string File, string ElementId);

    /// <summary>A method or constructor whose parameters the model changed: its code, and its model element when its id is known.</summary>
    internal sealed record PendingParameters(string Aspect, BaseMethodDeclarationSyntax Code, string ElementId);

    /// <summary>
    /// The code as it will be once this run's pending changes are accepted, held in memory: the files on disk, with each
    /// type and member the model added inserted from its template's output, and each pending parameter list replaced by
    /// the template's. Facts about those elements are read from it. It isn't RoslynWeaver's merge, which only exists
    /// after the check has run: a new member goes before the closing brace of its type's first declaration, a new type
    /// before the namespace's closing brace (or at the end of a file-scoped one), and a new file is the template's output
    /// as a whole. The templates never write what a fact reads (routes, verbs, EF Core configuration), so a fact read from
    /// inserted code reads what the next run would once the changes are applied.
    /// </summary>
    internal sealed class AcceptedView
    {
        private readonly Dictionary<string, ChangedFile> _files;

        private AcceptedView(RecipeEngine engine, Dictionary<string, ChangedFile> files, IReadOnlySet<string> elementIds)
        {
            Engine = engine;
            _files = files;
            ElementIds = elementIds;
        }

        /// <summary>The recipe engine over the view.</summary>
        public RecipeEngine Engine { get; }

        /// <summary>The model elements the pending changes add, or whose parameters they change: facts about them are read from the view.</summary>
        public IReadOnlySet<string> ElementIds { get; }

        /// <summary>The view, or <see langword="null"/> when no pending change has a model element a fact could be about.</summary>
        public static AcceptedView Build(PendingChanges pending, StructureResult structure, CodeFiles code, RecipeEngine engine)
        {
            var elementIds = new HashSet<string>(
                pending.Additions.Select(x => x.ElementId).Concat(pending.ParameterLists.Select(x => x.ElementId)).Where(x => x != null),
                StringComparer.Ordinal);
            if (elementIds.Count == 0)
            {
                return null;
            }

            var files = new Dictionary<string, ChangedFile>(StringComparer.OrdinalIgnoreCase);
            var texts = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var paths = pending.Additions.Select(x => x.File).Concat(pending.ParameterLists.Select(x => x.Code.SyntaxTree.FilePath))
                .Where(x => x != null)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x, StringComparer.Ordinal);
            foreach (var path in paths)
            {
                var disk = code.Get(path);
                if (disk == null)
                {
                    if (structure.Texts.TryGetValue(path, out var generated))
                    {
                        texts[path] = generated;
                        files[path] = new ChangedFile(null, Array.Empty<(int, int, int)>());
                    }

                    continue;
                }

                var edits = disk.Syntax == null ? new List<(TextSpan Span, string Text)>() : Edits(disk, pending, structure);
                if (edits.Count == 0)
                {
                    continue;
                }

                var text = new StringBuilder();
                var changes = new List<(int Start, int OldLength, int NewLength)>();
                var at = 0;
                foreach (var (span, replacement) in edits.OrderBy(x => x.Span.Start))
                {
                    if (span.Start < at)
                    {
                        continue;
                    }

                    text.Append(disk.Text, at, span.Start - at).Append(replacement);
                    changes.Add((span.Start, span.Length, replacement.Length));
                    at = span.End;
                }

                text.Append(disk.Text, at, disk.Text.Length - at);
                texts[disk.RelativePath] = text.ToString();
                files[disk.RelativePath] = new ChangedFile(disk, changes);
            }

            return new AcceptedView(engine.WithFiles(texts), files, elementIds);
        }

        /// <summary>Where each pending change goes in a file on disk: the span it replaces there (empty for an insertion), and its text.</summary>
        private static List<(TextSpan Span, string Text)> Edits(CodeFile disk, PendingChanges pending, StructureResult structure)
        {
            var places = new Dictionary<TextSpan, (string Before, StringBuilder Text, string After)>();
            void Insert(TextSpan span, string text, string before = "", string after = "")
            {
                if (!places.TryGetValue(span, out var place))
                {
                    places[span] = place = (before, new StringBuilder(), after);
                }

                place.Text.Append(text);
            }

            var inserted = new HashSet<SyntaxNode>();
            foreach (var addition in pending.Additions.Where(x => string.Equals(x.File, disk.RelativePath, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Aspect, StringComparer.Ordinal))
            {
                // One field declaration can declare several modelled fields.
                if (!structure.Declarations.TryGetValue(addition.Aspect, out var declaration) || !inserted.Add(declaration))
                {
                    continue;
                }

                var dot = addition.Aspect.IndexOf('.');
                if (dot < 0)
                {
                    var @namespace = disk.Syntax.Members.OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
                    Insert(new TextSpan(@namespace?.CloseBraceToken.SpanStart ?? disk.Text.Length, 0), $"\n{declaration}\n");
                    continue;
                }

                switch (CodeLocator.FindTypes(disk.Syntax, addition.Aspect[..dot]).FirstOrDefault())
                {
                    case EnumDeclarationSyntax @enum when @enum.CloseBraceToken.IsKind(SyntaxKind.CloseBraceToken):
                        var needsComma = @enum.Members.Count > 0 && @enum.Members.SeparatorCount < @enum.Members.Count;
                        Insert(new TextSpan(@enum.CloseBraceToken.SpanStart, 0), $"\n{declaration},\n", before: needsComma ? "," : "");
                        break;
                    case TypeDeclarationSyntax type when type.CloseBraceToken.IsKind(SyntaxKind.CloseBraceToken):
                        Insert(new TextSpan(type.CloseBraceToken.SpanStart, 0), $"\n{declaration}\n");
                        break;
                    case TypeDeclarationSyntax type when type.SemicolonToken.IsKind(SyntaxKind.SemicolonToken):
                        // A record with no body gets one.
                        Insert(type.SemicolonToken.Span, $"\n{declaration}\n", before: "\n{", after: "}\n");
                        break;
                }
            }

            foreach (var change in pending.ParameterLists.Where(x => x.Code.SyntaxTree == disk.Syntax.SyntaxTree))
            {
                // Only the parameter list: the code's attributes and the rest of its signature stay.
                if (structure.Declarations.GetValueOrDefault(change.Aspect) is BaseMethodDeclarationSyntax generated)
                {
                    Insert(change.Code.ParameterList.Span, ParameterList(generated.ParameterList, change.Code.ParameterList));
                }
            }

            return places.Select(x => (x.Key, x.Value.Before + x.Value.Text + x.Value.After)).ToList();
        }

        /// <summary>
        /// The template's parameter list as RoslynWeaver writes it over the code's: it matches parameters by name, so one
        /// whose name the code has keeps the code's attributes (<c>[FromQuery]</c>), and a renamed one loses them.
        /// </summary>
        private static string ParameterList(ParameterListSyntax generated, ParameterListSyntax code)
        {
            var kept = code.Parameters.GroupBy(x => x.Identifier.Text, StringComparer.Ordinal).ToDictionary(x => x.Key, x => x.First().AttributeLists, StringComparer.Ordinal);
            return generated.ReplaceNodes(generated.Parameters, (parameter, _) =>
            {
                if (!kept.TryGetValue(parameter.Identifier.Text, out var attributes) || attributes.Count == 0)
                {
                    return parameter;
                }

                var texts = new HashSet<string>(attributes.Select(x => x.ToString()), StringComparer.Ordinal);
                return parameter.WithAttributeLists(attributes.AddRange(parameter.AttributeLists.Where(x => !texts.Contains(x.ToString()))));
            }).ToString();
        }

        /// <summary>Whether the path is a file the pending changes add, which isn't on disk yet.</summary>
        public bool IsNew(string path) => path != null && _files.TryGetValue(path, out var file) && file.Disk == null;

        /// <summary>
        /// A location in the view as the report can show it: its line when the file is unchanged, or the line is before the
        /// file's first change; else the file alone, since the code it points at isn't on disk yet.
        /// </summary>
        public string Locate(string location)
        {
            var (path, line) = DriftCheck.SplitLocation(location);
            if (path == null || !_files.TryGetValue(path, out var file))
            {
                return location;
            }

            return file.Disk != null && line > 0 && line < file.Disk.LineOf(file.Changes[0].Start) ? location : path;
        }

        /// <summary>A file the view changed: its version on disk (<see langword="null"/> for a new file), and its changes, in order, by their place on disk.</summary>
        private sealed record ChangedFile(CodeFile Disk, IReadOnlyList<(int Start, int OldLength, int NewLength)> Changes);
    }

    internal sealed class BaselineEntry
    {
        /// <summary>The model's fingerprint when the model and the code last agreed, or <see langword="null"/> if they never have.</summary>
        public string Model { get; set; }

        /// <summary>The code's fingerprint then, or <see langword="null"/> if they never have.</summary>
        public string Code { get; set; }

        /// <summary>
        /// For a type: the fingerprint of every one of its code members, at any access level, when the type and all its
        /// members last agreed (<see cref="StructureComparer.CodeMembers"/>). A type the model removes is only left to the
        /// Software Factory to delete while its code still has exactly these. <see langword="null"/> for a member, and for
        /// a type they have never agreed on.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
        public string Members { get; set; }
    }

    /// <summary>Everything one drift check needs: the model's facts, the contract templates' output and the code.</summary>
    internal sealed class DriftCheckInput
    {
        public string Designer { get; init; }

        /// <summary>The recipe file's path as the report shows it, e.g. <c>guardrails/domain.drift.json</c>.</summary>
        public string RecipePath { get; init; }

        /// <summary>The recipe file's absolute path, which a row about a problem in it links to; no link when <see langword="null"/>.</summary>
        public string RecipeFile { get; init; }

        /// <summary>The recipe file's content, or <see langword="null"/> when it doesn't exist, which turns the check off.</summary>
        public string RecipeJson { get; init; }

        public CodeFiles Code { get; init; }
        public IReadOnlyList<ModelFact> Facts { get; init; }
        public IReadOnlyList<GeneratedFile> Generated { get; init; }

        /// <summary>
        /// Every file any template of the run writes, relative to the codebase root: an ownership recipe doesn't report
        /// the types in them as unmodelled, whichever designer or module describes them.
        /// </summary>
        public IReadOnlyCollection<string> OutputPaths { get; init; }

        /// <summary>The application's name, for the report's links to model elements; none when <see langword="null"/>.</summary>
        public string Application { get; init; }

        /// <summary>
        /// The model as of the last run on this machine, or <see langword="null"/> when there is none: then only what the
        /// code lacks is left to the Software Factory.
        /// </summary>
        public ModelBaseline Baseline { get; init; }
    }

    /// <summary>
    /// Compares the model with the code: every fact through its recipe, every kind of code the recipes own, and
    /// every type the contract templates generate. Identical in both Guardrails modules apart from the namespace.
    /// </summary>
    internal static class DriftCheck
    {
        public static DriftReport Run(DriftCheckInput input)
        {
            // No recipe file, no check. Intent applies none of a failing run's changes, and the configure-drift-check skill
            // that writes the file is itself generated by a run, so failing without it would block its own set-up.
            if (input.RecipeJson == null)
            {
                return DriftReport.Off(input.Designer, input.RecipePath);
            }

            var findings = new List<Finding>();
            var structure = StructureComparer.Compare(input.Generated, input.OutputPaths, input.Code);
            var (structureFindings, pending, baseline) = ApplyBaseline(structure, input);
            findings.AddRange(structureFindings);

            var recipes = RecipeFile.Parse(input.RecipeJson, input.Designer);
            var exceptions = recipes.Exceptions;
            foreach (var problem in recipes.Problems)
            {
                var line = problem.Subject == null ? $"`{input.RecipePath}` {problem.Message}" : $"`{input.RecipePath}` `{problem.Subject}`: {problem.Message}";
                findings.Add(Finding.Unverifiable(line, new DriftRow(null, Array.Empty<string>(), input.RecipeFile, 0, $"The recipe file is wrong: {line}")));
            }

            var engine = new RecipeEngine(input.Code, recipes.Files, structure.Types);
            var view = AcceptedView.Build(pending, structure, input.Code, engine);
            var elementFiles = ElementFiles(input.Generated, input.Code);
            var notChecked = new List<string>();
            var checkedFacts = 0;
            foreach (var group in input.Facts.GroupBy(x => x.Kind).OrderBy(x => x.Key.Key, StringComparer.Ordinal))
            {
                if (recipes.BrokenKinds.Contains(group.Key.Key))
                {
                    continue;
                }

                // A kind with no recipe isn't checked, and only warns: a module upgrade that adds a kind of fact must not
                // fail every run until the recipe file catches up. A recipe that can't read a fact is still Wrong.
                if (!recipes.Facts.TryGetValue(group.Key.Key, out var recipe))
                {
                    notChecked.Add($"`{group.Key.Key}` ({Count(group.Count(), "fact")})");
                    continue;
                }

                foreach (var fact in group)
                {
                    // A fact about something this run's pending changes add, or about a method whose parameters they
                    // change, is read from the code as it will be once they are applied: accepting them isn't a gap in
                    // the code, and a gap that remains then (no IsRequired(), no [Route]) is one the Software Factory won't fill.
                    checkedFacts++;
                    var viewed = view != null && fact.ElementIds.Any(view.ElementIds.Contains);
                    var reader = viewed ? view.Engine : engine;
                    var reading = viewed ? reader.Read(recipe, fact).Relocated(view.Locate) : reader.Read(recipe, fact);
                    var (path, lineNumber) = FactPlace(reading, fact, elementFiles);
                    var fileExists = !viewed || !view.IsNew(path);
                    if (!reading.Verified)
                    {
                        // The row's Code file column shows the location, so only the plain line repeats it.
                        var location = reading.Location == null ? string.Empty : $". Location: `{reading.Location}`";
                        findings.Add(Finding.Unverifiable(
                            $"`{fact.Element}` {fact.Kind.Label}: {reading.Problem}{location}",
                            new DriftRow(fact.Element, fact.ElementIds, path, lineNumber, $"`{fact.Element}` {fact.Kind.Label} can't be verified: {reading.Problem}", fileExists)));
                    }
                    else if (!string.Equals(reading.Value, fact.Value?.Trim() ?? string.Empty, StringComparison.Ordinal))
                    {
                        // When the code says nothing, the row says what that means, in the recipe's words when it has them.
                        var drift = $"`{fact.Element}` {fact.Kind.Label}: model {Code(fact.Value)}, code {Code(reading.Value)}{(reading.Unstated == null ? string.Empty : $" ({reading.Unstated})")}";
                        var where = reading.Location != null ? $". Location: `{reading.Location}`" : reading.Near != null ? $". Near: `{reading.Near}`" : string.Empty;
                        findings.Add(Finding.Contradicted(
                            drift + where,
                            reading.Location,
                            null,
                            new DriftRow(fact.Element, fact.ElementIds, path, lineNumber, drift, fileExists)));
                    }
                }
            }

            // What the view's reads located in files it didn't change is the code on disk, so it counts as described too.
            view?.Engine.HandBackLocated();

            // Ownership is checked once every fact has been located, so that located code counts as described, as do generated types.
            foreach (var own in recipes.Own)
            {
                findings.AddRange(NotModelled(engine, own));
            }

            // Code that an ownership recipe reports is the same code as an unmodelled member of a generated type. And code
            // the model has just removed (its structure line was skipped above) is the Software Factory's to remove.
            var ownedNodes = new HashSet<(SyntaxTree, TextSpan)>(findings.Where(x => x.Section == DriftSection.Undescribed && x.Name != null && x.Node != null).Select(x => (x.Node.SyntaxTree, x.Node.Span)));
            findings.RemoveAll(x => x.Section == DriftSection.Undescribed && x.Name == null && x.Node != null && ownedNodes.Contains((x.Node.SyntaxTree, x.Node.Span)));
            findings.RemoveAll(x => x.Section == DriftSection.Undescribed && x.Name != null && x.Node != null && pending.Nodes.Contains((x.Node.SyntaxTree, x.Node.Span)));
            ApplyBaselineToRemovedTypes(findings, input.Baseline, pending, baseline);

            var used = exceptions.ToDictionary(x => x, _ => 0);
            findings.RemoveAll(finding =>
            {
                if (finding.Section != DriftSection.Undescribed)
                {
                    return false;
                }

                var exception = exceptions.FirstOrDefault(x => Covers(x, finding));
                if (exception == null)
                {
                    return false;
                }

                used[exception]++;
                return true;
            });

            // Only an exception that matches nothing is worth reporting: it is stale, and should be removed.
            return new DriftReport(
                input.Designer,
                input.RecipePath,
                checkedFacts,
                structure.TypeCount,
                findings,
                exceptions.Where(x => used[x] == 0).Select(x => $"`{x.Pattern}`{(x.Reason == null ? string.Empty : $" ({x.Reason})")}").ToList(),
                notChecked)
            {
                Application = input.Application,
                Baseline = baseline,
                PendingCount = pending.Count
            };
        }

        /// <summary>The code an ownership recipe finds that no fact located and no template generates, one line each.</summary>
        private static IEnumerable<Finding> NotModelled(RecipeEngine engine, OwnRecipe own) =>
            engine.Own(own).Where(x => !engine.IsDescribed(x.Hit)).Select(owned => Finding.Undescribed(
                $"{own.Kind} `{owned.Name}`: not modelled. Location: `{owned.Hit.Location}`",
                owned.Hit.File.RelativePath, owned.Hit.Node, owned.TypeName, owned.MemberName, owned.Name,
                new DriftRow(null, Array.Empty<string>(), owned.Hit.File.RelativePath, owned.Hit.Line, $"{own.Kind} `{owned.Name}` isn't modelled")));

        /// <summary>
        /// Splits the structure lines into those to report and those the Software Factory is applying. A type or member the
        /// code lacks is always left to it. Any other difference is left until the code agrees only when the model changed
        /// it since the model and the code last agreed and the code is exactly as it was then: applying it overwrites
        /// nothing. A difference the code made is reported, and so is one where both changed. Without a baseline, only what
        /// the code lacks is skipped. Returns the next baseline.
        /// </summary>
        private static (List<Finding> Report, PendingChanges Pending, ModelBaseline Baseline) ApplyBaseline(StructureResult structure, DriftCheckInput input)
        {
            var previous = input.Baseline;
            var report = new List<Finding>();
            var pending = new PendingChanges();
            var next = new ModelBaseline();
            var byAspect = structure.Findings.Where(x => x.Aspect != null).GroupBy(x => x.Aspect, StringComparer.Ordinal).ToDictionary(x => x.Key, x => x.ToList(), StringComparer.Ordinal);
            report.AddRange(structure.Findings.Where(x => x.Aspect == null));

            // Agreeing: the model and the code as they are now. Not compared (its type isn't in the code): as it was. A type's
            // members are only remembered once the type and every member agree, so a member the code added isn't among them.
            foreach (var (aspect, model) in structure.Fingerprints.Where(x => !byAspect.ContainsKey(x.Key)))
            {
                var before = previous?.Aspects.GetValueOrDefault(aspect);
                if (structure.Compared.Contains(aspect) && structure.CodeFingerprints.TryGetValue(aspect, out var agreeing))
                {
                    var allAgree = structure.CodeMembers.TryGetValue(aspect, out var members) && !byAspect.Keys.Any(x => x.StartsWith(aspect + ".", StringComparison.Ordinal));
                    next.Aspects[aspect] = new BaselineEntry { Model = model, Code = agreeing.Fingerprint, Members = allAgree ? members : before?.Members };
                }
                else if (before != null)
                {
                    next.Aspects[aspect] = before;
                }
            }

            var codeKeys = new HashSet<string>(structure.CodeFingerprints.Values.Select(x => x.Key), StringComparer.Ordinal);
            foreach (var (aspect, differences) in byAspect)
            {
                // The code's side is looked up by the code's own key first: a method whose parameters the model changed is
                // still the code's method under its old key. When both changed its parameters, neither key is remembered,
                // and what they last agreed on is the entry for its old signature, which neither has any more.
                var model = structure.Fingerprints.GetValueOrDefault(aspect);
                var code = structure.CodeFingerprints.GetValueOrDefault(aspect);
                var key = code != null && previous?.Aspects.ContainsKey(code.Key) == true ? code.Key : aspect;
                var before = previous?.Aspects.GetValueOrDefault(key);
                if (before == null && OldSignature(previous, aspect, structure.Fingerprints, codeKeys) is { } old)
                {
                    (key, before) = (old.Key, old.Value);
                }

                // Something the model has and the code doesn't is always the Software Factory's to write, and its Changes
                // pane already shows it: whether the model added it or the code removed it, whether or not it's applied, and
                // with or without a baseline (user decision, 2026-10-28).
                var added = differences.FirstOrDefault(x => x.Section == DriftSection.Missing);
                var codeUnchanged = code != null && before?.Code == code.Fingerprint;
                var modelChanged = before == null ? model != null : before.Model != null && before.Model != model;
                if (added != null || previous != null && codeUnchanged && modelChanged)
                {
                    pending.Add(differences);
                    if (added != null)
                    {
                        pending.Additions.Add(new PendingAddition(aspect, added.File, added.OwnElementId));
                    }
                    else if (differences.Select(x => x.Node).OfType<BaseMethodDeclarationSyntax>().FirstOrDefault() is { } method &&
                        structure.Declarations.GetValueOrDefault(aspect) is BaseMethodDeclarationSyntax generated && !StructureComparer.SameParameters(generated, method))
                    {
                        pending.ParameterLists.Add(new PendingParameters(aspect, method, differences[0].OwnElementId));
                    }

                    if (before != null)
                    {
                        next.Aspects[key] = before;
                    }

                    continue;
                }

                // Reported until the model and the code agree again. Never agreed (no baseline, or already different
                // when the check first saw it): reported until they do.
                next.Aspects[key] = before ?? new BaselineEntry();
                var bothChanged = before?.Model != null && before.Code != null && model != before.Model && code?.Fingerprint != before.Code;
                report.AddRange(bothChanged ? differences.Select(x => x.ChangedOnBothSides()) : differences);
            }

            // A whole type the model removed, whose file the Software Factory is deleting, as an ownership recipe would find it.
            foreach (var aspect in previous?.Aspects.Keys ?? Enumerable.Empty<string>())
            {
                if (!aspect.Contains('.') && !structure.Fingerprints.ContainsKey(aspect))
                {
                    pending.RemovedTypes.Add(aspect);
                }
            }

            return (report, pending, next);
        }

        /// <summary>
        /// The remembered entry for a method, constructor or indexer of the aspect's name whose signature neither the model
        /// nor the code has any more, when there is exactly one.
        /// </summary>
        private static KeyValuePair<string, BaselineEntry>? OldSignature(ModelBaseline previous, string aspect, IReadOnlyDictionary<string, string> model, IReadOnlySet<string> codeKeys)
        {
            var signature = aspect.IndexOfAny(new[] { '(', '[' });
            if (previous == null || signature < 0)
            {
                return null;
            }

            var prefix = aspect[..(signature + 1)];
            var old = previous.Aspects.Where(x => x.Key.StartsWith(prefix, StringComparison.Ordinal) && !model.ContainsKey(x.Key) && !codeKeys.Contains(x.Key)).Take(2).ToList();
            return old.Count == 1 ? old[0] : null;
        }

        /// <summary>
        /// A type the model removed, as an ownership recipe finds it in the code: its file is the Software Factory's to
        /// delete if the code is exactly as it was when the model and the code last agreed, every member included, and
        /// reported otherwise. The type stays in the baseline while the code has it.
        /// </summary>
        private static void ApplyBaselineToRemovedTypes(List<Finding> findings, ModelBaseline previous, PendingChanges pending, ModelBaseline next)
        {
            for (var i = 0; i < findings.Count; i++)
            {
                var finding = findings[i];
                if (finding.Section != DriftSection.Undescribed || finding.Name == null || finding.Node is not BaseTypeDeclarationSyntax type ||
                    !pending.RemovedTypes.Contains(finding.TypeName))
                {
                    continue;
                }

                var entries = previous.Aspects.Where(x => x.Key == finding.TypeName || x.Key.StartsWith(finding.TypeName + ".", StringComparison.Ordinal)).ToList();
                foreach (var (key, entry) in entries)
                {
                    next.Aspects[key] = entry;
                }

                // A baseline that predates remembering a type's members can't show that the code didn't add one: reported.
                var code = StructureComparer.CodeFingerprints(type);
                var members = previous.Aspects.GetValueOrDefault(finding.TypeName)?.Members;
                var agreed = members != null && entries.All(x => x.Value.Code != null);
                if (agreed && entries.All(x => x.Value.Code == code.GetValueOrDefault(x.Key)) && members == StructureComparer.CodeMembers(type))
                {
                    pending.Add(new[] { finding });
                    findings.RemoveAt(i--);
                }
                else if (agreed)
                {
                    findings[i] = finding.ChangedOnBothSides();
                }
            }
        }

        /// <summary>
        /// Where a fact's row points: the code that states it, else the one place its find reached before it stopped
        /// matching (where the code would state it), else the file generated for its element.
        /// </summary>
        private static (string Path, int Line) FactPlace(FactReading reading, ModelFact fact, IReadOnlyDictionary<string, string> elementFiles)
        {
            if ((reading.Location ?? reading.Near) is { } location)
            {
                return SplitLocation(location);
            }

            var file = fact.ElementIds.Reverse().Select(x => elementFiles.GetValueOrDefault(x)).FirstOrDefault(x => x != null);
            return (file, 0);
        }

        /// <summary>The file generated for each model element, by id, relative to the codebase root.</summary>
        private static IReadOnlyDictionary<string, string> ElementFiles(IReadOnlyList<GeneratedFile> generated, CodeFiles code)
        {
            var result = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var file in generated ?? Array.Empty<GeneratedFile>())
            {
                foreach (var id in file.ElementIds?.Values ?? Enumerable.Empty<string>())
                {
                    result.TryAdd(id, code.ToRelative(file.RelativePath));
                }
            }

            return result;
        }

        /// <summary>A location's file and line (<c>src/Order.cs:12</c>); line 0 when it has none.</summary>
        public static (string Path, int Line) SplitLocation(string location)
        {
            var colon = location?.LastIndexOf(':') ?? -1;
            return colon > 0 && int.TryParse(location[(colon + 1)..], out var line) ? (location[..colon], line) : (location, 0);
        }

        /// <summary>
        /// Whether an exception covers unmodelled code: a file pattern covers everything in its files, and an ownership name
        /// the code it names. A type or <c>Type.Member</c> covers that declaration, and a type its members' declarations,
        /// but not code a convention finds inside a member, such as a route mapping or a configuration call: that is exempt
        /// only by its own name or a file pattern.
        /// </summary>
        private static bool Covers(DriftException exception, Finding finding)
        {
            if (exception.Glob != null)
            {
                return finding.File != null && exception.Glob.IsMatch(finding.File);
            }

            var pattern = exception.Pattern;
            if (pattern == finding.Name)
            {
                return true;
            }

            if (finding.Node is not MemberDeclarationSyntax or BaseNamespaceDeclarationSyntax or GlobalStatementSyntax)
            {
                return false;
            }

            var simpleType = finding.TypeName?[(finding.TypeName.LastIndexOf('.') + 1)..];
            return pattern == finding.TypeName || pattern == simpleType ||
                finding.MemberName != null && (pattern == $"{finding.TypeName}.{finding.MemberName}" || pattern == $"{simpleType}.{finding.MemberName}");
        }

        public static string Code(string value) => string.IsNullOrEmpty(value) ? "(none)" : $"`{value.Replace("`", "'")}`";

        public static string Count(int count, string noun) => $"{count} {noun}{(count == 1 ? string.Empty : "s")}";
    }

    /// <summary>
    /// The drift report, an MDX document: only what needs acting on, in up to two tables. Model and code drift: the
    /// model element, the code file and what the drift is, one row per Wrong line. Warnings: kinds of fact with no recipe,
    /// exceptions that match nothing, or the check being off. An empty table is left out. It has no timestamps. Whether
    /// the recipes read the code is checked on demand, by the ValidateDriftRecipes module task (<see cref="RecipeValidation"/>).
    /// </summary>
    internal sealed class DriftReport
    {
        private static readonly DriftSection[] Sections = { DriftSection.Contradicted, DriftSection.Unverifiable, DriftSection.Undescribed, DriftSection.Missing };

        private readonly IReadOnlyList<Finding> _findings;

        public DriftReport(
            string designer,
            string recipePath,
            int factCount,
            int typeCount,
            IReadOnlyList<Finding> findings,
            IReadOnlyList<string> unusedExceptions,
            IReadOnlyList<string> notChecked = null)
        {
            Designer = designer;
            RecipePath = recipePath;
            FactCount = factCount;
            TypeCount = typeCount;
            _findings = Sections
                .SelectMany(section => findings.Where(f => f.Section == section).GroupBy(f => f.Line, StringComparer.Ordinal).Select(g => g.First()).OrderBy(f => f.Line, StringComparer.Ordinal))
                .ToList();
            Lines = Sections.ToDictionary(x => x, x => (IReadOnlyList<string>)_findings.Where(f => f.Section == x).Select(f => f.Line).ToList());
            UnusedExceptions = unusedExceptions.OrderBy(x => x, StringComparer.Ordinal).ToList();
            NotChecked = (notChecked ?? Array.Empty<string>()).OrderBy(x => x, StringComparer.Ordinal).ToList();
        }

        /// <summary>The report of a designer with no recipe file: nothing is checked until one is written.</summary>
        public static DriftReport Off(string designer, string recipePath) =>
            new(designer, recipePath, 0, 0, Array.Empty<Finding>(), Array.Empty<string>()) { IsOff = true };

        public bool IsOff { get; private init; }
        public string Designer { get; }
        public string RecipePath { get; }

        /// <summary>The application's name, for the links to model elements; plain names when <see langword="null"/>.</summary>
        public string Application { get; init; }

        /// <summary>The facts compared through a recipe.</summary>
        public int FactCount { get; }

        public int TypeCount { get; }
        public IReadOnlyDictionary<DriftSection, IReadOnlyList<string>> Lines { get; }

        /// <summary>Exceptions that cover no line: stale, so they should be removed.</summary>
        public IReadOnlyList<string> UnusedExceptions { get; }

        /// <summary>The kinds of fact the model states that have no recipe, with their counts. They only warn.</summary>
        public IReadOnlyList<string> NotChecked { get; }

        /// <summary>The model baseline to keep for the next run; <see langword="null"/> when the check didn't compare the structure.</summary>
        public ModelBaseline Baseline { get; init; }

        /// <summary>Structure lines left to the Software Factory this run, because the model made them.</summary>
        public int PendingCount { get; init; }


        /// <summary>
        /// The reports' folder: <c>.cache/guardrails/&lt;application&gt;</c> in the folder holding the solution's <c>.isln</c>,
        /// which every Intent solution's own <c>.gitignore</c> ignores.
        /// </summary>
        public static string FolderIn(string solutionFolder, string application) =>
            Path.GetFullPath(Path.Combine(solutionFolder, ".cache", "guardrails", application));

        public IReadOnlyList<string> WrongLines => _findings.Select(x => x.Line).ToList();

        /// <summary>Whether the report has anything to act on: drift or a warning.</summary>
        public bool NeedsAttention => IsOff || _findings.Count > 0 || NotChecked.Count > 0 || UnusedExceptions.Count > 0;

        public string ToMarkdown()
        {
            var text = new StringBuilder();
            text.Append("# ").Append(Designer).Append(" drift report\n\n");
            if (_findings.Count > 0)
            {
                text.Append("## Model and code drift\n\n");
                text.Append("| Model element | Code file | Drift |\n| --- | --- | --- |\n");
                foreach (var finding in _findings)
                {
                    var row = finding.Row ?? new DriftRow(null, Array.Empty<string>(), finding.File, 0, finding.Line);
                    text.Append("| ").Append(ElementCell(row)).Append(" | ").Append(FileCell(row)).Append(" | ").Append(Cell(row.Drift)).Append(" |\n");
                }

                text.Append('\n');
            }

            var warnings = new List<(string Warning, string Detail)>();
            if (IsOff)
            {
                warnings.Add(("The check is off", $"`{RecipePath}` doesn't exist. Load the configure-drift-check skill to write it."));
            }

            warnings.AddRange(NotChecked.Select(x => ("No recipe", $"{x}: `{RecipePath}` has no recipe for it, so it isn't checked. Load the configure-drift-check skill to add one.")));
            warnings.AddRange(UnusedExceptions.Select(x => ("Exception matches nothing", $"{x}: remove it from `{RecipePath}`.")));
            if (warnings.Count > 0)
            {
                text.Append("## Warnings\n\n");
                text.Append("| Warning | Detail |\n| --- | --- |\n");
                foreach (var (warning, detail) in warnings)
                {
                    text.Append("| ").Append(warning).Append(" | ").Append(Cell(detail)).Append(" |\n");
                }

                text.Append('\n');
            }

            if (!NeedsAttention)
            {
                text.Append("No drift.\n");
            }

            return text.ToString().TrimEnd() + "\n";
        }

        // The messages are plain text: the Software Factory's output shows Markdown as literal characters.

        public string OffMessage(string reportPath) =>
            $"The {Designer} drift check is off: {RecipePath} doesn't exist. Load the configure-drift-check skill to set it up. Report: {reportPath}";

        /// <summary>The model element of each line that fails the run, in the report's order, when it is known (the attribute, not its class).</summary>
        public IReadOnlyList<string> FailureElementIds =>
            _findings.Select(x => x.Row?.ElementIds is { Count: > 0 } ids ? ids[^1] : null).Where(x => x != null).ToList();

        /// <summary>
        /// The run's one failure: how many drifts, and where the report is, in plain text (forward slashes, which a card's
        /// Markdown leaves alone). On CI, where no report is written, the lines themselves instead, as a Markdown list.
        /// </summary>
        public string FailureMessage(string reportPath, bool listLines)
        {
            var summary = $"The {Designer} model and the code have drifted: {DriftCheck.Count(WrongLines.Count, "drift")} detected";
            return listLines
                ? $"{summary}:\n\n{string.Join("\n", WrongLines.Select(x => $"- {x}"))}"
                : $"{summary} and listed in the drift report ({reportPath.Replace('\\', '/')}).";
        }

        public string NotCheckedMessage(string reportPath) =>
            $"The {Designer} drift check doesn't check {DriftCheck.Count(NotChecked.Count, "kind")} of fact, which {RecipePath} has no recipe for: {string.Join(", ", NotChecked.Select(Plain))}. Load the configure-drift-check skill to add them. Report: {reportPath}";

        public string UnusedExceptionsMessage(string reportPath) =>
            $"{DriftCheck.Count(UnusedExceptions.Count, "exception")} in {RecipePath} {(UnusedExceptions.Count == 1 ? "matches" : "match")} nothing; remove {(UnusedExceptions.Count == 1 ? "it" : "them")}: {string.Join(", ", UnusedExceptions.Select(Plain))}. Report: {reportPath}";

        public string NoDriftMessage() =>
            $"The {Designer} drift check found no drift in {DriftCheck.Count(FactCount, "fact")} and {DriftCheck.Count(TypeCount, "type")}{(NeedsAttention ? "." : ", so it wrote no report.")}";

        public string PendingMessage() =>
            $"The {Designer} drift check left {DriftCheck.Count(PendingCount, "difference")} to this run's changes: the model changed {(PendingCount == 1 ? "that type or member" : "those types or members")} since it last agreed with the code, so {(PendingCount == 1 ? "it isn't" : "they aren't")} checked until the changes are applied.";

        private static string Plain(string line) => line.Replace("`", string.Empty);

        /// <summary>
        /// Live links to the model element and the elements it belongs to (<c>Order › Total</c>) when their ids are known,
        /// else its name; a dash when the code isn't modelled.
        /// </summary>
        private string ElementCell(DriftRow row)
        {
            if (row.ElementIds is { Count: > 0 } && Application != null)
            {
                return string.Join(" › ", row.ElementIds.Select(id =>
                    $"<ModelRef application=\"{Attribute(Application)}\" elementId=\"{Attribute(id)}\" designer=\"{Attribute(Designer)}\" />"));
            }

            return row.Element == null ? "—" : Cell($"`{row.Element}`");
        }

        /// <summary>A live link to the file and line; a file the model expects but the code doesn't have yet is marked added.</summary>
        private static string FileCell(DriftRow row)
        {
            if (string.IsNullOrEmpty(row.Path))
            {
                return "—";
            }

            var path = Attribute(row.Path.Replace('\\', '/'));
            return !row.FileExists ? $"<FileRef path=\"{path}\" change=\"added\" />"
                : row.Line > 0 ? $"<FileRef path=\"{path}\" line={{{row.Line}}} />"
                : $"<FileRef path=\"{path}\" />";
        }

        /// <summary>
        /// A table cell's Markdown, safe in MDX: outside code spans a brace or an angle bracket would start JSX, so it is
        /// written as an entity; a pipe would end the cell anywhere, so it is escaped.
        /// </summary>
        private static string Cell(string markdown)
        {
            var text = new StringBuilder();
            var inCode = false;
            foreach (var c in markdown ?? string.Empty)
            {
                switch (c)
                {
                    case '`':
                        inCode = !inCode;
                        text.Append(c);
                        break;
                    case '|':
                        text.Append("\\|");
                        break;
                    case '\r':
                    case '\n':
                        text.Append(' ');
                        break;
                    case '{' when !inCode:
                        text.Append("&#123;");
                        break;
                    case '}' when !inCode:
                        text.Append("&#125;");
                        break;
                    case '<' when !inCode:
                        text.Append("&lt;");
                        break;
                    case '>' when !inCode:
                        text.Append("&gt;");
                        break;
                    default:
                        text.Append(c);
                        break;
                }
            }

            return text.ToString();
        }

        private static string Attribute(string value) => value.Replace("&", "&amp;").Replace("\"", "&quot;");
    }
}
