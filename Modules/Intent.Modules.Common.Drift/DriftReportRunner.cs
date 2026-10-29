using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.Utils;

namespace Intent.Modules.Common.Drift
{
    /// <summary>
    /// One designer's drift check, for the drift report template of the module that contributes the designer: the
    /// template supplies its <see cref="DriftDesigner"/>, takes <see cref="ReportFile"/> as its file and answers
    /// <see cref="CanRun"/> from its <c>CanRunTemplate</c>. The check runs last: in the after-build step at the highest
    /// priority, once every contract template's file has been built (a C# file can't be rendered before the build step
    /// ends). The report is the template's output, an MDX document in the solution's git-ignored .cache folder (outside the
    /// codebase).
    /// </summary>
    public sealed class DriftReportRunner
    {
        private readonly IntentTemplateBase _template;
        private readonly DriftDesigner _designer;
        private readonly string _folder;
        private DriftReport _report;

        public DriftReportRunner(IntentTemplateBase template, DriftDesigner designer)
        {
            _template = template;
            _designer = designer;
            var context = template.ExecutionContext;
            _folder = DriftReport.FolderIn(context.GetSolutionConfig().SolutionRootLocation, context.GetApplicationConfig().Name);
            ReportFile = new MarkdownFile($"{designer.Name}DriftReport", "mdx", Path.GetRelativePath(Path.GetFullPath(template.OutputTarget.Location), _folder))
                .AfterBuild(file => file.FromMarkdown(BuildAndCheck().ToMarkdown()), int.MaxValue);

            // The last run's report is deleted at registration, so every run stages this run's report as a new file: the
            // Changes pane lists it even when nothing changed (it hides a change that matches the disk), and a failing
            // run, which applies nothing, leaves no stale report behind. Not on CI, where the report isn't written. The
            // .md is the report as earlier versions wrote it.
            if (Environment.GetEnvironmentVariable("TF_BUILD") == null)
            {
                foreach (var lastReport in new[] { "mdx", "md" }.Select(x => Path.Combine(_folder, $"{designer.Name}DriftReport.{x}")).Where(File.Exists))
                {
                    try
                    {
                        File.Delete(lastReport);
                    }
                    catch (Exception e) when (e is IOException or UnauthorizedAccessException)
                    {
                        Logging.Log.Warning($"The last {designer.Name} drift report couldn't be deleted, so this run's may not be listed as a change: {e.Message}");
                    }
                }
            }
        }

        /// <summary>The report, for the template's <c>MarkdownFile</c>.</summary>
        public IMarkdownFile ReportFile { get; }

        /// <summary>
        /// The report is only written when it needs attention: drift, a kind of fact with no recipe, a stale exception, or the
        /// check being off. A run with no drift writes none (the last one was deleted at registration, so nothing is proposed
        /// for deletion either) and logs that it found none. On CI (Azure Pipelines sets TF_BUILD) the check runs and fails
        /// the run, but the report is never written, which would otherwise be an outstanding change. The builder only
        /// builds templates that can run, so this stays true until the check has run.
        /// </summary>
        public bool CanRun => _report == null || (_report.NeedsAttention && Environment.GetEnvironmentVariable("TF_BUILD") == null);

        /// <summary>
        /// A designer's drift report as guidance names it, from the folder holding the solution's <c>.isln</c>, e.g.
        /// <c>intent/.cache/guardrails/BasislinkService/DomainDriftReport.mdx</c>. Not absolute: guidance is committed, and
        /// an absolute path differs per machine.
        /// </summary>
        public static string ReportPath(ISoftwareFactoryExecutionContext context, string designer)
        {
            var solutionFolder = Path.GetFileName(context.GetSolutionConfig().SolutionRootLocation.TrimEnd('/', '\\'));
            return $"{solutionFolder}/.cache/guardrails/{context.GetApplicationConfig().Name}/{designer}DriftReport.mdx";
        }

