using System;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Drift.EngineMigration;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Common.Drift.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GuardrailsOutputLogPruner : FactoryExtensionBase
    {
        public override string Id => "Intent.Common.Drift.GuardrailsOutputLogPruner";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateExecution(IApplication application)
        {
            Prune(application);
        }

        protected override void OnAfterCommitChanges(IApplication application)
        {
            Prune(application);
        }

        /// <summary>
        /// The move onto the drift engine on every other machine. The released contract files and the committed records reach a
        /// teammate through git, but their own <c>.intent/output-log.xml</c> still lists the contract templates. Intent reads it
        /// before any module code runs, so <see cref="ReleasedContractFileRegistration"/> keeps this run from proposing to delete
        /// those files, and this removes their entries so later runs don't need it. Once no Guardrails module is installed, this
        /// keeps the contract entries out of the records (<see cref="OutputLogPruning"/>) and deletes the old baselines. It runs
        /// at After Template Execution, because the run rewrites the log and the managed-files inventory from memory before then,
        /// so a run whose changes aren't applied leaves the pruned records on disk; and at After Commit Changes, because applying
        /// the run's changes writes them again. An agent's <c>apply_staged_file_changes</c> writes them with no hook after it, so
        /// the entries come back on that machine until its next run, which keeps the files again and prunes again.
        /// </summary>
        private static void Prune(IApplication application)
        {
            if (application.GetInstalledModules().Any(x => V1Footprint.ModuleIds.Contains(x.ModuleId, StringComparer.OrdinalIgnoreCase)))
            {
                return;
            }

            var config = application.GetApplicationConfig();
            var outputFolder = Path.GetFullPath(Path.Combine(config.DirectoryPath, config.OutputLocation));
            var removed = OutputLogPruning.Prune(config.FilePath, outputFolder).Values.Sum();
            if (removed > 0)
            {
                Logging.Log.Info($"Removed {removed} contract file {(removed == 1 ? "entry" : "entries")} the Guardrails Contracts modules left in this application's Software Factory records, so their files aren't deleted.");
            }

            var baselines = Path.GetFullPath(Path.Combine(application.GetSolutionConfig().SolutionRootLocation, ".cache", "guardrails", config.Name));
            if (Directory.Exists(baselines))
            {
                Directory.Delete(baselines, recursive: true);
            }
        }
    }
}
