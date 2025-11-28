using System.Diagnostics;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.Pagination.Migrations
{
    public class Migration_04_01_04_Pre_00 : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_04_01_04_Pre_00(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.Application.Dtos.Pagination";
        [IntentFully]
        public string ModuleVersion => "4.1.4-pre.0";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

            const string paginationGroupId = "f095c190-7906-4618-b2e6-fb8fe26708e6";
            var group = app.ModuleSettingGroups.FirstOrDefault(x => x.Id == paginationGroupId);
            if (group == null)
            {
                group = new ApplicationModuleSettingsPersistable
                {
                    Id = paginationGroupId,
                    Module = "Intent.Application.Dtos.Pagination",
                    Title = "Pagination Settings",
                    Settings = []
                };

                group.Settings.Add(new ModuleSettingPersistable
                {
                    Id = "0556e2b8-9fc5-4376-ae15-978cc5c18f3f",
                    ControlType = ModuleSettingControlType.TextBox,
                    IsRequired = false
                });

                group.Settings.Add(new ModuleSettingPersistable
                {
                    Id = "b406f718-bd47-4e5d-890f-b96b7c71ad2e",
                    ControlType = ModuleSettingControlType.Number,
                    IsRequired = false,
                    Value = null
                });

                app.ModuleSettingGroups.Add(group);

                app.SaveAllChanges();
            }
        }

        public void Down()
        {
        }
    }
}