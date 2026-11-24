using System;
using Intent.Engine;
using Intent.Modules.Common.Drift.EngineMigration;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Common.Drift.Migrations
{
    /// <summary>
    /// Updating from 1.x moves the application onto the drift engine (<see cref="DriftEngineMigration"/>). Its constructor takes
    /// only an injected service: a migration constructor that throws stops every migration in the application.
    /// </summary>
    public class Migration_02_00_00_Pre_00 : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_02_00_00_Pre_00(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.Common.Drift";
        [IntentFully]
        public string ModuleVersion => "2.0.0-pre.0";

        public void Up()
        {
            Move(_configurationProvider);
        }

        public void Down()
        {
            Logging.Log.Warning("Intent.Common.Drift 1.x checks drift through the Guardrails Contracts modules and recipe files, which the move to 2.0 removed. Going back doesn't restore them: reinstall the Guardrails Contracts modules and restore the recipe files from git.");
        }

        /// <summary>
        /// Runs the move and logs a failure instead of throwing, so Intent finishes the update. A failed move leaves the v1 check
        /// working, and the Migrate to Drift Engine module task completes it.
        /// </summary>
        internal static void Move(IApplicationConfigurationProvider configurationProvider)
        {
            var application = configurationProvider.GetApplicationConfig();
            try
            {
                DriftEngineMigration.Run(application, configurationProvider.GetSolutionConfig(), new MigrationLog(application.Name));
            }
            catch (Exception e)
            {
                Logging.Log.Failure($"Moving {application.Name} onto the drift engine failed, so its v1 drift check is still in place: {e.Message} Fix the cause, then run the module task Migrate to Drift Engine (Intent.Common.Drift.MigrateToDriftEngine) with {{ \"applicationId\": \"{application.Id}\" }}.");
                Logging.Log.Debug(e.ToString());
            }
        }
    }
}
