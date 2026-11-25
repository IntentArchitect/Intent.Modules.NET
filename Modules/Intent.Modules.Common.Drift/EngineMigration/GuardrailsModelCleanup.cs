using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using EngineIds = Intent.Modules.Common.Drift.EngineMigration.V1Footprint.DriftEngine;

namespace Intent.Modules.Common.Drift.EngineMigration
{
    /// <summary>
    /// The model side of the move, on the packages this application owns (<c>GetPackages</c> also gives packages other
    /// applications own, such as shared Eventing packages, which may still be on v1). In order: the converted rows, the
    /// subscription links, the Guardrails stereotypes, then the Guardrails modules, which is what an uninstall does: their
    /// module entries, installed packages, designer settings reference and Template Outputs. A migration can't call Intent's
    /// uninstall, and the platform's <c>RemoveTemplateOutputs</c> cleans only a designer's first package and throws on a designer
    /// without outputs. The links go before the modules: without the Services module's association types the Eventing packages
    /// holding them fail to load. Nothing is saved here; <see cref="DriftEngineMigration"/> saves it all at once.
    /// </summary>
    internal static class GuardrailsModelCleanup
    {
        private static readonly JsonSerializerOptions DisplayOptions = new() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

        /// <returns>Whether it wrote the Drift Check, so its exceptions are the move's own.</returns>
        public static bool Apply(ApplicationPersistable application, ConvertedRecipes rows, ICollection<string> log)
        {
            var designers = application.GetDesigners().ToList();
            var wroteCheck = WriteRows(application, designers.FirstOrDefault(x => x.Id == V1Footprint.CodebaseStructureDesignerId), rows, log);

            foreach (var designer in designers)
            {
                foreach (var package in Owned(application, designer))
                {
                    var links = AllAssociations(package).Where(x => V1Footprint.SubscriptionAssociationTypeIds.Contains(x.AssociationTypeId)).ToList();
                    foreach (var link in links)
                    {
                        RemoveAssociation(package, link);
                    }

                    if (links.Count > 0)
                    {
                        log.Add($"Deleted {Count(links.Count, "subscription link")} from {designer.Name} package {package.Name}.");
                    }
                }
            }

            foreach (var designer in designers)
            {
                foreach (var package in Owned(application, designer))
                {
                    var stripped = Strip(package.Stereotypes) +
                                   package.FindAllChildElements().OfType<ElementPersistable>().Sum(x => Strip(x.Stereotypes)) +
                                   AllAssociations(package).Sum(x => Strip(x.Stereotypes) + Strip(x.SourceEnd?.Stereotypes) + Strip(x.TargetEnd?.Stereotypes));
                    if (stripped > 0)
                    {
                        log.Add($"Removed {Count(stripped, "Guardrails stereotype")} from {designer.Name} package {package.Name}.");
                    }
                }
            }

            RemoveModules(application, designers, log);
            return wroteCheck;
        }

        /// <summary>Whether a Guardrails module is installed in the application.</summary>
        public static bool HasGuardrails(ApplicationPersistable application) =>
            application.Modules.Any(x => V1Footprint.ModuleIds.Contains(x.ModuleId, StringComparer.OrdinalIgnoreCase));

