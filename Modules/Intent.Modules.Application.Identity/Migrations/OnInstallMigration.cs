using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modules.Application.Identity.Migrations
{
    public class OnInstallMigration : IModuleOnInstallMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public OnInstallMigration(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.Application.Identity";

        public void OnInstall()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

            var group = app.ModuleSettingGroups.FirstOrDefault(x => x.Id == "1045dea6-d28f-4ab8-9b5e-6f360035fdb6");//Identity Settings
            if (group == null)
            {
                group = new ApplicationModuleSettingsPersistable
                {
                    Id = "1045dea6-d28f-4ab8-9b5e-6f360035fdb6",
                    Module = "Intent.Application.Identity",
                    Title = "Identity Settings",
                    Settings = []
                };

                app.ModuleSettingGroups.Add(group);
            }

            var syncAccessorsConvention = group.Settings.FirstOrDefault(s => s.Id == "fb7918a2-c5e5-4eb1-b840-5359765e2392");

            if (syncAccessorsConvention == null)
            {
                group.Settings.Add(new ModuleSettingPersistable
                {
                    Id = "fb7918a2-c5e5-4eb1-b840-5359765e2392",
                    Title = "Keep Sync Accessors",
                    Module = "Intent.Application.Identity",
                    Hint = "Synchronous accessors for UserName and UserId have been removed as the async variants .",
                    ControlType = ModuleSettingControlType.Switch,
                    Value = "false",
                });
            }
            app.SaveAllChanges();

        }
    }
}