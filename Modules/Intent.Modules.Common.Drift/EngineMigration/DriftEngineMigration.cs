using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Intent.Configuration;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Serialization;
using Intent.Utils;

namespace Intent.Modules.Common.Drift.EngineMigration
{
    /// <summary>
    /// Moves an application from the v1 drift check (the Guardrails Contracts modules and their recipe files) onto
    /// Intent.Common.DriftEngine. Run by Common.Drift's 2.0.0-pre.0 Version Migration, its On-Install Migration and the Migrate
    /// to Drift Engine module task, which is how a failed run is recovered: Intent doesn't run a migration again once
    /// <c>modules.config</c> is at the new version. Every step looks before it acts, so running it again changes nothing.
    /// <list type="number">
    /// <item>v1 is there when a Guardrails module is installed or a recipe file exists; otherwise only old baselines go.</item>
    /// <item>The recipe files are read (<see cref="RecipeConversion"/>).</item>
    /// <item>The contract files are released (<see cref="ContractRelease"/>) and the Software Factory forgets them
    /// (<see cref="OutputLogPruning"/>). This is before the model changes: if saving the model then fails, the Guardrails modules
    /// are still installed, and their next run merges the attributes back and logs the files again, so nothing is deleted.</item>
    /// <item>The model changes, saved together (<see cref="GuardrailsModelCleanup"/>).</item>
    /// <item>The recipe files and the old baselines are deleted.</item>
    /// </list>
    /// It changes source files outside Change Review, so every file and row it writes is logged: the log and <c>git diff</c> are
    /// how a developer reviews it.
    /// </summary>
    internal static class DriftEngineMigration
    {
        /// <param name="log">Gets each change as it is made, so a run that fails part way still says what it did.</param>
        /// <returns>Whether there was a v1 drift check to move.</returns>
        public static bool Run(IApplicationConfig application, ISolutionConfig solution, ICollection<string> log)
        {
            var recipeFolder = Path.Combine(application.DirectoryPath, V1Footprint.RecipeFolder);
            var recipeFiles = Directory.Exists(recipeFolder)
                ? Directory.GetFiles(recipeFolder, V1Footprint.RecipeFilePattern).OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList()
                : new List<string>();

            ClearPersistenceCache();
            var model = ApplicationPersistable.Load(application.FilePath, true);
            if (!GuardrailsModelCleanup.HasGuardrails(model) && recipeFiles.Count == 0)
            {
                DeleteBaselines(application, solution, log);
                log.Add($"{application.Name} has no v1 drift check (no Guardrails Contracts module or recipe file), so there was nothing to move.");
                return false;
            }

            var rows = RecipeConversion.Convert(recipeFiles.Select(x => new RecipeSource(Path.GetFileName(x), File.ReadAllText(x))));
            foreach (var note in rows.Notes)
            {
                log.Add(note);
            }

            ReleaseContractFiles(application, solution, log);

            GuardrailsModelCleanup.Apply(model, rows, log);
            model.SaveAllChanges();

            foreach (var file in recipeFiles)
            {
                File.Delete(file);
                log.Add($"Deleted {V1Footprint.RecipeFolder}/{Path.GetFileName(file)}.");
            }

            if (Directory.Exists(recipeFolder) && !Directory.EnumerateFileSystemEntries(recipeFolder).Any())
            {
                Directory.Delete(recipeFolder);
            }

            DeleteBaselines(application, solution, log);
            log.Add($"Moved {application.Name} onto the drift engine. Review the changes with git diff; the next Software Factory run proposes deleting the old drift skills and guides.");
            return true;
        }

        private static void ReleaseContractFiles(IApplicationConfig application, ISolutionConfig solution, ICollection<string> log)
        {
            var outputFolder = OutputFolder(application);
            var others = (solution.GetApplicationConfigs() ?? Enumerable.Empty<IApplicationConfig>())
                .Where(x => x != null && !string.Equals(x.Id, application.Id, StringComparison.OrdinalIgnoreCase))
                .Select(OutputFolder)
                .ToList();

            // Files the logs list, and those whose header names a contract template. When another application writes to the
            // same folder, its contract files can't be told apart, so only the logs say which are this application's.
            var files = OutputLogPruning.ContractFiles(application.FilePath, outputFolder)
                .Concat(others.Contains(outputFolder, StringComparer.OrdinalIgnoreCase) ? Enumerable.Empty<string>() : ContractRelease.FindContractFiles(outputFolder, others))
                .Distinct(StringComparer.OrdinalIgnoreCase);
            foreach (var file in files)
            {
                if (file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) && ContractRelease.ReleaseFile(Path.Combine(outputFolder, file)))
                {
                    log.Add($"Removed the Intent attributes from {file}.");
                }
            }

            foreach (var (record, removed) in OutputLogPruning.Prune(application.FilePath, outputFolder).Where(x => x.Value > 0))
            {
                log.Add($"Removed {removed} contract file {(removed == 1 ? "entry" : "entries")} from {record}, so the Software Factory doesn't delete them.");
            }
        }

        private static string OutputFolder(IApplicationConfig application) =>
            Path.GetFullPath(Path.Combine(application.DirectoryPath, application.OutputLocation)).TrimEnd(Path.DirectorySeparatorChar);

        /// <summary>
        /// The task host is reused and caches what it has read, and the migration needs the model as saved. Where caching was
        /// never switched on there is nothing to clear, and <c>ClearCache</c> throws on its missing cache.
        /// </summary>
        private static void ClearPersistenceCache()
        {
            try
            {
                XmlSerializationHelper.ClearCache();
            }
            catch (NullReferenceException)
            {
            }
        }

        /// <summary>v1's per-machine baselines, <c>.cache/guardrails/&lt;application&gt;/</c> beside the solution.</summary>
        private static void DeleteBaselines(IApplicationConfig application, ISolutionConfig solution, ICollection<string> log)
        {
            var folder = Path.GetFullPath(Path.Combine(solution.SolutionRootLocation, ".cache", "guardrails", application.Name));
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, recursive: true);
                log.Add($"Deleted the old drift baselines in .cache/guardrails/{application.Name}.");
            }
        }
    }

    /// <summary>The migration's log: each line goes to the install's output as it is added. A line starting <c>Warning: </c> is a warning.</summary>
    internal sealed class MigrationLog : Collection<string>
    {
        private readonly string _application;

        public MigrationLog(string application) => _application = application;

        protected override void InsertItem(int index, string item)
        {
            base.InsertItem(index, item);
            if (item.StartsWith("Warning: ", StringComparison.Ordinal))
            {
                Logging.Log.Warning($"[{_application}] {item["Warning: ".Length..]}");
            }
            else
            {
                Logging.Log.Info($"[{_application}] {item}");
            }
        }
    }
}
