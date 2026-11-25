using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.Common.DriftEngine;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks;
using Intent.Modules.Common.DriftEngine.Stacks.TypeScript;
using EngineFinding = Intent.Modules.Common.DriftEngine.Finding;
using EngineIds = Intent.Modules.Common.Drift.EngineMigration.V1Footprint.DriftEngine;

namespace Intent.Modules.Common.Drift.EngineMigration
{
    /// <summary>
    /// Brings the moved model into line with the code, which hasn't changed, so a model clean under v1 is clean on the drift
    /// engine (user decision, 2026-11-25). v1's recipes could verify what the code doesn't say (a <c>new</c> message counted as
    /// sent, an unrouted Web API 2 action read as at its controller's route) and never looked for a send the diagram leaves out,
    /// so a v1-clean model can still state what the engine reads as false. The engine runs on the model as the move left it,
    /// and each correction its findings give (<see cref="ModelCorrection"/>, given only where the code settles it) is applied,
    /// then it runs again, since a correction can let another claim be read (an operation given Http Settings now has a route
    /// to compare). Stale exceptions go last, once the model has stopped changing what the engine locates.
    /// <para>
    /// Called through <see cref="DriftEngineMigration"/>, which catches the engine's assemblies failing to load: they are the
    /// Intent.Common.DriftEngine module's, which the task host loads from its <c>lib</c> folder.
    /// </para>
    /// </summary>
    internal static class ModelCorrections
    {
        private const int MostPasses = 4;

        private static readonly JsonSerializerOptions DisplayOptions = new() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

        /// <param name="moduleCache">The solution's <c>.cache/modules</c> folder, where installed modules' packages are.</param>
        /// <param name="tidyExceptions">Whether the move wrote the Drift Check, so its exceptions are the move's to remove.</param>
        public static void Apply(ApplicationPersistable application, string outputFolder, string moduleCache, bool tidyExceptions, ICollection<string> log)
        {
            var attempted = new HashSet<string>(StringComparer.Ordinal);
            for (var pass = 1; pass <= MostPasses; pass++)
            {
                var model = PersistedModel.Read(application, EngineIds.DesignerNames, moduleCache);
                var result = DriftEngine.DriftEngine.Run(new DriftInput(model.Designers, outputFolder,
                    EngineIds.StockPacks.Select(x => Pack.Stock(x)).Where(x => x != null).ToList(),
                    StockExtensions.With(NoNode.Instance))
                {
                    References = model.References
                });

                var corrections = result.Findings
                    .Where(x => x.Severity != "info")
                    .SelectMany(x => x.Corrections.Select(c => (Finding: x, Correction: c)))
                    .Where(x => x.Correction.Kind != ModelCorrection.RemoveException || tidyExceptions)
                    .Where(x => !attempted.Contains(Key(x.Correction)))
                    .ToList();

                // The model first, then its exceptions, judged on the finished model: what the engine locates still moves while
                // links are added. A pass whose model changes all fail leaves this run's reading of the exceptions current.
                if (Correct(application, model, corrections.Where(x => x.Correction.Kind != ModelCorrection.RemoveException), attempted, log) > 0)
                {
                    continue;
                }

                // From the last row up: a row left empty is removed, which would renumber the rows after it.
                Correct(application, model, corrections.Where(x => x.Correction.Kind == ModelCorrection.RemoveException).OrderByDescending(x => x.Correction.Row), attempted, log);
                return;
            }
        }

        /// <returns>How many corrections changed the model.</returns>
        private static int Correct(ApplicationPersistable application, PersistedModel model, IEnumerable<(EngineFinding Finding, ModelCorrection Correction)> corrections,
            ISet<string> attempted, ICollection<string> log)
        {
            var changed = 0;
            foreach (var (finding, correction) in corrections)
            {
                if (!attempted.Add(Key(correction)))
                {
                    continue;
                }

                if (Apply(application, model, correction) is { } done)
                {
                    changed++;
                    log.Add($"{done}, as the code does. The drift engine read: {finding.Message}");
                }
                else
                {
                    log.Add($"Warning: couldn't correct the model for this drift engine finding, so it stays: {finding.Message}");
                }
            }

            return changed;
        }

        private static string Key(ModelCorrection correction) =>
            $"{correction.Kind}|{correction.Element}|{correction.Association}|{correction.Target}|{correction.Stereotype}|{correction.Row}|{correction.Entry}|" +
            string.Join(",", (correction.Properties ?? new Dictionary<string, string>()).OrderBy(x => x.Key, StringComparer.Ordinal).Select(x => $"{x.Key}={x.Value}"));

        /// <returns>What was changed, for the log; null when the correction couldn't be made.</returns>
        private static string Apply(ApplicationPersistable application, PersistedModel model, ModelCorrection correction) => correction.Kind switch
        {
            ModelCorrection.AddLink => AddLink(model, correction),
            ModelCorrection.RemoveLink => RemoveLink(model, correction),
            ModelCorrection.SetStereotype => SetStereotype(model, correction),
            ModelCorrection.RemoveException => RemoveException(model, correction),
            _ => null
        };

