using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.DriftReporting.Api;
using Intent.Modules.Common.Templates;
using Intent.Utils;

namespace Intent.Modules.Common.Drift
{
    /// <summary>
    /// One designer's drift check, for the drift reporter of the module that contributes the designer: the reporter calls
    /// <see cref="Report"/> with its <see cref="DriftDesigner"/>. It runs after template execution, once every contract
    /// template's C# file has been built, and adds what it finds to the designer's Area of the application's drift report
    /// (<see cref="HubMapping"/>); the Intent.Common.DriftReporting module logs the findings and writes the report.
    /// </summary>
    public static class DriftCheckRunner
    {
        public static void Report(IDriftContext context, DriftDesigner designer) => Report(context, designer, IsCi(context.Application));

        internal static void Report(IDriftContext context, DriftDesigner designer, bool onCi)
        {
            var application = context.Application;
            var config = application.GetApplicationConfig();
            var folder = DriftReport.FolderIn(application.GetSolutionConfig().SolutionRootLocation, config.Name);
            DeleteOldReports(folder, designer.Name);

            var recipePath = Path.Combine(config.DirectoryPath, "guardrails", $"{designer.Name.ToLowerInvariant()}.drift.json");
            var recipeFile = Path.GetFullPath(recipePath);
            if (!File.Exists(recipePath))
            {
                // The check is off until the recipe file exists, and then nothing is read. It only warns, and has no Area (which
                // would show as checked and clean): Intent applies none of a failing run's changes, and the skill that writes
                // the file is one of them.
                Logging.Log.Warning(DriftReport.Off(designer.Name, $"guardrails/{Path.GetFileName(recipePath)}").OffMessage());
                return;
            }

            var area = context.Area(designer.Name, DriftCategory.ModelCode, DriftCategory.Code);

            var codeRoot = Path.GetFullPath(Path.Combine(config.DirectoryPath, config.OutputLocation));
            IReadOnlyDictionary<string, IElement> elements = new Dictionary<string, IElement>();
            DriftReport report;
            try
            {
                var code = new CodeFiles(codeRoot);
                var model = application.MetadataManager.GetDesigner(config.Id, designer.DesignerId);
                elements = model == null ? elements : designer.ReadElements(model, config.Id);

                // Every template has run, so every contract template's C# file is built.
                var templates = application.OutputTargets
                    .SelectMany(x => x.TemplateInstances)
                    .Where(x => x.CanRunTemplate())
                    .ToList();
                var generated = templates
                    .OfType<IDriftStructureSource>()
                    .Where(x => x.Designer == designer.Name)
                    .Select(x => new GeneratedFile
                    {
                        RelativePath = code.ToRelative(x.GetMetadata().GetFilePath()),
                        Text = x.CSharpFile.ToString(),
                        ElementIds = ElementIds(x.CSharpFile),
                        ModellableTypes = ModellableTypes(x.CSharpFile),
                        EmptyPrimaryConstructors = EmptyPrimaryConstructors(x.CSharpFile)
                    })
                    .ToList();

                // The check's memory, per machine and never on CI: the model as of the last run.
                var baselinePath = Path.Combine(folder, $"{designer.Name}.model.json");
                report = DriftCheck.Run(new DriftCheckInput
                {
                    Designer = designer.Name,
                    Kinds = designer.Kinds,
                    RecipePath = $"guardrails/{Path.GetFileName(recipePath)}",
                    RecipeFile = recipeFile,
                    RecipeJson = File.ReadAllText(recipePath),
                    Code = code,
                    Facts = model == null ? Array.Empty<ModelFact>() : designer.ReadFacts(model, config.Id),
                    Generated = generated,
                    OutputPaths = templates.Select(x => code.ToRelative(x.GetMetadata().GetFilePath())).ToList(),
                    Application = config.Name,
                    Baseline = File.Exists(baselinePath) ? ModelBaseline.Parse(File.ReadAllText(baselinePath)) : null
                });
                SaveMemory(report, folder, baselinePath, designer.Name, onCi);
            }
            catch (Exception e)
            {
                report = new DriftReport(designer.Name, $"guardrails/{Path.GetFileName(recipePath)}", 0, 0,
                    new[] { Finding.Unverifiable($"The drift check failed, so nothing could be verified: {e.GetType().Name}: {e.Message}") },
                    Array.Empty<string>());
            }

            HubMapping.Map(report, area, elements, codeRoot, recipeFile);
            if (report.WrongLines.Count == 0)
            {
                Logging.Log.Info(report.NoDriftMessage());
            }
        }

        /// <summary>The per-designer reports earlier versions wrote beside the check's memory, which the drift report replaces.</summary>
        private static void DeleteOldReports(string folder, string designer)
        {
            foreach (var oldReport in new[] { "mdx", "md" }.Select(x => Path.Combine(folder, $"{designer}DriftReport.{x}")).Where(File.Exists))
            {
                try
                {
                    File.Delete(oldReport);
                }
                catch (Exception e) when (e is IOException or UnauthorizedAccessException)
                {
                    Logging.Log.Warning($"The {designer} drift report an earlier version wrote couldn't be deleted: {e.Message}");
                }
            }
        }