        private static bool WriteRows(ApplicationPersistable application, ApplicationDesignerPersistable codebaseStructure, ConvertedRecipes rows, ICollection<string> log)
        {
            if (!rows.HasRows)
            {
                return false;
            }

            var packages = codebaseStructure == null ? new List<PackageModelPersistable>() : Owned(application, codebaseStructure).ToList();
            if (packages.Count == 0)
            {
                log.Add("Warning: the application has no Codebase Structure package, so the recipe files' scope and conventions weren't written. Set up a Drift Scope on its Root Folder.");
                return false;
            }

            var hosts = packages.Select(x => (Name: x.Name, Stereotypes: x.Stereotypes))
                .Concat(packages.SelectMany(x => x.FindAllChildElements().OfType<ElementPersistable>()).Select(x => (Name: x.Name, Stereotypes: x.Stereotypes)));
            var setUp = hosts.FirstOrDefault(x => x.Stereotypes.Any(s => s.DefinitionId is EngineIds.DriftScope.DefinitionId or EngineIds.DriftCheck.DefinitionId));
            if (setUp.Stereotypes != null)
            {
                log.Add($"The drift engine is already set up on '{setUp.Name}', so the recipe files' scope and conventions weren't written.");
                return false;
            }

            var root = packages[0];
            if (rows.Scope.Count > 0)
            {
                var scope = Stereotype(EngineIds.DriftScope.DefinitionId, EngineIds.DriftScope.Name);
                scope.Properties.Add(ItemList(EngineIds.DriftScope.Entries, "Entries", rows.Scope.Select(x => Row(EngineIds.DriftScopeEntry.DefinitionId, EngineIds.DriftScopeEntry.Name,
                    (EngineIds.DriftScopeEntry.Glob, "Glob", Lines(x.Globs)),
                    (EngineIds.DriftScopeEntry.Types, "Types", null),
                    (EngineIds.DriftScopeEntry.Scope, "Scope", x.Scope),
                    (EngineIds.DriftScopeEntry.Reason, "Reason", x.Reason))).ToList(),
                    rows.Scope.Select(x => Lines(x.Globs))));
                root.Stereotypes.Add(scope);
                foreach (var row in rows.Scope)
                {
                    log.Add($"Drift Scope on {root.Name}: {row.Scope} {string.Join(", ", row.Globs)} ({row.Reason}).");
                }
            }

            if (rows.OptIn.Count > 0 || rows.Exceptions.Count > 0)
            {
                var check = Stereotype(EngineIds.DriftCheck.DefinitionId, EngineIds.DriftCheck.Name);
                check.Properties.Add(ItemList(EngineIds.DriftCheck.OptInRules, "Opt-in Rules", rows.OptIn.Select(x => Row(EngineIds.OptInRule.DefinitionId, EngineIds.OptInRule.Name,
                    (EngineIds.OptInRule.Rule, "Rule", x.Rule),
                    (EngineIds.OptInRule.Glob, "Glob", x.Globs.Count == 0 ? null : Lines(x.Globs)),
                    (EngineIds.OptInRule.Description, "Description", x.Description))).ToList(),
                    rows.OptIn.Select(x => $"{x.Description} · {x.Rule["optin/".Length..]}")));
                check.Properties.Add(ItemList(EngineIds.DriftCheck.Exceptions, "Exceptions", rows.Exceptions.Select(x => Row(EngineIds.Exception.DefinitionId, EngineIds.Exception.Name,
                    (EngineIds.Exception.Rule, "Rule", x.Rule),
                    (EngineIds.Exception.Match, "Match", Lines(x.Matches)),
                    (EngineIds.Exception.Reason, "Reason", x.Reason))).ToList(),
                    rows.Exceptions.Select(x => $"{string.Join(", ", x.Matches)} — {x.Reason}")));
                root.Stereotypes.Add(check);
                foreach (var row in rows.OptIn)
                {
                    log.Add($"Drift Check opt-in rule on {root.Name}: {row.Rule} over {(row.Globs.Count == 0 ? "every file in scope" : string.Join(", ", row.Globs))} ({row.Description}).");
                }

                foreach (var row in rows.Exceptions)
                {
                    log.Add($"Drift Check exception on {root.Name}: {row.Rule} for {string.Join(", ", row.Matches)} ({row.Reason}).");
                }

                return true;
            }

            return false;
        }

        private static void RemoveModules(ApplicationPersistable application, List<ApplicationDesignerPersistable> designers, ICollection<string> log)
        {
            foreach (var designer in designers)
            {
                foreach (var reference in designer.DesignerReferences
                             .Where(x => IsGuardrails(x.Module) || x.Id == V1Footprint.EventingExtensionDesignerSettingsId).ToList())
                {
                    designer.RemoveDesignerSettings(reference);
                    log.Add($"Removed the {reference.Name} designer settings from the {designer.Name} designer.");
                }

                foreach (var package in Owned(application, designer))
                {
                    // Not RemoveReference, which saves the package and publishes an event straight away.
                    var references = package.References.Where(x => IsGuardrails(x.Module) || IsGuardrails(x.Name)).ToList();
                    foreach (var reference in references)
                    {
                        package.References.Remove(reference);
                        log.Add($"Removed the {reference.Name} package from {designer.Name} package {package.Name}.");
                    }

                    if (!designer.ProvidesOutputTargets())
                    {
                        continue;
                    }

                    var outputs = package.GetElementsOfType(designer.OutputConfiguration.TemplateOutputTypeId)
                        .OfType<ElementPersistable>()
                        .Where(x => V1Footprint.IsGuardrailsTemplateOutput(x.ExternalReference, x.Name))
                        .ToList();
                    foreach (var output in outputs)
                    {
                        output.Delete();
                    }

                    if (outputs.Count > 0)
                    {
                        log.Add($"Removed {Count(outputs.Count, "Guardrails Template Output")} from {designer.Name} package {package.Name}.");
                    }
                }
            }

            foreach (var module in application.Modules.Where(x => IsGuardrails(x.ModuleId)).Select(x => x.ModuleId).ToList())
            {
                application.UninstallModule(module);
                log.Add($"Removed the {module} module.");
            }
        }

