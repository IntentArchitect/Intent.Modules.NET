using Intent.Persistence;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Migrations
{
    public class Migration_04_01_11_Pre_00 : IModuleMigration
    {
        private readonly IPersistenceLoader _persistenceLoader;

        public Migration_04_01_11_Pre_00(IPersistenceLoader persistenceLoader)
        {
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.VisualStudio.Projects";
        [IntentFully]
        public string ModuleVersion => "4.1.11-pre.0";

        public void Up()
        {
            // "Generate AppSettings File" / "Generate LaunchSettings File" are now honoured for host
            // projects, which always had both files. Turn the flags on so that nothing changes for them.
            var application = _persistenceLoader.LoadCurrentApplication();
            MigrationHelper.ApplyGenerateSettingsDefaults(application);

            // HasLaunchSettings now reads the flag, so the 3.9.2-pre.0 migration (run before this one when
            // upgrading from an earlier version) skipped host projects. Repeat its call now the flags are on.
            MigrationHelper.ApplyLaunchSettingsStereotype(application);
        }

        public void Down()
        {
        }
    }
}