        /// <summary>
        /// Keeps the model as of this run for the next one, per machine and outside staging: it is the check's own memory,
        /// not output. Not on CI, where every run starts without one and nothing is skipped. The recipe memory earlier
        /// versions kept (<c>&lt;Designer&gt;.recipe.json</c>) is deleted: the recipes are now validated on demand.
        /// </summary>
        private static void SaveMemory(DriftReport report, string folder, string baselinePath, string designer, bool onCi)
        {
            if (onCi)
            {
                return;
            }

            try
            {
                Directory.CreateDirectory(folder);
                if (report.Baseline != null)
                {
                    File.WriteAllText(baselinePath, report.Baseline.ToJson());
                }

                File.Delete(Path.Combine(folder, $"{designer}.recipe.json"));
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                Logging.Log.Warning($"The {designer} drift check couldn't save what it remembers of this run, so the next run may report the model's changes as drift: {e.Message}");
            }
        }

        /// <summary>The model element each generated type and member represents, from the builder's RepresentsModel.</summary>
        /// <summary>
        /// Whether this is a CI/CD run: the CLI's <c>ensure-no-outstanding-changes</c> parameter is <c>true</c>. <c>GetParameter</c>
        /// arrived in SDK 3.16, after the one this module builds against, so it is found by reflection: an older host has no
        /// parameters, so its runs never are.
        /// </summary>
        private static bool IsCi(ISoftwareFactoryExecutionContext context)
        {
            // TODO: when Intent.SoftwareFactory.SDK 3.16 is published, build against it and replace this reflection with
            // context.GetParameter("ensure-no-outstanding-changes").
            var getParameter = typeof(ISoftwareFactoryExecutionContext).GetMethod("GetParameter", new[] { typeof(string) });
            try
            {
                return string.Equals(getParameter?.Invoke(context, new object[] { "ensure-no-outstanding-changes" }) as string, "true", StringComparison.OrdinalIgnoreCase);
            }
            catch (TargetInvocationException)
            {
                return false;
            }
        }

        internal static IReadOnlyDictionary<string, string> ElementIds(CSharpFile file)
        {
            var ids = new Dictionary<string, string>(StringComparer.Ordinal);
            void Add(string key, object declaration)
            {
                if (declaration.GetType().GetProperty("RepresentedModel")?.GetValue(declaration) is IMetadataModel model && !ids.ContainsKey(key))
                {
                    ids[key] = model.Id;
                }
            }

            foreach (var type in file.TypeDeclarations)
            {
                Add(type.Name, type);
                foreach (var member in type.Properties.Cast<object>().Concat(type.Methods).Concat(type.Fields))
                {
                    Add($"{type.Name}.{member.GetType().GetProperty("Name")?.GetValue(member)}", member);
                }

                foreach (var constructor in type.Constructors)
                {
                    Add($"{type.Name}.{type.Name}", constructor);
                }
            }

            foreach (var type in file.Interfaces)
            {
                Add(type.Name, type);
                foreach (var member in type.Properties.Cast<object>().Concat(type.Methods))
                {
                    Add($"{type.Name}.{member.GetType().GetProperty("Name")?.GetValue(member)}", member);
                }
            }

            return ids;
        }

        /// <summary>
        /// The types whose builder declares a primary constructor with no parameters, which the C# builder doesn't write
        /// (<see cref="GeneratedFile.EmptyPrimaryConstructors"/>).
        /// </summary>
        internal static IReadOnlyCollection<string> EmptyPrimaryConstructors(CSharpFile file)
        {
            return file.TypeDeclarations
                .Where(type => type.Constructors.Any(x => x.IsPrimaryConstructor && x.Parameters.Count == 0))
                .Select(type => type.Name)
                .ToHashSet(StringComparer.Ordinal);
        }

        /// <summary>What the model can state about each generated type, as its builder recorded it (<see cref="Modellable"/>).</summary>
        internal static IReadOnlyDictionary<string, Modellable> ModellableTypes(CSharpFile file)
        {
            var result = new Dictionary<string, Modellable>(StringComparer.Ordinal);
            void Add(string name, CSharpMetadataBase declaration)
            {
                if (declaration.TryGetMetadata<Modellable>(Modellable.MetadataKey, out var modellable))
                {
                    result.TryAdd(name, modellable);
                }
            }

            foreach (var type in file.TypeDeclarations)
            {
                Add(type.Name, type);
            }

            foreach (var type in file.Interfaces)
            {
                Add(type.Name, type);
            }

            foreach (var type in file.Enums)
            {
                Add(type.Name, type);
            }

            return result;
        }
    }
}
