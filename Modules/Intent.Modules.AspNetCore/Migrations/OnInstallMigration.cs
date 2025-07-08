using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Migrations
{
    public class OnInstallMigration : IModuleOnInstallMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public OnInstallMigration(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.AspNetCore";

        public void OnInstall()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

            var group = app.ModuleSettingGroups.FirstOrDefault(x => x.Id == "9677c813-b334-4b0d-be2c-3b149f1f6ae8");//ASPNET Core Settings
            if (group == null)
            {
                group = new ApplicationModuleSettingsPersistable
                {
                    Id = "9677c813-b334-4b0d-be2c-3b149f1f6ae8",
                    Module = "Intent.AspNetCore",
                    Title = "ASP.NET Core Settings",
                    Settings = [
                        new ModuleSettingPersistable
                        {
                            Id = "d8308c84-a42a-4ca2-b871-f556c5f4ef4d",
                            Title = "Add Required Attribute To Parameters",
                            Module = "Intent.AspNetCore",
                            Hint = "Controls whether the [Required] attribute is added to qualifying parameters.",
                            ControlType = ModuleSettingControlType.Switch,
                            Value = "true"
                        }
                    ]
                };

                app.ModuleSettingGroups.Add(group);

                app.SaveAllChanges();
            }
        }
    }
}