        /// <summary>
        /// A link like the application's other links of its type, which say its association and end types and their traits, from
        /// the source to the target. Already there, it is reported done: an earlier correction of the same run drew it.
        /// </summary>
        private static string AddLink(PersistedModel model, ModelCorrection correction)
        {
            var source = NameOf(model, correction.Element);
            var target = model.Element(correction.Target);
            var associations = model.Packages.SelectMany(x => GuardrailsModelCleanup.AllAssociations(x.Package)).Where(x => x.AssociationType == correction.Association).ToList();
            var template = associations.FirstOrDefault(x => x.SourceEnd?.TypeReference != null && x.TargetEnd?.TypeReference != null);
            var package = model.Element(correction.Element) is { } element ? model.PackageOf(element) : model.End(correction.Element)?.Package;
            if (source == null || target == null || template == null || package == null)
            {
                return null;
            }

            if (associations.Any(x => model.SourceOf(x) == correction.Element && x.TargetEnd?.TypeReference?.TypeId == correction.Target))
            {
                return $"Found the {correction.Association} link from {source} to {target.Name} already added";
            }

            var targetPackage = model.PackageOf(target);
            var id = Guid.NewGuid().ToString();
            var association = new AssociationPersistable
            {
                Id = id,
                AssociationType = template.AssociationType,
                AssociationTypeId = template.AssociationTypeId,
                SourceEnd = new AssociationEndPersistable
                {
                    Id = Guid.NewGuid().ToString(),
                    SpecializationType = template.SourceEnd.SpecializationType,
                    SpecializationTypeId = template.SourceEnd.SpecializationTypeId,
                    Traits = template.SourceEnd.Traits?.ToList(),
                    TypeReference = new TypeReferencePersistable
                    {
                        Id = Guid.NewGuid().ToString(),
                        TypeId = correction.Element,
                        // Its own name, as the designer writes it (a subscription end's is "Handler"), which names the link's file.
                        TypeName = model.Element(correction.Element)?.Name ?? model.End(correction.Element)?.End.Name,
                        TypePackageId = package.Id,
                        TypePackageName = package.Name,
                        IsNavigable = template.SourceEnd.TypeReference.IsNavigable,
                        IsNullable = template.SourceEnd.TypeReference.IsNullable,
                        IsCollection = template.SourceEnd.TypeReference.IsCollection
                    }
                },
                // Intent gives a target end its association's id.
                TargetEnd = new AssociationEndPersistable
                {
                    Id = id,
                    SpecializationType = template.TargetEnd.SpecializationType,
                    SpecializationTypeId = template.TargetEnd.SpecializationTypeId,
                    Traits = template.TargetEnd.Traits?.ToList(),
                    TypeReference = new TypeReferencePersistable
                    {
                        Id = Guid.NewGuid().ToString(),
                        TypeId = target.Id,
                        TypeName = target.Name,
                        TypePackageId = targetPackage?.Id,
                        TypePackageName = targetPackage?.Name,
                        IsNavigable = template.TargetEnd.TypeReference.IsNavigable,
                        IsNullable = template.TargetEnd.TypeReference.IsNullable,
                        IsCollection = template.TargetEnd.TypeReference.IsCollection
                    }
                }
            };
            GuardrailsModelCleanup.AddAssociation(package, association);
            return $"Added a {correction.Association} link from {source} to {target.Name}";
        }

        /// <summary>Removes the link, and its line on any diagram of its package.</summary>
        private static string RemoveLink(PersistedModel model, ModelCorrection correction)
        {
            if (model.End(correction.Element) is not { } end)
            {
                return null;
            }

            var source = NameOf(model, model.SourceOf(end.Association)) ?? "its source";
            var target = end.Association.TargetEnd?.TypeReference?.TypeName ?? NameOf(model, end.Association.TargetEnd?.TypeReference?.TypeId) ?? "its target";
            GuardrailsModelCleanup.RemoveAssociation(end.Package, end.Association);
            foreach (var diagram in end.Package.FindAllChildElements().Select(x => x.Diagram).Where(x => x != null))
            {
                foreach (var visual in diagram.AssociationVisuals.Where(x => x.Id == end.Association.Id).ToList())
                {
                    diagram.AssociationVisuals.Remove(visual);
                }
            }

            return $"Removed the {end.Association.AssociationType} link from {source} to {target}";
        }