        private static bool IsGuardrails(string moduleId) => moduleId != null && V1Footprint.ModuleIds.Contains(moduleId, StringComparer.OrdinalIgnoreCase);

        /// <summary>The designer's packages this application owns.</summary>
        internal static IEnumerable<PackageModelPersistable> Owned(ApplicationPersistable application, ApplicationDesignerPersistable designer) =>
            designer.GetPackages(false, false).Where(x => string.Equals(x.ApplicationId, application.Id, StringComparison.OrdinalIgnoreCase));

        private static int Strip(List<StereotypePersistable> stereotypes) =>
            stereotypes?.RemoveAll(x => IsGuardrails(x.DefinitionPackageName)) ?? 0;

        /// <summary>
        /// Every association of the package, those the nested formats store inside elements included. The client's persistence
        /// has <c>GetAllAssociations</c> and <c>RemoveAssociation</c> for them; the 3.7.4 package this compiles against doesn't.
        /// </summary>
        internal static List<AssociationPersistable> AllAssociations(PackageModelPersistable package)
        {
            var method = package.GetType().GetMethod("GetAllAssociations", Type.EmptyTypes);
            return method == null
                ? package.Associations.ToList()
                : ((IEnumerable<AssociationPersistable>)method.Invoke(package, null)!).ToList();
        }

        internal static void RemoveAssociation(PackageModelPersistable package, AssociationPersistable association)
        {
            var method = package.GetType().GetMethod("RemoveAssociation", new[] { typeof(AssociationPersistable) });
            if (method == null)
            {
                package.Associations.Remove(association);
                return;
            }

            method.Invoke(package, new object[] { association });
        }

        /// <summary>
        /// Adds an association to its package: under its source element in the nested formats (<c>AddAssociation</c>, which the
        /// 3.7.4 package this compiles against doesn't have), else to the package's list, which the client's save moves there.
        /// </summary>
        internal static void AddAssociation(PackageModelPersistable package, AssociationPersistable association)
        {
            var method = package.GetType().GetMethod("AddAssociation", new[] { typeof(AssociationPersistable) });
            if (method == null)
            {
                package.Associations.Add(association);
                return;
            }

            method.Invoke(package, new object[] { association });
        }

        private static StereotypePersistable Stereotype(string definitionId, string name) => new()
        {
            DefinitionId = definitionId,
            Name = name,
            DefinitionPackageId = EngineIds.PackageId,
            DefinitionPackageName = EngineIds.PackageName,
        };

        private static StereotypePersistable Row(string definitionId, string name, params (string Id, string Name, string Value)[] properties)
        {
            var row = Stereotype(definitionId, name);
            foreach (var (id, property, value) in properties)
            {
                row.Properties.Add(new StereotypePropertyPersistable { DefinitionId = id, Name = property, Value = value });
            }

            return row;
        }

        /// <summary>An item list: its rows, and as its value the rows' labels, which is all the designer keeps there.</summary>
        private static StereotypePropertyPersistable ItemList(string id, string name, List<StereotypePersistable> rows, IEnumerable<string> labels) => new()
        {
            DefinitionId = id,
            Name = name,
            Value = JsonSerializer.Serialize(labels.ToArray(), DisplayOptions),
            ItemList = rows,
        };

        private static string Lines(IEnumerable<string> values) => string.Join("\n", values);

        private static string Count(int count, string noun) => $"{count} {noun}{(count == 1 ? string.Empty : "s")}";
    }
}
