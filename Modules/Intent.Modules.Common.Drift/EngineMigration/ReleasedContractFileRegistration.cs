using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Registrations;
using Intent.Utils;

namespace Intent.Modules.Common.Drift.EngineMigration
{
    /// <summary>
    /// Keeps a teammate's first run after the move from proposing to delete the contract files. Their own
    /// <c>.intent/output-log.xml</c> still lists the contract templates' files, and the Software Factory reads it before any module
    /// code runs, so it would propose deleting every file this run doesn't generate. This registers a
    /// <see cref="ReleasedContractFileTemplate"/> for each contract file that log, or the committed legacy log, still lists and that
    /// is still on disk, into the output target the file was generated into. The Software Factory then counts each file as
    /// generated, so it proposes no delete. <see cref="FactoryExtensions.GuardrailsOutputLogPruner"/> removes the entries during the
    /// run, so the next run registers nothing. Only once no Guardrails Contracts module is installed: while one is, its own
    /// templates generate these files.
    /// </summary>
    public class ReleasedContractFileRegistration : ITemplateRegistration
    {
        public string TemplateId => ReleasedContractFileTemplate.TemplateId;

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            if (application.GetInstalledModules().Any(x => V1Footprint.ModuleIds.Contains(x.ModuleId, StringComparer.OrdinalIgnoreCase)))
            {
                return;
            }

            var config = application.GetApplicationConfig();
            var outputFolder = Path.GetFullPath(Path.Combine(config.DirectoryPath, config.OutputLocation));
            var outputTargets = application.OutputTargets.ToList();
            var registered = 0;
            foreach (var entry in OutputLogPruning.ContractEntries(config.FilePath, outputFolder))
            {
                var fullPath = Path.GetFullPath(Path.Combine(outputFolder, entry.RelativePath));
                if (!File.Exists(fullPath) || OutputTargetFor(outputTargets, entry.ProjectId, fullPath) is not { } outputTarget)
                {
                    continue;
                }

                var file = new ReleasedContractFileTemplate.ReleasedFile(fullPath, entry.CorrelationId);
                registry.RegisterTemplate(TemplateId, outputTarget, target => new ReleasedContractFileTemplate(target, file));
                registered++;
            }

            if (registered > 0)
            {
                Logging.Log.Info($"Kept {registered} contract {(registered == 1 ? "file" : "files")} the Guardrails Contracts modules generated, which this machine's Software Factory records still list, so they aren't deleted.");
            }
        }

        /// <summary>
        /// The output target the log recorded for the file, if it still exists and holds the file; otherwise the innermost output
        /// target that holds it. None when no output target holds it.
        /// </summary>
        internal static IOutputTarget OutputTargetFor(IReadOnlyCollection<IOutputTarget> outputTargets, string projectId, string fullPath)
        {
            var holding = outputTargets.Where(x => Holds(x.Location, fullPath)).ToList();
            return holding.FirstOrDefault(x => string.Equals(x.Id, projectId, StringComparison.OrdinalIgnoreCase))
                ?? holding.OrderByDescending(x => Path.GetFullPath(x.Location).Length).FirstOrDefault();
        }

        private static bool Holds(string location, string fullPath)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return false;
            }

            var folder = Path.TrimEndingDirectorySeparator(Path.GetFullPath(location)) + Path.DirectorySeparatorChar;
            return fullPath.StartsWith(folder, StringComparison.OrdinalIgnoreCase);
        }
    }
}
