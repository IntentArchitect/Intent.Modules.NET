using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.Modules.Domain.Guardrails.Contracts.Drift;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Domain.Guardrails.Contracts.Templates.DriftReport
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DriftReportTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Domain.Guardrails.Contracts.DriftReport";

        [IntentManaged(Mode.Ignore)]
        private const string DesignerName = "Domain";

        [IntentManaged(Mode.Ignore)]
        private Drift.DriftReport _report;

        [IntentManaged(Mode.Ignore)]
        private readonly string _folder;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DriftReportTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            // The check runs last: in the after-build step at the highest priority, once every contract template's
            // file has been built (a C# file can't be rendered before the build step ends). The report is this template's
            // output, an MDX document in the solution's git-ignored .cache folder (outside the codebase).
            WithContentHashing = false;
            _folder = Drift.DriftReport.FolderIn(ExecutionContext.GetSolutionConfig().SolutionRootLocation, ExecutionContext.GetApplicationConfig().Name);
            MarkdownFile = new MarkdownFile($"{DesignerName}DriftReport", "mdx", Path.GetRelativePath(Path.GetFullPath(outputTarget.Location), _folder))
                .AfterBuild(file => file.FromMarkdown(BuildAndCheck().ToMarkdown()), int.MaxValue);

            // The last run's report is deleted at registration, so every run stages this run's report as a new file: the
            // Changes pane lists it even when nothing changed (it hides a change that matches the disk), and a failing
            // run, which applies nothing, leaves no stale report behind. Not on CI, where the report isn't written. The
            // .md is the report as earlier versions wrote it.
            if (Environment.GetEnvironmentVariable("TF_BUILD") == null)
            {
                foreach (var lastReport in new[] { "mdx", "md" }.Select(x => Path.Combine(_folder, $"{DesignerName}DriftReport.{x}")).Where(File.Exists))
                {
                    try
                    {
                        File.Delete(lastReport);
                    }
                    catch (Exception e) when (e is IOException or UnauthorizedAccessException)
                    {
                        Logging.Log.Warning($"The last {DesignerName} drift report couldn't be deleted, so this run's may not be listed as a change: {e.Message}");
                    }
                }
            }
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

        /// <summary>
        /// The report is only written when it needs attention: drift, a kind of fact with no recipe, a stale exception, or the
        /// check being off. A run with no drift writes none (the last one was deleted at registration, so nothing is proposed
        /// for deletion either) and logs that it found none. On CI (Azure Pipelines sets TF_BUILD) the check runs and fails
        /// the run, but the report is never written, which would otherwise be an outstanding change. The builder only
        /// builds templates that can run, so this stays true until the check has run.
        /// </summary>
        [IntentManaged(Mode.Ignore)]
        public override bool CanRunTemplate()
        {
            if (_report != null && (!_report.NeedsAttention || Environment.GetEnvironmentVariable("TF_BUILD") != null))
            {
                return false;
            }

            return base.CanRunTemplate();
        }

        [IntentManaged(Mode.Ignore)]
        private Drift.DriftReport BuildAndCheck()
        {
            var config = ExecutionContext.GetApplicationConfig();
            var recipePath = Path.Combine(config.DirectoryPath, "guardrails", $"{DesignerName.ToLowerInvariant()}.drift.json");
            var reportPath = Path.GetFullPath(GetMetadata().GetFilePath());
            if (!File.Exists(recipePath))
            {
                // The check is off until the recipe file exists, and then nothing is read or rendered. It only warns: Intent
                // applies none of a failing run's changes, and the skill that writes the file is one of them.
                _report = Drift.DriftReport.Off(DesignerName, $"guardrails/{Path.GetFileName(recipePath)}");
                Logging.Log.Warning(_report.OffMessage(reportPath));
                return _report;
            }

            IReadOnlyDictionary<string, IElement> elements = new Dictionary<string, IElement>();
            try
            {
                var code = new CodeFiles(Path.GetFullPath(Path.Combine(config.DirectoryPath, config.OutputLocation)));
                var designer = ExecutionContext.MetadataManager.GetDesigner(config.Id, ApiMetadataDesignerExtensions.DomainDesignerId);
                elements = designer == null ? elements : DriftModel.ElementsById(designer, config.Id);
                var templates = ExecutionContext.OutputTargets
                    .SelectMany(x => x.TemplateInstances)
                    .Where(x => x != this && x.CanRunTemplate())
                    .ToList();
                var generated = templates
                    .OfType<IDomainContractTemplate>()
                    .Select(x => new GeneratedFile
                    {
                        RelativePath = code.ToRelative(x.GetMetadata().GetFilePath()),
                        Text = x.CSharpFile.ToString(),
                        ElementIds = ElementIds(x.CSharpFile),
                        ModellableTypes = ModellableTypes(x.CSharpFile)
                    })
                    .ToList();
                // The check's memory, per machine and never on CI: the model as of the last run.
                var baselinePath = Path.Combine(_folder, $"{DesignerName}.model.json");
                _report = DriftCheck.Run(new DriftCheckInput
                {
                    Designer = DesignerName,
                    RecipePath = $"guardrails/{Path.GetFileName(recipePath)}",
                    RecipeFile = Path.GetFullPath(recipePath),
                    RecipeJson = File.ReadAllText(recipePath),
                    Code = code,
                    Facts = designer == null ? Array.Empty<ModelFact>() : DriftModel.Read(designer, config.Id),
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
                _report = new Drift.DriftReport(DesignerName, "guardrails", 0, 0,
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
        [IntentManaged(Mode.Ignore)]
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
        [IntentManaged(Mode.Ignore)]
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

                File.Delete(Path.Combine(_folder, $"{DesignerName}.recipe.json"));
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                Logging.Log.Warning($"The {DesignerName} drift check couldn't save what it remembers of this run, so the next run may report the model's changes as drift: {e.Message}");
            }
        }

        /// <summary>The model element each generated type and member represents, from the builder's RepresentsModel.</summary>
        [IntentManaged(Mode.Ignore)]
        internal static IReadOnlyDictionary<string, string> ElementIds(Intent.Modules.Common.CSharp.Builder.CSharpFile file)
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
        [IntentManaged(Mode.Ignore)]
        internal static IReadOnlyDictionary<string, Modellable> ModellableTypes(Intent.Modules.Common.CSharp.Builder.CSharpFile file)
        {
            var result = new Dictionary<string, Modellable>(StringComparer.Ordinal);
            void Add(string name, Intent.Modules.Common.CSharp.Builder.CSharpMetadataBase declaration)
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
