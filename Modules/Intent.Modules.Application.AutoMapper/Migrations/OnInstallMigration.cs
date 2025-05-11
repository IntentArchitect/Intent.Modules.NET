using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modules.Application.AutoMapper.Migrations
{
    public class OnInstallMigration : IModuleOnInstallMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public OnInstallMigration(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.Application.AutoMapper";

        public void OnInstall()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

            var group = app.ModuleSettingGroups.FirstOrDefault(x => x.Id == "137e5f2f-8432-4ade-b797-9a8e5e703e6d");//Automapper Settings
            if (group == null)
            {
                group = new ApplicationModuleSettingsPersistable
                {
                    Id = "137e5f2f-8432-4ade-b797-9a8e5e703e6d",
                    Module = "Intent.Application.AutoMapper",
                    Title = "AutoMapper Settings",
                    Settings = []
                };

                app.ModuleSettingGroups.Add(group);
            }

            var entityConvention = group.Settings.FirstOrDefault(s => s.Title == "e2a4568b-8eac-49a4-b7df-9dbe9dc04d20");

            if (entityConvention == null)
            {
                group.Settings.Add(new ModuleSettingPersistable
                {
                    Id = "e2a4568b-8eac-49a4-b7df-9dbe9dc04d20",
                    Title = "Profile location",
                    Module = "Intent.Application.AutoMapper",
                    Hint = "Put AutoMapper profiles in Dtos files or separate them out.",
                    ControlType = ModuleSettingControlType.Select,
                    Value = "profile-separate",
                    Options =
                    [
                        new SettingFieldOptionPersistable { Description = "Profile in Dto", Value = "profile-in-dto" },
                        new SettingFieldOptionPersistable { Description = "Profile Separate", Value = "profile-separate" },
                    ]
                });
            }
            app.SaveAllChanges();
        }
    }
}