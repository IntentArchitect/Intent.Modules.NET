using System.Diagnostics;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.MongoDb.Migrations
{
    public class Migration_02_00_00_Alpha_02 : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;
        public Migration_02_00_00_Alpha_02(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.MongoDb";
        [IntentFully]
        public string ModuleVersion => "2.0.0-alpha.2";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            if (app == null)
                return;

            var mongoDb = app.ModuleSettingGroups.FirstOrDefault(x => x.Id == "65b66781-0c91-48b4-990e-b9456f203ca6");
            if (mongoDb is null)
            {
                var group = new ApplicationModuleSettingsPersistable
                {
                    Id = "65b66781-0c91-48b4-990e-b9456f203ca6",
                    Module = "Intent.MongoDb",
                    Title = "MongoDB Settings",
                    Settings = new System.Collections.Generic.List<ModuleSettingPersistable>
                    {
                        new ModuleSettingPersistable
                        {
                            ControlType = ModuleSettingControlType.Switch,
                            Id = "5a17d713-b7f7-45c2-ab23-3614e2f95509",
                            Title = "Persist primary key as ObjectId",
                            Value = "true"
                        },
                        new ModuleSettingPersistable
                        {
                            ControlType = ModuleSettingControlType.Switch,
                            Id = "24bcf37a-a0fe-49f3-8121-bdad543c4fe6",
                            Title = "Always include discriminator in documents",
                            Value = "false"
                        }
                    }
                };

                app.ModuleSettingGroups.Add(group);

                app.SaveAllChanges();
            }
        }

        public void Down()
        {
        }
    }
}