        private DriftReport BuildAndCheck()
        {
            var context = _template.ExecutionContext;
            var config = context.GetApplicationConfig();
            var recipePath = Path.Combine(config.DirectoryPath, "guardrails", $"{_designer.Name.ToLowerInvariant()}.drift.json");
            var reportPath = Path.GetFullPath(_template.GetMetadata().GetFilePath());
            if (!File.Exists(recipePath))
            {
                // The check is off until the recipe file exists, and then nothing is read or rendered. It only warns: Intent
                // applies none of a failing run's changes, and the skill that writes the file is one of them.
                _report = DriftReport.Off(_designer.Name, $"guardrails/{Path.GetFileName(recipePath)}");
                Logging.Log.Warning(_report.OffMessage(reportPath));
                return _report;
            }

            IReadOnlyDictionary<string, IElement> elements = new Dictionary<string, IElement>();
            try
            {
                var code = new CodeFiles(Path.GetFullPath(Path.Combine(config.DirectoryPath, config.OutputLocation)));
                var designer = context.MetadataManager.GetDesigner(config.Id, _designer.DesignerId);
                elements = designer == null ? elements : _designer.ReadElements(designer, config.Id);
                var templates = context.OutputTargets
                    .SelectMany(x => x.TemplateInstances)
                    .Where(x => x != _template && x.CanRunTemplate())
                    .ToList();
                var generated = templates
                    .OfType<IDriftStructureSource>()
                    .Where(x => x.Designer == _designer.Name)
                    .Select(x => new GeneratedFile
                    {
                        RelativePath = code.ToRelative(x.GetMetadata().GetFilePath()),
                        Text = x.CSharpFile.ToString(),
                        ElementIds = ElementIds(x.CSharpFile),
                        ModellableTypes = ModellableTypes(x.CSharpFile)
                    })
                    .ToList();
                // The check's memory, per machine and never on CI: the model as of the last run.
                var baselinePath = Path.Combine(_folder, $"{_designer.Name}.model.json");
                _report = DriftCheck.Run(new DriftCheckInput
                {
                    Designer = _designer.Name,
                    Kinds = _designer.Kinds,
                    RecipePath = $"guardrails/{Path.GetFileName(recipePath)}",
                    RecipeFile = Path.GetFullPath(recipePath),
                    RecipeJson = File.ReadAllText(recipePath),
                    Code = code,
                    Facts = designer == null ? Array.Empty<ModelFact>() : _designer.ReadFacts(designer, config.Id),
                    Generated = generated,
                    OutputPaths = templates.Select(x => code.ToRelative(x.GetMetadata().GetFilePath())).ToList(),
                    Application = config.Name,
                    Baseline = File.Exists(baselinePath) ? ModelBaseline.Parse(File.ReadAllText(baselinePath)) : null
                });
                SaveMemory(baselinePath);
            }
            catch (Exception e)
            {
                Logging.Log.Failure(e);
                _report = new DriftReport(_designer.Name, "guardrails", 0, 0,
                    new[] { Finding.Unverifiable($"The drift check failed, so nothing could be verified: {e.GetType().Name}: {e.Message}") },
                    Array.Empty<string>());
            }

            if (_report.PendingCount > 0)
            {
                Logging.Log.Info(_report.PendingMessage());
            }

            // The model may be incomplete, but it must never be wrong: a Wrong line fails the run (and the CLI) without
            // blocking the report. A kind of fact with no recipe only warns, so a module upgrade that adds facts never
            // breaks a run.
            if (_report.WrongLines.Count > 0)
            {
                LogFailures(elements, reportPath);
            }
            else
            {
                Logging.Log.Info(_report.NoDriftMessage());
            }

            if (_report.NotChecked.Count > 0)
            {
                Logging.Log.Warning(_report.NotCheckedMessage(reportPath));
            }

            if (_report.UnusedExceptions.Count > 0)
            {
                Logging.Log.Warning(_report.UnusedExceptionsMessage(reportPath));
            }

            return _report;
        }

        /// <summary>
        /// Fails the run with one error naming the report and how many drifts it lists. It is an element error on the first
        /// line's model element when one is known, so Intent offers to fix it with AI; otherwise a friendly error. Logged,
        /// not thrown: a thrown exception ends the run before anything is staged, so the report and the run's other changes
        /// would never land, and it files a problem report with Intent. On CI no report is written, so a friendly error lists
        /// the lines instead.
        /// </summary>
        private void LogFailures(IReadOnlyDictionary<string, IElement> elements, string reportPath)
        {
            var onCi = Environment.GetEnvironmentVariable("TF_BUILD") != null;
            var message = _report.FailureMessage(reportPath, listLines: onCi);
            var element = onCi ? null : _report.FailureElementIds.Where(elements.ContainsKey).Select(x => elements[x]).FirstOrDefault();
            Logging.Log.Failure(element == null ? new FriendlyException(message) : new ElementException(element, message));
        }

        /// <summary>
        /// Keeps the model as of this run for the next one, per machine and outside staging: it is the check's own memory,
        /// not output. Not on CI, where every run starts without one and nothing is skipped. The recipe memory earlier
        /// versions kept (<c>&lt;Designer&gt;.recipe.json</c>) is deleted: the recipes are now validated on demand.
        /// </summary>
        private void SaveMemory(string baselinePath)
        {
            if (Environment.GetEnvironmentVariable("TF_BUILD") != null)
            {
                return;
            }

            try
            {
                Directory.CreateDirectory(_folder);
                if (_report.Baseline != null)
                {
                    File.WriteAllText(baselinePath, _report.Baseline.ToJson());
                }

                File.Delete(Path.Combine(_folder, $"{_designer.Name}.recipe.json"));
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                Logging.Log.Warning($"The {_designer.Name} drift check couldn't save what it remembers of this run, so the next run may report the model's changes as drift: {e.Message}");
            }
        }

        /// <summary>The model element each generated type and member represents, from the builder's RepresentsModel.</summary>
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
