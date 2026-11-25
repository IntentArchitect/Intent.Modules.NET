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

            var recipes = recipeFiles.Select(x => new RecipeSource(Path.GetFileName(x), File.ReadAllText(x))).ToList();
            var rows = RecipeConversion.Convert(recipes);
            foreach (var note in rows.Notes)
            {
                log.Add(note);
            }

            // The code as v1 read it, before the contract files are released: what v1 didn't report becomes exceptions, and a
            // Table Name v1 read from EF Core's convention is cleared, so the first run reports what v1 did.
            var outputFolder = OutputFolder(application);
            var code = new CodeFiles(outputFolder);
            var contractFiles = ContractFiles(application, solution, outputFolder);
            var modelledTypes = ModelledTypes(model);
            DropUnmatched(rows, V1Coverage.NeverUnmodelled(code, modelledTypes),
                V1Coverage.NeverUnmodelledMembers(code, rows.Exceptions.Where(x => x.Rule == RecipeConversion.UnmodelledMember).SelectMany(x => x.Matches)), log);
            var converted = new HashSet<string>(rows.Exceptions.SelectMany(x => x.Matches), StringComparer.Ordinal);
            foreach (var row in V1Coverage.Exceptions(code, recipes, rows, contractFiles, modelledTypes, log))
            {
                var matches = row.Matches.Where(x => !converted.Contains(x)).ToList();
                if (matches.Count > 0)
                {
                    rows.Exceptions.Add(row with { Matches = matches });
                }
            }

            ConventionalTableNames.Clear(model, code, log);

            ReleaseContractFiles(outputFolder, contractFiles, application, log);

            var wroteCheck = GuardrailsModelCleanup.Apply(model, rows, log);
            CorrectModel(model, outputFolder, Path.Combine(solution.SolutionRootLocation, ".cache", "modules"), wroteCheck, log);
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

        /// <summary>
        /// The contract files, relative to the output folder: those the logs list, and those whose header names a contract
        /// template. When another application writes to the same folder, its contract files can't be told apart, so only the
        /// logs say which are this application's.
        /// </summary>
        private static IReadOnlyList<string> ContractFiles(IApplicationConfig application, ISolutionConfig solution, string outputFolder)
        {
            var others = (solution.GetApplicationConfigs() ?? Enumerable.Empty<IApplicationConfig>())
                .Where(x => x != null && !string.Equals(x.Id, application.Id, StringComparison.OrdinalIgnoreCase))
                .Select(OutputFolder)
                .ToList();
            return OutputLogPruning.ContractFiles(application.FilePath, outputFolder)
                .Concat(others.Contains(outputFolder, StringComparer.OrdinalIgnoreCase) ? Enumerable.Empty<string>() : ContractRelease.FindContractFiles(outputFolder, others))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        /// <summary>
        /// Drops the converted exceptions that name what the drift engine never reports as unmodelled: a modelled type
        /// (<see cref="V1Coverage.NeverUnmodelled"/>), whose v1 exception covered a finding of its own conventions, and an
        /// override or a member every type has (<see cref="V1Coverage.NeverUnmodelledMembers"/>), which v1 reported and the
        /// engine doesn't. On the engine each row would match nothing and warn on the first run.
        /// </summary>
        private static void DropUnmatched(ConvertedRecipes rows, IReadOnlyCollection<string> neverUnmodelled, IReadOnlyCollection<string> neverUnmodelledMembers,
            ICollection<string> log)
        {
            for (var i = rows.Exceptions.Count - 1; i >= 0; i--)
            {
                var row = rows.Exceptions[i];
                var never = row.Rule == RecipeConversion.UnmodelledType ? neverUnmodelled
                    : row.Rule == RecipeConversion.UnmodelledMember ? neverUnmodelledMembers
                    : new HashSet<string>();
                var dropped = row.Matches.Where(never.Contains).ToList();
                if (dropped.Count == 0)
                {
                    continue;
                }

                foreach (var match in dropped)
                {
                    log.Add(row.Rule == RecipeConversion.UnmodelledType
                        ? $"Left out v1's exception for `{match}` ({row.Reason}): it names a modelled type, which the drift engine never reports as unmodelled."
                        : $"Left out v1's exception for `{match}` ({row.Reason}): it names an override, or a member every type has, which the drift engine never reports as unmodelled.");
                }

                var kept = row.Matches.Where(x => !dropped.Contains(x)).ToList();
                if (kept.Count == 0)
                {
                    rows.Exceptions.RemoveAt(i);
                }
                else
                {
                    rows.Exceptions[i] = row with { Matches = kept };
                }
            }
        }

        /// <summary>
        /// Brings the moved model into line with the unchanged code (<see cref="ModelCorrections"/>), before the one save. The
        /// engine's assemblies are Intent.Common.DriftEngine's, which the task host loads from that module's <c>lib</c> folder;
        /// if they won't load, or the pass fails, the move still completes and the first run reports what it would have corrected.
        /// Each correction is a true statement on its own, so one made before a failure is kept.
        /// </summary>
        private static void CorrectModel(ApplicationPersistable model, string outputFolder, string moduleCache, bool tidyExceptions, ICollection<string> log)
        {
            try
            {
                ModelCorrections.Apply(model, outputFolder, moduleCache, tidyExceptions, log);
            }
            catch (Exception e)
            {
                log.Add($"Warning: the drift engine couldn't bring the model into line with the code ({e.GetType().Name}: {e.Message}), so the first Software Factory run may report drift v1 never did.");
            }
        }

        /// <summary>The application's type elements, which the drift engine locates in the code by name.</summary>
        private static IReadOnlyList<ModelledType> ModelledTypes(ApplicationPersistable model) =>
            model.GetDesigners()
                .SelectMany(x => GuardrailsModelCleanup.Owned(model, x))
                .SelectMany(x => x.FindAllChildElements().OfType<ElementPersistable>())
                .Where(x => V1Coverage.TypeElements.Contains(x.SpecializationType))
                .Select(x => new ModelledType(x.Name, x.SpecializationType, x.ChildElements?.Select(c => c.Name).ToList() ?? new List<string>()))
                .ToList();

        private static void ReleaseContractFiles(string outputFolder, IReadOnlyList<string> files, IApplicationConfig application, ICollection<string> log)
        {
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