        /// <summary>
        /// Applies the stereotype as its definition gives it, each property at its default, when the element hasn't it, then sets
        /// the properties the code states.
        /// </summary>
        private static string SetStereotype(PersistedModel model, ModelCorrection correction)
        {
            if (model.Element(correction.Element) is not { } element || correction.Stereotype == null || correction.Properties == null)
            {
                return null;
            }

            var stereotype = element.Stereotypes.FirstOrDefault(x => x.Name == correction.Stereotype);
            var definition = model.Packages
                .SelectMany(x => (x.Package.StereotypeDefinitions ?? new List<StereotypeDefinitionPersistable>()).Select(d => (Definition: d, x.Package)))
                .FirstOrDefault(x => stereotype != null ? x.Definition.Id == stereotype.DefinitionId : x.Definition.Name == correction.Stereotype);
            if (stereotype == null)
            {
                if (definition.Definition == null)
                {
                    return null;
                }

                stereotype = new StereotypePersistable
                {
                    DefinitionId = definition.Definition.Id,
                    Name = definition.Definition.Name,
                    DefinitionPackageId = definition.Package.Id,
                    DefinitionPackageName = definition.Package.Name,
                    Properties = (definition.Definition.Properties ?? new List<StereotypePropertyDefinitionPersistable>())
                        .Select(x => new StereotypePropertyPersistable { DefinitionId = x.Id, Name = x.Name, Value = x.DefaultValue })
                        .ToList()
                };
                element.Stereotypes.Add(stereotype);
            }

            foreach (var (name, value) in correction.Properties)
            {
                var property = stereotype.Properties.FirstOrDefault(x => x.Name == name);
                if (property == null)
                {
                    var known = definition.Definition?.Properties?.FirstOrDefault(x => x.Name == name);
                    if (known == null)
                    {
                        return null;
                    }

                    property = new StereotypePropertyPersistable { DefinitionId = known.Id, Name = known.Name };
                    stereotype.Properties.Add(property);
                }

                property.Value = value;
            }

            var owner = ParentName(model, element);
            return $"Set {correction.Stereotype} {string.Join(", ", correction.Properties.Select(x => $"{x.Key} `{x.Value}`"))} on {(owner == null ? "" : owner + ".")}{element.Name}";
        }

        /// <summary>Removes an entry of a Drift Check exception row the engine says matches nothing, and the row when none is left.</summary>
        private static string RemoveException(PersistedModel model, ModelCorrection correction)
        {
            var host = model.Packages.Select(x => x.Package).FirstOrDefault(x => x.Id == correction.Element)?.Stereotypes
                       ?? model.Element(correction.Element)?.Stereotypes;
            var exceptions = host?.FirstOrDefault(x => x.DefinitionId == EngineIds.DriftCheck.DefinitionId)?.Properties.FirstOrDefault(x => x.DefinitionId == EngineIds.DriftCheck.Exceptions);
            if (exceptions?.ItemList == null || correction.Row is not { } number || number < 1 || number > exceptions.ItemList.Count)
            {
                return null;
            }

            var row = exceptions.ItemList[number - 1];
            var match = row.Properties.FirstOrDefault(x => x.DefinitionId == EngineIds.Exception.Match);
            var lines = (match?.Value ?? "").Split('\n').Select(x => x.Trim()).Where(x => x.Length > 0).ToList();
            if (match == null || !lines.Remove(correction.Entry))
            {
                return null;
            }

            match.Value = string.Join("\n", lines);
            if (lines.Count == 0)
            {
                exceptions.ItemList.Remove(row);
            }

            // The list's value is its rows' labels, which is all the designer keeps there.
            exceptions.Value = JsonSerializer.Serialize(exceptions.ItemList.Select(x =>
                $"{string.Join(", ", (x.Properties.FirstOrDefault(p => p.DefinitionId == EngineIds.Exception.Match)?.Value ?? "").Split('\n').Where(l => l.Length > 0))} — {x.Properties.FirstOrDefault(p => p.DefinitionId == EngineIds.Exception.Reason)?.Value}").ToArray(), DisplayOptions);
            return $"Removed `{correction.Entry}` from Drift Check exception row {number}, which the drift engine never reports";
        }

        /// <summary>What a log line calls an element or end: a subscription is its handler's handling of its message.</summary>
        private static string NameOf(PersistedModel model, string id)
        {
            if (model.Element(id) is { } element)
            {
                return ParentName(model, element) is { } parent && element.SpecializationType is "Operation" or "Class Constructor" ? $"{parent}.{element.Name}" : element.Name;
            }

            if (model.End(id) is { } end)
            {
                var holder = NameOf(model, model.SourceOf(end.Association)) ?? "its handler";
                return $"{holder}'s handling of {end.End.TypeReference?.TypeName ?? NameOf(model, end.End.TypeReference?.TypeId) ?? "its message"}";
            }

            return null;
        }

        private static string ParentName(PersistedModel model, ElementPersistable element) =>
            model.Packages.Select(x => x.Package).SelectMany(x => x.FindAllChildElements()).FirstOrDefault(x => x.ChildElements?.Contains(element) == true)?.Name;

        /// <summary>No Node.js while moving: TypeScript isn't read, so its claims can't be verified and give no corrections.</summary>
        private sealed class NoNode : INodeRunner
        {
            public static NoNode Instance { get; } = new();

            public NodeRun Run(string script, IReadOnlyList<string> arguments) => new(null, "TypeScript isn't read while moving onto the drift engine.");
        }
    }
}
