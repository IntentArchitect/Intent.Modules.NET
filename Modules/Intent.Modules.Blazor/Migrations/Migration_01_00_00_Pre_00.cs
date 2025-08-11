using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Blazor.Migrations
{
    public class Migration_01_00_00_Pre_00 : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_01_00_00_Pre_00(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.Blazor";
        [IntentFully]
        public string ModuleVersion => "1.0.0-pre.0";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            if (app == null)
                return;

            bool changes = false;
            changes |= MigrationHelper.InitializeIncludeSamplesSetting(app, "false");
            if (changes)
            {
                app.SaveAllChanges();
            }
        }

        public void Down()
        {
        }
    }
}