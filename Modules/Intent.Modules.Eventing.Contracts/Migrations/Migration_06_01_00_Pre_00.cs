using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Eventing.Contracts.Migrations
{
    public class Migration_06_01_00_Pre_00 : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_06_01_00_Pre_00(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.Eventing.Contracts";
        [IntentFully]
        public string ModuleVersion => "6.1.0-pre.0";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

            const string EventingSettingsGroupId = "38416eff-a5f1-4842-b77c-ab301d7d1d67";
            const string LegacySettingId = "0420c177-417d-4073-bf26-250f1907a565";
            var group = app.ModuleSettingGroups.FirstOrDefault(x => x.Id == EventingSettingsGroupId);
            if (group is null)
            {
                group = new ApplicationModuleSettingsPersistable
                {
                    Id = EventingSettingsGroupId,
                    Module = "Intent.Eventing.Contracts",
                    Title = "Eventing Settings",
                };
                app.ModuleSettingGroups.Add(group);
            }

            var legacySetting = group.Settings.FirstOrDefault(x => x.Id == LegacySettingId);
            if (legacySetting is not null)
            {
                return;
            }
            
            var newSetting = new ModuleSettingPersistable
            {
                Id = LegacySettingId,
                Title = "Use legacy interface name",
                Module = "Intent.Eventing.Contracts",
                ControlType = ModuleSettingControlType.Switch,
                Value = "true",
                Hint = "Use legacy `IEventBus` interface name instead of `IMessageBus`.",
                IsRequired = false
            };
            group.Settings.Add(newSetting);
            app.SaveAllChanges();
        }

        public void Down()
        {
        }
    }
}