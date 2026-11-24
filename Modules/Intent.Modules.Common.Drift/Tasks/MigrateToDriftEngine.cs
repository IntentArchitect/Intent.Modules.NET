using System;
using System.Text.Json;
using Intent.Configuration;
using Intent.Modules.Common.Drift.EngineMigration;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.ModuleTask", Version = "1.0")]

namespace Intent.Modules.Common.Drift.Tasks
{
    /// <summary>
    /// Runs the move onto the drift engine again (<see cref="DriftEngineMigration"/>), for an application whose update failed
    /// to move it: Intent doesn't run a migration again once the module is at its new version. Argument:
    /// <c>{ "applicationId": "…" }</c>. Result: <c>{ "log": [ … ] }</c>, every change made, or <c>{ "errorMessage": "…", "log": [ … ] }</c>.
    /// </summary>
    [IntentManaged(Mode.Merge)]
    public class MigrateToDriftEngine : IModuleTask
    {
        [IntentManaged(Mode.Ignore)]
        private static readonly JsonSerializerOptions ArgumentOptions = new() { PropertyNameCaseInsensitive = true };

        [IntentManaged(Mode.Ignore)]
        private readonly ISolutionConfig _solution;

        [IntentManaged(Mode.Merge)]
        public MigrateToDriftEngine(ISolutionConfig solution)
        {
            _solution = solution;
        }

        public string TaskTypeId => "Intent.Common.Drift.MigrateToDriftEngine";
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public string TaskTypeName => "Migrate to Drift Engine";
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public int Order => 0;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public string Execute(params string[] args)
        {
            string applicationId;
            try
            {
                applicationId = args.Length == 0 ? null : JsonSerializer.Deserialize<TaskArguments>(args[0], ArgumentOptions)?.ApplicationId;
            }
            catch (JsonException e)
            {
                return JsonSerializer.Serialize(new { errorMessage = e.Message });
            }

            var application = string.IsNullOrWhiteSpace(applicationId) ? null : _solution.GetApplicationConfig(applicationId);
            if (application == null)
            {
                return JsonSerializer.Serialize(new
                {
                    errorMessage = string.IsNullOrWhiteSpace(applicationId)
                    ? "Expected one argument: { \"applicationId\": \"<the application's id>\" }."
                    : $"No application has the id {applicationId}."
                });
            }

            var log = new MigrationLog(application.Name);
            try
            {
                // After a migration Intent reloads the application from disk; after a task it keeps the modules it loaded.
                if (DriftEngineMigration.Run(application, _solution, log))
                {
                    log.Add("Close and reopen the solution before the next Software Factory run, so Intent loads the application without the Guardrails Contracts modules.");
                }

                return JsonSerializer.Serialize(new { log });
            }
            catch (Exception e)
            {
                return JsonSerializer.Serialize(new { errorMessage = e.Message, log });
            }
        }

        [IntentManaged(Mode.Ignore)]
        private sealed class TaskArguments
        {
            public string ApplicationId { get; set; }
        }
    }
}
