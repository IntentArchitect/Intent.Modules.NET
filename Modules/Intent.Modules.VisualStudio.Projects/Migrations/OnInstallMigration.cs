using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Modules.Constants;
using Intent.Persistence;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Migrations
{
    public class OnInstallMigration : IModuleOnInstallMigration
    {
        private readonly IPersistenceLoader _persistenceLoader;

        public OnInstallMigration(IPersistenceLoader persistenceLoader)
        {
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.VisualStudio.Projects";

        public void OnInstall()
        {
            var application = _persistenceLoader.LoadCurrentApplication();
            MigrationHelper.ApplyLaunchSettingsStereotype(application, true);
        }
    }
}