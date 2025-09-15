using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using Newtonsoft.Json.Linq;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modules.Blazor.FluentValidation.Migrations
{
    public class OnInstallMigration : IModuleOnInstallMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public OnInstallMigration(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.Blazor.FluentValidation";

        public void OnInstall()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

            var group = app.ModuleSettingGroups.FirstOrDefault(x => x.Id == "489a67db-31b2-4d51-96d7-52637c3795be");//Blazor
            if (group == null)
            {
                group = new ApplicationModuleSettingsPersistable
                {
                    Id = "489a67db-31b2-4d51-96d7-52637c3795be",
                    Module = "Intent.Blazor",
                    Title = "Blazor Settings",
                    Settings = []
                };

                app.ModuleSettingGroups.Add(group);
            }

            var includeSamplesSetting = group.Settings.FirstOrDefault(s => s.Id == "ab557f4a-cd5b-4ae1-bbc2-ba0522503956");//Create Model Definition Validators

            if (includeSamplesSetting == null)
            {
                group.Settings.Add(new ModuleSettingPersistable
                {
                    Id = "ab557f4a-cd5b-4ae1-bbc2-ba0522503956",
                    Title = "Create Model Definition Validators",
                    Module = "Intent.Blazor.FluentValidation",
                    ControlType = ModuleSettingControlType.Switch,
                    Value = "false",
                });
                app.SaveAllChanges();
            }
        }
    }
}