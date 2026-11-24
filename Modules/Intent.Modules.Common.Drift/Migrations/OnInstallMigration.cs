using Intent.Engine;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modules.Common.Drift.Migrations
{
    /// <summary>
    /// Installing this module moves the application onto the drift engine too. Guardrails Contracts 1.0.0 carried its own
    /// engine and doesn't depend on this module, so its applications have nothing to update. In an application without v1 it
    /// changes nothing. On-Install Migrations don't run on an update, so the move never runs twice.
    /// </summary>
    public class OnInstallMigration : IModuleOnInstallMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public OnInstallMigration(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.Common.Drift";

        public void OnInstall()
        {
            Migration_02_00_00_Pre_00.Move(_configurationProvider);
        }
    }
}
